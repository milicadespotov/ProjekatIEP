namespace WebApplication3.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Bid")]
    public partial class Bid
    {
        [Key]
        [Column(Order = 0)]
        public DateTime TimeOfBidding { get; set; }

        public int? TokensGiven { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid IdAuction { get; set; }

        [Key]
        [Column(Order = 2)]
        public string IdUser { get; set; }

        public virtual Auction Auction { get; set; }

        public virtual User User { get; set; }
    }
}
