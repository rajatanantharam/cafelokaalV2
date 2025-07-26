namespace CafeLokaal.Api.Models;

public class CafeOrderModel
{
    public Guid OrderId { get; set; }
    public string CafeId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public OrderStates OrderStates { get; set; } = new();
}

public class OrderItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class OrderStates
{
    public OrderState OrderReceived { get; set; } = new();
    public OrderState OrderPrepared { get; set; } = new();
    public OrderState OrderServed { get; set; } = new();
}

public class OrderState
{
    public DateTime StartTimestamp { get; set; }
    public DateTime EndTimestamp { get; set; }
}
