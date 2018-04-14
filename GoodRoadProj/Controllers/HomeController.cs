using GoodRoadProj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace GoodRoadProj.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        GoodRoadDBContext db = new GoodRoadDBContext();
        public ActionResult Index()
        {
            ViewBag.Title = "Главная";

            var routesDB = db.Routes.Include("UserData").ToList();
            return View(routesDB);            
        }


        [HttpPost] 
        public JsonResult GetRoutes()
        {
            //Searching records from list using LINQ query
            var routes = db.Routes.Include("Marks").ToList();
            return Json(JsonConstruct(routes));
        }

        [HttpPost]
        [Authorize]
        public JsonResult GetMyRoutes()
        {
            
            //Searching records from list using LINQ query
            var us = db.UserDatas.First(u => u.userName.Equals(User.Identity.Name));
            var routes = db.Routes.Include("Marks").Where(r => r.userID.Equals(us.userID)).ToList();
            return Json(JsonConstruct(routes));

        }

        [HttpPost]
        public JsonResult GetClosestRoutes(string positionData)
        {
            if (!String.IsNullOrEmpty(positionData))
            {
                string xPattern = @"\[\d{1,2}.\d{1,20},";
                string yPattern = @",\d{1,2}.\d{1,20}\]";
                Match xCol = Regex.Match(positionData, xPattern);
                Match yCol = Regex.Match(positionData, yPattern);
                double x, y;

                string buff = "";
                buff = xCol.Value.Substring(1, xCol.Value.Length - 2);
                if (buff.Length > 9)
                {
                    buff = buff.Substring(0, 9);
                }

                x = Convert.ToDouble(buff.Replace('.', ','));

                buff = yCol.Value.Substring(1, yCol.Value.Length - 2);
                if (buff.Length > 9)
                {
                    buff = buff.Substring(0, 9);
                }
                y = Convert.ToDouble(buff.Replace('.', ','));

                double xMin = x - 0.05;
                double xMax = x + 0.05;
                double yMin = y - 0.05;
                double yMax = y + 0.05;
                var marks = db.Marks.Include("Route").Where(m => ((m.mX > xMin) && (m.mY > yMin) && (m.mX < xMax) && (m.mY < yMax))).ToList();
                List<Route> result = new List<Route>();
                foreach (var item in marks)
                {
                    if (!result.Contains(item.Route))
                    {
                        result.Add(item.Route);
                    }
                    var m = item.Route;
                }

                return Json(JsonConstruct(result));
            }
            return Json(JsonConstruct(new List<Route>()));
        }

        public JsonResult GetRouteByID(int id)
        {
            var routes = db.Routes.Include("Marks").Where(r => r.routeID == id).ToList();
            return Json(JsonConstruct(routes));
        }

        public static string JsonConstruct(List<Route> routes)
        {
            StringBuilder resBuild = new StringBuilder("{ \"routes\": [ ");
            foreach (var route in routes)
            {
                resBuild.Append("{ \"marks\": [ ");
                foreach (var mark in route.Marks)
                {
                    resBuild.Append("{ \"x\": ");
                    resBuild.Append(mark.mX.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture));
                    resBuild.Append(", \"y\": ");
                    resBuild.Append(mark.mY.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture));
                    resBuild.Append(", \"name\": \"");
                    resBuild.Append(mark.mName);
                    resBuild.Append("\", \"description\": \"");
                    resBuild.Append(mark.mDescription);
                    resBuild.Append("\"}, ");
                }
                resBuild.Remove(resBuild.Length - 2, 2);
                resBuild.Append("] }, ");
            }
            resBuild.Remove(resBuild.Length - 2, 2);
            resBuild.Append("] } ");           

            return resBuild.ToString();
        }
        public ActionResult Constructor()
        {
            return View();
        }


        [HttpPost]
        [Authorize]
        public ActionResult GetNewRoute(string routeData, string routeName, string routeDiscription)
        {
            Route newR = new Route();
            newR.routeName = routeName;
            newR.routeDiscription = routeDiscription;
            var us = db.UserDatas.First(u => u.userName.Equals(User.Identity.Name));
            newR.UserData = us;
            List<Mark> m = new List<Mark>();
            string xPattern = @"\[\[\d{1,2}.\d{1,20},";
            string yPattern = @",\d{1,2}.\d{1,20}\]\]";
            string descrPattern = @"(balloonContent)([^\>]*?)("",)";
            string namePattern = @"(hintContent)([^\>]*?)(""})";
            MatchCollection xCol = Regex.Matches(routeData, xPattern);
            MatchCollection yCol = Regex.Matches(routeData, yPattern);
            MatchCollection nameCol = Regex.Matches(routeData, namePattern);
            MatchCollection descrCol = Regex.Matches(routeData, descrPattern);
            

            for (int i = 0; i < xCol.Count; i++)
            {
                Mark mItem = new Mark();
                //mItem.mX = Convert.ToDouble(xCol[i].Value.Substring(1, xCol[i].Value.Length - 3).Replace('.', ','));
                
                string buff = "";                
                buff = xCol[i].Value.Substring(2, xCol[i].Value.Length - 3);
                if (buff.Length > 9)
                {
                    buff = buff.Substring(0, 9);
                }
                mItem.mX = Convert.ToDouble(buff.Replace('.', ','));

                buff = yCol[i].Value.Substring(1, yCol[i].Value.Length - 3);
                if (buff.Length > 9)
                {
                    buff = buff.Substring(0, 9);
                }
                mItem.mY = Convert.ToDouble(buff.Replace('.', ','));

                //mItem.mY = Convert.ToDouble(yCol[i].Value.Substring(1, yCol[i].Value.Length - 3).Replace('.', ','));
                mItem.mDescription = descrCol[i].Value.Substring(17, descrCol[i].Value.Length - 19);
                mItem.mName = nameCol[i].Value.Substring(14, nameCol[i].Value.Length - 16);
                mItem.Route = newR;
                db.Marks.Add(mItem);
            }
            db.Routes.Add(newR);
            db.SaveChanges();
            

            return RedirectToAction("Index", "Home");
        }
        
    }
}

