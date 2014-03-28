using System;

namespace MvcMusicStore.CQRS.Core.Exceptions
{
    public class CommandHandlerNotFoundException : Exception
    {
        private readonly Type _type;

        public CommandHandlerNotFoundException(Type type)
        {
            _type = type;
        }
    }
}