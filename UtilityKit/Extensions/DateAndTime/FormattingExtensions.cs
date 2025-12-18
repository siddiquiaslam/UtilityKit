namespace UtilityKit.Extensions.DateAndTime;

/// <summary>
/// Provides extension methods for formatting values.
/// </summary>
/// <remarks>
/// This static class is intended to contain a variety of formatting helpers
/// that improve readability and consistency across applications.
/// </remarks>
public static class FormattingExtensions
{
    /// <summary>
    /// Converts a <see cref="DateTime"/> value into a human-readable relative time string.
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to convert. The input is normalized to UTC before comparison.
    /// </param>
    /// <returns>
    /// A string representing the relative time difference between the specified <paramref name="dateTime"/>
    /// and the current UTC time. Examples include:
    /// <list type="bullet">
    /// <item><description>"just now"</description></item>
    /// <item><description>"5 minutes ago"</description></item>
    /// <item><description>"yesterday"</description></item>
    /// <item><description>"2 weeks ago"</description></item>
    /// <item><description>"3 months ago"</description></item>
    /// <item><description>"2 years ago"</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// This method uses approximate calculations for months and years based on days
    /// (30 days ≈ 1 month, 365 days ≈ 1 year). For precise calendar calculations,
    /// consider using <see cref="System.Globalization.Calendar"/> methods.
    /// </remarks>
    public static string ToRelativeTime(this DateTime dateTime)
    {
        var ts = DateTime.UtcNow - dateTime.ToUniversalTime();
        var delta = Math.Abs(ts.TotalSeconds);

        return delta switch
        {
            < 60 => "just now",
            < 120 => "a minute ago",
            < 3600 => $"{ts.Minutes} minutes ago",
            < 7200 => "an hour ago",
            < 86400 => $"{ts.Hours} hours ago",
            < 172800 => "yesterday",
            < 604800 => $"{ts.Days} days ago",
            < 2419200 => $"{ts.Days / 7} weeks ago",
            < 29030400 => $"{ts.Days / 30} months ago", // ~12 months
            _ => $"{ts.Days / 365} years ago"
        };
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> value into an ISO 8601 formatted string (UTC).
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to format. The input is normalized to UTC before conversion.
    /// </param>
    /// <returns>
    /// A string representing the <paramref name="dateTime"/> in ISO 8601 format, e.g. "2025-12-15T21:39:00Z".
    /// </returns>
    public static string ToIso8601String(this DateTime dateTime)
    {
        return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> value into a Unix timestamp (seconds since 1970-01-01 UTC).
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to convert. The input is normalized to UTC before conversion.
    /// </param>
    /// <returns>
    /// A long representing the number of seconds elapsed since 1970-01-01T00:00:00Z.
    /// </returns>
    public static long ToUnixTimestamp(this DateTime dateTime)
    {
        var utc = dateTime.ToUniversalTime();
        var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return Convert.ToInt64((utc - unixEpoch).TotalSeconds);
    }

    /// <summary>
    /// Converts a Unix timestamp (seconds since 1970-01-01 UTC) into a <see cref="DateTime"/> value.
    /// </summary>
    /// <param name="timestamp">
    /// The Unix timestamp to convert, expressed as the number of seconds since 1970-01-01T00:00:00Z.
    /// </param>
    /// <returns>
    /// A <see cref="DateTime"/> value in UTC corresponding to the given Unix timestamp.
    /// </returns>
    public static DateTime FromUnixTimestamp(this long timestamp)
    {
        var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return unixEpoch.AddSeconds(timestamp);
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> value into a friendly, human-readable date string.
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to format.
    /// </param>
    /// <returns>
    /// A string representing the date in a friendly format, e.g. "Monday, December 15, 2025".
    /// </returns>
    public static string ToFriendlyDateString(this DateTime dateTime)
    {
        return dateTime.ToString("dddd, MMMM dd, yyyy");
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> value into a friendly, human-readable time string.
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to format.
    /// </param>
    /// <returns>
    /// A string representing the time in a friendly format, e.g. "9:45 PM".
    /// </returns>
    public static string ToFriendlyTimeString(this DateTime dateTime)
    {
        return dateTime.ToString("h:mm tt");
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> value into a friendly, human-readable date and time string.
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to format.
    /// </param>
    /// <returns>
    /// A string representing the date and time in a friendly format,
    /// e.g. "Monday, December 15, 2025 at 9:45 PM".
    /// </returns>
    public static string ToFriendlyDateTimeString(this DateTime dateTime)
    {
        return dateTime.ToString("dddd, MMMM dd, yyyy 'at' h:mm tt");
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> value into a relative day string such as "Today", "Tomorrow", or "Yesterday".
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to evaluate.
    /// </param>
    /// <returns>
    /// A string representing the relative day name if the date is today, tomorrow, or yesterday;
    /// otherwise the date is returned in a friendly format (e.g. "Monday, December 15, 2025").
    /// </returns>
    public static string ToRelativeDayString(this DateTime dateTime)
    {
        var today = DateTime.Today;
        var deltaDays = (dateTime.Date - today).Days;

        return deltaDays switch
        {
            0 => "Today",
            1 => "Tomorrow",
            -1 => "Yesterday",
            _ => dateTime.ToString("dddd, MMMM dd, yyyy")
        };
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> value into a relative week string such as "This week", "Next week", or "Last week".
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to evaluate.
    /// </param>
    /// <returns>
    /// A string representing the relative week name if the date falls in the current, next, or previous week;
    /// otherwise the week number and year are returned (e.g. "Week 51 of 2025").
    /// </returns>
    public static string ToRelativeWeekString(this DateTime dateTime)
    {
        var calendar = System.Globalization.CultureInfo.InvariantCulture.Calendar;
        var currentWeek = calendar.GetWeekOfYear(DateTime.Today, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        var targetWeek = calendar.GetWeekOfYear(dateTime.Date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

        var deltaWeeks = targetWeek - currentWeek;

        return deltaWeeks switch
        {
            0 => "This week",
            1 => "Next week",
            -1 => "Last week",
            _ => $"Week {targetWeek} of {dateTime.Year}"
        };
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> value into a quarter string such as "Q1 2025".
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to evaluate.
    /// </param>
    /// <returns>
    /// A string representing the quarter of the year for the given <paramref name="dateTime"/>,
    /// e.g. "Q4 2025".
    /// </returns>
    public static string ToQuarterString(this DateTime dateTime)
    {
        var quarter = (dateTime.Month - 1) / 3 + 1;

        return quarter switch
        {
            1 => $"Q1 {dateTime.Year}",
            2 => $"Q2 {dateTime.Year}",
            3 => $"Q3 {dateTime.Year}",
            4 => $"Q4 {dateTime.Year}",
            _ => $"Unknown quarter {dateTime.Year}"
        };
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> value into a month name string such as "December 2025".
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to evaluate.
    /// </param>
    /// <returns>
    /// A string representing the month name and year for the given <paramref name="dateTime"/>,
    /// e.g. "December 2025".
    /// </returns>
    public static string ToMonthNameString(this DateTime dateTime)
    {
        return dateTime.ToString("MMMM yyyy");
    }


    /// <summary>
    /// Converts a <see cref="DateTime"/> value into a relative month string such as "This month", "Next month", or "Last month".
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to evaluate.
    /// </param>
    /// <returns>
    /// A string representing the relative month name if the date falls in the current, next, or previous month;
    /// otherwise the month name and year are returned (e.g. "December 2025").
    /// </returns>
    public static string ToRelativeMonthString(this DateTime dateTime)
    {
        var today = DateTime.Today;
        var deltaMonths = (dateTime.Year - today.Year) * 12 + (dateTime.Month - today.Month);

        return deltaMonths switch
        {
            0 => "This month",
            1 => "Next month",
            -1 => "Last month",
            _ => dateTime.ToString("MMMM yyyy")
        };
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> value into a relative year string such as "This year", "Next year", or "Last year".
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to evaluate.
    /// </param>
    /// <returns>
    /// A string representing the relative year name if the date falls in the current, next, or previous year;
    /// otherwise the year is returned (e.g. "2025").
    /// </returns>
    public static string ToRelativeYearString(this DateTime dateTime)
    {
        var today = DateTime.Today;
        var deltaYears = dateTime.Year - today.Year;

        return deltaYears switch
        {
            0 => "This year",
            1 => "Next year",
            -1 => "Last year",
            _ => dateTime.Year.ToString()
        };
    }

    /// <summary>
    /// Converts two <see cref="DateTime"/> values into a friendly date range string.
    /// </summary>
    /// <param name="startDate">
    /// The starting <see cref="DateTime"/> of the range.
    /// </param>
    /// <param name="endDate">
    /// The ending <see cref="DateTime"/> of the range.
    /// </param>
    /// <returns>
    /// A string representing the date range in a compact, human-readable format.
    /// Examples:
    /// - "Dec 15–17, 2025" (same month and year)
    /// - "Dec 30, 2025 – Jan 2, 2026" (different months/years)
    /// </returns>
    public static string ToFriendlyDateRangeString(DateTime startDate, DateTime endDate)
    {
        if (startDate.Year == endDate.Year)
        {
            if (startDate.Month == endDate.Month)
            {
                // Same month and year → compact range
                return $"{startDate:MMM d}–{endDate:d}, {startDate:yyyy}";
            }
            else
            {
                // Same year, different months
                return $"{startDate:MMM d} – {endDate:MMM d}, {startDate:yyyy}";
            }
        }
        else
        {
            // Different years
            return $"{startDate:MMM d, yyyy} – {endDate:MMM d, yyyy}";
        }
    }

    /// <summary>
    /// Converts two <see cref="DateTime"/> values into a friendly time range string.
    /// </summary>
    /// <param name="startTime">
    /// The starting <see cref="DateTime"/> of the range.
    /// </param>
    /// <param name="endTime">
    /// The ending <see cref="DateTime"/> of the range.
    /// </param>
    /// <returns>
    /// A string representing the time range in a compact, human-readable format.
    /// Examples:
    /// - "9:00 AM – 11:30 AM" (same day)
    /// - "9:00 PM Dec 15 – 1:00 AM Dec 16" (spanning midnight)
    /// </returns>
    public static string ToFriendlyTimeRangeString(DateTime startTime, DateTime endTime)
    {
        if (startTime.Date == endTime.Date)
        {
            // Same day → compact range
            return $"{startTime:h:mm tt} – {endTime:h:mm tt}";
        }
        else
        {
            // Spanning multiple days → include date context
            return $"{startTime:h:mm tt MMM d} – {endTime:h:mm tt MMM d}";
        }
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> value into an ordinal date string such as "15th December 2025".
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to format.
    /// </param>
    /// <returns>
    /// A string representing the date with an ordinal suffix for the day of the month,
    /// e.g. "1st January 2026", "2nd February 2026", "3rd March 2026", "15th December 2025".
    /// </returns>
    public static string ToOrdinalDateString(this DateTime dateTime)
    {
        int day = dateTime.Day;
        string suffix = day switch
        {
            1 or 21 or 31 => "st",
            2 or 22 => "nd",
            3 or 23 => "rd",
            _ => "th"
        };

        return $"{day}{suffix} {dateTime:MMMM yyyy}";
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> value into an ordinal date string such as "15th December 2025".
    /// Supports optional two-digit day formatting (e.g., "01st January 2026").
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to format.
    /// </param>
    /// <param name="twoDigitDateFormat">
    /// If true (default), the day will be formatted with two digits (e.g., "01st").
    /// If false, the day will be formatted without leading zeros (e.g., "1st").
    /// </param>
    /// <returns>
    /// A string representing the date with an ordinal suffix for the day of the month,
    /// e.g. "01st January 2026" or "1st January 2026".
    /// </returns>
    public static string ToOrdinalDateString(this DateTime dateTime, bool twoDigitDateFormat)
    {
        int day = dateTime.Day;
        string suffix = day switch
        {
            1 or 21 or 31 => "st",
            2 or 22 => "nd",
            3 or 23 => "rd",
            _ => "th"
        };

        string dayFormatted = twoDigitDateFormat
            ? day.ToString("00") + suffix
            : day + suffix;

        return $"{dayFormatted} {dateTime:MMMM yyyy}";
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> value into a weekday + ordinal date string 
    /// such as "Monday, 01st January 2026" or "Monday, 1st January 2026".
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to format.
    /// </param>
    /// <param name="twoDigitDateFormat">
    /// If true (default), the day will be formatted with two digits (e.g., "01st").
    /// If false, the day will be formatted without leading zeros (e.g., "1st").
    /// </param>
    /// <returns>
    /// A string representing the weekday and date with an ordinal suffix for the day of the month.
    /// </returns>
    public static string ToOrdinalWeekdayDateString(this DateTime dateTime, bool twoDigitDateFormat = true)
    {
        int day = dateTime.Day;
        string suffix = day switch
        {
            1 or 21 or 31 => "st",
            2 or 22 => "nd",
            3 or 23 => "rd",
            _ => "th"
        };

        string dayFormatted = twoDigitDateFormat
            ? day.ToString("00") + suffix
            : day + suffix;

        return $"{dateTime:dddd}, {dayFormatted} {dateTime:MMMM yyyy}";
    }

    /// <summary>
    /// Converts two <see cref="DateTime"/> values into a weekday + ordinal date range string.
    /// Examples:
    /// - "Monday, 15th December 2025 – Wednesday, 17th December 2025"
    /// - "Thursday, 01st January 2026 – Friday, 02nd January 2026"
    /// </summary>
    /// <param name="startDate">
    /// The starting <see cref="DateTime"/> of the range.
    /// </param>
    /// <param name="endDate">
    /// The ending <see cref="DateTime"/> of the range.
    /// </param>
    /// <param name="twoDigitDateFormat">
    /// If true (default), the day will be formatted with two digits (e.g., "01st").
    /// If false, the day will be formatted without leading zeros (e.g., "1st").
    /// </param>
    /// <returns>
    /// A string representing the weekday + ordinal date range.
    /// </returns>
    public static string ToOrdinalWeekdayDateRangeString(DateTime startDate, DateTime endDate, bool twoDigitDateFormat = true)
    {
        string FormatOrdinal(DateTime dt)
        {
            int day = dt.Day;
            string suffix = day switch
            {
                1 or 21 or 31 => "st",
                2 or 22 => "nd",
                3 or 23 => "rd",
                _ => "th"
            };

            string dayFormatted = twoDigitDateFormat
                ? day.ToString("00") + suffix
                : day + suffix;

            return $"{dt:dddd}, {dayFormatted} {dt:MMMM yyyy}";
        }

        return $"{FormatOrdinal(startDate)} – {FormatOrdinal(endDate)}";
    }

    /// <summary>
    /// Converts two <see cref="DateTime"/> values into a compact ordinal date + time range string.
    /// Examples:
    /// - "15th–17th Dec 2025, 9:00 AM – 5:00 PM"
    /// - "01st–02nd Jan 2026, 10:00 AM – 11:30 AM"
    /// </summary>
    /// <param name="startDateTime">
    /// The starting <see cref="DateTime"/> of the range.
    /// </param>
    /// <param name="endDateTime">
    /// The ending <see cref="DateTime"/> of the range.
    /// </param>
    /// <param name="twoDigitDateFormat">
    /// If true (default), the day will be formatted with two digits (e.g., "01st").
    /// If false, the day will be formatted without leading zeros (e.g., "1st").
    /// </param>
    /// <returns>
    /// A compact string representing the ordinal date + time range.
    /// </returns>
    public static string ToCompactOrdinalDateTimeRangeString(DateTime startDateTime, DateTime endDateTime, bool twoDigitDateFormat = true)
    {
        string GetOrdinalDay(int day)
        {
            string suffix = day switch
            {
                1 or 21 or 31 => "st",
                2 or 22 => "nd",
                3 or 23 => "rd",
                _ => "th"
            };
            return twoDigitDateFormat ? day.ToString("00") + suffix : day + suffix;
        }

        if (startDateTime.Month == endDateTime.Month && startDateTime.Year == endDateTime.Year)
        {
            // Same month/year → compact day range
            return $"{GetOrdinalDay(startDateTime.Day)}–{GetOrdinalDay(endDateTime.Day)} {startDateTime:MMM yyyy}, {startDateTime:h:mm tt} – {endDateTime:h:mm tt}";
        }
        else
        {
            // Different month/year → show both fully
            return $"{GetOrdinalDay(startDateTime.Day)} {startDateTime:MMM yyyy} {startDateTime:h:mm tt} – {GetOrdinalDay(endDateTime.Day)} {endDateTime:MMM yyyy} {endDateTime:h:mm tt}";
        }
    }

    /// <summary>
    /// Converts two <see cref="DateTime"/> values into a super-compact ordinal date + time range string.
    /// Examples:
    /// - "15th–17th Dec, 9–17h"
    /// - "01st–02nd Jan, 10–11h"
    /// </summary>
    /// <param name="startDateTime">
    /// The starting <see cref="DateTime"/> of the range.
    /// </param>
    /// <param name="endDateTime">
    /// The ending <see cref="DateTime"/> of the range.
    /// </param>
    /// <param name="twoDigitDateFormat">
    /// If true (default), the day will be formatted with two digits (e.g., "01st").
    /// If false, the day will be formatted without leading zeros (e.g., "1st").
    /// </param>
    /// <returns>
    /// A super-compact string representing the ordinal date + time range.
    /// </returns>
    public static string ToSuperCompactOrdinalDateTimeRangeString(DateTime startDateTime, DateTime endDateTime, bool twoDigitDateFormat = true)
    {
        string GetOrdinalDay(int day)
        {
            string suffix = day switch
            {
                1 or 21 or 31 => "st",
                2 or 22 => "nd",
                3 or 23 => "rd",
                _ => "th"
            };
            return twoDigitDateFormat ? day.ToString("00") + suffix : day + suffix;
        }

        if (startDateTime.Month == endDateTime.Month && startDateTime.Year == endDateTime.Year)
        {
            // Same month/year → compact day range
            return $"{GetOrdinalDay(startDateTime.Day)}–{GetOrdinalDay(endDateTime.Day)} {startDateTime:MMM}, {startDateTime:HH}–{endDateTime:HH}h";
        }
        else
        {
            // Different month/year → show both fully
            return $"{GetOrdinalDay(startDateTime.Day)} {startDateTime:MMM} {startDateTime:HH}h – {GetOrdinalDay(endDateTime.Day)} {endDateTime:MMM} {endDateTime:HH}h";
        }
    }
}