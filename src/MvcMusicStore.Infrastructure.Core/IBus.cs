using System.Threading.Tasks;
using MvcMusicStore.CQRS.Core;

namespace MvcMusicStore.Infrastructure.Core
{
    public interface IBus
    {
        Task SendAsync<T>(T command) where T : ICommand;
        Task PublishAsync<T>(T @event) where T : IEvent;
    }
}