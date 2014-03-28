using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MvcMusicStore.Infrastructure;
using MvcMusicStore.Infrastructure.Core;
using MvcMusicStore.Model;
using MvcMusicStore.ViewModels;

namespace MvcMusicStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IBus _bus;
        private readonly ICartStoreService _cartStoreService;
        private readonly MusicStoreEntities _storeDb = new MusicStoreEntities();

        public ShoppingCartController(IBus bus, ICartStoreService cartStoreService)
        {
            _bus = bus;
            _cartStoreService = cartStoreService;
        }

        //
        // GET: /ShoppingCart/

        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart(_cartStoreService);

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };

            // Return the view
            return View(viewModel);
        }

        //
        // GET: /Store/AddToCart/5

        public async Task<ActionResult> AddToCart(int id)
        {
            // Retrieve the album from the database
            Album addedAlbum = _storeDb.Albums
                .Single(album => album.AlbumId == id);

            // Add it to the shopping cart
            ShoppingCart cart = ShoppingCart.GetCart(_cartStoreService);

            cart.AddToCart(addedAlbum);

            // Go back to the main store page for more shopping
            return RedirectToAction("Index");
        }

        //
        // AJAX: /ShoppingCart/RemoveFromCart/5

        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            // Remove the item from the cart
            ShoppingCart cart = ShoppingCart.GetCart(_cartStoreService);

            // Get the name of the album to display confirmation
            string albumName = _storeDb.Carts
                .Single(item => item.RecordId == id).Album.Title;

            // Remove from cart
            int itemCount = cart.RemoveFromCart(id);

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message = Server.HtmlEncode(albumName) +
                          " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };

            return Json(results);
        }

        //
        // GET: /ShoppingCart/CartSummary

        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            ShoppingCart cart = ShoppingCart.GetCart(_cartStoreService);

            ViewData["CartCount"] = cart.GetCount();

            return PartialView("CartSummary");
        }
    }
}