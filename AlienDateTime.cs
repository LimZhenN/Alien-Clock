namespace AlienClock{
    public class AlienDateTime
{
    private static readonly int[] DaysInMonth = new int[]
    {
        44, 42, 48, 40, 48, 44, 40, 44, 42,
        40, 40, 42, 44, 48, 42, 40, 44, 38
    };

    public int Year { get; private set; }
    public int Month { get; private set; }
    public int Day { get; private set; }
    public int Hour { get; private set; }
    public int Minute { get; private set; }
    public int Second { get; private set; }

    // Reference point
    private static readonly DateTime EarthEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly AlienDateTime AlienEpoch = new AlienDateTime(2804, 18, 31, 2, 2, 88);

    public AlienDateTime(int year, int month, int day, int hour, int minute, int second)
    {
        if (!IsValid(year, month, day, hour, minute, second))
            throw new ArgumentException("Invalid alien date/time values");

        Year = year;
        Month = month;
        Day = day;
        Hour = hour;
        Minute = minute;
        Second = second;
    }

    public static bool IsValid(int year, int month, int day, int hour, int minute, int second)
    {
        if (month < 1 || month > 18) return false;
        if (day < 1 || day > DaysInMonth[month - 1]) return false;
        if (hour < 0 || hour >= 36) return false;
        if (minute < 0 || minute >= 90) return false;
        if (second < 0 || second >= 90) return false;
        return true;
    }

    public void AddSecond()
    {
        Second++;
        if (Second >= 90)
        {
            Second = 0;
            Minute++;
            if (Minute >= 90)
            {
                Minute = 0;
                Hour++;
                if (Hour >= 36)
                {
                    Hour = 0;
                    Day++;
                    if (Day > DaysInMonth[Month - 1])
                    {
                        Day = 1;
                        Month++;
                        if (Month > 18)
                        {
                            Month = 1;
                            Year++;
                        }
                    }
                }
            }
        }
    }

    public DateTime ToEarthTime()
    {
        // Calculate total alien seconds from epoch
        long alienSeconds = GetTotalSeconds() - AlienEpoch.GetTotalSeconds();
        // Convert to earth seconds (1 alien second = 0.5 earth seconds)
        double earthSeconds = alienSeconds * 0.5;
        return EarthEpoch.AddSeconds(earthSeconds);
    }

    public long GetTotalSeconds()
    {
        // Implementation to calculate total seconds from year 0
        // This is a simplified version - you'll need to implement the full calculation
        return Second + (Minute * 90L) + (Hour * 90L * 90L) + 
               (Day * 36L * 90L * 90L) + GetTotalDaysForMonths();
    }

    private long GetTotalDaysForMonths()
    {
        long days = 0;
        for (int y = 0; y < Year; y++)
        {
            days += DaysInMonth.Sum();
        }
        for (int m = 0; m < Month - 1; m++)
        {
            days += DaysInMonth[m];
        }
        return days;
    }
} 
}
