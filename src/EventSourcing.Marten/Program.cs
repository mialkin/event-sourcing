var builder = WebApplication.CreateBuilder(args);

var application = builder.Build();

application.MapGet("orders/{orderId:guid}", async (Guid orderId) => { await Task.CompletedTask; });

application.MapGet("orders", async () => { await Task.CompletedTask; });

application.MapPost("orders", async (CreateOrderRequest request) => { await Task.CompletedTask; });

application.MapPost("orders/{orderId:guid}/address",
    async (Guid orderId, DeliveryAddressUpdateRequest request) => { await Task.CompletedTask; });

application.MapPost("orders/{orderId:guid}/dispatch", async (Guid orderId) => { await Task.CompletedTask; });

application.MapGet("orders/{orderId:guid}/out-for-delivery", async () => { await Task.CompletedTask; });

application.MapGet("orders/{orderId:guid}/delivered", async () => { await Task.CompletedTask; });


application.Run();

public record CreateOrderRequest(string ProductName, string DeliveryAddress);

public record DeliveryAddressUpdateRequest(string DeliveryAddress);