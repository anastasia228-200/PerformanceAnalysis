using System.Security.Claims;

namespace PerformanceAnalysis.Infrastructure.Auth.Extensions;

/// <summary>Константы ролей пользователей.</summary>
public static class UserRoles
{
    public const string Student = "Student";
    public const string Manager = "Manager";
}

/// <summary>Extension-методы для получения информации о пользователе из HttpContext.</summary>
public static class HttpContextExtensions
{
    /// <summary>Получить роль текущего пользователя из Claims.</summary>
    public static string? GetUserRole(this HttpContext context)
        => context.User?.FindFirst(ClaimTypes.Role)?.Value
           ?? context.User?.FindFirst("Role")?.Value;

    /// <summary>Проверить, является ли текущий пользователь менеджером.</summary>
    public static bool IsManager(this HttpContext context)
        => context.GetUserRole() == UserRoles.Manager;

    /// <summary>Проверить, является ли текущий пользователь студентом.</summary>
    public static bool IsStudent(this HttpContext context)
        => context.GetUserRole() == UserRoles.Student;

    /// <summary>
    /// Получить StudentId из Claims (добавляется при входе через JWT или Cookie).
    /// </summary>
    public static int? GetStudentId(this HttpContext context)
    {
        var value = context.User?.FindFirst("StudentId")?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }
}
