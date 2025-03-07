using EventSourcing.Marten;
using Marten;
using Serilog;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMarten(options =>
{
    options.Connection("Server=localhost;Port=5140;Database=marten; User ID=marten;Password=marten");
    options.UseSystemTextJsonForSerialization();

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
    var order = await session.Events.AggregateStreamAsync<Order>(orderId);
    return order is not null ? Results.Ok(order) : Results.NotFound();
});

application.MapGet("orders", async () => { await Task.CompletedTask; });

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
    async (Guid orderId, DeliveryAddressUpdateRequest request) => { await Task.CompletedTask; });

application.MapPost("orders/{orderId:guid}/dispatch", async (Guid orderId) => { await Task.CompletedTask; });

application.MapGet("orders/{orderId:guid}/out-for-delivery", async () => { await Task.CompletedTask; });

application.MapGet("orders/{orderId:guid}/delivered", async () => { await Task.CompletedTask; });


application.Run();

public record CreateOrderRequest(string ProductName, string DeliveryAddress);

public record DeliveryAddressUpdateRequest(string DeliveryAddress);