using System;
using System.Threading.Tasks;
using MvcMusicStore.CQRS.Commands;
using MvcMusicStore.CQRS.Core;
using MvcMusicStore.CQRS.Events;
using MvcMusicStore.Infrastructure.Core;
using MvcMusicStore.Model;

namespace MvcMusicStore.CQRS.CommandsHandlers
{
    public class PlaceOrderCommandHandler : ICommandHandler<PlaceOrderCommand>
    {
        private readonly ICartStoreService _cartStoreService;
        private readonly IBus _bus;

        public PlaceOrderCommandHandler(
            IBus bus,
            ICartStoreService cartStoreService)
        {
            _bus = bus;
            _cartStoreService = cartStoreService;
        }

        public async Task HandleAsync(PlaceOrderCommand command)
        {
            var order = command.Order;
            order.Username = command.UserName;
            order.OrderDate = DateTime.UtcNow;

            var cart = ShoppingCart.GetCart(_cartStoreService);

            await cart.CreateOrderAsync(order);
            await _bus.PublishAsync(
                new EntityCreatedEvent<Order>(
                    order.Username, 
                    order));
        }
    }
}