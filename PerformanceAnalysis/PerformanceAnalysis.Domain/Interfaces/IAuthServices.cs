using PerformanceAnalysis.Domain.Entities;

namespace PerformanceAnalysis.Domain.Interfaces;

/// <summary>Вход по логину/паролю — возвращает JWT-токен.</summary>
public interface IJwtAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
}

/// <summary>Вход через Cookie — создаёт stateful-сессию на сервере.</summary>
public interface ICookieAuthService
{
    Task<AuthResponse> LoginWithCookieAsync(LoginWithCookieRequest request, CancellationToken ct = default);
    Task LogoutAsync(CancellationToken ct = default);
}
