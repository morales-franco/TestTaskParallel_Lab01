using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppTest.Models
{
    public class DashboardVM
    {
        public List<string> Tags { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Posts { get; set; }

        public DashboardVM()
        {
            Tags = new List<string>();
            Categories = new List<string>();
            Posts = new List<string>();
        }
    }
}