namespace PerformanceAnalysis.Infrastructure.Reports;

/// <summary>
/// Все SQL-запросы для отчётов. Параметризованы через Dapper (@ParamName).
/// </summary>
public static class ReportQueries
{
    /// <summary>
    /// Лидер и отстающий в каждой группе.
    /// Параметры: @DirectionId (int?), @CourseId (int?)
    /// </summary>
    public const string GroupLeadersAndLaggards = @"
        WITH StudentScores AS (
            SELECT s.id            AS studentid,
                   u.firstname || ' ' || u.lastname AS fullname,
                   g.id            AS groupid,
                   g.name          AS groupname,
                   d.name          AS direction,
                   c.name          AS course,
                   COALESCE(SUM(a.score), 0) AS totalscore
            FROM students s
            JOIN users          u  ON s.userid      = u.id
            JOIN student_groups sg ON s.id          = sg.studentsid
            JOIN groups         g  ON sg.groupsid   = g.id
            JOIN directions     d  ON g.directionid = d.id
            JOIN courses        c  ON g.courseid    = c.id
            LEFT JOIN attempts  a  ON s.id          = a.studentid
            WHERE (@DirectionId IS NULL OR g.directionid = @DirectionId)
              AND (@CourseId    IS NULL OR g.courseid    = @CourseId)
            GROUP BY s.id, u.firstname, u.lastname, g.id, g.name, d.name, c.name
        ),
        GroupMaxMin AS (
            SELECT groupid,
                   MAX(totalscore) AS max_score,
                   MIN(totalscore) AS min_score
            FROM StudentScores
            GROUP BY groupid
        )
        SELECT ss.groupid,
               ss.groupname,
               ss.direction,
               ss.course,
               MAX(CASE WHEN ss.totalscore = gmm.max_score THEN ss.fullname   END) AS leadername,
               MAX(CASE WHEN ss.totalscore = gmm.max_score THEN ss.totalscore END) AS leaderscore,
               MAX(CASE WHEN ss.totalscore = gmm.min_score THEN ss.fullname   END) AS laggardname,
               MAX(CASE WHEN ss.totalscore = gmm.min_score THEN ss.totalscore END) AS laggardscore
        FROM StudentScores ss
        JOIN GroupMaxMin gmm ON ss.groupid = gmm.groupid
        GROUP BY ss.groupid, ss.groupname, ss.direction, ss.course
        ORDER BY ss.groupname;";

    /// <summary>
    /// Результаты тестов конкретного студента (лучшая попытка по каждому тесту).
    /// Параметры: @StudentId (int)
    /// </summary>
    public const string StudentTestResults = @"
        WITH BestAttempt AS (
            SELECT DISTINCT ON (tr.testid)
                   tr.testid, tr.attemptid, a.score, tr.passed, a.submittedat
            FROM testresults tr
            JOIN attempts a ON tr.attemptid = a.id
            WHERE tr.studentid = @StudentId
            ORDER BY tr.testid, a.score DESC NULLS LAST
        ),
        TestMaxScore AS (
            SELECT testid, COALESCE(SUM(maxscore), 10) AS maxscore
            FROM questions
            GROUP BY testid
        )
        SELECT t.id           AS testid,
               t.title        AS testtitle,
               COALESCE(ba.score, 0)             AS bestscore,
               COALESCE(tms.maxscore, 10)        AS maxpossiblescore,
               COALESCE(ba.passed, FALSE)        AS passed,
               ba.submittedat                    AS completedat,
               COUNT(a.id)                       AS attemptscount
        FROM tests t
        JOIN test_groups    tg ON t.id           = tg.testsid
        JOIN student_groups sg ON tg.groupsid    = sg.groupsid
        JOIN students        s ON sg.studentsid  = s.id
        LEFT JOIN BestAttempt   ba  ON ba.testid  = t.id
        LEFT JOIN TestMaxScore  tms ON tms.testid = t.id
        LEFT JOIN attempts       a  ON a.studentid = s.id AND a.testid = t.id
        WHERE s.id = @StudentId
        GROUP BY t.id, t.title, ba.score, ba.passed, ba.submittedat, tms.maxscore
        ORDER BY ba.submittedat DESC NULLS LAST, t.title;";

    /// <summary>
    /// Динамика среднего балла по группам помесячно.
    /// Параметры: @GroupIds (int[]?), @DateFrom (DateTime?), @DateTo (DateTime?)
    /// </summary>
    public const string GroupTrend = @"
        SELECT g.id                                              AS groupid,
               g.name                                           AS groupname,
               DATE_TRUNC('month', a.startedat)                 AS month,
               TO_CHAR(a.startedat, 'Mon YYYY')                 AS monthlabel,
               ROUND(AVG(a.score)::NUMERIC, 2)                  AS averagescore,
               COUNT(a.id)                                      AS attemptscount
        FROM groups         g
        JOIN student_groups sg ON g.id          = sg.groupsid
        JOIN students        s ON sg.studentsid = s.id
        JOIN attempts        a ON s.id          = a.studentid AND a.score IS NOT NULL
        WHERE (@GroupIds IS NULL OR g.id = ANY(@GroupIds))
          AND (@DateFrom IS NULL OR a.startedat >= @DateFrom)
          AND (@DateTo   IS NULL OR a.startedat <= @DateTo)
        GROUP BY g.id, g.name,
                 DATE_TRUNC('month', a.startedat),
                 TO_CHAR(a.startedat, 'Mon YYYY')
        ORDER BY g.name, month;";

