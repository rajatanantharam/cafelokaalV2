namespace CafeLokaal.Api.Models
{
    public class CafeOrderModel
    {
        public string OrganizationId { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public List<OrderItem> Orders { get; set; } = [];
    }

    public class OrderItem
    {
        public string OrderId { get; set; } = string.Empty;
        public OrderStep OrderStep { get; set; } = OrderStep.Unknown;
        public int ProcessTime { get; set; }
        public DateTime ProcessDate { get; set; }
    }
    
    public sealed class OrderStep
    {
        public string Value { get; }

        private OrderStep(string value) => Value = value;

        public static readonly OrderStep OrderReceived = new("OrderReceived");
        public static readonly OrderStep OrderPrepared = new("OrderPrepared");
        public static readonly OrderStep OrderServed = new("OrderServed");
        public static readonly OrderStep Unknown = new("Unknown");
        public static IEnumerable<OrderStep> List() => [OrderReceived, OrderPrepared, OrderServed, Unknown];

        public override string ToString() => Value;
    }
}
