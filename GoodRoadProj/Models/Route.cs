using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoodRoadProj.Models
{
    public class Route
    {
        [Key]
        public int routeID { get; set; }
        public UserData UserData { get; set; }
        [ForeignKey("UserData")]
        [HiddenInput]
        public int userID { get; set; }
        [Display(Name = "Маршрут")]
        public string routeName { get; set; }
        [Display(Name = "Описание")]
        public string routeDiscription { get; set; }
        public List<Mark> Marks { get; set; }
        public override bool Equals(object obj)
        {
            Route r = (Route)obj;
            if (r.routeID.Equals(this.routeID))
            {
                return true;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}