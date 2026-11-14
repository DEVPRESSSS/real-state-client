using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using REAL_STATE.Data;
using REAL_STATE.Models;
using REAL_STATE.Models.VM;
using REAL_STATE.Repositories;
using REAL_STATE.Services;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;
using Xunit;

namespace REAL_STATE.Tests;

// SQLite executes real relational transactions and conditional updates without touching SQL Server.
public sealed class TestDatabase : IAsyncDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private ServiceProvider _services = default!;
    public IServiceScope Scope { get; private set; } = default!;
    public ApplicationDbContext Db => Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    public UserManager<ApplicationUser> Users => Scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    public RoleManager<IdentityRole> Roles => Scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
        _connection.CreateFunction("GETUTCDATE", () => DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDataProtection().UseEphemeralDataProtectionProvider();
        services.AddAuthentication();
        services.AddDbContext<ApplicationDbContext, SqliteTestContext>(o => o.UseSqlite(_connection));
        services.AddIdentityCore<ApplicationUser>().AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
        _services = services.BuildServiceProvider();
        Scope = _services.CreateScope();
        await Db.Database.EnsureCreatedAsync();
        await Db.PropertyImages.ExecuteDeleteAsync();
        await Db.Properties.ExecuteDeleteAsync();
        await Db.Agents.ExecuteDeleteAsync();
        await Db.Developers.ExecuteDeleteAsync();
        foreach (var role in new[] { RoleSD.Admin, RoleSD.Agent, RoleSD.User })
            (await Roles.CreateAsync(new IdentityRole(role))).ThrowIfFailed();
    }

    public async Task<ApplicationUser> CreateUserAsync(string name, string role = RoleSD.User)
    {
        var user = new ApplicationUser { UserName = name + "@example.test", Email = name + "@example.test", FullName = name, EmailConfirmed = true };
        (await Users.CreateAsync(user, "Testing123!xyz")).ThrowIfFailed();
        (await Users.AddToRoleAsync(user, role)).ThrowIfFailed();
        return user;
    }

    public async Task<(Agent Owner, Agent Other, Property Property)> SeedListingsAsync()
    {
        var ownerUser = await CreateUserAsync("owner", RoleSD.Agent);
        var otherUser = await CreateUserAsync("other", RoleSD.Agent);
        var owner = new Agent { UserId = ownerUser.Id, FullName = "Owner", Email = ownerUser.Email! };
        var other = new Agent { UserId = otherUser.Id, FullName = "Other", Email = otherUser.Email! };
        var developer = new Developers { Name = "Developer", Slug = "developer" };
        var property = new Property { Developer = developer, Agent = owner, Title = "Test property", Slug = "test-property", City = "Manila", PropertyType = "House" };
        Db.AddRange(owner, other, property);
        await Db.SaveChangesAsync();
        return (owner, other, property);
    }

    public async ValueTask DisposeAsync()
    {
        Scope?.Dispose();
        if (_services != null) await _services.DisposeAsync();
        await _connection.DisposeAsync();
    }
}

public sealed class SqliteTestContext(DbContextOptions<SqliteTestContext> options) : ApplicationDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // SQLite lacks a native DateTimeOffset comparison; store UTC ticks in the test database.
        foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetProperties()))
            if (property.ClrType == typeof(DateTimeOffset) || property.ClrType == typeof(DateTimeOffset?))
                property.SetValueConverter(new ValueConverter<DateTimeOffset, long>(v => v.UtcTicks, v => new DateTimeOffset(v, TimeSpan.Zero)));
    }
}

public sealed class RecordingConfirmation : IAccountConfirmationService
{
    public List<string> UserIds { get; } = [];
    public Task SendAsync(ApplicationUser user) { UserIds.Add(user.Id); return Task.CompletedTask; }
}

public sealed class SecurityAndQueryTests
{
    [Fact]
    public async Task Approval_requires_active_owner_or_administrator_and_succeeds_only_once()
    {
        await using var test = new TestDatabase(); await test.InitializeAsync();
        var (owner, other, property) = await test.SeedListingsAsync();
        var appointment = new ViewingAppointment { Property = property, Agent = owner, ContactName = "Customer", ContactEmail = "customer@example.test", ScheduledAt = DateTime.UtcNow.AddDays(1) };
        test.Db.Add(appointment); await test.Db.SaveChangesAsync();
        var repository = new ViewingAppointmentRepository(test.Db);
        Assert.False(await repository.ApproveAsync(appointment.Id, other.UserId!, false));
        owner.IsActive = false; await test.Db.SaveChangesAsync();
        Assert.False(await repository.ApproveAsync(appointment.Id, owner.UserId!, false));
        owner.IsActive = true; await test.Db.SaveChangesAsync();
        Assert.True(await repository.ApproveAsync(appointment.Id, owner.UserId!, false));
        Assert.False(await repository.ApproveAsync(appointment.Id, owner.UserId!, false));
        var unassigned = new ViewingAppointment { Property = property, ContactName = "Visitor", ContactEmail = "visitor@example.test" };
        test.Db.Add(unassigned); await test.Db.SaveChangesAsync();
        Assert.True(await repository.ApproveAsync(unassigned.Id, "admin", true));
        Assert.False(await repository.ApproveAsync("missing", owner.UserId!, false));
    }

