using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PerformanceAnalysis.Domain.Entities;
using PerformanceAnalysis.Domain.Interfaces;
using PerformanceAnalysis.Infrastructure.Auth;

namespace PerformanceAnalysis.Application.Auth.Services;

/// <summary>
/// Реализует аутентификацию через Cookie (stateful).
/// Отвечает только за создание/удаление сессии — без JWT, без токенов.
/// </summary>
public class CookieAuthService : ICookieAuthService
{
    private readonly AuthDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieAuthService(AuthDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthResponse> LoginWithCookieAsync(LoginWithCookieRequest request,
        CancellationToken ct = default)
    {
        var user = await _dbContext.Users
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u =>
                u.Login == request.LoginOrEmail || u.Email == request.LoginOrEmail, ct);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Passwordhash))
            throw new InvalidOperationException("Неверный логин или пароль");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        if (user.Role == "Student" && user.Student != null)
            claims.Add(new Claim("StudentId", user.Student.Id.ToString()));

        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

        var expiresAt = request.RememberMe
            ? DateTimeOffset.UtcNow.AddDays(30)
            : DateTimeOffset.UtcNow.AddHours(8);

        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = expiresAt,
            IsPersistent = request.RememberMe
        };

        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext недоступен");

        await httpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity), authProperties);

        return new AuthResponse
        {
            UserId = user.Id,
            Login = user.Login,
            Email = user.Email,
            FirstName = user.Firstname,
            LastName = user.Lastname,
            Role = user.Role,
            UseCookies = true
        };
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
            await httpContext.SignOutAsync("Cookies");
    }
}
