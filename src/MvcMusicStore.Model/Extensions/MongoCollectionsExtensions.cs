using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MvcMusicStore.Model.DTO;

namespace MvcMusicStore.Model.Extensions
{
    public static class MongoCollectionsExtensions
    {
        public static List<OrderDto> GetLastOrders(this MongoCollection<OrderDto> mongoCollection, string userName)
        {
            var lastOrders = mongoCollection
                .AsQueryable()
                .Where(order => order.UserName.ToLowerInvariant() == userName.ToLowerInvariant())
                .OrderByDescending(order => order.OrderDate)
                .Take(5)
                .Select(order => new OrderDto
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    Items = order.Items,
                    Total = order.Total
                })
                .ToList();

            return lastOrders;
        }
    }
}