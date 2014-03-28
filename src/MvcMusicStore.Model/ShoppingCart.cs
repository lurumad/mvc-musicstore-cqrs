using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMusicStore.Model
{
    public class ShoppingCart
    {
        private readonly MusicStoreEntities _storeDb = new MusicStoreEntities();

        private string ShoppingCartId { get; set; }

        public static ShoppingCart GetCart(ICartStoreService cartStoreService)
        {
            var cart = new ShoppingCart
            {
                ShoppingCartId = cartStoreService.GetCartId()
            };

            return cart;
        }

        public void AddToCart(Album album)
        {
            // Get the matching cart and album instances
            var cartItem = _storeDb.Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId &&
                     c.AlbumId == album.AlbumId);

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new Cart
                {
                    AlbumId = album.AlbumId,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };

                _storeDb.Carts.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, then add one to the quantity
                cartItem.Count++;
            }

            // Save changes
            _storeDb.SaveChanges();
        }

        public int RemoveFromCart(int id)
        {
            // Get the cart
            Cart cartItem = _storeDb.Carts.Single(
                cart => cart.CartId == ShoppingCartId &&
                        cart.RecordId == id);

            int itemCount = 0;

            if (cartItem == null)
                return itemCount;

            if (cartItem.Count > 1)
            {
                cartItem.Count--;
                itemCount = cartItem.Count;
            }
            else
            {
                _storeDb.Carts.Remove(cartItem);
            }

            // Save changes
            _storeDb.SaveChanges();

            return itemCount;
        }

        public void EmptyCart()
        {
            IQueryable<Cart> cartItems = _storeDb.Carts.Where(cart => cart.CartId == ShoppingCartId);

            foreach (Cart cartItem in cartItems)
            {
                _storeDb.Carts.Remove(cartItem);
            }

            // Save changes
            _storeDb.SaveChanges();
        }

        public List<Cart> GetCartItems()
        {
            return _storeDb.Carts.Where(cart => cart.CartId == ShoppingCartId).ToList();
        }

        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in _storeDb.Carts
                where cartItems.CartId == ShoppingCartId
                select (int?) cartItems.Count).Sum();

            // Return 0 if all entries are null
            return count ?? 0;
        }

        public decimal GetTotal()
        {
            // Multiply album price by count of that album to get 
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total
            decimal? total = (from cartItems in _storeDb.Carts
                where cartItems.CartId == ShoppingCartId
                select (int?) cartItems.Count*cartItems.Album.Price).Sum();

            return total ?? decimal.Zero;
        }

        public async Task<int> CreateOrderAsync(Order order)
        {
            _storeDb.Orders.Add(order);
            await _storeDb.SaveChangesAsync();

            decimal orderTotal = 0;

            var cartItems = GetCartItems();

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    AlbumId = item.AlbumId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Album.Price,
                    Quantity = item.Count
                };

                orderTotal += (item.Count * item.Album.Price);

                _storeDb.OrderDetails.Add(orderDetail);
            }

            order.Total = orderTotal;
            _storeDb.SaveChanges();
            EmptyCart();

            return order.OrderId;
        }

        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public void MigrateCart(string userName)
        {
            IQueryable<Cart> shoppingCart = _storeDb.Carts.Where(c => c.CartId == ShoppingCartId);

            foreach (Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
            _storeDb.SaveChanges();
        }
    }
}