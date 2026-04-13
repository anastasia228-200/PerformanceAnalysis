using Moq;
using PerformanceAnalysis.Application.Reports;
using PerformanceAnalysis.Infrastructure.Reports;
using PerformanceAnalysis.Reports.Filters;
using PerformanceAnalysis.Reports.Models;

namespace PerformanceAnalysis.Tests;

/// <summary>
/// Урок 49: Unit-тесты для ReportService.
/// ReportService — тонкая обёртка над IDapperExecutor.
/// Тесты проверяют, что каждый метод сервиса:
///   - вызывает QueryAsync или QueryFirstOrDefaultAsync с правильным SQL
///   - передаёт правильные параметры
///   - возвращает результат без изменений
/// </summary>
public class ReportServiceTests
{
    // ==================== GetGroupLeadersAndLaggardsAsync ====================

    [Fact]
    public async Task GetGroupLeadersAndLaggards_CallsQueryAsync_WithCorrectSqlAndParams()
    {
        // Arrange
        var expected = new List<GroupLeadersAndLaggardsItem>
        {
            new() { GroupId = 1, GroupName = "Группа 1", LeaderName = "Иванов", LeaderScore = 100 }
        };
        var mockDapper = new Mock<IDapperExecutor>();
        mockDapper
            .Setup(x => x.QueryAsync<GroupLeadersAndLaggardsItem>(It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(expected);

        var filter = new GroupLeadersAndLaggardsFilter { DirectionId = 5, CourseId = 10 };
        var service = new ReportService(mockDapper.Object);

        // Act
        var result = await service.GetGroupLeadersAndLaggardsAsync(filter);

        // Assert
        Assert.Equal(expected, result);
        mockDapper.Verify(x => x.QueryAsync<GroupLeadersAndLaggardsItem>(
            It.Is<string>(sql => sql.Contains("StudentScores")),
            It.IsAny<object>()));
        mockDapper.Verify(x => x.QueryAsync<GroupLeadersAndLaggardsItem>(
            It.IsAny<string>(),
            It.Is<GroupLeadersAndLaggardsFilter>(f => f.DirectionId == 5 && f.CourseId == 10)));
    }

    // ==================== GetStudentTestResultsAsync ====================

    [Fact]
    public async Task GetStudentTestResults_CallsQueryAsync_WithCorrectSqlAndParams()
    {
        // Arrange
        var expected = new List<StudentTestResultsItem>
        {
            new() { TestId = 1, TestTitle = "Тест 1", BestScore = 90, AttemptsCount = 2 }
        };
        var mockDapper = new Mock<IDapperExecutor>();
        mockDapper
            .Setup(x => x.QueryAsync<StudentTestResultsItem>(It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(expected);

        var filter = new StudentTestResultsFilter { StudentId = 42 };
        var service = new ReportService(mockDapper.Object);

        // Act
        var result = await service.GetStudentTestResultsAsync(filter);

        // Assert
        Assert.Equal(expected, result);
        mockDapper.Verify(x => x.QueryAsync<StudentTestResultsItem>(
            It.Is<string>(sql => sql.Contains("BestAttempt")),
            It.IsAny<object>()));
        mockDapper.Verify(x => x.QueryAsync<StudentTestResultsItem>(
            It.IsAny<string>(),
            It.Is<StudentTestResultsFilter>(f => f.StudentId == 42)));
    }

    // ==================== GetGroupTrendAsync ====================

    [Fact]
    public async Task GetGroupTrend_CallsQueryAsync_WithCorrectSqlAndParams()
    {
        // Arrange
        var expected = new List<GroupTrendItem>
        {
            new() { GroupId = 1, GroupName = "Группа А", Month = new DateTime(2024, 3, 1), MonthLabel = "Mar 2024", AverageScore = 75.5m, AttemptsCount = 10 }
        };
        var mockDapper = new Mock<IDapperExecutor>();
        mockDapper
            .Setup(x => x.QueryAsync<GroupTrendItem>(It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(expected);

        var filter = new GroupTrendFilter
        {
            GroupIds = new List<int> { 1, 2 },
            DateFrom = new DateTime(2024, 1, 1),
            DateTo = new DateTime(2024, 12, 31)
        };
        var service = new ReportService(mockDapper.Object);

        // Act
        var result = await service.GetGroupTrendAsync(filter);

        // Assert
        Assert.Equal(expected, result);
        mockDapper.Verify(x => x.QueryAsync<GroupTrendItem>(
            It.Is<string>(sql => sql.Contains("DATE_TRUNC")),
            It.IsAny<object>()));
        mockDapper.Verify(x => x.QueryAsync<GroupTrendItem>(
            It.IsAny<string>(),
            It.Is<object>(p =>
                (DateTime?)p.GetType().GetProperty("DateFrom")!.GetValue(p) == new DateTime(2024, 1, 1)
                && (DateTime?)p.GetType().GetProperty("DateTo")!.GetValue(p) == new DateTime(2024, 12, 31))));
    }

    // ==================== GetStudentRatingAsync ====================

    [Fact]
    public async Task GetStudentRating_CallsQueryAsync_WithCorrectSqlAndParams()
    {
        // Arrange
        var expected = new List<StudentRatingItem>
        {
            new() { Rank = 1, FullName = "Иванов", TotalScore = 200 }
        };
        var mockDapper = new Mock<IDapperExecutor>();
        mockDapper
            .Setup(x => x.QueryAsync<StudentRatingItem>(It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(expected);

        var filter = new StudentRatingFilter { DirectionId = 3, CourseId = 7, GroupId = 12 };
        var service = new ReportService(mockDapper.Object);

        // Act
        var result = await service.GetStudentRatingAsync(filter);

        // Assert
        Assert.Equal(expected, result);
        mockDapper.Verify(x => x.QueryAsync<StudentRatingItem>(
            It.Is<string>(sql => sql.Contains("ROW_NUMBER")),
            It.IsAny<object>()));
        mockDapper.Verify(x => x.QueryAsync<StudentRatingItem>(
            It.IsAny<string>(),
            It.Is<StudentRatingFilter>(f => f.DirectionId == 3 && f.CourseId == 7 && f.GroupId == 12)));
    }

    // ==================== GetStudentMonthlyProgressAsync ====================

    [Fact]
    public async Task GetStudentMonthlyProgress_CallsQueryAsync_WithCorrectSqlAndParams()
    {
        // Arrange
        var expected = new List<StudentMonthlyProgressItem>
        {
            new() { Month = new DateTime(2024, 3, 1), MonthLabel = "Mar 2024", Score = 50, CumulativeScore = 150 }
        };
        var mockDapper = new Mock<IDapperExecutor>();
        mockDapper
            .Setup(x => x.QueryAsync<StudentMonthlyProgressItem>(It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(expected);

        var filter = new StudentMonthlyProgressFilter { StudentId = 99 };
        var service = new ReportService(mockDapper.Object);

        // Act
        var result = await service.GetStudentMonthlyProgressAsync(filter);

        // Assert
        Assert.Equal(expected, result);
        mockDapper.Verify(x => x.QueryAsync<StudentMonthlyProgressItem>(
            It.Is<string>(sql => sql.Contains("MonthlyScores")),
            It.IsAny<object>()));
        mockDapper.Verify(x => x.QueryAsync<StudentMonthlyProgressItem>(
            It.IsAny<string>(),
            It.Is<StudentMonthlyProgressFilter>(f => f.StudentId == 99)));
    }

    // ==================== GetStudentPassRateAsync ====================

    [Fact]
    public async Task GetStudentPassRate_CallsQueryAsync_WithCorrectSqlAndParams()
    {
        // Arrange
        var expected = new List<StudentPassRateItem>
        {
            new() { StudentId = 1, FullName = "Сидоров", TestsAvailable = 10, TestsPassed = 8, PassRate = 80m }
        };
        var mockDapper = new Mock<IDapperExecutor>();
        mockDapper
            .Setup(x => x.QueryAsync<StudentPassRateItem>(It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(expected);

        var filter = new StudentPassRateFilter { GroupId = 4 };
        var service = new ReportService(mockDapper.Object);

        // Act
        var result = await service.GetStudentPassRateAsync(filter);

        // Assert
        Assert.Equal(expected, result);
        mockDapper.Verify(x => x.QueryAsync<StudentPassRateItem>(
            It.Is<string>(sql => sql.Contains("testsavailable")),
            It.IsAny<object>()));
        mockDapper.Verify(x => x.QueryAsync<StudentPassRateItem>(
            It.IsAny<string>(),
            It.Is<StudentPassRateFilter>(f => f.GroupId == 4)));
    }

    // ==================== GetStudentPassRateSummaryAsync ====================

    [Fact]
    public async Task GetStudentPassRateSummary_CallsQueryFirstOrDefaultAsync_WithCorrectSql()
    {
        // Arrange
        var expected = new StudentPassRateSummaryItem
        {
            StudentId = 3, FullName = "Кузнецов", TestsAttempted = 10, TestsPassed = 7,
            PassRate = 70m, TotalScore = 700, AverageScore = 70m
        };
        var mockDapper = new Mock<IDapperExecutor>();
        mockDapper
            .Setup(x => x.QueryFirstOrDefaultAsync<StudentPassRateSummaryItem>(It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(expected);

        var filter = new StudentPassRateSummaryFilter { StudentId = 3 };
        var service = new ReportService(mockDapper.Object);

        // Act
        var result = await service.GetStudentPassRateSummaryAsync(filter);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Кузнецов", result.FullName);
        mockDapper.Verify(x => x.QueryFirstOrDefaultAsync<StudentPassRateSummaryItem>(
            It.Is<string>(sql => sql.Contains("testsattempted")),
            It.IsAny<object>()));
        mockDapper.Verify(x => x.QueryFirstOrDefaultAsync<StudentPassRateSummaryItem>(
            It.IsAny<string>(),
            It.Is<StudentPassRateSummaryFilter>(f => f.StudentId == 3)));
    }

    // ==================== GetDayOfWeekActivityAsync ====================

    [Fact]
    public async Task GetDayOfWeekActivity_CallsQueryAsync_WithCorrectSqlAndParams()
    {
        // Arrange
        var expected = new List<DayOfWeekActivityItem>
        {
            new() { DayOfWeek = 1, TestsCompleted = 15, UniqueStudents = 5 }
        };
        var mockDapper = new Mock<IDapperExecutor>();
        mockDapper
            .Setup(x => x.QueryAsync<DayOfWeekActivityItem>(It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(expected);

        var filter = new DayOfWeekActivityFilter
        {
            DateFrom = new DateTime(2024, 1, 1),
            DateTo = new DateTime(2024, 12, 31),
            GroupId = 2
        };
        var service = new ReportService(mockDapper.Object);

        // Act
        var result = await service.GetDayOfWeekActivityAsync(filter);

        // Assert
        Assert.Equal(expected, result);
        mockDapper.Verify(x => x.QueryAsync<DayOfWeekActivityItem>(
            It.Is<string>(sql => sql.Contains("EXTRACT")),
            It.IsAny<object>()));
        mockDapper.Verify(x => x.QueryAsync<DayOfWeekActivityItem>(
            It.IsAny<string>(),
            It.Is<DayOfWeekActivityFilter>(f => f.GroupId == 2 && f.DateFrom == new DateTime(2024, 1, 1))));
    }

    // ==================== Пустые результаты ====================

    [Fact]
    public async Task GetGroupLeadersAndLaggards_ReturnsEmpty_WhenDapperReturnsEmpty()
    {
        var mockDapper = new Mock<IDapperExecutor>();
        mockDapper
            .Setup(x => x.QueryAsync<GroupLeadersAndLaggardsItem>(It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(new List<GroupLeadersAndLaggardsItem>());

        var result = await new ReportService(mockDapper.Object)
            .GetGroupLeadersAndLaggardsAsync(new GroupLeadersAndLaggardsFilter());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetStudentRating_ReturnsEmpty_WhenDapperReturnsEmpty()
    {
        var mockDapper = new Mock<IDapperExecutor>();
        mockDapper
            .Setup(x => x.QueryAsync<StudentRatingItem>(It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(new List<StudentRatingItem>());

        var result = await new ReportService(mockDapper.Object)
            .GetStudentRatingAsync(new StudentRatingFilter());

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGroupTrend_ReturnsEmpty_WhenDapperReturnsEmpty()
    {
        var mockDapper = new Mock<IDapperExecutor>();
        mockDapper
            .Setup(x => x.QueryAsync<GroupTrendItem>(It.IsAny<string>(), It.IsAny<object?>()))
            .ReturnsAsync(new List<GroupTrendItem>());

        var result = await new ReportService(mockDapper.Object)
            .GetGroupTrendAsync(new GroupTrendFilter());

        Assert.Empty(result);
    }
}
