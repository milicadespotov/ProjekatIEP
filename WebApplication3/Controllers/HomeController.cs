using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;


namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        public dm150321db db; 

        public HomeController()
        {
                db = new dm150321db();
        }


        [AllowAnonymous]
        public ActionResult Index()
        {
            var auctions = db.Auction.OrderByDescending(a => a.TimeOpening).ToList() ;
            FormAuctionViewModel viewModel = new FormAuctionViewModel();
            int pageNumber = 1;
            int pageSize = (int)db.InformationsForAdministrator.First().ItemsPerPage;
           

            viewModel.Auctions = auctions.ToPagedList(pageNumber, pageSize);
           
            return RedirectToAction("Index", "Auctions", viewModel);
            
        }

  
    }
}