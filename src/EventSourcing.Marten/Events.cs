namespace EventSourcing.Marten;

public class Events
{
    public class OrderCreated
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string ProductName { get; set; }
        public required string DeliveryAddress { get; set; }
    }

    public class OrderAddressUpdated
    {
        public Guid Id { get; set; }
        public required string DeliveryAddress { get; set; }
    }

    public class OrderDispatched
    {
        public Guid Id { get; set; }
        public DateTime DispatchedAtUtc { get; set; }
    }

    public class OrderOutForDelivery
    {
        public Guid Id { get; set; }
        public DateTime OutForDeliveryAtUtc { get; set; }
    }

    public class OrderDelivered
    {
        public Guid Id { get; set; }
        public DateTime DeliveredAtUtc { get; set; }
    }
}