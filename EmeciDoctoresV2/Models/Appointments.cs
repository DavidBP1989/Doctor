using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmeciDoctoresV2.Models
{
    public class Appointment
    {
         public int id;
         public  int timestamp;
         public string reason;
         public  string   remarks;
         public bool needs_payment;
         public double payment_amount;
    }

    public class ListAppoiments
    {
        public List<Appointment> List_Appoiments;
        public bool status;
        public int total;
    }
}