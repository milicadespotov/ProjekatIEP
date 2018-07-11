namespace WebApplication3.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web;

    [Table("Auction")]
    public partial class Auction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Auction()
        {
            Bid = new HashSet<Bid>();
        }

        [Key]
        public Guid IdAuction { get; set; }

        public string Name { get; set; }

        public byte[] Image { get; set; }

        public int? Duration { get; set; }

        public int? StartingPrice { get; set; }

        public int? CurrentPrice { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? TimeOpening { get; set; }

        public DateTime? TimeClosing { get; set; }

        public string State { get; set; }

        [StringLength(128)]
        public string IdOwner { get; set; }

        [StringLength(128)]
        public string IdWinner { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] AuctionRowVersion { get; set; }

        public virtual User Owner { get; set; }

        public virtual User Winner { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bid> Bid { get; set; }

        [NotMapped]
        public bool hasEnoughTokens { get; set; }

        [NotMapped]
        public HttpPostedFileBase ImageToUpload { get; set; }
    }
}
