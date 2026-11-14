using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using REAL_STATE.Models;
using REAL_STATE.Services.Interfaces;

namespace REAL_STATE.Services;

public sealed class AccountConfirmationService(UserManager<ApplicationUser> users,
    IEmailSender sender, IOptions<PublicSiteOptions> site, ILogger<AccountConfirmationService> logger) : IAccountConfirmationService
{
    public async Task SendAsync(ApplicationUser user)
    {
        var token = await users.GenerateEmailConfirmationTokenAsync(user);
        var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var url = QueryHelpers.AddQueryString(site.Value.BaseUrl.TrimEnd('/') + "/Identity/Account/ConfirmEmail",
            new Dictionary<string, string?> { ["userId"] = user.Id, ["code"] = code });
        try
        {
            await sender.SendEmailAsync(user.Email!, "Confirm your email",
            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(url)}'>clicking here</a>.");
        }
        catch (System.Net.Mail.SmtpException exception)
        {
            logger.LogError(exception, "Unable to send account confirmation.");
            throw new REAL_STATE.Utilities.GlobalExceptionHandler("Your account was saved, but email delivery failed. Use Resend email confirmation on the login page to try again.");
        }
    }
}

public sealed class PublicSiteOptions
{
    public string BaseUrl { get; set; } = "";
}
