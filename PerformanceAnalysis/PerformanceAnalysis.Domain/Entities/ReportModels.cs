namespace PerformanceAnalysis.Domain.Entities;

public class DayOfWeekActivityItem
{
    public int DayOfWeek { get; set; }
    public int TestsCompleted { get; set; }
    public int UniqueStudents { get; set; }
}

public class GroupLeadersAndLaggardsItem
{
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string LeaderName { get; set; } = string.Empty;
    public int LeaderScore { get; set; }
    public string LaggardName { get; set; } = string.Empty;
    public int LaggardScore { get; set; }
}

public class GroupTrendItem
{
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public DateTime Month { get; set; }
    public string MonthLabel { get; set; } = string.Empty;
    public decimal AverageScore { get; set; }
    public int AttemptsCount { get; set; }
}

public class StudentMonthlyProgressItem
{
    public DateTime Month { get; set; }
    public string MonthLabel { get; set; } = string.Empty;
    public int Score { get; set; }
    public int CumulativeScore { get; set; }
}

public class StudentPassRateItem
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public int TestsAvailable { get; set; }
    public int TestsPassed { get; set; }
    public decimal PassRate { get; set; }
}

public class StudentPassRateSummaryItem
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int TestsAttempted { get; set; }
    public int TestsPassed { get; set; }
    public decimal PassRate { get; set; }
    public int TotalScore { get; set; }
    public decimal AverageScore { get; set; }
}

public class StudentRatingItem
{
    public int Rank { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public int TotalScore { get; set; }
}

public class StudentTestResultsItem
{
    public int TestId { get; set; }
    public string TestTitle { get; set; } = string.Empty;
    public int BestScore { get; set; }
    public int MaxPossibleScore { get; set; }
    public bool Passed { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int AttemptsCount { get; set; }
}
