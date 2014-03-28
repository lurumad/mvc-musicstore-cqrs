using MvcMusicStore.CQRS.Commands;
using MvcMusicStore.Infrastructure.Core;
using MvcMusicStore.Model;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcMusicStore.Controllers
{
    public class CheckoutController : Controller
    {
        private const string PromoCode = "FREE";
        private readonly IBus _bus;

        public CheckoutController(IBus bus)
        {
            _bus = bus;
        }

        public ActionResult AddressAndPayment()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddressAndPayment(FormCollection values)
        {
            var order = new Order();
            TryUpdateModel(order);

            try
            {
                if (string.Equals(values["PromoCode"], PromoCode,
                    StringComparison.OrdinalIgnoreCase) == false)
                {
                    return View(order);
                }

                await _bus.SendAsync(
                    new PlaceOrderCommand(User.Identity.Name,order));

                return RedirectToAction("Complete");
            }
            catch (Exception ex)
            {
                return View(order);
            }
        }

        public ActionResult Complete()
        {
            return View();
        }
    }
}