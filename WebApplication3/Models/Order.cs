namespace WebApplication3.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        [Key]
        public Guid IdOrder { get; set; }

        public int? NumberOfTokens { get; set; }

        public decimal? RealPrice { get; set; }

        public string CurrentState { get; set; }

        [StringLength(128)]
        public string IdUser { get; set; }

        public virtual User User { get; set; }
    }
}
