using Microsoft.EntityFrameworkCore;
using Webhooks.Api.http.Data;
using Webhooks.Api.http.Models;


namespace Webhooks.Api.http.Services
{
    internal sealed class WebhookDispatcher
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly WebhooksDbContext _dbContext;

        public WebhookDispatcher(IHttpClientFactory httpClientFactory, WebhooksDbContext dbContext)
        {
            _httpClientFactory = httpClientFactory;
            _dbContext = dbContext;
        }

        public async Task DispatchAsync<T>(string eventType, T data, CancellationToken cancellationToken = default)
        {
            var subscriptions = await _dbContext.WebhookSubscriptions.ToListAsync(cancellationToken);
            foreach (var subscription in subscriptions)
            {
                using var httpClient = _httpClientFactory.CreateClient();

                var payload = new WebhookPayload<T>
                {
                    Id = Guid.NewGuid(),
                    EventType = eventType,
                    SubscriptionId = subscription.Id,
                    Data = data,
                    Timestamp = DateTime.UtcNow
                };
                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
                try
                {
                    var response = await httpClient.PostAsJsonAsync(subscription.WebhookUrl, payload, cancellationToken);

                    var attempt = new WebhookDeliveryAttempt
                    {
                        Id = Guid.NewGuid(),
                        WebhookSubscriptionId = subscription.Id,
                        Timestamp = DateTime.UtcNow,
                        ResponseStatusCode = (int)response.StatusCode,
                        Success = response.IsSuccessStatusCode,
                        Payload = jsonPayload
                    };
                    _dbContext.WebhookDeliveryAttempts.Add(attempt);

                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                catch (Exception)
                {
                    var attempt = new WebhookDeliveryAttempt
                    {
                        Id = Guid.NewGuid(),
                        WebhookSubscriptionId = subscription.Id,
                        Timestamp = DateTime.UtcNow,
                        ResponseStatusCode = null,
                        Success = false,
                        Payload = jsonPayload
                    };
                    _dbContext.WebhookDeliveryAttempts.Add(attempt);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }
}