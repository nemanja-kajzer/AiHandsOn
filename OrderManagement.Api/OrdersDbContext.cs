namespace OrderManagement.Api;

using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Models;

public sealed class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
}
