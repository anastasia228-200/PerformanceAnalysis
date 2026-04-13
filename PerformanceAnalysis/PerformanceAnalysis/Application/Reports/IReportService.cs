using PerformanceAnalysis.Reports.Filters;
using PerformanceAnalysis.Reports.Models;

namespace PerformanceAnalysis.Application.Reports;

/// <summary>Сервис построения аналитических отчётов по успеваемости.</summary>
public interface IReportService
{
    Task<IEnumerable<GroupLeadersAndLaggardsItem>> GetGroupLeadersAndLaggardsAsync(GroupLeadersAndLaggardsFilter filter);
    Task<IEnumerable<StudentTestResultsItem>> GetStudentTestResultsAsync(StudentTestResultsFilter filter);
    Task<IEnumerable<GroupTrendItem>> GetGroupTrendAsync(GroupTrendFilter filter);
    Task<IEnumerable<StudentRatingItem>> GetStudentRatingAsync(StudentRatingFilter filter);
    Task<IEnumerable<StudentMonthlyProgressItem>> GetStudentMonthlyProgressAsync(StudentMonthlyProgressFilter filter);
    Task<IEnumerable<StudentPassRateItem>> GetStudentPassRateAsync(StudentPassRateFilter filter);
    Task<StudentPassRateSummaryItem?> GetStudentPassRateSummaryAsync(StudentPassRateSummaryFilter filter);
    Task<IEnumerable<DayOfWeekActivityItem>> GetDayOfWeekActivityAsync(DayOfWeekActivityFilter filter);
}
