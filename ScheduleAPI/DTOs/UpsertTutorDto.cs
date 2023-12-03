using System.ComponentModel.DataAnnotations;
using ScheduleAPI.Validations;

namespace ScheduleAPI.DTOs;

public class UpsertTutorDto
{
    [Required] [StudentId] public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; set; } = "";

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; set; } = "";

    [Required] [EmailAddress] public string Email { get; set; } = "";
}