using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerformanceAnalysis.Application.Auth;
using PerformanceAnalysis.Application.Auth.DTOs;

namespace PerformanceAnalysis.Controllers;

/// <summary>Аутентификация: регистрация и вход (JWT и Cookie).</summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>Регистрация нового студента.</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Вход по логину/email и паролю — возвращает JWT.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Урок 47: Вход через Cookie.
    /// Браузер автоматически сохранит Cookie и будет отправлять его с каждым запросом.
    /// </summary>
    [HttpPost("login-with-cookie")]
    public async Task<IActionResult> LoginWithCookie([FromBody] LoginWithCookieRequest request)
    {
        try
        {
            var result = await _authService.LoginWithCookieAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>Урок 47: Выход — удаляет Cookie-сессию на сервере.</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _authService.LogoutAsync();
            return Ok(new { message = "Выход выполнен успешно" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Урок 47: Тестовый эндпоинт — возвращает claims текущего пользователя.
    /// Работает как с JWT (заголовок Authorization: Bearer), так и с Cookie.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return Ok(new
        {
            IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
            AuthenticationType = User.Identity?.AuthenticationType,
            Claims = claims
        });
    }
}
