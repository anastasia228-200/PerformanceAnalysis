using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerformanceAnalysis.Application.Reports;
using PerformanceAnalysis.Infrastructure.Auth.Extensions;
using PerformanceAnalysis.Reports.Filters;

namespace PerformanceAnalysis.Controllers;

/// <summary>Аналитические отчёты по успеваемости студентов.</summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService) => _reportService = reportService;

    /// <summary>Лидеры и отстающие по каждой группе. Доступно менеджерам и студентам.</summary>
    [HttpGet("group-leaders")]
    public async Task<IActionResult> GetGroupLeaders([FromQuery] GroupLeadersAndLaggardsFilter filter) =>
        Ok(await _reportService.GetGroupLeadersAndLaggardsAsync(filter));

    /// <summary>
    /// Результаты тестов студента.
    /// Студенты могут смотреть только свои результаты; менеджеры — любые.
    /// </summary>
    [HttpGet("student-test-results")]
    public async Task<IActionResult> GetStudentTestResults([FromQuery] StudentTestResultsFilter filter)
    {
        if (!ValidateStudentAccess(filter.StudentId))
            return Forbid();

        if (HttpContext.IsStudent() && filter.StudentId == null)
            filter.StudentId = HttpContext.GetStudentId();

        return Ok(await _reportService.GetStudentTestResultsAsync(filter));
    }

    /// <summary>Динамика среднего балла по группам. Доступно менеджерам и студентам.</summary>
    [HttpGet("group-trend")]
    public async Task<IActionResult> GetGroupTrend([FromQuery] GroupTrendFilter filter) =>
        Ok(await _reportService.GetGroupTrendAsync(filter));

    /// <summary>
    /// Ежемесячный прогресс студента.
    /// Студенты могут смотреть только свои данные; менеджеры — любые.
    /// </summary>
    [HttpGet("student-monthly-progress")]
    public async Task<IActionResult> GetStudentMonthlyProgress([FromQuery] StudentMonthlyProgressFilter filter)
    {
        if (!ValidateStudentAccess(filter.StudentId))
            return Forbid();

        if (HttpContext.IsStudent() && filter.StudentId == null)
            filter.StudentId = HttpContext.GetStudentId();

        return Ok(await _reportService.GetStudentMonthlyProgressAsync(filter));
    }

    /// <summary>Общий рейтинг студентов. Доступно менеджерам и студентам.</summary>
    [HttpGet("student-rating")]
    public async Task<IActionResult> GetStudentRating([FromQuery] StudentRatingFilter filter) =>
        Ok(await _reportService.GetStudentRatingAsync(filter));

    /// <summary>Процент пройденных тестов по студентам группы. Доступно менеджерам и студентам.</summary>
    [HttpGet("student-pass-rate")]
    public async Task<IActionResult> GetStudentPassRate([FromQuery] StudentPassRateFilter filter) =>
        Ok(await _reportService.GetStudentPassRateAsync(filter));

    /// <summary>
    /// Сводка по прохождению тестов конкретного студента.
    /// Студенты могут смотреть только свои данные; менеджеры — любые.
    /// </summary>
    [HttpGet("student-pass-rate-summary")]
    public async Task<IActionResult> GetStudentPassRateSummary([FromQuery] StudentPassRateSummaryFilter filter)
    {
        if (!ValidateStudentAccess(filter.StudentId))
            return Forbid();

        if (HttpContext.IsStudent() && filter.StudentId == null)
            filter.StudentId = HttpContext.GetStudentId();

        var result = await _reportService.GetStudentPassRateSummaryAsync(filter);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Активность студентов по дням недели. Доступно менеджерам и студентам.</summary>
    [HttpGet("day-of-week-activity")]
    public async Task<IActionResult> GetDayOfWeekActivity([FromQuery] DayOfWeekActivityFilter filter) =>
        Ok(await _reportService.GetDayOfWeekActivityAsync(filter));

    /// <summary>
    /// Проверка доступа к отчётам с фильтром по StudentId.
    /// Менеджер — видит все данные.
    /// Студент — только свои (studentId == null → подставляется из токена; чужой studentId → 403).
    /// </summary>
    private bool ValidateStudentAccess(int? studentId)
    {
        if (HttpContext.IsManager())
            return true;

        if (HttpContext.IsStudent())
        {
            var currentStudentId = HttpContext.GetStudentId();

            // StudentId не указан — разрешаем (будет подставлен из claims)
            if (studentId == null)
                return currentStudentId != null;

            // StudentId указан — разрешаем только если совпадает с текущим
            return currentStudentId == studentId;
        }

        return false;
    }
}
