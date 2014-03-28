using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using MongoDB.Driver;
using MvcMusicStore.CQRS.CommandsHandlers;
using MvcMusicStore.CQRS.Core;
using MvcMusicStore.Infrastructure;
using MvcMusicStore.Infrastructure.Cache;
using MvcMusicStore.Infrastructure.Core;
using MvcMusicStore.Model;
using Newtonsoft.Json.Linq;
using Ninject;
using Ninject.Extensions.Conventions;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using ReflectionMagic;

namespace MvcMusicStore.EventHandlerWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private CloudQueue _eventsQueue;
        private IKernel _kernel;

        public override void Run()
        {
            RunAsync().Wait();
        }

        public async Task RunAsync()
        {
            while (true)
            {
                try
                {
                    var brokeredMessage = _eventsQueue.GetMessage();

                    if (brokeredMessage != null)
                    {
                        dynamic dataMessage = JObject.Parse(brokeredMessage.AsString);
                        var typeName = dataMessage.type;
                        var typeArguments = Type.GetType(typeName.ToString());
                        Type type = typeof(IEventHandler<>).MakeGenericType(typeArguments);
                        var eventHandlers = _kernel.GetAll(type);
                        var messageBodyJObject = (dataMessage.content as JObject);

                        if (messageBodyJObject == null)
                            continue;

                        var messageBody = messageBodyJObject.ToObject(typeArguments);

                        foreach (var handler in eventHandlers)
                        {
                            await handler.AsDynamic().HandleAsync(messageBody);
                        }

                        _eventsQueue.DeleteMessage(brokeredMessage);
                    }
                    else
                    {
                        Thread.Sleep(5000);
                    }
                }
                catch (Exception exception)
                {
                    var innerException = exception.InnerException;

                    Trace.TraceError(
                        "An error ocurred on Knowledge Worker Role (Processor Queue Message)", exception);

                    while (innerException != null)
                    {
                        Trace.TraceError(
                            "An error ocurred on Knowledge Worker Role (Processor Queue Message) - Inner Exception",
                            innerException);

                        innerException = innerException.InnerException;
                    }
                }
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            ConfigureStorageQueues();
            ConfigureIoC();
            ConfigureSignalR();

            return base.OnStart();
        }

        private void ConfigureSignalR()
        {
            var redisServer = CloudConfigurationManager.GetSetting("MvcMusicStore.Redis.ConnectionString");
            var redisPort = CloudConfigurationManager.GetSetting("MvcMusicStore.Redis.Port");
            var redisPassword = CloudConfigurationManager.GetSetting("MvcMusicStore.Redis.Password");
            var redisEventKey = CloudConfigurationManager.GetSetting("MvcMusicStore.Redis.EventKey");
            
            GlobalHost.DependencyResolver.UseRedis(
                new RedisScaleoutConfiguration(redisServer, int.Parse(redisPort), redisPassword, redisEventKey));
        }

        private void ConfigureStorageQueues()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("MvcMusicStore.Storage.Account"));
            var cloudQueueClient = storageAccount.CreateCloudQueueClient();

            _eventsQueue = cloudQueueClient.GetQueueReference(CloudConfigurationManager.GetSetting("MvcMusicStore.Storage.QueueName"));
            _eventsQueue.CreateIfNotExists();
        }

        private void ConfigureIoC()
        {
            var connectionString = CloudConfigurationManager.GetSetting("MvcMusicStore.Mongodb.Connectionstring");
            var database = CloudConfigurationManager.GetSetting("MvcMusicStore.Mongodb.Database");
            var mongoClient = new MongoClient(connectionString);
            var mongoServer = mongoClient.GetServer();
            var mongoDatabase = mongoServer.GetDatabase(database);

            _kernel = new StandardKernel();

            _kernel.Bind<ICache>().
                To<AzureCache>();

            _kernel.Bind<MusicStoreEntities>().ToSelf();
            _kernel.Bind<MongoDatabase>().ToConstant(mongoDatabase);

            _kernel.Bind(
                x => x.FromAssemblyContaining(typeof(PlaceOrderCommandHandler))
                    .SelectAllClasses()
                    .BindAllInterfaces()
                    .Configure(c => c.InSingletonScope()));
        }
    }
}