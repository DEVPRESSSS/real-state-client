namespace REAL_STATE.Utilities;

public static class ViewingTime
{
    private static readonly TimeZoneInfo PhilippineTime = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");

    // ScheduledAt represents the local time at the property, including existing records.
    public static DateTime Now => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, PhilippineTime).DateTime;
}
