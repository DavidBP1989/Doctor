using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EmeciDoctoresV2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmeciDoctoresV2.Controllers
{
    public class ConsultaController : Controller
    {
        //
        // GET: /Comsulta/
        emeciEntities _Db = new emeciEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BuscaDiagnostico(string name)
        {
            int idmed = (int)Session["idmedico"];
            var diag = (from cd in _Db.catdiagnostico
                        where cd.idmedico == idmed && cd.nombre.Contains(name)
                        orderby cd.nombre
                        select cd).ToList();

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            
            string jsoncons = serializer.Serialize(diag.Select(
                d => new
                {
                    id=d.idcatdiagnostico,
                    nombre = d.nombre,
                    lineas = d.lineas
                    
                }
                ));

            return Content(jsoncons, "application/json");


        }

        [HttpPost]
        public ActionResult SaveDiagnostico(string nombre, string lineas)
        {

            EmeciDoctoresV2.Models.catdiagnostico c1 = new catdiagnostico();
            c1.idmedico = (int)Session["idmedico"];
            c1.lineas = lineas;
            c1.nombre = nombre;
            _Db.catdiagnostico.AddObject(c1);
            _Db.SaveChanges();

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string jsoncons = serializer.Serialize(new { status = "OK" });
            return Content(jsoncons, "application/json");

        }

        [HttpPost]
        public ActionResult SaveTratamiento(string nombre, string lineas)
        {

            EmeciDoctoresV2.Models.catrecetas c1 = new catrecetas();
            c1.idmedico = (int)Session["idmedico"];
            c1.lineas = lineas;
            c1.nombre = nombre;
            _Db.catrecetas.AddObject(c1);
            _Db.SaveChanges();
            
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string jsoncons = serializer.Serialize(new { status = "OK" });
            return Content(jsoncons, "application/json");

        }

        [HttpPost]
        public ActionResult BuscaTratamiento(string name)
        {
            int idmed = (int)Session["idmedico"];
            var diag = (from cd in _Db.catrecetas
                        where cd.idmedico == idmed && cd.nombre.Contains(name)
                        orderby cd.nombre
                        select new { id= cd.idcatreceta ,nombre = cd.nombre, lineas = cd.lineas }).ToList();

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            string jsoncons = serializer.Serialize(diag.Select(
                d => new
                {
                    id = d.id,
                    nombre = d.nombre,
                    lineas = d.lineas
                }
                ));

            return Content(jsoncons, "application/json");


        }

        [HttpPost]
        public ActionResult EliminaTratamiento(int id)
        {
            int idmed = (int)Session["idmedico"];
            var diag = (from cd in _Db.catrecetas
                        where cd.idmedico == idmed && cd.idcatreceta ==id
                        select cd).ToList();
            
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string strSuc="Error";
            if (diag.Count > 0)
            {
                _Db.DeleteObject(diag[0]);
                _Db.SaveChanges();
                strSuc = "OK";
            }

            string jsoncons = serializer.Serialize( new
                    {
                        success = strSuc
                        
                    }
                    );

            return Content(jsoncons, "application/json");


        }

        [HttpPost]
        public ActionResult EliminaDiagnostico(int id)
        {
            int idmed = (int)Session["idmedico"];
            var diag = (from cd in _Db.catdiagnostico
                        where cd.idmedico == idmed && cd.idcatdiagnostico == id
                        select cd).ToList();

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string strSuc = "Error";
            if (diag.Count > 0)
            {
                _Db.DeleteObject(diag[0]);
                _Db.SaveChanges();
                strSuc = "OK";
            }

            string jsoncons = serializer.Serialize(new
            {
                success = strSuc

            }
                    );

            return Content(jsoncons, "application/json");


        }
        public ActionResult KeepAlive(int idmed=0)
        {
            if (idmed != 0) Session["idmedico"]=idmed;
            return Content("ok" + Session["idmedico"].ToString());
        }


        [HttpPost]
        public ActionResult SaveMargenes( int recetatop, int recetabottom , int recetaleft , int recetaright ,
            int consultatop , int consultabottom , int  consultaleft , int consultaright )
        {
            int idmed = (int)Session["idmedico"];
            var med = (from m in _Db.Medico where m.Idmedico == idmed select m).ToList();
            if (med.Count>0)
            {
                med.First().recetatop = recetatop;
                med.First().recetabottom = recetabottom;
                med.First().recetaleft = recetaleft;
                med.First().recetaright = recetaright;
                med.First().consultatop = consultatop;
                med.First().consultabottom = consultabottom;
                med.First().consultaleft = consultaleft;
                med.First().consultaright = consultaright;
              _Db.SaveChanges();

            }
            
            
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string jsoncons = serializer.Serialize(new { status = "OK" });
            return Content(jsoncons, "application/json");

        }

        public ActionResult Save_Margenes(int left, int top)
        {
            int idMed = (int)Session["idmedico"];
            var med = (from m in _Db.Medico
                       where m.Idmedico == idMed
                       select m).ToList();

            if (med.Count > 0) 
            {
                med.First().recetaleft = left;
                med.First().recetatop = top;
                _Db.SaveChanges();
            }

            var serealizer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = serealizer.Serialize(new { status = "OK" });

            return Content(json, "application/json");
        }

        


    }
}
