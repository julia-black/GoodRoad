using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoodRoadProj.Models
{
    public class Mark
    {
        public Mark()
        {

        }
        [Key]
        public int markID { get; set; }
        public Route Route { get; set; }
        [ForeignKey("Route")]
        [HiddenInput]
        public int routeID { get; set; }
        [Display(Name = "X-координата")]
        public double mX { get; set; }
        [Display(Name = "Y-координата")]
        public double mY { get; set; }
        [Display(Name = "Наименование достопримечательности")]
        public string mName { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Описание достопримечательности")]
        public string  mDescription { get; set; }


    }
}