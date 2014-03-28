using System.Collections.Generic;
using MvcMusicStore.Model.DTO;

namespace MvcMusicStore.ViewModels
{
    public class OrdersViewModel
    {
        public List<OrderDto> LastOrders { get; set; }
    }
}