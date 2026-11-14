using System.ComponentModel.DataAnnotations;

namespace REAL_STATE.Utilities;

public sealed class HttpUrlAttribute : ValidationAttribute
{
    public HttpUrlAttribute() : base("Enter an absolute HTTP or HTTPS URL.") { }

    public override bool IsValid(object? value) => value is null || value is string text &&
        (string.IsNullOrWhiteSpace(text) || IsHttpUrl(text));

    public static bool IsHttpUrl(string? value) => Uri.TryCreate(value, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp)
        && string.IsNullOrEmpty(uri.UserInfo);
}
