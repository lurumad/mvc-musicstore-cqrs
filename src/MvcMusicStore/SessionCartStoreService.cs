using System;
using System.Web;
using MvcMusicStore.Model;

namespace MvcMusicStore
{
    public class SessionCartStoreService : ICartStoreService
    {
        public const string CartSessionKey = "CartId";

        public string GetCartId()
        {
            if (HttpContext.Current.Session[CartSessionKey] != null)
                return HttpContext.Current.Session[CartSessionKey].ToString();

            if (!string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
            {
                HttpContext.Current.Session[CartSessionKey] = HttpContext.Current.User.Identity.Name;
            }
            else
            {
                // Generate a new random GUID using System.Guid class
                var tempCartId = Guid.NewGuid();

                // Send tempCartId back to client as a cookie
                HttpContext.Current.Session[CartSessionKey] = tempCartId.ToString();
            }

            return HttpContext.Current.Session[CartSessionKey].ToString();
        }
    }
}