using System;
using MongoDB.Bson.Serialization.Attributes;

namespace MvcMusicStore.Model.DTO
{
    public class OrderDto
    {
        public string UserName { get; set; }

        [BsonId]
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }
        public int Items { get; set; }
        public Decimal Total { get; set; }
    }
}