using System.ComponentModel.DataAnnotations;

namespace ScheduleAPI.Validations;

public class StudentId : ValidationAttribute
{
    public StudentId()
    {
        ErrorMessage = "The {0} field must be a 9-digit integer.";
    }

    public override bool IsValid(object? value)
    {
        return value?.ToString()?.Length == 9 && long.TryParse(value.ToString(), out _);
    }
}