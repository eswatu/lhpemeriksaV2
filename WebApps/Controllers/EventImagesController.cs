using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApps.Models;

namespace WebApps.Controllers
{
    public class EventImagesController : Controller
    {
        private LHPContext db = new LHPContext();

        // GET: EventImages
        public ActionResult Index()
        {
            return View(db.EventImages.ToList());
        }

        // GET: EventImages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventImages eventImages = db.EventImages.Find(id);
            if (eventImages == null)
            {
                return HttpNotFound();
            }
            return View(eventImages);
        }

        // GET: EventImages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EventImages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventID,direktori,deskripsi")] EventImages eventImages)
        {
            if (ModelState.IsValid)
            {
                db.EventImages.Add(eventImages);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(eventImages);
        }

        // GET: EventImages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventImages eventImages = db.EventImages.Find(id);
            if (eventImages == null)
            {
                return HttpNotFound();
            }
            return View(eventImages);
        }

        // POST: EventImages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventID,direktori,deskripsi")] EventImages eventImages)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventImages).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(eventImages);
        }

        // GET: EventImages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventImages eventImages = db.EventImages.Find(id);
            if (eventImages == null)
            {
                return HttpNotFound();
            }
            return View(eventImages);
        }

        // POST: EventImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventImages eventImages = db.EventImages.Find(id);
            db.EventImages.Remove(eventImages);
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
