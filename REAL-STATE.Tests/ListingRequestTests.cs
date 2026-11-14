using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using REAL_STATE.Data;
using REAL_STATE.Models;
using REAL_STATE.Utilities;
using Xunit;

namespace REAL_STATE.Tests;

public sealed class AuthenticatedAppFactory(TestDatabase test, ClaimsPrincipal principal) : AppFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ApplicationDbContext>();
            services.AddScoped<ApplicationDbContext>(_ => new SqliteTestContext(
                new DbContextOptionsBuilder<SqliteTestContext>()
                    .UseSqlite((SqliteConnection)test.Db.Database.GetDbConnection()).Options));
            services.AddSingleton(principal);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
                options.DefaultForbidScheme = "Test";
            }).AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", _ => { });
        });
    }
}

public sealed class TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger, UrlEncoder encoder, ClaimsPrincipal principal)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        => Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name)));
}

public sealed class ListingRequestTests
{
    private static ClaimsPrincipal AgentPrincipal(string id) => new(new ClaimsIdentity(
        [new Claim(ClaimTypes.NameIdentifier, id), new Claim(ClaimTypes.Role, RoleSD.Agent)], "Test"));

    private static async Task<string> GetTokenAsync(HttpClient client, string path)
    {
        var response = await client.GetAsync(path);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        var match = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        Assert.True(match.Success, "Antiforgery token must be present.");
        return WebUtility.HtmlDecode(match.Groups[1].Value);
    }

    [Fact]
    public async Task Crafted_listing_form_cannot_create_related_developers_agents_or_featured_listings()
    {
        await using var test = new TestDatabase(); await test.InitializeAsync();
        var (owner, _, property) = await test.SeedListingsAsync();
        await using var factory = new AuthenticatedAppFactory(test, AgentPrincipal(owner.UserId!));
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
        var token = await GetTokenAsync(client, "/AgentAndBroker/Listing/Upsert");
        var response = await client.PostAsync("/AgentAndBroker/Listing/Upsert", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Property.Id"] = Guid.NewGuid().ToString(), ["Property.Title"] = "Crafted listing",
            ["Property.Slug"] = "crafted-listing", ["Property.DeveloperId"] = property.DeveloperId,
            ["Property.PropertyType"] = "House", ["Property.ListingType"] = "Sale",
            ["Property.City"] = "Manila", ["Property.Country"] = "Philippines", ["Property.Status"] = "Available",
            ["Property.IsFeatured"] = "true", ["Property.Price"] = "1000", ["Property.FloorAreaSqFt"] = "100",
            ["Property.Developer.Name"] = "Unauthorized developer", ["Property.Developer.Slug"] = "unauthorized-developer",
            ["Property.Agent.FullName"] = "Injected agent", ["Property.Agent.Email"] = "injected@example.test",
            ["Property.Images[0].ImageUrl"] = "https://example.test/injected.jpg"
        }));
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var saved = await test.Db.Properties.AsNoTracking().SingleAsync(p => p.Slug == "crafted-listing");
        Assert.Equal(owner.Id, saved.AgentId); Assert.False(saved.IsFeatured);
        Assert.Equal(1, await test.Db.Developers.CountAsync()); Assert.Equal(2, await test.Db.Agents.CountAsync());
        Assert.Empty(await test.Db.PropertyImages.Where(i => i.PropertyId == saved.Id).ToListAsync());
    }

    [Fact]
    public async Task Inactive_agent_cannot_delete_a_listing()
    {
        await using var test = new TestDatabase(); await test.InitializeAsync();
        var (owner, _, property) = await test.SeedListingsAsync(); owner.IsActive = false; await test.Db.SaveChangesAsync();
        await using var factory = new AuthenticatedAppFactory(test, AgentPrincipal(owner.UserId!));
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
        var token = await GetTokenAsync(client, "/AgentAndBroker/Listing/Index");
        var response = await client.PostAsync("/AgentAndBroker/Listing/Delete", new FormUrlEncodedContent(new Dictionary<string, string>
        { ["__RequestVerificationToken"] = token, ["id"] = property.Id }));
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.True(await test.Db.Properties.AnyAsync(p => p.Id == property.Id));
    }

    [Fact]
    public async Task Agent_cannot_approve_another_agents_appointment_over_http()
    {
        await using var test = new TestDatabase(); await test.InitializeAsync();
        var (owner, other, property) = await test.SeedListingsAsync();
        var appointment = new ViewingAppointment { Property = property, Agent = owner, ContactName = "Customer", ContactEmail = "customer@example.test" };
        test.Db.Add(appointment); await test.Db.SaveChangesAsync();
        await using var factory = new AuthenticatedAppFactory(test, AgentPrincipal(other.UserId!));
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
        var token = await GetTokenAsync(client, "/Admin/Appointment/Index");
        var response = await client.PostAsync("/Admin/Appointment/Approve", new FormUrlEncodedContent(new Dictionary<string, string>
        { ["__RequestVerificationToken"] = token, ["appointmentId"] = appointment.Id }));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Pending", await test.Db.ViewingAppointments.Where(a => a.Id == appointment.Id).Select(a => a.Status).SingleAsync());
    }
}
