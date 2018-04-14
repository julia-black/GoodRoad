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
    public class RouteController : Controller
    {
        private GoodRoadDBContext db = new GoodRoadDBContext();

        //
        // GET: /Route/

        public ActionResult Index()
        {
            var routes = db.Routes.Include(r => r.UserData);
            return View(routes.ToList());
        }

        //
        // GET: /Route/Details/5

        public ActionResult Details(int id = 0)
        {
            Route route = db.Routes.Find(id);
            if (route == null)
            {
                return HttpNotFound();
            }
            return View(route);
        }

        //
        // GET: /Route/Create

        public ActionResult Create()
        {
            ViewBag.userID = new SelectList(db.UserDatas, "userID", "userName");
            return View();
        }

        //
        // POST: /Route/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Route route)
        {
            if (ModelState.IsValid)
            {
                db.Routes.Add(route);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.userID = new SelectList(db.UserDatas, "userID", "userName", route.userID);
            return View(route);
        }

        //
        // GET: /Route/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Route route = db.Routes.Find(id);
            if (route == null)
            {
                return HttpNotFound();
            }
            ViewBag.userID = new SelectList(db.UserDatas, "userID", "userName", route.userID);
            return View(route);
        }

        //
        // POST: /Route/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Route route)
        {
            if (ModelState.IsValid)
            {
                db.Entry(route).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userID = new SelectList(db.UserDatas, "userID", "userName", route.userID);
            return View(route);
        }

        //
        // GET: /Route/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Route route = db.Routes.Find(id);
            if (route == null)
            {
                return HttpNotFound();
            }
            return View(route);
        }

        //
        // POST: /Route/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Route route = db.Routes.Find(id);
            db.Routes.Remove(route);
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