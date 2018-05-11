using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmeciDoctoresV2.Models
{
    public class images
    {
        public string title { get; set; }
        public string url { get; set; }
        public string image { get; set; }
        public string category { get; set; }
    }

    public class galleries
    {
        public string name { get; set; }
        public List<images> pictures { get; set; }
    }
}