namespace PerformanceAnalysis.Reports.Filters;

/// <summary>Фильтр для отчёта «Активность по дням недели».</summary>
public class DayOfWeekActivityFilter
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int? GroupId { get; set; }
}

/// <summary>Фильтр для отчёта «Лидеры и отстающие в группе».</summary>
public class GroupLeadersAndLaggardsFilter
{
    public int? DirectionId { get; set; }
    public int? CourseId { get; set; }
}

/// <summary>Фильтр для отчёта «Динамика среднего балла по группам».</summary>
public class GroupTrendFilter
{
    public List<int>? GroupIds { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

/// <summary>Фильтр для отчёта «Ежемесячный прогресс студента».</summary>
public class StudentMonthlyProgressFilter
{
    /// <summary>ID студента. Если не указан — берётся из токена/cookie для студентов.</summary>
    public int? StudentId { get; set; }
}

/// <summary>Фильтр для отчёта «Процент пройденных тестов по студентам».</summary>
public class StudentPassRateFilter
{
    public int? GroupId { get; set; }
}

/// <summary>Фильтр для сводного отчёта по прохождению тестов студентом.</summary>
public class StudentPassRateSummaryFilter
{
    /// <summary>ID студента. Если не указан — берётся из токена/cookie для студентов.</summary>
    public int? StudentId { get; set; }
}

/// <summary>Фильтр для отчёта «Общий рейтинг студентов».</summary>
public class StudentRatingFilter
{
    public int? DirectionId { get; set; }
    public int? CourseId { get; set; }
    public int? GroupId { get; set; }
}

/// <summary>Фильтр для отчёта «Результаты тестов студента».</summary>
public class StudentTestResultsFilter
{
    /// <summary>ID студента. Если не указан — берётся из токена/cookie для студентов.</summary>
    public int? StudentId { get; set; }
}
