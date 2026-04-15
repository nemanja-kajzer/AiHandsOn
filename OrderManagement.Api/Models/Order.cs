namespace OrderManagement.Api.Models;

public sealed class Order
{
    public int Id { get; set; }
    public required string Product { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
}
