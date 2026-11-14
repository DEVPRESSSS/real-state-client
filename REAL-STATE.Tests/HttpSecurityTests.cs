using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace REAL_STATE.Tests;

public class AppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.ConfigureServices(services => services.AddDataProtection().UseEphemeralDataProtectionProvider());
        builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Server=(local);Database=unused;Integrated Security=true;Encrypt=false",
            ["PublicSite:BaseUrl"] = "https://localhost",
            ["Smtp:Host"] = "localhost", ["Smtp:From"] = "noreply@localhost.test", ["Smtp:EnableSsl"] = "false"
        }));
    }
}

public sealed class HttpSecurityTests
{
    [Fact]
    public async Task Public_page_renders_nonce_protected_scripts_and_security_headers()
    {
        await using var factory = new AppFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
        var response = await client.GetAsync("/Home/Hero");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var csp = Assert.Single(response.Headers.GetValues("Content-Security-Policy"));
        var nonce = Regex.Match(csp, "'nonce-([^']+)'").Groups[1].Value;
        Assert.NotEmpty(nonce);
        var html = await response.Content.ReadAsStringAsync();
        foreach (Match script in Regex.Matches(html, "<script\\b[^>]*>"))
            Assert.Contains("nonce=\"" + nonce + "\"", WebUtility.HtmlDecode(script.Value));
        Assert.DoesNotContain("unpkg.com", html); Assert.DoesNotContain("cdn.jsdelivr.net", html);
        Assert.Contains("nosniff", response.Headers.GetValues("X-Content-Type-Options"));
    }

    [Fact]
    public async Task Confirmation_page_never_looks_up_accounts_or_displays_confirmation_tokens()
    {
        await using var factory = new AppFactory(); using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        var response = await client.GetAsync("/Identity/Account/RegisterConfirmation?email=someone@example.test");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("confirm-link", html); Assert.DoesNotContain("code=", html); Assert.DoesNotContain("someone@example.test", html);
    }

    [Fact]
    public async Task Repeated_identity_posts_are_throttled_even_when_antiforgery_validation_fails()
    {
        await using var factory = new AppFactory(); using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var response = await client.PostAsync("/Identity/Account/Login", new FormUrlEncodedContent([]));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        Assert.Equal(HttpStatusCode.TooManyRequests, (await client.PostAsync("/Identity/Account/Login", new FormUrlEncodedContent([]))).StatusCode);
    }

    [Fact]
    public async Task Anonymous_visitors_cannot_access_administration()
    {
        await using var factory = new AppFactory(); using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
        var response = await client.GetAsync("/Admin/Property/Index");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location!.ToString());
    }
}
