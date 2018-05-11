using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmeciDoctoresV2.Models
{
    public class RecordarPwd
    {

        
            [Required]
            [Display(Name = "Número Emeci")]
            public string UserName { get; set; }

          

  
    }
}