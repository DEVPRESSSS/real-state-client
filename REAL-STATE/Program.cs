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

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); 

builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
        options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() 
    .AddEntityFrameworkStores<ApplicationDbContext>();

#region -- Register Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IDeveloperRepository, DeveloperRepository>();
builder.Services.AddScoped<IAgentRepository, AgentRepository>();
#endregion

#region -- Register services
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IAgentService, AgentService>();
#endregion
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages().WithStaticAssets();

// Seed roles
// Seed roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in new[] { RoleSD.Admin, RoleSD.Agent, RoleSD.User })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Seed admin user
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    const string adminEmail = "xmontemorjerald@gmail.com";
    var adminPassword = "Montemor123#!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        if (string.IsNullOrWhiteSpace(adminPassword))
        {
            app.Logger.LogWarning("SeedAdmin:Password is not configured. Admin user was not created.");
        }
        else
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Administrator"
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (!result.Succeeded)
            {
                app.Logger.LogError("Failed to create admin user: {Errors}",
                    string.Join("; ", result.Errors.Select(e => e.Description)));
                adminUser = null;
            }
        }
    }

    if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, RoleSD.Admin))
    {
        await userManager.AddToRoleAsync(adminUser, RoleSD.Admin);
    }
}
app.Run();