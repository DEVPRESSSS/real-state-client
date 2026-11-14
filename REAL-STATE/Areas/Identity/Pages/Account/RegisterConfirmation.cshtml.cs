using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace REAL_STATE.Areas.Identity.Pages.Account;
[AllowAnonymous]
public sealed class RegisterConfirmationModel : PageModel
{
    // Same response for every address; tokens are delivered only through email.
    public void OnGet() { }
}
