using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ScheduleAPI.Validations;

public partial class TimeString : ValidationAttribute
{
    public TimeString()
    {
        ErrorMessage = "The {0} field must be a string representing time in correct 'hh:mm:ss' format.";
    }

    public override bool IsValid(object? value)
    {
        return TimeStringRegex().IsMatch(value?.ToString() ?? string.Empty);
    }

    [GeneratedRegex("^([0-1]?[0-9]|2[0-3]):([0-5]?[0-9]):([0-5]?[0-9])$")]
    private static partial Regex TimeStringRegex();
}