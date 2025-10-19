namespace Webhooks.Api.http.Services
{
    public sealed class WebhookPayload<T>
    {
        public Guid Id { get; init; }
        public string EventType { get; init; }
        public Guid SubscriptionId { get; init; }
        public DateTime Timestamp { get; init; }
        public T Data { get; init; }
    }
}
