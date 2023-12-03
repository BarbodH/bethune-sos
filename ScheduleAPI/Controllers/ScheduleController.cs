using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ScheduleAPI.Data;
using ScheduleAPI.DTOs;
using ScheduleAPI.Utils;

namespace ScheduleAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly DataContextDapper _dapper;
    private readonly ScheduleUtils _utils;

    public ScheduleController(IConfiguration config)
    {
        _config = config;
        _dapper = new DataContextDapper(config);
        _utils = new ScheduleUtils(config);
    }

    [HttpPut("UpsertTutor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpsertTutor([FromBody] UpsertTutorDto upsertTutor)
    {
        var parameters = new DynamicParameters();
        parameters.Add("in_id", upsertTutor.Id, DbType.Int32, ParameterDirection.Input);
        parameters.Add("in_last_name", upsertTutor.LastName, DbType.String, ParameterDirection.Input);
        parameters.Add("in_first_name", upsertTutor.FirstName, DbType.String, ParameterDirection.Input);
        parameters.Add("in_email", upsertTutor.Email, DbType.String, ParameterDirection.Input);
        parameters.Add("num_rows_affected", null, DbType.Int32, ParameterDirection.Output);

        try
        {
            _dapper.Execute(
                sql: "schedule_tutoring.sp_upsert_tutor",
                param: parameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception error)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Query execution failed. Reason: {error.Message}");
        }

        IActionResult result = parameters.Get<int>("num_rows_affected") switch
        {
            > 1 => StatusCode(StatusCodes.Status500InternalServerError, "Multiple records were unexpectedly affected."),
            0 => StatusCode(StatusCodes.Status500InternalServerError, "No rows were affected."),
            < 0 => StatusCode(StatusCodes.Status500InternalServerError, "Unknown error occurred while updating."),
            _ => Ok() // 1
        };
        return result;
    }

    [HttpPut("UpdateTutorCourses")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateTutorCourses([FromBody] UpdateTutorCoursesDto updateTutorCourses)
    {
        try
        {
            if (!_utils.ValidateTutorId(updateTutorCourses.TutorId))
                return StatusCode(StatusCodes.Status404NotFound,
                    $"Unable to find tutor with ID {updateTutorCourses.TutorId}.");

            var parameters = new DynamicParameters();
            parameters.Add("in_tutor_id", updateTutorCourses.TutorId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("in_course_codes", updateTutorCourses.Courses);

            _dapper.Execute(
                sql: "schedule_tutoring.sp_update_tutor_courses",
                param: parameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception error)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Query execution failed. Reason: {error.Message}");
        }

        return Ok();
    }

    [HttpPut("UpdateTutorShifts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateTutorShifts([FromBody] UpdateTutorShiftsDto updateTutorShiftsDto)
    {
        try
        {
            if (!_utils.ValidateTutorId(updateTutorShiftsDto.TutorId))
                return StatusCode(StatusCodes.Status404NotFound,
                    $"Unable to find tutor with ID {updateTutorShiftsDto.TutorId}.");

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var shiftsJson = JsonConvert.SerializeObject(updateTutorShiftsDto.Shifts, settings);
            Console.WriteLine(shiftsJson);

            var parameters = new DynamicParameters();
            parameters.Add("in_tutor_id", updateTutorShiftsDto.TutorId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("in_shifts_json", shiftsJson, DbType.String, ParameterDirection.Input);

            _dapper.Execute(
                sql: "schedule_tutoring.sp_update_tutor_shifts",
                param: parameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (Exception error)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Query execution failed. Reason: {error.Message}");
        }

        return Ok();
    }

    [HttpDelete("DeleteTutor/{tutorId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult DeleteTutor(long tutorId)
    {
        if (!_utils.ValidateTutorId(tutorId))
            return StatusCode(StatusCodes.Status404NotFound,
                $"Unable to find tutor with ID {tutorId}.");

        var numRowsAffected = _dapper.Execute(
            sql: "DELETE FROM schedule_tutoring.tutors WHERE id = @id",
            param: new { id = tutorId });
        if (numRowsAffected == 0)
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Failed to remove the tutor.");

        return Ok();
    }
}