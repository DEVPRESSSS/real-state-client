using Microsoft.AspNetCore.Identity;

namespace REAL_STATE.Utilities;

public static class IdentityResultExtensions
{
    public static void ThrowIfFailed(this IdentityResult result)
    {
        if (!result.Succeeded)
            throw new GlobalExceptionHandler(string.Join(" ", result.Errors.Select(e => e.Description)));
    }
}
