using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;



namespace WebApplication3.Controllers
{

    [AuthorizeForAdmin(Roles = "Admin")]
    public class InformationsForAdministratorsController : Controller
    {
        private dm150321db db = new dm150321db();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(InformationsForAdministratorsController));
        // GET: InformationsForAdministrators
        public ActionResult Index()
        {
            log.Info("Action Index/InfomrationsForAdministratorController has been fired");
            return View(db.InformationsForAdministrator.ToList());
        }



        // GET: InformationsForAdministrators/Edit/5
        [AuthorizeForAdmin(Roles = "Admin")]
        public ActionResult Edit()
        {
            log.Info("Get  action Edit/InfomrationsForAdministratorController");
            InformationsForAdministrator informationsForAdministrator = db.InformationsForAdministrator.FirstOrDefault();
            if (informationsForAdministrator == null)
            {
                return View("NotHere");
            }
            return View(informationsForAdministrator);
        }

        // POST: InformationsForAdministrators/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AuthorizeForAdmin(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemsPerPage,SilverPack,GoldPack,PlatinumPack,ValueToken,Time,Currency")] InformationsForAdministrator informationsForAdministrator)
        {
            log.Info("Post action Edit/InformationsForAdministratorController has been fired");
            if (ModelState.IsValid)
            {

                db.Entry(informationsForAdministrator).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(informationsForAdministrator);
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
