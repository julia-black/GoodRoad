using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GoodRoadProj.Models
{
    public class GoodRoadDBContext : DbContext
    {
        public GoodRoadDBContext() : base("name=GoodRoadDBContext")
        {
        }

        public DbSet<Mark> Marks { get; set; }

        public DbSet<Route> Routes { get; set; }
        public DbSet<UserData> UserDatas { get; set; }
        //public DbSet<Route> UserData { get; set; }

    }
}