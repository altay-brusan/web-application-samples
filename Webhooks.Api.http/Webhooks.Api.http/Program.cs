using Webhooks.Api.http.Models;
using Webhooks.Api.http.Repositories;
using Webhooks.Api.http.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<InMemoryOrderRepository>();
builder.Services.AddSingleton<InMemoryWebhookSubcriptionRepository>();

builder.Services.AddHttpClient<WebhookDispatcher>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapPost("webhooks/subscription",(CreateWebhookRequest request, InMemoryWebhookSubcriptionRepository repository) => 
{
    WebhookSubscription subscription = new(
        Guid.NewGuid(), 
        request.EventType, 
        request.WebhookUrl, 
        DateTime.UtcNow);
    repository.Add(subscription);
    return Results.Ok(subscription);
});

app.MapPost("/orders", async (CreateOrderRequest request, InMemoryOrderRepository repository, WebhookDispatcher dispatcher) =>
{
  var order = new Order(
        Guid.NewGuid(),
        request.CustomerName,
        request.Amount,
        DateTime.UtcNow);
     repository.Add(order);
     await dispatcher.DispatchAsync("order.created", order);
    return Results.Ok(order);
}).WithTags("Orders");

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
