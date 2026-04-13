# PerformanceAnalysis

Система анализа успеваемости студентов на .NET 8 + Dapper + EF Core + PostgreSQL.

## Быстрый старт

1. Восстановите БД `testing_results` из бэкапа в pgAdmin
2. Проверьте строку подключения в `PerformanceAnalysis/appsettings.json`
3. Запустите:

```bash
dotnet run --project PerformanceAnalysis
```

4. Откройте Swagger: `https://localhost:{PORT}/swagger`

## Структура проекта

| Папка | Описание |
|---|---|
| `Domain/Auth/` | EF Core-сущности (scaffold из БД) |
| `Infrastructure/Auth/` | AuthDbContext |
| `Infrastructure/Reports/` | SQL-запросы (ReportQueries) |
| `Application/Auth/` | IAuthService + AuthService |
| `Application/Reports/` | IReportService + ReportService |
| `Controllers/` | AuthController + ReportsController |
| `Reports/Models/` | Классы-результаты отчётов |
| `Reports/Filters/` | Классы-фильтры отчётов |

## API эндпоинты

### Аутентификация
- `POST /api/auth/register` — регистрация студента
- `POST /api/auth/login` — вход, возвращает JWT

### Отчёты
- `GET /api/reports/group-leaders`
- `GET /api/reports/student-test-results?studentId=`
- `GET /api/reports/group-trend`
- `GET /api/reports/student-monthly-progress?studentId=`
- `GET /api/reports/student-rating`
- `GET /api/reports/student-pass-rate`
- `GET /api/reports/student-pass-rate-summary?studentId=`
- `GET /api/reports/day-of-week-activity`