    /// <summary>
    /// Ежемесячный прогресс студента с нарастающим итогом.
    /// Параметры: @StudentId (int)
    /// </summary>
    public const string StudentMonthlyProgress = @"
        WITH MonthlyScores AS (
            SELECT DATE_TRUNC('month', a.startedat)      AS month,
                   TO_CHAR(a.startedat, 'Mon YYYY')      AS month_label,
                   COALESCE(SUM(a.score), 0)             AS month_score
            FROM attempts a
            WHERE a.studentid      = @StudentId
              AND a.submittedat IS NOT NULL
            GROUP BY DATE_TRUNC('month', a.startedat),
                     TO_CHAR(a.startedat, 'Mon YYYY')
        )
        SELECT month,
               month_label                                    AS monthlabel,
               month_score                                    AS score,
               SUM(month_score) OVER (ORDER BY month)        AS cumulativescore
        FROM MonthlyScores
        ORDER BY month;";

    /// <summary>
    /// Рейтинг студентов с оконной функцией ROW_NUMBER (топ-50).
    /// Параметры: @DirectionId (int?), @CourseId (int?), @GroupId (int?)
    /// </summary>
    public const string StudentRating = @"
        SELECT ROW_NUMBER() OVER (ORDER BY SUM(a.score) DESC) AS rank,
               u.firstname || ' ' || u.lastname               AS fullname,
               c.name                                         AS course,
               g.name                                         AS group,
               d.name                                         AS direction,
               SUM(a.score)                                   AS totalscore
        FROM students        s
        JOIN users          u  ON s.userid      = u.id
        JOIN student_groups sg ON s.id          = sg.studentsid
        JOIN groups         g  ON sg.groupsid   = g.id
        JOIN directions     d  ON g.directionid = d.id
        JOIN courses        c  ON g.courseid    = c.id
        JOIN testresults    tr ON tr.studentid  = s.id
        JOIN attempts        a ON tr.attemptid  = a.id
        WHERE (@DirectionId IS NULL OR g.directionid = @DirectionId)
          AND (@CourseId    IS NULL OR g.courseid    = @CourseId)
          AND (@GroupId     IS NULL OR sg.groupsid   = @GroupId)
        GROUP BY s.id, u.firstname, u.lastname, c.name, g.name, d.name
        ORDER BY totalscore DESC
        LIMIT 50;";

    /// <summary>
    /// Процент пройденных тестов из всех доступных — по каждому студенту группы.
    /// Параметры: @GroupId (int?)
    /// </summary>
    public const string StudentPassRate = @"
        SELECT s.id AS studentid,
               u.firstname || ' ' || u.lastname AS fullname,
               g.name AS group,
               COUNT(DISTINCT t.id) AS testsavailable,
               COUNT(DISTINCT CASE WHEN tr.passed = TRUE THEN t.id END) AS testspassed,
               ROUND(COUNT(DISTINCT CASE WHEN tr.passed = TRUE THEN t.id END)::NUMERIC * 100.0
                     / NULLIF(COUNT(DISTINCT t.id), 0), 2) AS passrate
        FROM students s
        JOIN users          u  ON s.userid      = u.id
        JOIN student_groups sg ON s.id          = sg.studentsid
        JOIN groups         g  ON sg.groupsid   = g.id
        JOIN test_groups    tg ON g.id           = tg.groupsid
        JOIN tests          t  ON tg.testsid     = t.id
        LEFT JOIN testresults tr ON tr.studentid = s.id AND tr.testid = t.id
        WHERE (@GroupId IS NULL OR sg.groupsid = @GroupId)
        GROUP BY s.id, u.firstname, u.lastname, g.name
        ORDER BY passrate DESC;";

    /// <summary>
    /// Сводка по прохождению тестов конкретным студентом.
    /// Параметры: @StudentId (int)
    /// </summary>
    public const string StudentPassRateSummary = @"
        SELECT s.id AS studentid,
               u.firstname || ' ' || u.lastname AS fullname,
               COUNT(DISTINCT tr.testid) AS testsattempted,
               COUNT(DISTINCT CASE WHEN tr.passed = TRUE THEN tr.testid END) AS testspassed,
               ROUND(COUNT(DISTINCT CASE WHEN tr.passed = TRUE THEN tr.testid END)::NUMERIC * 100.0
                     / NULLIF(COUNT(DISTINCT tr.testid), 0), 2) AS passrate,
               COALESCE(SUM(a.score), 0) AS totalscore,
               ROUND(AVG(a.score)::NUMERIC, 2) AS averagescore
        FROM students s
        JOIN users u ON s.userid = u.id
        LEFT JOIN testresults tr ON tr.studentid = s.id
        LEFT JOIN attempts    a  ON tr.attemptid  = a.id
        WHERE s.id = @StudentId
        GROUP BY s.id, u.firstname, u.lastname;";

    /// <summary>
    /// Активность студентов по дням недели.
    /// EXTRACT(DOW): 0=Sunday … 6=Saturday.
    /// Параметры: @DateFrom (DateTime?), @DateTo (DateTime?), @GroupId (int?)
    /// </summary>
    public const string DayOfWeekActivity = @"
        SELECT EXTRACT(DOW FROM a.startedat)::INT AS dayofweek,
               COUNT(a.id)                        AS testscompleted,
               COUNT(DISTINCT s.id)               AS uniquestudents
        FROM attempts a
        JOIN students s ON a.studentid = s.id
        WHERE a.submittedat IS NOT NULL
          AND (@DateFrom IS NULL OR a.startedat >= @DateFrom)
          AND (@DateTo   IS NULL OR a.startedat <= @DateTo)
          AND (@GroupId  IS NULL OR EXISTS (
                SELECT 1 FROM student_groups sg
                WHERE sg.studentsid = s.id AND sg.groupsid = @GroupId
          ))
        GROUP BY EXTRACT(DOW FROM a.startedat)
        ORDER BY dayofweek;";
}
