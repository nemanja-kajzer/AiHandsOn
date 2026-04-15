namespace OrderManagement.Api.Services;

using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Models;

public sealed class OrderService(OrdersDbContext db)
{
    private readonly OrdersDbContext _db = db;

    public async Task<List<Order>> GetAllAsync() => await _db.Orders.AsNoTracking().ToListAsync();

    public async Task<Order?> GetByIdAsync(int id) => await _db.Orders.FindAsync(id);

    public async Task<Order> CreateAsync(Order order)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return order;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _db.Orders.FindAsync(id);
        if (item is null) return false;
        _db.Orders.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }
}
