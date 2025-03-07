namespace EventSourcing.Marten;

public class Order
{
    public Guid Id { get; set; }
    public required string ProductName { get; set; }
    public required string DeliveryAddress { get; set; }
    public DateTime? DispatchedAtUtc { get; set; }
    public DateTime? OutForDeliveryAtUtc { get; set; }
    public DateTime? DeliveredAtUtc { get; set; }
}