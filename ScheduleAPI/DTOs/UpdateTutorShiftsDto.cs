using ScheduleAPI.Models;
using ScheduleAPI.Validations;

namespace ScheduleAPI.DTOs;

public class UpdateTutorShiftsDto
{
    [StudentId] public long TutorId { get; set; }
    public ICollection<ShiftTime> Shifts { get; set; } = new List<ShiftTime>();
}