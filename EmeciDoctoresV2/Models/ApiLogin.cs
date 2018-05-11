using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmeciDoctoresV2.Models
{
    public class ApiLoginResp
    {

       
     public bool success{ get; set; }

     public string  token{ get; set; }

    }

    public class ApiLogin
    { 
    
        public  string user{ get; set; }
        public string password{ get; set; }

        public string coord { get; set; }
        public string value{ get; set; }

    }
}