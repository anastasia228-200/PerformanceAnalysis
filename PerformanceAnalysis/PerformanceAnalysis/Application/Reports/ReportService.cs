using PerformanceAnalysis.Infrastructure.Reports;
using PerformanceAnalysis.Reports.Filters;
using PerformanceAnalysis.Reports.Models;

namespace PerformanceAnalysis.Application.Reports;

/// <inheritdoc />
/// <remarks>
/// Урок 49: ReportService переведён на IDapperExecutor вместо IDbConnection,
/// чтобы Dapper-вызовы можно было мокать в unit-тестах.
/// </remarks>
public class ReportService : IReportService
{
    private readonly IDapperExecutor _dapper;

    public ReportService(IDapperExecutor dapper) => _dapper = dapper;

    public async Task<IEnumerable<GroupLeadersAndLaggardsItem>> GetGroupLeadersAndLaggardsAsync(
        GroupLeadersAndLaggardsFilter filter) =>
        await _dapper.QueryAsync<GroupLeadersAndLaggardsItem>(ReportQueries.GroupLeadersAndLaggards, filter);

    public async Task<IEnumerable<StudentTestResultsItem>> GetStudentTestResultsAsync(
        StudentTestResultsFilter filter) =>
        await _dapper.QueryAsync<StudentTestResultsItem>(ReportQueries.StudentTestResults, filter);

    public async Task<IEnumerable<GroupTrendItem>> GetGroupTrendAsync(GroupTrendFilter filter) =>
        await _dapper.QueryAsync<GroupTrendItem>(ReportQueries.GroupTrend,
            new { GroupIds = filter.GroupIds?.ToArray(), filter.DateFrom, filter.DateTo });

    public async Task<IEnumerable<StudentRatingItem>> GetStudentRatingAsync(StudentRatingFilter filter) =>
        await _dapper.QueryAsync<StudentRatingItem>(ReportQueries.StudentRating, filter);

    public async Task<IEnumerable<StudentMonthlyProgressItem>> GetStudentMonthlyProgressAsync(
        StudentMonthlyProgressFilter filter) =>
        await _dapper.QueryAsync<StudentMonthlyProgressItem>(ReportQueries.StudentMonthlyProgress, filter);

    public async Task<IEnumerable<StudentPassRateItem>> GetStudentPassRateAsync(StudentPassRateFilter filter) =>
        await _dapper.QueryAsync<StudentPassRateItem>(ReportQueries.StudentPassRate, filter);

    public async Task<StudentPassRateSummaryItem?> GetStudentPassRateSummaryAsync(
        StudentPassRateSummaryFilter filter) =>
        await _dapper.QueryFirstOrDefaultAsync<StudentPassRateSummaryItem>(
            ReportQueries.StudentPassRateSummary, filter);

    public async Task<IEnumerable<DayOfWeekActivityItem>> GetDayOfWeekActivityAsync(
        DayOfWeekActivityFilter filter) =>
        await _dapper.QueryAsync<DayOfWeekActivityItem>(ReportQueries.DayOfWeekActivity, filter);
}
