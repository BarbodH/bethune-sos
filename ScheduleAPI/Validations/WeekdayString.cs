using System.ComponentModel.DataAnnotations;

namespace ScheduleAPI.Validations;

public class WeekdayString : ValidationAttribute
{
    public WeekdayString()
    {
        ErrorMessage = "The {0} field must be a string representing a valid weekday.";
    }

    public override bool IsValid(object? value)
    {
        string[] weekdays = { "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday" };
        return weekdays.Contains(value?.ToString()?.ToLower());
    }
}