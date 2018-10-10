using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyVeggieStore.Models;

namespace MyVeggieStore.Controllers
{
    public class tblLocationsController : Controller
    {
        private LocationModels db = new LocationModels();

        // GET: tblLocations
        public ActionResult Index()
        {
            return View(db.tblLocations.ToList());
        }

        // GET: tblLocations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblLocation tblLocation = db.tblLocations.Find(id);
            if (tblLocation == null)
            {
                return HttpNotFound();
            }
            return View(tblLocation);
        }

        // GET: tblLocations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tblLocations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Latitude,Longitude")] tblLocation tblLocation)
        {
            if (ModelState.IsValid)
            {
                db.tblLocations.Add(tblLocation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblLocation);
        }

        // GET: tblLocations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblLocation tblLocation = db.tblLocations.Find(id);
            if (tblLocation == null)
            {
                return HttpNotFound();
            }
            return View(tblLocation);
        }

        // POST: tblLocations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Latitude,Longitude")] tblLocation tblLocation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblLocation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblLocation);
        }

        // GET: tblLocations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblLocation tblLocation = db.tblLocations.Find(id);
            if (tblLocation == null)
            {
                return HttpNotFound();
            }
            return View(tblLocation);
        }

        // POST: tblLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblLocation tblLocation = db.tblLocations.Find(id);
            db.tblLocations.Remove(tblLocation);
            db.SaveChanges();
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
