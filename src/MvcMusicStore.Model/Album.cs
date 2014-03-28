using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace MvcMusicStore.Model
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class Album
    {
        [ScaffoldColumn(false)]
        [DataMember]
        public int AlbumId { get; set; }

        [DisplayName("Genre")]
        [DataMember]
        public int GenreId { get; set; }

        [DisplayName("Artist")]
        [DataMember]
        public int ArtistId { get; set; }

        [Required(ErrorMessage = "An Album Title is required")]
        [StringLength(160)]
        [DataMember]
        public string Title { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 100.00,
            ErrorMessage = "Price must be between 0.01 and 100.00")]
        [DataMember]
        public decimal Price { get; set; }

        [DisplayName("Album Art URL")]
        [StringLength(1024)]
        [DataMember]
        public string AlbumArtUrl { get; set; }

        [DataMember]
        public virtual Genre Genre { get; set; }

        [DataMember]
        public virtual Artist Artist { get; set; }

        [DataMember]
        public virtual List<OrderDetail> OrderDetails { get; set; }
    }
}