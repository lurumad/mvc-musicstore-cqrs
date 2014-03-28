using System.Threading.Tasks;

namespace MvcMusicStore.CQRS.Core
{
    public interface IEventHandler<T> where T : IEvent
    {
        Task HandleAsync(T domainEvent);
    }
}