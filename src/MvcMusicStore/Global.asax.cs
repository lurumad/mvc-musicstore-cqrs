using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.WindowsAzure;
using MongoDB.Driver;
using MvcMusicStore.App_Start;
using MvcMusicStore.CQRS.CommandsHandlers;
using MvcMusicStore.Infrastructure;
using MvcMusicStore.Infrastructure.Cache;
using MvcMusicStore.Infrastructure.Core;
using MvcMusicStore.Infrastructure.Messaging;
using MvcMusicStore.Model;
using MvcMusicStore.Models;
using Ninject;
using Ninject.Extensions.Conventions;

namespace MvcMusicStore
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer<MusicStoreEntities>(null);

            var kernel = BuildNinjectKernel();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DependencyResolver.SetResolver(new NinjectMvcDependencyResolver(kernel));
        }

        private IKernel BuildNinjectKernel()
        {
            var storageAccount = CloudConfigurationManager.GetSetting("MvcMusicStore.Storage.Account");
            var queueName = CloudConfigurationManager.GetSetting("MvcMusicStore.Storage.QueueName");
            var connectionString = CloudConfigurationManager.GetSetting("MvcMusicStore.Mongodb.Connectionstring");
            var database = CloudConfigurationManager.GetSetting("MvcMusicStore.Mongodb.Database");
            var mongoClient = new MongoClient(connectionString);
            var mongoServer = mongoClient.GetServer();
            var mongoDatabase = mongoServer.GetDatabase(database);

            var kernel = new StandardKernel();

            kernel.Bind<MongoDatabase>()
                .ToConstant(mongoDatabase);

            kernel.Bind<ICartStoreService>()
                .To<SessionCartStoreService>();

            kernel.Bind(
                x => x.FromAssemblyContaining(typeof (PlaceOrderCommandHandler))
                    .SelectAllClasses()
                    .BindAllInterfaces());

            kernel.Bind<ICache>()
                .To<AzureCache>();

            var userManager =
                new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(
                        new ApplicationDbContext()));

            var userValidator = new UserValidator<ApplicationUser>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false
            };

            userManager.UserValidator = userValidator;

            kernel.Bind<UserManager<ApplicationUser>>().ToMethod(cfg => userManager);

            kernel.Bind<IBus>()
                .To<Bus>()
                .InSingletonScope()
                .WithConstructorArgument("kernel", kernel)
                .WithConstructorArgument("connectionString", storageAccount)
                .WithConstructorArgument("queue", queueName);

            return kernel;
        }
    }
}