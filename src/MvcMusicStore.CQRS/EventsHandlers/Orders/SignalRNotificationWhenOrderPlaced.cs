using Microsoft.AspNet.SignalR;
using MvcMusicStore.CQRS.Core;
using MvcMusicStore.CQRS.Events;
using MvcMusicStore.CQRS.Hubs;
using MvcMusicStore.Model;
using System;
using System.Threading.Tasks;

namespace MvcMusicStore.CQRS.EventsHandlers.Orders
{
    public class SignalRNotificationWhenOrderPlaced : IEventHandler<EntityCreatedEvent<Order>>
    {
        public Task HandleAsync(EntityCreatedEvent<Order> domainEvent)
        {
            try
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<OrderHub>();
                context.Clients.User(domainEvent.CreatedByUser).newOrder(domainEvent.Sender);
            }
            catch (Exception exception)
            {
                // Stream is not open bug in the Redis backplane
            }
            
            return Task.FromResult(0);
        }
    }
}