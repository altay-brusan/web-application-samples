using Webhooks.Api.http.Models;

namespace Webhooks.Api.http.Repositories
{
    internal sealed class InMemoryWebhookSubcriptionRepository
    {
        private readonly List<WebhookSubscription> _subscription = [];

        public void Add(WebhookSubscription order)
        {
            _subscription.Add(order);
        }
        public IReadOnlyList<WebhookSubscription> GetAll(string EventType) => _subscription.Where(s=>s.EventType==EventType).ToList().AsReadOnly();
    }
}
