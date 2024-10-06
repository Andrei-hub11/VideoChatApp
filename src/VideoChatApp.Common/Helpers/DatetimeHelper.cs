using NodaTime;

namespace VideoChatApp.Common.Helpers;

public static class DateTimeHelper
{
    private static readonly DateTimeZone BrasiliaTimeZone = DateTimeZoneProviders.Tzdb["America/Sao_Paulo"];
    private static readonly DateTimeZone NewYorkTimeZone = DateTimeZoneProviders.Tzdb["America/New_York"];

    public static DateTime NowInBrasilia()
    {
        var now = SystemClock.Instance.GetCurrentInstant()
                     .InZone(BrasiliaTimeZone)
                     .ToDateTimeUnspecified(); // Transforms directly into DateTime (without time zone)

        return now;
    }

    public static DateTime NowInNewYork()
    {
        var now = SystemClock.Instance.GetCurrentInstant()
                     .InZone(NewYorkTimeZone)
                     .ToDateTimeUnspecified();

        return now;
    }
}
