using System.IdentityModel.Claims;
using System.Web.Helpers;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.WindowsAzure;
using MvcMusicStore;
using MvcMusicStore.SignalR;
using Owin;

[assembly: OwinStartup(typeof (Startup))]

namespace MvcMusicStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var redisServer = CloudConfigurationManager.GetSetting("MvcMusicStore.Redis.ConnectionString");
            var redisPort = CloudConfigurationManager.GetSetting("MvcMusicStore.Redis.Port");
            var redisPassword = CloudConfigurationManager.GetSetting("MvcMusicStore.Redis.Password");
            var redisEventKey = CloudConfigurationManager.GetSetting("MvcMusicStore.Redis.EventKey");

            GlobalHost.DependencyResolver.UseRedis(
                new RedisScaleoutConfiguration(redisServer, int.Parse(redisPort), redisPassword, redisEventKey));

            GlobalHost.DependencyResolver.Register(typeof (IUserIdProvider), () => new SignalRUserIdProvider());
            app.MapSignalR();
            ConfigureAuth(app);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
        }
    }
}