-- ДЗ №22 — Практика 45, Уровень 2
-- Запрос для отчёта DayOfWeekActivity
-- Выполнять в pgAdmin, база данных: testing_results
--
-- EXTRACT(DOW FROM ...): 0 = воскресенье, 1 = понедельник … 6 = суббота
--
-- Замените значения параметров при необходимости:
--   date_from = '2024-01-01'
--   date_to   = '2024-12-31'
--   group_id  = NULL (или конкретный id группы)

SELECT
    EXTRACT(DOW FROM a.startedat)::INT   AS dayofweek,
    TO_CHAR(a.startedat, 'Day')          AS dayname,
    COUNT(a.id)                          AS testscompleted,
    COUNT(DISTINCT s.id)                 AS uniquestudents
FROM attempts a
JOIN students s ON a.studentid = s.id
WHERE a.submittedat IS NOT NULL
  -- Фильтр по периоду (раскомментируйте при необходимости):
  -- AND a.startedat >= '2024-01-01'
  -- AND a.startedat <= '2024-12-31'
  -- Фильтр по группе (раскомментируйте при необходимости):
  -- AND EXISTS (
  --     SELECT 1 FROM student_groups sg
  --     WHERE sg.studentsid = s.id AND sg.groupsid = 1
  -- )
GROUP BY EXTRACT(DOW FROM a.startedat), TO_CHAR(a.startedat, 'Day')
ORDER BY dayofweek;
