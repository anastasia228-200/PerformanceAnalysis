using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerformanceAnalysis.Domain.Entities;
using PerformanceAnalysis.Domain.Interfaces;

namespace PerformanceAnalysis.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IJwtAuthService _jwtAuthService;
    private readonly ICookieAuthService _cookieAuthService;

    public AuthController(IJwtAuthService jwtAuthService, ICookieAuthService cookieAuthService)
    {
        _jwtAuthService = jwtAuthService;
        _cookieAuthService = cookieAuthService;
    }

    /// <summary>Регистрация нового студента.</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try { return Ok(await _jwtAuthService.RegisterAsync(request)); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    /// <summary>Вход через JWT.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try { return Ok(await _jwtAuthService.LoginAsync(request)); }
        catch (InvalidOperationException ex) { return Unauthorized(new { message = ex.Message }); }
    }

    /// <summary>Вход через Cookie.</summary>
    [HttpPost("login-with-cookie")]
    public async Task<IActionResult> LoginWithCookie([FromBody] LoginWithCookieRequest request)
    {
        try { return Ok(await _cookieAuthService.LoginWithCookieAsync(request)); }
        catch (InvalidOperationException ex) { return Unauthorized(new { message = ex.Message }); }
    }

    /// <summary>Выход — удаляет Cookie-сессию.</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _cookieAuthService.LogoutAsync();
        return Ok(new { message = "Выход выполнен успешно" });
    }

    /// <summary>Тест авторизации — возвращает claims текущего пользователя.</summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser() => Ok(new
    {
        IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
        AuthenticationType = User.Identity?.AuthenticationType,
        Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
    });
}
