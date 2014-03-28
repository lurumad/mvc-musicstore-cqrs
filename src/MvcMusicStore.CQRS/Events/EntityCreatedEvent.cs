using System.Runtime.Serialization;

namespace MvcMusicStore.CQRS.Events
{
    [DataContract]
    public class EntityCreatedEvent<T> : EventBase<T> where T : class
    {
        public EntityCreatedEvent(string createdByUser, T sender) : 
            base(createdByUser, sender)
        {
        }
    }
}