using System.Globalization;
using ScheduleAPI.Validations;

namespace ScheduleAPI.Models;

public class ShiftTime
{
    private string _day = "";

    [WeekdayString]
    public string Day
    {
        get => _day;
        set => _day = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
    }

    [TimeString] public string StartTime { get; set; } = "";
}