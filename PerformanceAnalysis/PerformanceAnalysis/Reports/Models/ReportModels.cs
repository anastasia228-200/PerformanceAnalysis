namespace PerformanceAnalysis.Reports.Models;

/// <summary>Элемент отчёта об активности студентов по дням недели.</summary>
public class DayOfWeekActivityItem
{
    /// <summary>Номер дня недели (0=Sunday … 6=Saturday).</summary>
    public int DayOfWeek { get; set; }
    /// <summary>Количество завершённых попыток.</summary>
    public int TestsCompleted { get; set; }
    /// <summary>Уникальные студенты, активные в этот день.</summary>
    public int UniqueStudents { get; set; }
}

/// <summary>Элемент отчёта «Лидеры и отстающие» по группе.</summary>
public class GroupLeadersAndLaggardsItem
{
    /// <summary>Идентификатор группы.</summary>
    public int GroupId { get; set; }
    /// <summary>Название группы.</summary>
    public string GroupName { get; set; } = string.Empty;
    /// <summary>Направление обучения.</summary>
    public string Direction { get; set; } = string.Empty;
    /// <summary>Курс.</summary>
    public string Course { get; set; } = string.Empty;
    /// <summary>ФИО лидера группы.</summary>
    public string LeaderName { get; set; } = string.Empty;
    /// <summary>Суммарный балл лидера.</summary>
    public int LeaderScore { get; set; }
    /// <summary>ФИО отстающего студента.</summary>
    public string LaggardName { get; set; } = string.Empty;
    /// <summary>Суммарный балл отстающего.</summary>
    public int LaggardScore { get; set; }
}

/// <summary>Элемент отчёта о динамике среднего балла группы по месяцам.</summary>
public class GroupTrendItem
{
    /// <summary>Идентификатор группы.</summary>
    public int GroupId { get; set; }
    /// <summary>Название группы.</summary>
    public string GroupName { get; set; } = string.Empty;
    /// <summary>Начало месяца (например, 2024-03-01).</summary>
    public DateTime Month { get; set; }
    /// <summary>Подпись месяца для отображения («Мар 2024»).</summary>
    public string MonthLabel { get; set; } = string.Empty;
    /// <summary>Средний балл попыток за месяц.</summary>
    public decimal AverageScore { get; set; }
    /// <summary>Количество попыток за месяц.</summary>
    public int AttemptsCount { get; set; }
}

/// <summary>Элемент отчёта о ежемесячном прогрессе студента.</summary>
public class StudentMonthlyProgressItem
{
    /// <summary>Начало месяца.</summary>
    public DateTime Month { get; set; }
    /// <summary>Подпись месяца («Мар 2024»).</summary>
    public string MonthLabel { get; set; } = string.Empty;
    /// <summary>Баллы за месяц.</summary>
    public int Score { get; set; }
    /// <summary>Нарастающий итог баллов.</summary>
    public int CumulativeScore { get; set; }
}

/// <summary>Элемент отчёта о проценте пройденных тестов по студентам группы.</summary>
public class StudentPassRateItem
{
    /// <summary>Идентификатор студента.</summary>
    public int StudentId { get; set; }
    /// <summary>ФИО студента.</summary>
    public string FullName { get; set; } = string.Empty;
    /// <summary>Название группы студента.</summary>
    public string Group { get; set; } = string.Empty;
    /// <summary>Количество доступных тестов.</summary>
    public int TestsAvailable { get; set; }
    /// <summary>Количество пройденных тестов.</summary>
    public int TestsPassed { get; set; }
    /// <summary>Процент прохождения (0–100).</summary>
    public decimal PassRate { get; set; }
}

/// <summary>Сводный отчёт о прохождении тестов конкретным студентом.</summary>
public class StudentPassRateSummaryItem
{
    /// <summary>Идентификатор студента.</summary>
    public int StudentId { get; set; }
    /// <summary>ФИО студента.</summary>
    public string FullName { get; set; } = string.Empty;
    /// <summary>Количество тестов, к которым была хотя бы одна попытка.</summary>
    public int TestsAttempted { get; set; }
    /// <summary>Количество успешно пройденных тестов.</summary>
    public int TestsPassed { get; set; }
    /// <summary>Процент успешного прохождения (0–100).</summary>
    public decimal PassRate { get; set; }
    /// <summary>Сумма всех баллов студента.</summary>
    public int TotalScore { get; set; }
    /// <summary>Средний балл за одну попытку.</summary>
    public decimal AverageScore { get; set; }
}

/// <summary>Элемент отчёта «Общий рейтинг студентов».</summary>
public class StudentRatingItem
{
    /// <summary>Место в рейтинге.</summary>
    public int Rank { get; set; }
    /// <summary>ФИО студента.</summary>
    public string FullName { get; set; } = string.Empty;
    /// <summary>Курс.</summary>
    public string Course { get; set; } = string.Empty;
    /// <summary>Группа.</summary>
    public string Group { get; set; } = string.Empty;
    /// <summary>Направление.</summary>
    public string Direction { get; set; } = string.Empty;
    /// <summary>Суммарный балл.</summary>
    public int TotalScore { get; set; }
}

/// <summary>Элемент отчёта о результатах тестов конкретного студента.</summary>
public class StudentTestResultsItem
{
    /// <summary>Идентификатор теста.</summary>
    public int TestId { get; set; }
    /// <summary>Название теста.</summary>
    public string TestTitle { get; set; } = string.Empty;
    /// <summary>Лучший балл среди всех попыток.</summary>
    public int BestScore { get; set; }
    /// <summary>Максимально возможный балл теста.</summary>
    public int MaxPossibleScore { get; set; }
    /// <summary>Пройден ли тест.</summary>
    public bool Passed { get; set; }
    /// <summary>Дата последней завершённой попытки.</summary>
    public DateTime? CompletedAt { get; set; }
    /// <summary>Общее количество попыток.</summary>
    public int AttemptsCount { get; set; }
}
