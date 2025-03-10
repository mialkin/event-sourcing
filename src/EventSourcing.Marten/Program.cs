using EventSourcing.Marten;
using Marten;
using Marten.Events.Projections;
using Serilog;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMarten(options =>
{
    options.Connection("Server=localhost;Port=5140;Database=marten; User ID=marten;Password=marten");
    options.UseSystemTextJsonForSerialization();

    options.Projections.Add<OrderProjection>(ProjectionLifecycle.Inline);

    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }
});

builder.Host.UseSerilog(
    (context, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration);
        configuration.WriteTo.Console();
    });

var application = builder.Build();

application.MapGet("orders/{orderId:guid}", async (IQuerySession session, Guid orderId) =>
{
    var order = await session.LoadAsync<Order>(orderId);
    return order is not null ? Results.Ok(order) : Results.NotFound();
});

application.MapGet("orders", async (IQuerySession session) =>
{
    var orders = await session.Query<Order>().ToListAsync();
    return Results.Ok(orders);
});

application.MapPost(pattern: "orders", handler: async (IDocumentStore store, CreateOrderRequest request) =>
{
    var order = new Events.OrderCreated
    {
        ProductName = request.ProductName,
        DeliveryAddress = request.DeliveryAddress
    };

    await using var session = store.LightweightSession();
    session.Events.StartStream<Order>(id: order.Id, events: order);
    await session.SaveChangesAsync();

    return Results.Ok(order);
});

application.MapPost("orders/{orderId:guid}/address",
    async (IDocumentStore store, Guid orderId, DeliveryAddressUpdateRequest request) =>
    {
        var addressUpdated = new Events.OrderAddressUpdated
        {
            Id = orderId,
            DeliveryAddress = request.DeliveryAddress
        };

        await using var session = store.LightweightSession();
        session.Events.Append(orderId, addressUpdated);
        await session.SaveChangesAsync();
        return Results.Ok();
    });

application.MapPost("orders/{orderId:guid}/dispatch", async (IDocumentStore store, Guid orderId) =>
{
    var orderDispatched = new Events.OrderDispatched
    {
        Id = orderId,
        DispatchedAtUtc = DateTime.UtcNow
    };

    await using var session = store.LightweightSession();
    session.Events.Append(orderId, orderDispatched);
    await session.SaveChangesAsync();
    return Results.Ok();
});

application.MapGet("orders/{orderId:guid}/out-for-delivery", async (IDocumentStore store, Guid orderId) =>
{
    var orderOutForDelivery = new Events.OrderOutForDelivery
    {
        Id = orderId,
        OutForDeliveryAtUtc = DateTime.UtcNow
    };

    await using var session = store.LightweightSession();
    session.Events.Append(orderId, orderOutForDelivery);
    await session.SaveChangesAsync();
    return Results.Ok();
});

application.MapGet("orders/{orderId:guid}/delivered", async (IDocumentStore store, Guid orderId) =>
{
    var orderDelivered = new Events.OrderDelivered
    {
        Id = orderId,
        DeliveredAtUtc = DateTime.UtcNow
    };

    await using var session = store.LightweightSession();
    session.Events.Append(orderId, orderDelivered);
    await session.SaveChangesAsync();
    return Results.Ok();
});


application.Run();

public record CreateOrderRequest(string ProductName, string DeliveryAddress);

public record DeliveryAddressUpdateRequest(string DeliveryAddress);