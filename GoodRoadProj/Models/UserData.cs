using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoodRoadProj.Models
{
    public class UserData
    {
        public UserData()
        {

        }
        public UserData(string userName)
        {
            this.userName = userName;
        }
        [Key]
        public int userID { get; set; }
        [Display(Name="Автор")]
        public string userName { get; set; }
        public List<Route> Routes { get; set; }
    }
}