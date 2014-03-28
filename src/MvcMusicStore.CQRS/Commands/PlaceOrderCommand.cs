using MvcMusicStore.CQRS.Core;
using MvcMusicStore.Model;

namespace MvcMusicStore.CQRS.Commands
{
    public class PlaceOrderCommand : ICommand
    {
        public readonly string UserName;
        public readonly Order Order;

        public PlaceOrderCommand(
            string userName, 
            Order order)
        {
            UserName = userName;
            Order = order;
        }
    }
}