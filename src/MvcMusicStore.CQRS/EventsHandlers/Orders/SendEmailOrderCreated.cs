using Microsoft.AspNet.SignalR;
using MvcMusicStore.CQRS.Core;
using MvcMusicStore.CQRS.Events;
using MvcMusicStore.CQRS.Hub;
using MvcMusicStore.Model;

namespace MvcMusicStore.CQRS.EventsHandlers.Orders
{
    public class SendEmailOrderCreated: IEventHandler<EntityCreatedEvent<Order>>
    {
        public void Handle(EntityCreatedEvent<Order> domainEvent)
        {
            var context=GlobalHost.ConnectionManager.GetHubContext<OrderHub>();
            context.Clients.User(domainEvent.CreatedByUser).newOrder(domainEvent.Sender);
        }
    }
}