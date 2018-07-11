using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication3.Models
{
    public class AuctionViewModel
    {

        public AuctionViewModel()
        {
            Bid = new List<Bid>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

      //  [Required]
        public HttpPostedFileBase ImageToUpload { get; set; }

        public int Duration { get; set; }

        [Required]
        public int StartingPrice { get; set; }

        public IList<Bid> Bid { get; set; }
    }

    public class FormAuctionViewModel
    {
        
        
       public IPagedList<Auction> Auctions { get; set; }
    
        [Display ( Name = "Name or partial name")]
       public string Name { get; set; }

        [Display (Name = "State") ]
       public string List { get; set; }
         
        [Display (Name = "Low Price")]
        public decimal? LowPrice { get; set; }

        [Display (Name = "High price")]
        public decimal? HighPrice { get; set; }

        [Display(Name = "Only mine")]
        public bool Only {get; set;}

        public IEnumerable<SelectListItem> thisState { get; set; } 


    }
    }

    
