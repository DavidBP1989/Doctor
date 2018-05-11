using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmeciDoctoresV2.Models;

namespace EmeciDoctoresV2.Controllers
{
    public class PacienteController : Controller
    {
        //
        // GET: /Paciente/
        emeciEntities _Db = new emeciEntities();
        public ActionResult Index(string notarj)
        {
            var valores= (from dt in _Db.DatosTarjeta
                  where dt.noTarjeta == notarj
                          orderby dt.Coordenada.Length,   dt.Coordenada.Substring(1)
                   select dt).ToList();

            ActionResult ar; 
            
            ar =  new RazorPDF.PdfResult(valores.ToList(), "DatosTarjeta");
            return ar;
           
           // return View();
        
        }


    


    }
}
