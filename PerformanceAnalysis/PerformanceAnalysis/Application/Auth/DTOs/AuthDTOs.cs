namespace PerformanceAnalysis.Application.Auth.DTOs;

/// <summary>Ответ API после успешного входа или регистрации.</summary>
public class AuthResponse
{
    public int UserId { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? AccessToken { get; set; }
    public DateTime? AccessTokenExpiresAt { get; set; }
    /// <summary>True, если аутентификация выполнена через Cookie (а не JWT).</summary>
    public bool UseCookies { get; set; }
}

/// <summary>Запрос на вход через Cookie.</summary>
public class LoginWithCookieRequest
{
    public string LoginOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    /// <summary>Если true — Cookie живёт 30 дней, иначе 8 часов.</summary>
    public bool RememberMe { get; set; }
}

/// <summary>Запрос на вход (логин или email + пароль).</summary>
public class LoginRequest
{
    public string LoginOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>Запрос на регистрацию нового студента.</summary>
public class RegisterRequest
{
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? VkProfileLink { get; set; }
    public int GroupId { get; set; }
}
