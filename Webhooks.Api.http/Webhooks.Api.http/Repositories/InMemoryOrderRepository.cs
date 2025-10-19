using Webhooks.Api.http.Models;

namespace Webhooks.Api.http.Repositories
{
    

    internal sealed class InMemoryOrderRepository
    {
        private readonly List<Order> _orders = [];

        public void Add(Order order)
        {
            _orders.Add(order);
        }
        public IReadOnlyList<Order> GetAll() => _orders.AsReadOnly();

    }
}
