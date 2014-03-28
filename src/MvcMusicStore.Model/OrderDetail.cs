using System;
using System.Runtime.Serialization;

namespace MvcMusicStore.Model
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class OrderDetail
    {
        [DataMember]
        public int OrderDetailId { get; set; }

        [DataMember]
        public int OrderId { get; set; }

        [DataMember]
        public int AlbumId { get; set; }

        [DataMember]
        public int Quantity { get; set; }

        [DataMember]
        public decimal UnitPrice { get; set; }

        public virtual Album Album { get; set; }

        public virtual Order Order { get; set; }
    }
}