    [Fact]
    public async Task Agent_list_uses_assigned_agent_and_scoped_total_not_requesting_customer()
    {
        await using var test = new TestDatabase(); await test.InitializeAsync();
        var (owner, other, property) = await test.SeedListingsAsync();
        test.Db.AddRange(
            new ViewingAppointment { Property = property, Agent = owner, UserId = other.UserId, ContactName = "Owned", ContactEmail = "owned@example.test" },
            new ViewingAppointment { Property = property, Agent = other, UserId = owner.UserId, ContactName = "Not owned", ContactEmail = "other@example.test" });
        await test.Db.SaveChangesAsync();
        var repository = new ViewingAppointmentRepository(test.Db);
        var page = await repository.GetPagedForAgentAsync(int.MaxValue, int.MaxValue, owner.UserId!);
        Assert.Equal(1, page.TotalItems); Assert.Equal(1, page.CurrentPage); Assert.Equal(100, page.PageSize);
        Assert.Equal("Owned", Assert.Single(page.Items).ContactName);
    }

    [Fact]
    public async Task Read_queries_do_not_track_entities_and_paging_bounds_extreme_input()
    {
        await using var test = new TestDatabase(); await test.InitializeAsync();
        await test.SeedListingsAsync(); test.Db.ChangeTracker.Clear();
        var repository = new PropertyRepository(test.Db);
        Assert.Single(await repository.GetAllAsync());
        Assert.Empty(test.Db.ChangeTracker.Entries());
        var page = await repository.GetPageAsync(q => q.OrderBy(p => p.Id), int.MaxValue, int.MaxValue);
        Assert.Equal(1, page.CurrentPage); Assert.Equal(100, page.PageSize); Assert.Single(page.Items);
        await repository.GetAllAsync(tracked: true); Assert.Single(test.Db.ChangeTracker.Entries<Property>());
    }

    [Fact]
    public async Task Property_input_rejects_invalid_reference_and_never_persists_client_identity_or_navigation_graph()
    {
        await using var test = new TestDatabase(); await test.InitializeAsync();
        var (_, _, property) = await test.SeedListingsAsync();
        var service = new PropertyService(test.Db, new PropertyRepository(test.Db));
        var input = PropertyInput.FromEntity(property); input.Id = "attacker-controlled"; input.Slug = "new-listing";
        await service.AddProperty(new PropertyVM { Property = input });
        var saved = await test.Db.Properties.SingleAsync(p => p.Slug == "new-listing");
        Assert.NotEqual(input.Id, saved.Id); Assert.Equal(1, await test.Db.Developers.CountAsync());
        input.DeveloperId = "missing"; input.Slug = "bad-reference";
        await Assert.ThrowsAsync<GlobalExceptionHandler>(() => service.AddProperty(new PropertyVM { Property = input }));
        input.DeveloperId = property.DeveloperId; input.Status = "arbitrary-state";
        await Assert.ThrowsAsync<GlobalExceptionHandler>(() => service.AddProperty(new PropertyVM { Property = input }));
        Assert.Equal(2, await test.Db.Properties.CountAsync());
    }

    [Fact]
    public async Task Account_update_rolls_back_email_and_roles_when_password_reset_fails()
    {
        await using var test = new TestDatabase(); await test.InitializeAsync();
        var user = await test.CreateUserAsync("rollback");
        var service = new UserService(test.Users, test.Roles, test.Db, new RecordingConfirmation());
        await Assert.ThrowsAsync<GlobalExceptionHandler>(() => service.UpdateUser(new UserVM
        {
            Id = user.Id, FullName = "Changed", Email = "changed@example.test", Role = RoleSD.Agent, Password = "weak"
        }));
        test.Db.ChangeTracker.Clear();
        var saved = await test.Users.FindByIdAsync(user.Id);
        Assert.Equal("rollback@example.test", saved!.Email);
        Assert.True(await test.Users.IsInRoleAsync(saved, RoleSD.User));
        Assert.False(await test.Users.IsInRoleAsync(saved, RoleSD.Agent));
    }

