using System.Diagnostics.CodeAnalysis;
using Dapper;
using Npgsql;

namespace Orders.API.Application.Queries;

public interface IOrderQueries
{
    Task<OrderModel[]> GetOrdersAsync();
}

public class OrderQueries : IOrderQueries
{
    private readonly string _connectionString;

    public OrderQueries([NotNull] string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<OrderModel[]> GetOrdersAsync()
    {
        await using var conn = new NpgsqlConnection(_connectionString);

        var res = await conn.QueryAsync<OrderModel>("SELECT * FROM orders");
        return res.ToArray();
    }
}

public class OrderModel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public DateTime CreationDateTime { get; set; }
    public int OrderStatus { get; set; }
}
