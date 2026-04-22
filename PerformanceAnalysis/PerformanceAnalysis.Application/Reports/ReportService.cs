using System.Data;
using Dapper;
using PerformanceAnalysis.Domain.Entities;
using PerformanceAnalysis.Domain.Interfaces;
using PerformanceAnalysis.Infrastructure.Reports;

namespace PerformanceAnalysis.Application.Reports;

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

    public async Task<IEnumerable<GroupTrendItem>> GetGroupTrendAsync(GroupTrendFilter filter)
    {
        var p = new DynamicParameters();
        p.Add("GroupIds", filter.GroupIds?.ToArray(), DbType.Object);
        p.Add("DateFrom", filter.DateFrom, DbType.DateTime);
        p.Add("DateTo", filter.DateTo, DbType.DateTime);
        return await _dapper.QueryAsync<GroupTrendItem>(ReportQueries.GroupTrend, p);
    }

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
        DayOfWeekActivityFilter filter)
    {
        var p = new DynamicParameters();
        p.Add("DateFrom", filter.DateFrom, DbType.DateTime);
        p.Add("DateTo", filter.DateTo, DbType.DateTime);
        p.Add("GroupId", filter.GroupId, DbType.Int32);
        return await _dapper.QueryAsync<DayOfWeekActivityItem>(ReportQueries.DayOfWeekActivity, p);
    }
}