using System.Security.Cryptography;
namespace REAL_STATE.Utilities;

public sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    public const string NonceKey = "CspNonce";
    public async Task InvokeAsync(HttpContext context)
    {
        var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        context.Items[NonceKey] = nonce;
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;
            headers["Content-Security-Policy"] = $"default-src 'self'; script-src 'self' 'nonce-{nonce}'; " +
                "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; font-src 'self' https://fonts.gstatic.com; " +
                "img-src 'self' https: data:; connect-src 'self'; object-src 'none'; base-uri 'self'; " +
                "frame-ancestors 'none'; form-action 'self'; upgrade-insecure-requests";
            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            return Task.CompletedTask;
        });
        await next(context);
    }
}
