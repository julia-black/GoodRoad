using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using GoodRoadProj.Models;

namespace GoodRoadProj.Controllers
{
    public class WebController : ApiController
    {
        private GoodRoadDBContext db = new GoodRoadDBContext();

        // GET api/Web
        public IEnumerable<Route> GetRoutes()
        {
            var routes = db.Routes;
            return routes.AsEnumerable();
        }

        // GET api/Web/5
        public Route GetRoute(int id)
        {
            Route route = db.Routes.Find(id);
            if (route == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return route;
        }

        // PUT api/Web/5
        public HttpResponseMessage PutRoute(int id, Route route)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != route.routeID)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(route).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Web
        public HttpResponseMessage PostRoute(Route route)
        {
            if (ModelState.IsValid)
            {
                db.Routes.Add(route);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, route);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = route.routeID }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Web/5
        public HttpResponseMessage DeleteRoute(int id)
        {
            Route route = db.Routes.Find(id);
            if (route == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Routes.Remove(route);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, route);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}