    [Fact]
    public async Task Account_lock_and_role_changes_replace_security_stamp()
    {
        await using var test = new TestDatabase(); await test.InitializeAsync();
        var user = await test.CreateUserAsync("revocation"); var before = user.SecurityStamp;
        var service = new UserService(test.Users, test.Roles, test.Db, new RecordingConfirmation());
        await service.SetLock(user.Id, true, "another-admin");
        Assert.NotEqual(before, user.SecurityStamp); Assert.True(await test.Users.IsLockedOutAsync(user));
        before = user.SecurityStamp;
        await service.UpdateUser(new UserVM { Id = user.Id, FullName = user.FullName, Email = user.Email!, Role = RoleSD.Agent });
        Assert.NotEqual(before, user.SecurityStamp); Assert.True(await test.Users.IsInRoleAsync(user, RoleSD.Agent));
    }

    [Fact]
    public async Task User_page_projects_roles_in_sql_and_bounds_the_result()
    {
        await using var test = new TestDatabase(); await test.InitializeAsync();
        var user = await test.CreateUserAsync("first", RoleSD.Agent); await test.CreateUserAsync("second");
        var service = new UserService(test.Users, test.Roles, test.Db, new RecordingConfirmation());
        var page = await service.GetAllUsers(user.Id, pageSize: 1);
        Assert.Equal(2, page.TotalItems); Assert.Equal(RoleSD.Agent, Assert.Single(page.Items).Role);
        Assert.True(page.HasNextPage);
    }

    [Fact]
    public async Task Dashboard_and_public_cards_return_aggregates_and_database_pages()
    {
        await using var test = new TestDatabase(); await test.InitializeAsync();
        await test.SeedListingsAsync();
        var dashboard = await new DashboardService(test.Db).GetAsync();
        Assert.Equal(1, dashboard.TotalProperties); Assert.Equal(2, dashboard.TotalAgents); Assert.Equal(2, dashboard.TotalUsers);
        var agents = await new AgentService(new AgentRepository(test.Db), test.Users, test.Db, new RecordingConfirmation()).GetAgentsPage(1, 1);
        Assert.Single(agents.Agents); Assert.Equal(2, agents.TotalItems);
        var developers = await new DeveloperService(new DeveloperRepository(test.Db), test.Db).GetActiveDevelopers();
        Assert.Equal(1, Assert.Single(developers.Items).AvailableListings);
    }

    [Fact]
    public async Task Agent_registration_requires_email_confirmation_and_administrator_activation()
    {
        await using var test = new TestDatabase(); await test.InitializeAsync();
        var confirmation = new RecordingConfirmation();
        var service = new AgentService(new AgentRepository(test.Db), test.Users, test.Db, confirmation);
        await service.RegisterAgent(new AgentRegisterVM
        {
            FullName = "Pending agent", Email = "pending@example.test", LicenseNumber = "TEST-123",
            Password = "Testing123!xyz", ConfirmPassword = "Testing123!xyz"
        });
        var user = await test.Users.FindByEmailAsync("pending@example.test");
        Assert.NotNull(user); Assert.False(user.EmailConfirmed);
        Assert.True(await test.Users.IsInRoleAsync(user, RoleSD.Agent));
        Assert.False((await test.Db.Agents.SingleAsync(a => a.UserId == user.Id)).IsActive);
        Assert.Equal(user.Id, Assert.Single(confirmation.UserIds));
    }

    [Fact]
    public async Task Public_property_search_filters_before_counting_and_paging()
    {
        await using var test = new TestDatabase(); await test.InitializeAsync();
        var (_, _, property) = await test.SeedListingsAsync();
        test.Db.Add(new Property { DeveloperId = property.DeveloperId, Title = "Other city", Slug = "other-city", City = "Cebu", PropertyType = "House" });
        await test.Db.SaveChangesAsync();
        var service = new PropertyService(test.Db, new PropertyRepository(test.Db));
        var result = await service.Filter(new REAL_STATE.Models.DTO.PropertyDTO.CriteriaDTO
            { City = "Man", PropertyType = "House", PageSize = 1, Page = int.MaxValue });
        Assert.Equal(1, result.TotalItems); Assert.Equal(1, result.CurrentPage);
        Assert.Equal(property.Id, Assert.Single(result.Items).Id);
    }

    [Theory]
    [InlineData("javascript:alert(1)", false)]
    [InlineData("data:text/html,test", false)]
    [InlineData("//example.com", false)]
    [InlineData("https://user:password@example.com", false)]
    [InlineData("https://example.com/path", true)]
    public void Stored_links_allow_only_absolute_http_urls_without_credentials(string url, bool expected)
        => Assert.Equal(expected, HttpUrlAttribute.IsHttpUrl(url));
}
