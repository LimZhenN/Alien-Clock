using System;
using System.Linq;
using System.Threading;
using AlienClock;

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

class Program
{
    static AlienDateTime currentTime;
    static AlienDateTime? alarmTime;

    static void Main(string[] args)
    {
        currentTime = new AlienDateTime(2804, 18, 31, 2, 2, 88);
        
        while (true)
        {
            Console.Clear();
            DisplayClock();
            
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.S)
                    SetTime();
                else if (key == ConsoleKey.A)
                    SetAlarm();
                else if (key == ConsoleKey.Q)
                    break;
            }

            Thread.Sleep(500); // 1 alien second = 0.5 earth seconds
            currentTime.AddSecond();
            CheckAlarm();
        }
    }

    static void DisplayClock()
    {
        Console.WriteLine("╔════════════ ALIEN CLOCK ════════════╗");
        Console.WriteLine($"║ Year: {currentTime.Year}                          ║");
        Console.WriteLine($"║ Month: {currentTime.Month}/18                        ║");
        Console.WriteLine($"║ Day: {currentTime.Day}                             ║");
        Console.WriteLine($"║ Time: {currentTime.Hour:D2}:{currentTime.Minute:D2}:{currentTime.Second:D2}                      ║");
        Console.WriteLine("╚═════════════════════════════════════╝");
        
        var earthTime = currentTime.ToEarthTime();
        Console.WriteLine("\nEarth Time:");
        Console.WriteLine(earthTime.ToString("yyyy-MM-dd HH:mm:ss"));
        
        Console.WriteLine("\nPress 'S' to set time");
        Console.WriteLine("Press 'A' to set alarm");
        Console.WriteLine("Press 'Q' to quit");
    }

    static void SetTime()
    {
        try
        {
            Console.Clear();
            Console.Write("Enter Year: ");
            int year = int.Parse(Console.ReadLine());
            Console.Write("Enter Month (1-18): ");
            int month = int.Parse(Console.ReadLine());
            Console.Write("Enter Day: ");
            int day = int.Parse(Console.ReadLine());
            Console.Write("Enter Hour (0-35): ");
            int hour = int.Parse(Console.ReadLine());
            Console.Write("Enter Minute (0-89): ");
            int minute = int.Parse(Console.ReadLine());
            Console.Write("Enter Second (0-89): ");
            int second = int.Parse(Console.ReadLine());

            currentTime = new AlienDateTime(year, month, day, hour, minute, second);
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid input format! Please enter numeric values. Press any key to continue...");
            Console.ReadKey();
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"{ex.Message} Press any key to continue...");
            Console.ReadKey();
        }
    }

    static void SetAlarm()
    {
        try
        {
            Console.Clear();
            Console.Write("Enter alien minutes from now for alarm: ");
            int minutes = int.Parse(Console.ReadLine());
        
            // Create a new AlienDateTime by copying current time
            alarmTime = new AlienDateTime(
                currentTime.Year,
                currentTime.Month,
                currentTime.Day,
                currentTime.Hour,
                currentTime.Minute + minutes,  // Add the minutes
                currentTime.Second
            );
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid input format! Please enter a numeric value. Press any key to continue...");
            Console.ReadKey();
        }
        catch (ArgumentException)
        {
            Console.WriteLine("Invalid alarm time! Press any key to continue...");
            Console.ReadKey();
        }
    }

    static void CheckAlarm()
    {
        if (alarmTime == null) return; // Check if alarmTime is null

        if (currentTime.GetTotalSeconds() >= alarmTime.GetTotalSeconds()) // Directly use alarmTime
        {
            Console.Beep();
            alarmTime = null;
        }
    }
}
