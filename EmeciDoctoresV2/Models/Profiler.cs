using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmeciDoctoresV2.Models
{
    public class Profiler
    {
        public Profiler()
        {
            this.generals = new Models.generals();
            this.address = new Models.address();
            this.contacts = new Models.contacts(); 
        }
        
        public generals generals {get;set;}
        public address address {get;set;}
        public contacts contacts { get; set; }

    }

    public class generals
    {
        public string full_name{get;set;}
        public string birthday { get; set; }
        public string sex { get; set; }
        public string life_era { get; set; }
        public string scholarship { get; set; }
    }

    public class address
    {

        public string addres { get; set; }
        public string colony { get; set; }
        public string zipcode { get; set; }
        public string country { get; set; }

    }

    public class contacts
    {
        public string phones { get; set; }
        public string office { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
    }

}