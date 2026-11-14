using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using REAL_STATE.Data;
using REAL_STATE.Models;
using REAL_STATE.Repositories;
using REAL_STATE.Repositories.IRepositories;
using REAL_STATE.Services;
using REAL_STATE.Services.Interfaces;
using REAL_STATE.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options => options.Filters.Add<SafeExceptionFilter>());
builder.Services.AddScoped<SafeErrorMessages>();
builder.Services.AddApplicationRateLimiting();
builder.Services.AddRazorPages();
builder.Services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("database");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString) || connectionString == "SET_IN_SECRETS_OR_SERVER")
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.User.RequireUniqueEmail = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromMinutes(1));
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});
builder.Services.AddAuthorization(options => options.AddPolicy("ActiveAgent", policy =>
    policy.RequireAuthenticatedUser().AddRequirements(new ActiveAgentRequirement())));
builder.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, ActiveAgentHandler>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAccountConfirmationService, AccountConfirmationService>();
builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, SmtpEmailSender>();
builder.Services.AddOptions<SmtpOptions>().BindConfiguration("Smtp")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Host) && System.Net.Mail.MailAddress.TryCreate(o.From, out _)
        && o.Port > 0 && o.Port <= 65535 && (builder.Environment.IsDevelopment() || o.EnableSsl),
        "Configure Smtp with a host, valid sender and port; TLS is required outside Development.").ValidateOnStart();
builder.Services.AddOptions<PublicSiteOptions>().BindConfiguration("PublicSite")
    .Validate(o => Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out var uri)
        && (uri.Scheme == "https" || builder.Environment.IsDevelopment() && uri.Scheme == "http" && uri.IsLoopback)
        && string.IsNullOrEmpty(uri.UserInfo) && string.IsNullOrEmpty(uri.Query) && string.IsNullOrEmpty(uri.Fragment),
        "Configure PublicSite:BaseUrl with the public HTTPS origin.").ValidateOnStart();
if (!builder.Environment.IsDevelopment())
{
    var hosts = builder.Configuration["AllowedHosts"];
    if (string.IsNullOrWhiteSpace(hosts) || hosts.Split(';').Any(host => host.Trim() == "*"))
        throw new InvalidOperationException("Set AllowedHosts to the deployment host names.");
    var sql = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
    if (sql.TrustServerCertificate || sql.Encrypt == Microsoft.Data.SqlClient.SqlConnectionEncryptOption.Optional)
        throw new InvalidOperationException("Production SQL connections require encryption and certificate validation.");
}

#region -- Register Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IDeveloperRepository, DeveloperRepository>();
builder.Services.AddScoped<IAgentRepository, AgentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAppointmentRepository, ViewingAppointmentRepository>();
builder.Services.AddScoped<IReferralRepository, ReferralRepository>();
#endregion

#region -- Register services
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<IDeveloperService, DeveloperService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IRefferalService, ReferralService>();

#endregion



var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapStaticAssets();
app.MapHealthChecks("/health/ready");

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Hero}/{id?}")
    .WithStaticAssets();
if (args.Contains("--initialize-database", StringComparer.OrdinalIgnoreCase))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<IDbInitializer>().InitializeAsync();
    return;
}
app.MapRazorPages().WithStaticAssets();



app.Run();


public partial class Program { }
