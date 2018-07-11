using IepWebApp;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;
using PagedList;


namespace WebApplication3.Controllers
{
    public class OrdersController : Controller
    {
        private dm150321db db = new dm150321db();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(OrdersController));
        // GET: Orders
        [System.Web.Mvc.Authorize (Roles = "Admin, User")]
        public ActionResult Index(int? page)
        {
            log.Info("action /Index/Orders has been fired");
            var order = db.Order.Include(o => o.User).OrderBy(o => o.NumberOfTokens).ToList();
            order = order.Where(w => w.IdUser == User.Identity.GetUserId()).ToList();

            int pageSize = (int)db.InformationsForAdministrator.First().ItemsPerPage;


            int pageNumber = (page ?? 1);

            
            return View(order.ToPagedList(pageNumber, pageSize));
            
        }


        public ActionResult ManageOrders(int? page)
        {
            log.Info("action /ManageOrders/Orders has been fired");
            var order = db.Order.Include(w => w.User).OrderBy(w => w.NumberOfTokens).ToList();
            order = order.Where(w => w.CurrentState == "SUBMITTED").ToList();
            int pageSize = (int)db.InformationsForAdministrator.First().ItemsPerPage;


            int pageNumber = (page ?? 1);


            return View(order.ToPagedList(pageNumber, pageSize));
        }


        [System.Web.Mvc.Authorize (Roles = "Admin, User")]
        public ActionResult Purchase()
        {
            log.Info("Get action /Purchase/Orders has been fired");
            InformationsForAdministrator info = db.InformationsForAdministrator.FirstOrDefault();
            if (info == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            ViewBag.Silver = info.SilverPack;
            ViewBag.Gold = info.GoldPack;
            ViewBag.Platinum = info.PlatinumPack;
            ViewBag.UserId = User.Identity.GetUserId();
            ViewBag.SilverValue = info.SilverPack * info.ValueToken;
            ViewBag.GoldValue = info.GoldPack * info.ValueToken;
            ViewBag.PlatinumValue = info.PlatinumPack * info.PlatinumPack;
            ViewBag.Currency = info.Currency;
            return View();

        }

        [System.Web.Mvc.Authorize (Roles = "Admin, User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Purchase (FormCollection form)
        {
            log.Info("Post /Purchase/Orders has been fire");
            InformationsForAdministrator info = db.InformationsForAdministrator.FirstOrDefault();
            if (info == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            ViewBag.UserId = User.Identity.GetUserId();
            ViewBag.Silver = info.SilverPack;
            ViewBag.Gold = info.GoldPack;
            ViewBag.Platinum = info.PlatinumPack;


            if (form["package"] == null)
            {
                ModelState.AddModelError("packages", "You need to choose one of the packages");
                return View();
            }

            Order order = new Order();
            int numberOfTokens = 0;

            if (form["package"] == "gold")
            {
                numberOfTokens = Convert.ToInt32(info.GoldPack);
            }
            else if (form["package"] == "silver")
            {
                numberOfTokens = Convert.ToInt32(info.SilverPack);
            }
            else if (form["package"] == "platinum")
            {
                numberOfTokens = Convert.ToInt32(info.PlatinumPack);
            }
            order.IdOrder = Guid.NewGuid();
            order.NumberOfTokens = numberOfTokens;
            order.IdUser = User.Identity.GetUserId();
            order.RealPrice = numberOfTokens * info.ValueToken;
            order.CurrentState = "SUBMITTED";
            db.Order.Add(order);
            db.SaveChanges();

            ViewBag.IdOrder = order.IdOrder;
            return View("Payment");
        }


        [AllowAnonymous]
        public ActionResult CentiliPayment(Guid clientid, string status)
        {
            log.Info("action /CentiliPayment/Orders has been fired");
            
            Order order = db.Order.Find(clientid);
            if (order == null)
            {
                return View("NotHere");
            }
            if (order.CurrentState != "SUBMITTED")
            {
                return View("OrderAlreadyProcessed");
            }
            if (status == "failed" || status == "canceled")
            {
                order.CurrentState = "CANCELED";
            }
            else
            {
                order.CurrentState = "COMPLETED";
                User user = db.User.Find(order.IdUser);
                if (user == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                user.NumberOfTokens += Convert.ToInt32(order.NumberOfTokens); 
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<RefreshHub>();
                hubContext.Clients.All.tokenorder(user.Email, user.NumberOfTokens);
                Mailer.sendMail(user.Email, "Centili payment", "Your payment was succesfull. ");
            }
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();
           
            
            return new HttpStatusCodeResult(200);
        }
        // GET: Orders/Details/5
        

         public ActionResult CancelOrder(Guid reference)
        {
            Order order = db.Order.Find(reference);
            if (order == null)
            {
                return View("NotHere");
            }
            order.CurrentState = "CANCELED";
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "Auctions");
        }
            [System.Web.Mvc.Authorize (Roles = "Admin, User")]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        [System.Web.Mvc.Authorize (Roles = "Admin, User")]
        public ActionResult Create()
        {
            ViewBag.IdUser = new SelectList(db.User, "Id", "FirstName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [System.Web.Mvc.Authorize (Roles = "Admin, User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdOrder,NumberOfTokens,RealPrice,CurrentState,IdUser")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.IdOrder = Guid.NewGuid();
                db.Order.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdUser = new SelectList(db.User, "Id", "FirstName", order.IdUser);
            return View(order);
        }

        // GET: Orders/Edit/5
        [AuthorizeForAdmin(Roles = "Admin")]
       /* public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUser = new SelectList(db.User, "Id", "FirstName", order.IdUser);
            return View(order);
        }*/

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AuthorizeForAdmin(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
     /*   public ActionResult Edit([Bind(Include = "IdOrder,NumberOfTokens,RealPrice,CurrentState,IdUser")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUser = new SelectList(db.User, "Id", "FirstName", order.IdUser);
            return View(order);
        }*/

        // GET: Orders/Delete/5
        [AuthorizeForAdmin(Roles = "Admin")]
   /*     public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }*/
        /*
        // POST: Orders/Delete/5
        [AuthorizeForAdmin(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Order order = db.Order.Find(id);
            db.Order.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }*/

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
