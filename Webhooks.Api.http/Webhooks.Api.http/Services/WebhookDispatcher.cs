using Webhooks.Api.http.Repositories;

namespace Webhooks.Api.http.Services
{
    internal sealed class WebhookDispatcher(HttpClient httpClient,
                                            InMemoryWebhookSubcriptionRepository subcriptionRepository)
    {
        public async Task DispatchAsync(string eventType, object payload, CancellationToken cancellationToken = default)
        {
            var subscriptions = subcriptionRepository.GetAll(eventType);
            foreach (var subscription in subscriptions)
            {
                var request = new 
                { 
                    Id = Guid.NewGuid(),
                    SubscriptionId = subscription.Id,
                    EventType = eventType, 
                    Data = payload,
                    CreatedOnUtc = DateTime.UtcNow
                };
                var response = await httpClient.PostAsJsonAsync(subscription.WebhookUrl, request, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
