using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmeciDoctoresV2.Models
{
    public class PatientList
    {

        [Display(Name = "Nombre")]
        public string NombreApellido { get; set; }

       
   
        [Display(Name = "Ultima Consulta")]
        public string fecha { get; set; }

        [Display(Name = "idpaciente")]
        public string idpaciente { get; set; }

        public PatientList(string nombre, string fecha, string idpaciente)
        {
            this.NombreApellido = nombre;
            this.fecha = fecha;
            this.idpaciente = idpaciente;
        
        }

    }
}