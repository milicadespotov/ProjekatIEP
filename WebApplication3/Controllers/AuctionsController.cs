using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using PagedList;


namespace WebApplication3.Controllers
{
    public class AuctionsController : Controller
    {
        private dm150321db db = new dm150321db();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AuctionsController));
        // GET: Auctions
        [AllowAnonymous]
        public ActionResult Index([Bind(Include = "Name, LowPrice, HighPrice,List, Only")] FormAuctionViewModel auctionViewModel, int? page)
        {
            log.Info("Action /Index/Auction  has been fired");
            //List<Auction> listOfAuction = db.Auction.ToList();
            //List<Auction> listToRemove = new List<Auction>();
            var auctions = db.Auction.OrderByDescending(a => a.TimeOpening).ToList();
            foreach (var auction in auctions.ToList())
            {
                Auction auctionFrom = (Auction)auction;
                if ((DateTime.Compare(Convert.ToDateTime(auction.TimeClosing), DateTime.Now) < 0) && auction.State == "OPENED")
                {
                    auctionFrom.State = "COMPLETED";
                    db.Entry(auctionFrom).State = EntityState.Modified;
                    db.SaveChanges();
                   
                }
            }

            
            if (ModelState.IsValid)
            {


                auctions = db.Auction.OrderByDescending(a => a.TimeOpening).ToList();

                if (!String.IsNullOrEmpty(auctionViewModel.Name))
                {
                    auctions = auctions.Where(w => w.Name.Contains(auctionViewModel.Name)).ToList();
                }
                if (auctionViewModel.LowPrice != null)
                {
                   
                    auctions = auctions.Where(w => w.CurrentPrice > auctionViewModel.LowPrice).ToList();
                }
                if (auctionViewModel.HighPrice != null)
                {
                    
                    auctions = auctions.Where(w => w.CurrentPrice < auctionViewModel.HighPrice).ToList();

                }


                if (!String.IsNullOrEmpty(auctionViewModel.List))
                {
                    auctions = auctions.Where(w => w.State.Equals(auctionViewModel.List)).ToList();
                }

                if (auctionViewModel.Only == true)
                {
                    string id = User.Identity.GetUserId();
                    auctions = auctions.Where(w => w.IdOwner.Equals(id)).ToList();
                }
            }

            int pageSize = (int)db.InformationsForAdministrator.First().ItemsPerPage;
           
        
            int pageNumber = (page ?? 1);
            
            auctionViewModel.Auctions = auctions.ToPagedList(pageNumber, pageSize);
             return View(auctionViewModel);
        }

        // GET: Auctions/Details/5
        public ActionResult Details(Guid? id)
        {
            log.Info("Get action /Details/Auction  has been fired");
            if (id == null)
            {
                return View("NotHere");
            }
            Auction auction = db.Auction.Find(id);
            if (auction == null)
            {
                return View("NotHere");
            }
            return View(auction);
        }
        
        [AuthorizeForAdmin (Roles = "Admin")]
        // GET: Auctions/GetAllReady
        public ActionResult GetAllReady(int? page)
        {
            log.Info("Get action /GetAllReady/Auction  has been fired");
            var auctions = db.Auction.OrderByDescending(w => w.TimeOpening).ToList();


            var auction = auctions.Where(w => w.State.Equals("READY")).ToList();
            int pageSize = (int)db.InformationsForAdministrator.First().ItemsPerPage;


            int pageNumber = (page ?? 1);
            
         return View(auction.ToPagedList(pageNumber, pageSize));
        }


        // GET: Auctions/Create
        [System.Web.Mvc.Authorize(Roles = "Admin, User")]
        public ActionResult Create()
        {
            log.Info("Get action /Create/Auction  has been fired");
            return View();
        }

        // POST: Auctions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [System.Web.Mvc.Authorize(Roles = "Admin, User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdAuction,Name,ImageToUpload,Duration,StartingPrice")] AuctionViewModel auction)
        {
            log.Info("Post action /Create/Auction  has been fired");
            if (ModelState.IsValid)
            {
                Auction auctionEntity = new Auction();
                if (auction.ImageToUpload != null)
                {
                    auctionEntity.ImageToUpload = auction.ImageToUpload;
                    auctionEntity.Image = new byte[auction.ImageToUpload.ContentLength];
                    auctionEntity.ImageToUpload.InputStream.Read(auctionEntity.Image, 0, auctionEntity.Image.Length);
                }
                auctionEntity.IdOwner = User.Identity.GetUserId();
                auctionEntity.State = "READY";
                auctionEntity.Created = DateTime.Now;
                auctionEntity.CurrentPrice = auction.StartingPrice;
                auctionEntity.StartingPrice = auction.StartingPrice;
                auctionEntity.Duration = auction.Duration;
                auctionEntity.Name = auction.Name;
                auctionEntity.IdAuction = Guid.NewGuid();
                db.Auction.Add(auctionEntity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(auction);
        }

       

       
        // GET: Auctions/Delete/5
        
        public ActionResult Delete(Guid? id)
        {
            log.Info("Get action /Delete/Auction  has been fired");
            Auction auction = db.Auction.Find(id);
            if (auction == null)
            {
                return View("NotHere");
            }
            if (User.Identity.GetUserId() == auction.IdOwner || User.IsInRole("Admin"))
            {
                if (id == null)
                {
                    return View("NotHere");
                }
                
                if (auction == null)
                {
                    return View("NotHere");
                }
                return View(auction);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Auctions/Delete/5
     
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            log.Info("Post action /DeleteConfirmed/Auction has been fired");
            Auction auction = db.Auction.Find(id);
            if (User.Identity.GetUserId() == auction.IdOwner || User.IsInRole("Admin"))
            {
                //var bids = from Bid in db.Bid select Bid;
                /* bids = bids.Where(w => w.IdAuction.Equals(auction.IdAuction));
                 List<Bid> listBid = bids.ToList();

                 foreach(var bid in listBid)
                 {
                     db.Entry(bid).State = EntityState.Deleted;
                     db.Bid.Remove(bid);

                 }
                 db.SaveChanges();*/
                
                User winner = auction.Winner;
                if (winner != null && auction.State != "COMPLETED")
                {
                    winner.NumberOfTokens += Convert.ToInt32(auction.CurrentPrice);
                    db.Entry(winner).State = EntityState.Modified;
                    db.SaveChanges();
                }
                db.Auction.Remove(auction);
                db.SaveChanges();
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<RefreshHub>();
                hubContext.Clients.All.delete(id);
                if (auction.Winner != null)
                {
                    hubContext.Clients.All.tokens(null, auction.Winner.UserName, null, auction.Winner.NumberOfTokens);
                }
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [System.Web.Mvc.Authorize(Roles = "Admin")]
        public ActionResult Open(Guid id)
        {
            log.Info("Action /Open/Auction has been fired");
                Auction auction = db.Auction.Find(id);
            if (auction == null)
            {
                return View("NotHere");
            }
            if (auction.State != "READY")
            {
                return RedirectToAction("Index");
            }
            auction.State = "OPENED";
            auction.TimeOpening = DateTime.Now;
            auction.TimeClosing = (DateTime.Now).AddSeconds(Convert.ToDouble(auction.Duration));
            db.Entry(auction).State = EntityState.Modified;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<RefreshHub>();
            hubContext.Clients.All.open();
            db.SaveChanges();

            return RedirectToAction("Index") ;
        }

        [System.Web.Mvc.Authorize(Roles = "User, Admin")]
        public ActionResult WonAuctions(int? page)
        {

            log.Info("Action /WonAuctions/Auction has been fired");
            var auctions = db.Auction.OrderByDescending(w => w.TimeOpening).ToList();
            string id = User.Identity.GetUserId();
            var auction = auctions.Where(w => id.Equals(w.IdWinner)).ToList();
            auction = auction.Where(w => w.State.Equals("COMPLETED")).ToList();
            int pageSize = (int)db.InformationsForAdministrator.First().ItemsPerPage;
            int pageNumber = (page ?? 1);
            
            return View(auction.ToPagedList(pageNumber, pageSize));
            
        }

        [AjaxOnly]
        [HttpPost, ActionName("LoadAuction")]
        public void LoadAuction(Guid? id)
        {
            
            log.Info("Post action /LoadAuction/Auction has been fired");
            var auction = db.Auction.Find(id);
            
            if (auction == null)
            {
                return;
            }
            if (auction.State != "COMPLETED" && auction.State != "READY")
            {
                auction.State = "COMPLETED";

                db.Entry(auction).State = EntityState.Modified;
                db.SaveChanges();
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<RefreshHub>();
                if (auction.IdWinner != null)
                {
                    hubContext.Clients.All.finish(db.User.Find(auction.IdWinner).Email, auction.IdAuction);
                }
            }
            
        }


        [System.Web.Mvc.Authorize (Roles = "Admin, User")]
        public async Task<ActionResult> Bid(Guid Id, byte[] version)
        {

            log.Info("Action /Bid/Auction has been fired");
            if (Id == null)
            {
                return View("NotHere");
            }
            if (version == null)
            {
                return View("NotHere");
            }

            Auction auction = db.Auction.Find(Id);
            User user = db.User.Find(User.Identity.GetUserId());

            if (auction == null)
            {
                return View("NotHere");
            }

            

            if (Convert.ToDateTime(auction.TimeClosing).Subtract(Convert.ToDateTime(auction.TimeOpening)).TotalSeconds < 10)
            {
                auction.TimeClosing = (DateTime?)Convert.ToDateTime(auction.TimeClosing).AddSeconds(10);
            }

            
                if (auction.IdWinner != null && auction.IdWinner != User.Identity.GetUserId() && (user.NumberOfTokens < (auction.CurrentPrice + 1)))
                {
                return RedirectToAction("Purchase", "Orders");
                }
                if (auction.IdWinner == null && (user.NumberOfTokens < auction.CurrentPrice + 1))
                {

                return RedirectToAction("Purchase", "Orders");
                }
                if ((auction.IdWinner != null) && (auction.IdWinner == User.Identity.GetUserId()) && (user.NumberOfTokens == 0))
                {
                    return View("Purchase", "Orders");
                 }
            var idLastUser = auction.IdWinner;
                User LastUser = null;
                if (auction.IdWinner != null && auction.IdWinner != user.Id)
                {
                    LastUser = db.User.Find(auction.IdWinner);
                    LastUser.NumberOfTokens += Convert.ToInt32(auction.CurrentPrice);
                    
                    
                }
           
                //auction.Winner = user;
                auction.IdWinner = User.Identity.GetUserId();
                Bid bid = new Bid();
                bid.IdAuction = Id;
                bid.Auction = auction;
                bid.User = user;
                bid.TimeOfBidding = DateTime.Now;
                auction.CurrentPrice++;
                db.Bid.Add(bid);

            if (idLastUser == null || idLastUser != user.Id)
            {
                user.NumberOfTokens -= Convert.ToInt32(auction.CurrentPrice);
            }
            else
            {
                user.NumberOfTokens--;
            }
               

            bool flag = true;
            try
            {
                db.Entry(auction).OriginalValues["AuctionRowVersion"] = version;
                await db.SaveChangesAsync();
               
            }catch (DbUpdateConcurrencyException ex)
            {
                flag = false;
            }


            if (flag)
            {
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<RefreshHub>();
                var versionNew = Convert.ToBase64String(auction.AuctionRowVersion);
                hubContext.Clients.All.refresh(auction.IdAuction, auction.CurrentPrice, auction.Winner.Email, versionNew);
                if (LastUser == null)
                {
                    hubContext.Clients.All.token(null, auction.Winner.UserName, null, auction.Winner.NumberOfTokens);
                }
                else
                {
                    hubContext.Clients.All.token(LastUser.UserName, auction.Winner.UserName, LastUser.NumberOfTokens, auction.Winner.NumberOfTokens);
                }
            }
          
            
             return RedirectToAction("Index");
            

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

       
    }
}
