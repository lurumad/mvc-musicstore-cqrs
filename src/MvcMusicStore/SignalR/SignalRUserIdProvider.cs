using System.Security.Claims;
using Microsoft.AspNet.SignalR;

namespace MvcMusicStore.SignalR
{
    public class SignalRUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            return ClaimsPrincipal.Current.Identity.Name;
        }
    }
}