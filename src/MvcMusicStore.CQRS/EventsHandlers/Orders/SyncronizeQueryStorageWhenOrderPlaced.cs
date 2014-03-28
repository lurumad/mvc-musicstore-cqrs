using MongoDB.Driver;
using MvcMusicStore.CQRS.Core;
using MvcMusicStore.CQRS.Events;
using MvcMusicStore.Infrastructure.Cache;
using MvcMusicStore.Infrastructure.Core;
using MvcMusicStore.Model;
using MvcMusicStore.Model.DTO;
using MvcMusicStore.Model.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvcMusicStore.CQRS.EventsHandlers.Orders
{
    public class SyncronizeQueryStorageWhenOrderPlaced : IEventHandler<EntityCreatedEvent<Order>>
    {
        private readonly ICache _cache;
        private readonly MongoDatabase _mongoDatabase;

        public SyncronizeQueryStorageWhenOrderPlaced(ICache cache, MongoDatabase mongoDatabase)
        {
            _cache = cache;
            _mongoDatabase = mongoDatabase;
        }

        public Task HandleAsync(EntityCreatedEvent<Order> domainEvent)
        {
            var userName = domainEvent.CreatedByUser;
            var order = domainEvent.Sender;
            var orderDto = BuildOrderDto(userName, order);
            var mongoCollection = _mongoDatabase.GetCollection<OrderDto>("LastOrders");

            mongoCollection.Insert(orderDto);
            UpdateCache(mongoCollection, userName);

            return Task.FromResult(0);
        }

        private static OrderDto BuildOrderDto(string userName, Order order)
        {
            var orderDto = new OrderDto
            {
                UserName = userName,
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                Items = order.OrderDetails.Count,
                Total = order.Total
            };
            return orderDto;
        }

        private void UpdateCache(MongoCollection<OrderDto> mongoCollection, string userName)
        {
            var lastOrders = mongoCollection.GetLastOrders(userName);

            _cache.Put<List<OrderDto>>(CacheKeys.UserOrderKey(userName), lastOrders);
        }
    }
}