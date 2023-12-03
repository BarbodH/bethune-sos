using ScheduleAPI.Validations;

namespace ScheduleAPI.DTOs;

public class UpdateTutorCoursesDto
{
    [StudentId] public long TutorId { get; set; }
    public ICollection<string> Courses { get; set; } = new List<string>();
}