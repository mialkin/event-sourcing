using Marten.Events.Aggregation;

namespace EventSourcing.Marten;

public class OrderProjection : SingleStreamProjection<Order>
{
    public void Apply(Events.OrderCreated created, Order order)
    {
        order.Id = order.Id;
        order.ProductName = created.ProductName;
        order.DeliveryAddress = created.DeliveryAddress;
    }

    public void Apply(Events.OrderAddressUpdated addressUpdated, Order order)
    {
        order.DeliveryAddress = addressUpdated.DeliveryAddress;
    }

    public void Apply(Events.OrderDispatched dispatched, Order order)
    {
        order.DispatchedAtUtc = dispatched.DispatchedAtUtc;
    }

    public void Apply(Events.OrderOutForDelivery outForDelivery, Order order)
    {
        order.DispatchedAtUtc = outForDelivery.OutForDeliveryAtUtc;
    }

    public void Apply(Events.OrderDelivered orderDelivered, Order order)
    {
        order.DispatchedAtUtc = orderDelivered.DeliveredAtUtc;
    }
}