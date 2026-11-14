namespace REAL_STATE.Utilities;

// Existing form handlers can preserve their UX without disclosing infrastructure errors.
public sealed class SafeErrorMessages(ILogger<SafeErrorMessages> logger)
{
    public string For(Exception exception)
    {
        if (exception is GlobalExceptionHandler) return exception.Message;
        logger.LogError(exception, "An application operation failed.");
        return "The operation could not be completed. Please try again.";
    }
}
