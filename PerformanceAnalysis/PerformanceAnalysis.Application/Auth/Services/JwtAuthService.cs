using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PerformanceAnalysis.Domain.Entities;
using PerformanceAnalysis.Domain.Interfaces;
using PerformanceAnalysis.Infrastructure.Auth;

namespace PerformanceAnalysis.Application.Auth.Services;

/// <summary>
/// Реализует аутентификацию через JWT (stateless).
/// Отвечает только за регистрацию и выдачу токенов — без Cookie, без HttpContext.
/// </summary>
public class JwtAuthService : IJwtAuthService
{
    private readonly AuthDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public JwtAuthService(AuthDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _dbContext.Users
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u =>
                u.Login == request.LoginOrEmail || u.Email == request.LoginOrEmail, ct);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Passwordhash))
            throw new InvalidOperationException("Неверный логин или пароль");

        var (token, expiresAt) = GenerateAccessToken(user);
        return BuildResponse(user, token, expiresAt);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var exists = await _dbContext.Users
            .AnyAsync(u => u.Login == request.Login || u.Email == request.Email, ct);
        if (exists)
            throw new InvalidOperationException("Пользователь с таким логином или email уже существует");

        var group = await _dbContext.Groups.FindAsync(new object[] { request.GroupId }, ct)
            ?? throw new InvalidOperationException("Группа не найдена");

        var user = new User
        {
            Login = request.Login,
            Email = request.Email,
            Passwordhash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Firstname = request.FirstName,
            Middlename = request.MiddleName,
            Lastname = request.LastName,
            Role = "Student",
            Createdat = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(ct);

        var student = new Student
        {
            Userid = user.Id,
            Phone = request.Phone ?? string.Empty,
            Vkprofilelink = request.VkProfileLink ?? string.Empty
        };

        _dbContext.Students.Add(student);
        await _dbContext.SaveChangesAsync(ct);

        student.Groups.Add(group);
        await _dbContext.SaveChangesAsync(ct);

        var (token, expiresAt) = GenerateAccessToken(user);
        return BuildResponse(user, token, expiresAt);
    }

    private (string token, DateTime expiresAt) GenerateAccessToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        if (user.Role == "Student" && user.Student != null)
            claims.Add(new Claim("StudentId", user.Student.Id.ToString()));

        var expiresAt = DateTime.UtcNow.AddMinutes(
            jwtSettings.GetValue<int>("AccessTokenExpirationMinutes"));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    private static AuthResponse BuildResponse(User user, string token, DateTime expiresAt) => new()
    {
        UserId = user.Id,
        Login = user.Login,
        Email = user.Email,
        FirstName = user.Firstname,
        LastName = user.Lastname,
        Role = user.Role,
        AccessToken = token,
        AccessTokenExpiresAt = expiresAt
    };
}
