using System.Threading.Tasks;

namespace MvcMusicStore.CQRS.Core
{
    public interface ICommandHandler<T> where T : ICommand
    {
        Task HandleAsync(T command);
    }
}