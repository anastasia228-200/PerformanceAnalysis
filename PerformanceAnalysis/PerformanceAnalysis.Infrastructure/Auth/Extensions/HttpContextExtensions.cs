using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace PerformanceAnalysis.Infrastructure.Auth.Extensions;

public static class UserRoles
{
    public const string Student = "Student";
    public const string Manager = "Manager";
}

public static class HttpContextExtensions
{
    public static string? GetUserRole(this HttpContext context)
        => context.User?.FindFirst(ClaimTypes.Role)?.Value
           ?? context.User?.FindFirst("Role")?.Value;

    public static bool IsManager(this HttpContext context)
        => context.GetUserRole() == UserRoles.Manager;

    public static bool IsStudent(this HttpContext context)
        => context.GetUserRole() == UserRoles.Student;

    public static int? GetStudentId(this HttpContext context)
    {
        var value = context.User?.FindFirst("StudentId")?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }
}
