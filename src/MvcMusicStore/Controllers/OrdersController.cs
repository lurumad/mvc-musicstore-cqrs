using System.Collections.Generic;
using System.Web.Mvc;
using MvcMusicStore.Infrastructure.Cache;
using MvcMusicStore.Infrastructure.Core;
using MvcMusicStore.Model.DTO;
using MvcMusicStore.ViewModels;

namespace MvcMusicStore.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ICache _cache;

        public OrdersController(ICache cache)
        {
            _cache = cache;
        }

        public ActionResult Index()
        {
            var lastOrders = _cache.Get<List<OrderDto>>(CacheKeys.UserOrderKey(User.Identity.Name));
            
            var ordersViewModel = new OrdersViewModel
            {
                LastOrders = lastOrders ?? new List<OrderDto>()
            };

            return View(ordersViewModel);
        }

        public ActionResult Details()
        {
            return View();
        }
    }
}