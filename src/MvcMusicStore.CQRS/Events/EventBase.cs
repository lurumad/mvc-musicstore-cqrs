using System.Runtime.Serialization;
using MvcMusicStore.CQRS.Core;

namespace MvcMusicStore.CQRS.Events
{
    [DataContract]
    public abstract class EventBase<T> : IEvent where T : class
    {
        [DataMember] public string CreatedByUser;

        [DataMember] public T Sender;

        protected EventBase()
        {
        }

        protected EventBase(string createdByUser, T sender)
        {
            CreatedByUser = createdByUser;
            Sender = sender;
        }
    }
}