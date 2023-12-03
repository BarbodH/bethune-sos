using Microsoft.AspNetCore.Mvc;
using ScheduleAPI.Data;

namespace ScheduleAPI.Utils;

public class ScheduleUtils : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly DataContextDapper _dapper;

    public ScheduleUtils(IConfiguration config)
    {
        _config = config;
        _dapper = new DataContextDapper(config);
    }
    
    public bool ValidateTutorId(long tutorId)
    {
        var result = _dapper.Query<int>(
            sql: "SELECT id FROM schedule_tutoring.tutors WHERE id = @id",
            param: new { id = tutorId });
        return result.Any();
    }
}