using System.Data;
using Dapper;

namespace PerformanceAnalysis.Infrastructure.Reports;

/// <summary>
/// Урок 49: Обёртка над Dapper-методами.
/// Extension-методы Dapper нельзя мокать напрямую — этот интерфейс решает проблему,
/// позволяя подменять реализацию в unit-тестах через Moq.
/// </summary>
public interface IDapperExecutor
{
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null);
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null);
}

/// <summary>Реальная реализация — делегирует вызовы Dapper.</summary>
public class DapperExecutor : IDapperExecutor
{
    private readonly IDbConnection _connection;

    public DapperExecutor(IDbConnection connection) => _connection = connection;

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        => await _connection.QueryAsync<T>(sql, param);

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null)
        => await _connection.QueryFirstOrDefaultAsync<T>(sql, param);
}
