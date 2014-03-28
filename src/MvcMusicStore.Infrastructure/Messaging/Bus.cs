using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using MvcMusicStore.CQRS.Core;
using MvcMusicStore.CQRS.Core.Exceptions;
using MvcMusicStore.Infrastructure.Core;
using Newtonsoft.Json.Linq;
using Ninject;

namespace MvcMusicStore.Infrastructure.Messaging
{
    public class Bus : IBus
    {
        private readonly IKernel _kernel;
        private readonly CloudQueue _queue;

        public Bus(IKernel kernel, string connectionString, string queue)
        {
            _kernel = kernel;

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var cloudQueueClient = storageAccount.CreateCloudQueueClient();

            _queue = cloudQueueClient.GetQueueReference(queue);
            _queue.CreateIfNotExists();
        }

        public Task SendAsync<T>(T command) where T : ICommand
        {
            var commandHandler =
                _kernel.GetAll<ICommandHandler<T>>();

            if (commandHandler == null)
                throw new CommandHandlerNotFoundException(typeof(T));

            return commandHandler.First().HandleAsync(command);
        }

        public async Task PublishAsync<T>(T @event) where T : IEvent
        {
            dynamic message = new JObject();
            message.type = @event.GetType().AssemblyQualifiedName;
            message.content = JObject.FromObject(@event);
            await _queue.AddMessageAsync(new CloudQueueMessage(message.ToString()));
        }
    }
}