namespace PerformanceAnalysis.Domain.Entities;

public class DayOfWeekActivityFilter
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int? GroupId { get; set; }
}

public class GroupLeadersAndLaggardsFilter
{
    public int? DirectionId { get; set; }
    public int? CourseId { get; set; }
}

public class GroupTrendFilter
{
    public List<int>? GroupIds { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class StudentMonthlyProgressFilter
{
    /// <summary>Если не указан — берётся из токена/cookie для студентов.</summary>
    public int? StudentId { get; set; }
}

public class StudentPassRateFilter
{
    public int? GroupId { get; set; }
}

public class StudentPassRateSummaryFilter
{
    /// <summary>Если не указан — берётся из токена/cookie для студентов.</summary>
    public int? StudentId { get; set; }
}

public class StudentRatingFilter
{
    public int? DirectionId { get; set; }
    public int? CourseId { get; set; }
    public int? GroupId { get; set; }
}

public class StudentTestResultsFilter
{
    /// <summary>Если не указан — берётся из токена/cookie для студентов.</summary>
    public int? StudentId { get; set; }
}
