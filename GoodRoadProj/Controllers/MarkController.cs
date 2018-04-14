using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoodRoadProj.Models;

namespace GoodRoadProj.Controllers
{
    public class MarkController : Controller
    {
        private GoodRoadDBContext db = new GoodRoadDBContext();

        //
        // GET: /Mark/

        public ActionResult Index()
        {
            var marks = db.Marks.Include(m => m.Route);
            return View(marks.ToList());
        }

        //
        // GET: /Mark/Details/5

        public ActionResult Details(int id = 0)
        {
            Mark mark = db.Marks.Find(id);
            if (mark == null)
            {
                return HttpNotFound();
            }
            return View(mark);
        }

        //
        // GET: /Mark/Create

        public ActionResult Create()
        {
            ViewBag.routeID = new SelectList(db.Routes, "routeID", "routeName");
            return View();
        }

        //
        // POST: /Mark/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Mark mark)
        {
            if (ModelState.IsValid)
            {
                db.Marks.Add(mark);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.routeID = new SelectList(db.Routes, "routeID", "routeName", mark.routeID);
            return View(mark);
        }

        //
        // GET: /Mark/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Mark mark = db.Marks.Find(id);
            if (mark == null)
            {
                return HttpNotFound();
            }
            ViewBag.routeID = new SelectList(db.Routes, "routeID", "routeName", mark.routeID);
            return View(mark);
        }

        //
        // POST: /Mark/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Mark mark)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mark).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.routeID = new SelectList(db.Routes, "routeID", "routeName", mark.routeID);
            return View(mark);
        }

        //
        // GET: /Mark/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Mark mark = db.Marks.Find(id);
            if (mark == null)
            {
                return HttpNotFound();
            }
            return View(mark);
        }

        //
        // POST: /Mark/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Mark mark = db.Marks.Find(id);
            db.Marks.Remove(mark);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}