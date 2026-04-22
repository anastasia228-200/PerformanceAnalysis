# PerformanceAnalysis — Система анализа успеваемости студентов

## Содержание
1. [О проекте](#о-проекте)
2. [Архитектура](#архитектура)
3. [Технологии](#технологии)
4. [Запуск проекта](#запуск-проекта)
5. [Аутентификация](#аутентификация)
6. [Отчёты](#отчёты)
7. [Тесты](#тесты)
8. [CI/CD](#cicd)

---

## О проекте

REST API на ASP.NET Core 8 для анализа успеваемости студентов.  
База данных — PostgreSQL. ORM для аутентификации — Entity Framework Core.  
Для SQL-отчётов используется Dapper (микро-ORM).

**Тестовые учётные записи:**

| Роль | Логин | Пароль |
|------|-------|--------|
| Студент | student01 … student20 | student |
| Менеджер | manager | manager |

---

## Архитектура

Проект реализован по принципам **Clean Architecture** — каждый слой является отдельным `.csproj`-проектом. Зависимости направлены строго внутрь: внешние слои знают о внутренних, но не наоборот.

```
┌─────────────────────────────────────────┐
│           PerformanceAnalysis.API        │  Controllers, Program.cs
│         (ASP.NET Core Web API)           │
└────────────────┬────────────────────────┘
                 │ зависит от
┌────────────────▼────────────────────────┐
│       PerformanceAnalysis.Application    │  JwtAuthService, CookieAuthService,
│         (бизнес-логика)                  │  ReportService
└────────────────┬────────────────────────┘
                 │ зависит от
┌────────────────▼────────────────────────┐
│     PerformanceAnalysis.Infrastructure   │  AuthDbContext, DapperExecutor,
│         (доступ к данным)                │  HttpContextExtensions, ReportQueries
└────────────────┬────────────────────────┘
                 │ зависит от
┌────────────────▼────────────────────────┐
│        PerformanceAnalysis.Domain        │  Entities, Interfaces (IJwtAuthService,
│    (ядро, нет внешних зависимостей)      │  ICookieAuthService, IReportService)
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│        PerformanceAnalysis.Tests         │  xUnit + Moq, 11 unit-тестов
└─────────────────────────────────────────┘
```

### Ключевое архитектурное решение — разделение AuthService

В обычном подходе JWT и Cookie логика смешана в одном классе.  
В этом проекте они **разделены** на два независимых сервиса:

| Сервис | Интерфейс | Ответственность |
|--------|-----------|-----------------|
| `JwtAuthService` | `IJwtAuthService` | Регистрация, выдача JWT-токена. Не знает про HttpContext. |
| `CookieAuthService` | `ICookieAuthService` | Создание/удаление Cookie-сессии. Не знает про JWT. |

`AuthController` принимает оба сервиса через DI и делегирует каждому своё.

---

## Технологии

| Технология | Версия | Назначение |
|-----------|--------|-----------|
| ASP.NET Core | 8.0 | Веб-фреймворк |
| PostgreSQL | 16 | База данных |
| Entity Framework Core | 8.0 | ORM для аутентификации (Database-First) |
| Dapper | 2.1.35 | Микро-ORM для SQL-отчётов |
| BCrypt.Net | 4.0.3 | Хэширование паролей |
| JWT Bearer | 8.0 | Аутентификация через токены |
| Cookie Auth | — | Аутентификация через сессии |
| xUnit | 2.7.0 | Фреймворк для тестов |
| Moq | 4.20.72 | Мокирование зависимостей в тестах |
| Swashbuckle | 6.6.2 | Swagger UI |
| Docker / Docker Compose | — | Контейнеризация |
| GitHub Actions | — | CI/CD пайплайн |

---

## Запуск проекта

### Локально (через Visual Studio)

1. Клонировать репозиторий
2. В `PerformanceAnalysis.API/appsettings.json` указать строку подключения к PostgreSQL:
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=testing_results;Username=postgres;Password=ваш_пароль",
    "Auth":    "Host=localhost;Port=5432;Database=testing_results;Username=postgres;Password=ваш_пароль"
  }
}
```
3. Запустить проект `PerformanceAnalysis.API`
4. Swagger UI: `https://localhost:{port}/swagger`

### Через Docker Compose

```bash
# 1. Указать логин Docker Hub в .env
DOCKER_USERNAME=ваш_логин

# 2. Поместить дамп БД testing_results.sql в корень репозитория

# 3. Запустить
docker compose up -d --build

# 4. Swagger UI
http://localhost:8080/swagger

# Остановка
docker compose down

# Сброс БД
docker compose down -v
docker compose up -d --build
```

---

## Аутентификация

### JWT (stateless)

```
POST /api/auth/register     — регистрация нового студента
POST /api/auth/login        — вход, возвращает AccessToken
GET  /api/auth/me           — информация о текущем пользователе (требует авторизации)
```

**Как использовать в Swagger:**
1. `POST /api/auth/login` → скопировать `accessToken` из ответа
2. Нажать кнопку **Authorize** → вставить токен в поле `Bearer {токен}`
3. Теперь все запросы к защищённым эндпоинтам работают

**Как работает:**  
После входа сервер генерирует JWT-токен, подписанный секретным ключом. Токен содержит claims: `UserId`, `Login`, `Email`, `Role`, `StudentId` (для студентов). Сервер не хранит состояние — каждый запрос проверяется по подписи токена.

---

### Cookie (stateful)

```
POST /api/auth/login-with-cookie   — вход, устанавливает Cookie в браузере
POST /api/auth/logout              — выход, удаляет Cookie
GET  /api/auth/me                  — информация о текущем пользователе
```

**Как использовать в Swagger:**
1. `POST /api/auth/login-with-cookie` → выполнить запрос
2. Нажать кнопку **Authorize** → в поле Cookie вставить:  
   `PerformanceAnalysis.Auth=значение_из_браузера`

**Как работает:**  
После входа сервер создаёт зашифрованный Cookie (`PerformanceAnalysis.Auth`) и отправляет его браузеру. Браузер автоматически прикрепляет Cookie к каждому запросу. Cookie помечен флагами `HttpOnly` (недоступен JS) и `Secure`. При `RememberMe = true` сессия живёт 30 дней, иначе 8 часов.

---

### Разграничение доступа по ролям

| Роль | Что может |
|------|-----------|
| **Manager** | Видит все отчёты по всем студентам без ограничений |
| **Student** | Видит только свои данные. StudentId берётся из токена/cookie автоматически. Попытка запросить чужие данные — `403 Forbidden` |

---

## Отчёты

Все отчёты доступны по адресу `GET /api/reports/{название}`.  
**Требуют авторизации** (JWT или Cookie).

### 1. Лидеры и отстающие в группах
```
GET /api/reports/group-leaders
Параметры: DirectionId (опц.), CourseId (опц.)
```
Для каждой группы показывает студента с наибольшим суммарным баллом (лидер) и с наименьшим (отстающий).

---

### 2. Результаты тестов студента
```
GET /api/reports/student-test-results
Параметры: StudentId (опц. для студента — берётся из токена)
```
Лучшая попытка по каждому тесту, количество попыток, статус прохождения.

---

### 3. Динамика среднего балла по группам
```
GET /api/reports/group-trend
Параметры: GroupIds (список), DateFrom, DateTo
```
Средний балл группы помесячно — для построения графика динамики.

---

### 4. Ежемесячный прогресс студента
```
GET /api/reports/student-monthly-progress
Параметры: StudentId (опц. для студента)
```
Баллы за каждый месяц + нарастающий итог (накопленные баллы).

---

### 5. Общий рейтинг студентов
```
GET /api/reports/student-rating
Параметры: DirectionId, CourseId, GroupId (все опциональны)
```
Топ-50 студентов по суммарному баллу с местом в рейтинге.

---

### 6. Процент пройденных тестов по группе
```
GET /api/reports/student-pass-rate
Параметры: GroupId (опц.)
```
Для каждого студента: сколько тестов доступно, сколько пройдено, % прохождения.

---

### 7. Сводка по студенту
```
GET /api/reports/student-pass-rate-summary
Параметры: StudentId (опц. для студента)
```
Общая статистика студента: попыток, пройдено, средний балл, суммарный балл.

---

### 8. Активность по дням недели
```
GET /api/reports/day-of-week-activity
Параметры: DateFrom, DateTo, GroupId (все опциональны)
```
Количество завершённых тестов и уникальных студентов по каждому дню недели.

---

## Тесты

Тестовый проект: `PerformanceAnalysis.Tests`  
Фреймворк: **xUnit** + **Moq**

```bash
dotnet test PerformanceAnalysis.sln --verbosity normal
```

**11 тестов** покрывают все методы `ReportService`:

| Тест | Что проверяет |
|------|--------------|
| `GetGroupLeadersAndLaggards_CallsQueryAsync_WithCorrectSqlAndParams` | SQL содержит `StudentScores`, параметры переданы верно |
| `GetStudentTestResults_CallsQueryAsync_WithCorrectSqlAndParams` | SQL содержит `BestAttempt`, StudentId = 42 |
| `GetGroupTrend_CallsQueryAsync_WithCorrectSqlAndParams` | SQL содержит `DATE_TRUNC`, даты переданы верно |
| `GetStudentRating_CallsQueryAsync_WithCorrectSqlAndParams` | SQL содержит `ROW_NUMBER`, фильтры переданы верно |
| `GetStudentMonthlyProgress_CallsQueryAsync_WithCorrectSqlAndParams` | SQL содержит `MonthlyScores`, StudentId = 99 |
| `GetStudentPassRate_CallsQueryAsync_WithCorrectSqlAndParams` | SQL содержит `testsavailable`, GroupId = 4 |
| `GetStudentPassRateSummary_CallsQueryFirstOrDefaultAsync_WithCorrectSql` | Использует `QueryFirstOrDefaultAsync`, SQL содержит `testsattempted` |
| `GetDayOfWeekActivity_CallsQueryAsync_WithCorrectSqlAndParams` | SQL содержит `EXTRACT`, GroupId = 2 |
| `GetGroupLeadersAndLaggards_ReturnsEmpty_WhenDapperReturnsEmpty` | Пустой список → пустой результат |
| `GetStudentRating_ReturnsEmpty_WhenDapperReturnsEmpty` | Пустой список → пустой результат |
| `GetGroupTrend_ReturnsEmpty_WhenDapperReturnsEmpty` | Пустой список → пустой результат |

**Почему используется Moq и IDapperExecutor:**  
Extension-методы Dapper (`QueryAsync`, `QueryFirstOrDefaultAsync`) нельзя мокать напрямую, так как это статические методы. Поэтому введён интерфейс `IDapperExecutor` — тонкая обёртка, которую Moq легко подменяет. В тестах реальная БД не нужна.

---

## CI/CD

Пайплайн в GitHub Actions: `.github/workflows/ci-cd.yml`

### Job 1: build-and-test (на каждый push и PR в master)
1. Установка .NET 8
2. `dotnet restore` — восстановление пакетов
3. `dotnet build` — сборка в Release
4. `dotnet test` — запуск тестов, результаты сохраняются как артефакт

### Job 2: publish (только при push в master)
1. Сборка Docker-образа из `PerformanceAnalysis.API/Dockerfile`
2. Публикация в Docker Hub с двумя тегами: `latest` и SHA коммита
3. Кэширование слоёв для ускорения повторных сборок

**Секреты GitHub Actions:**
- `DOCKER_USERNAME` — логин Docker Hub
- `DOCKER_PASSWORD` — токен Docker Hub (не пароль аккаунта!)
