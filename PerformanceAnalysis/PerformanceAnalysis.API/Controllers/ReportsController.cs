using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerformanceAnalysis.Domain.Entities;
using PerformanceAnalysis.Domain.Interfaces;
using PerformanceAnalysis.Infrastructure.Auth.Extensions;

namespace PerformanceAnalysis.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService) => _reportService = reportService;

    [HttpGet("group-leaders")]
    public async Task<IActionResult> GetGroupLeaders([FromQuery] GroupLeadersAndLaggardsFilter filter) =>
        Ok(await _reportService.GetGroupLeadersAndLaggardsAsync(filter));

    [HttpGet("student-test-results")]
    public async Task<IActionResult> GetStudentTestResults([FromQuery] StudentTestResultsFilter filter)
    {
        if (!ValidateStudentAccess(filter.StudentId)) return Forbid();
        if (HttpContext.IsStudent() && filter.StudentId == null)
            filter.StudentId = HttpContext.GetStudentId();
        return Ok(await _reportService.GetStudentTestResultsAsync(filter));
    }

    [HttpGet("group-trend")]
    public async Task<IActionResult> GetGroupTrend([FromQuery] GroupTrendFilter filter) =>
        Ok(await _reportService.GetGroupTrendAsync(filter));

    [HttpGet("student-monthly-progress")]
    public async Task<IActionResult> GetStudentMonthlyProgress([FromQuery] StudentMonthlyProgressFilter filter)
    {
        if (!ValidateStudentAccess(filter.StudentId)) return Forbid();
        if (HttpContext.IsStudent() && filter.StudentId == null)
            filter.StudentId = HttpContext.GetStudentId();
        return Ok(await _reportService.GetStudentMonthlyProgressAsync(filter));
    }

    [HttpGet("student-rating")]
    public async Task<IActionResult> GetStudentRating([FromQuery] StudentRatingFilter filter) =>
        Ok(await _reportService.GetStudentRatingAsync(filter));

    [HttpGet("student-pass-rate")]
    public async Task<IActionResult> GetStudentPassRate([FromQuery] StudentPassRateFilter filter) =>
        Ok(await _reportService.GetStudentPassRateAsync(filter));

    [HttpGet("student-pass-rate-summary")]
    public async Task<IActionResult> GetStudentPassRateSummary([FromQuery] StudentPassRateSummaryFilter filter)
    {
        if (!ValidateStudentAccess(filter.StudentId)) return Forbid();
        if (HttpContext.IsStudent() && filter.StudentId == null)
            filter.StudentId = HttpContext.GetStudentId();
        var result = await _reportService.GetStudentPassRateSummaryAsync(filter);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("day-of-week-activity")]
    public async Task<IActionResult> GetDayOfWeekActivity([FromQuery] DayOfWeekActivityFilter filter) =>
        Ok(await _reportService.GetDayOfWeekActivityAsync(filter));

    private bool ValidateStudentAccess(int? studentId)
    {
        if (HttpContext.IsManager()) return true;
        if (HttpContext.IsStudent())
        {
            var currentId = HttpContext.GetStudentId();
            return studentId == null ? currentId != null : currentId == studentId;
        }
        return false;
    }
}
