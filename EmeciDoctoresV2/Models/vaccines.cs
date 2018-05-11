using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmeciDoctoresV2.Models
{
    public class vaccines
    {
         public int order ;
         public string code;
         public string name;
         public string prevents;
         public List<doses> doses;

         

    }

    public class doses
    {
        public int id { get; set; }
        public string name { get; set; }
        public string when { get; set; }
        public string date { get; set; }
          //public string token;
    }
}