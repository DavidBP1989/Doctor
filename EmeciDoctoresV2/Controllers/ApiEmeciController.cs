using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using EmeciDoctoresV2.Models;
using System.IO;
using System.Threading.Tasks;
using System.Data.Objects;

namespace EmeciDoctoresV2.Controllers
{
    public class EmeciController : ApiController
    {

        emeciEntities _Db = new emeciEntities();

        //Login API
        [HttpPost]
        public ApiLoginResp login(ApiLogin lg)
        {
            var pac = from p in _Db.Paciente
                      join r in _Db.Registro on p.IdRegistro equals r.idRegistro
                      where r.Emeci == lg.user && r.clave == lg.password
                      select new { r.idRegistro, p.idPaciente, r.FechaExpiracion };
            ApiLoginResp lgr = new ApiLoginResp();
            lgr.success = false;
            if (pac.Count() > 0 && pac.First().FechaExpiracion.Value.Date >= DateTime.Now.Date)
            {
                var dat = from dt in _Db.DatosTarjeta
                          where dt.noTarjeta == lg.user && dt.Coordenada == lg.coord.Replace(",", "") && dt.Dato == lg.value
                          select new { dt.noTarjeta };
                if (dat.Count() > 0)
                {
                    lgr.success = true;
                    lgr.token = System.Guid.NewGuid().ToString().Replace("-", "");
                    System.Web.HttpContext.Current.Cache.Insert(lgr.token, pac.First().idPaciente.ToString(), null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(20));
                }
            }
            return lgr;
        }

        //Trae Consultas de la Base de Datos
        [HttpGet]
        public HttpResponseMessage appointments(string token)
        {


            int i = GetToken(token);

            if (i > 0)
            {
                var appo = from c in _Db.Consultas
                           where c.idpaciente == i
                           orderby c.Fecha descending
                           select c;


                var response = new
                {
                    success = true,
                    total = appo.Count(),
                    appointments = appo.ToList().Select(d => new
                           {
                               id = d.idconsulta,
                               date = d.Fecha.Value.ToString("dd/MMM/yyyy"),
                               reason = d.motivo,
                               remarks = "Cons. Ant. " + d.Fecha.Value.ToString("dd/MMM/yyyy"),
                               needs_payment = false,
                               payment_amount = 0
                           })
                };
               
                 var r =   Request.CreateResponse(HttpStatusCode.OK, response);

                 return r;
            }
            else
            {
                var r = Request.CreateResponse(HttpStatusCode.Forbidden, new {success= false,
                        error_type= token,
                        error_message = "Invalid or expired token"  });

                return r;

            }

        }

         [HttpGet] //consulta by id
        public HttpResponseMessage appointments(string token, int id)
        {
             int i = GetToken(token);

             if (i > 0)
             {
                 var appo = from c in _Db.Consultas
                            where c.idconsulta == id
                            select c;


                 // diagnosticos
                 DateTime Fecha;
                 Fecha = appo.First().Fecha.Value.Date;
                 var diag = from d in _Db.Diagnosticos
                            where d.idpaciente == i && EntityFunctions.TruncateTime(d.Fecha) == EntityFunctions.TruncateTime(Fecha)
                            orderby d.Fecha descending, d.idconsulta descending
                            select d;

                 string strDiag = "";
                 
                 if (diag.Count() > 0) strDiag = diag.First().Lineas;


                 // recetas
                 var rec = from re in _Db.Recetas
                           where re.idpaciente == i && EntityFunctions.TruncateTime(re.Fecha) == EntityFunctions.TruncateTime(Fecha)
                           orderby re.Fecha descending, re.idconsulta descending
                           select re;
                 string strRecetas = "";

                 if (rec.Count() > 0)
                     strRecetas = rec.First().Lineas;


                 //Estudios Lab
                 var estLab = from el in _Db.EstudiosLab
                              where el.idpaciente == i && EntityFunctions.TruncateTime(el.Fecha) == EntityFunctions.TruncateTime(Fecha)
                              orderby el.Fecha descending, el.idconsulta descending
                              select el;

                 string strEstLab = "";
                 if (estLab.Count() > 0) strEstLab = estLab.First().Lineas;

                 // Estudios Gab
                 var estGab = from eG in _Db.EstudiosGab
                              where eG.idpaciente == i && EntityFunctions.TruncateTime(eG.Fecha) == EntityFunctions.TruncateTime(Fecha)
                              orderby eG.Fecha descending, eG.idconsulta descending
                              select eG;

                 string strEstGab = "";

                 if (estGab.Count() > 0) strEstGab = estGab.First().Lineas;


                 var ap = new
                 {
                     success = true,
                     total = appo.Count(),
                     date = appo.First().Fecha.Value.ToString("dd/MMM/yyyy"),
                     measurements = new {weight= appo.First().Peso.ToString() + " kg.",
                            height=  appo.First().Altura.ToString() + " M.",
                            bmi= (appo.First().Altura /(appo.First().Peso*appo.First().Peso)).ToString() + " kg/m2",
                            temp= appo.First().Temperatura.ToString() + " C",
                            pressure= appo.First().TensionArterial.ToString() +  "/" +  appo.First().TensionArterialB.ToString()  + "mmHg",
                            head_circunference= appo.First().perimetroCefalico.ToString() + " cm",
                            heart_rate= appo.First().FrecuenciaCardiaca + " x min",
                            respiratory_rate = appo.First().FrecuenciaRespiratoria + " x min"
                     },

                     consultation = new {  diagnostic = formateaCadena(strDiag) ,
                                           treatment = formateaCadena(strRecetas),
                                        reason = appo.First().motivo, phyisical_exploration = appo.First().SignosSintomas1 + appo.First().SignosSintomas2 + appo.First().SignosSintomas3
                                        ,
                                          preventive_measures = appo.First().MedidasPreventivas,
                                          remarks = appo.First().observaciones
                     },

                     studies = new { laboratory = formateaCadena(strEstLab), desk = formateaCadena(strEstGab) }


                 };

                 var r = Request.CreateResponse(HttpStatusCode.OK, ap);

                 return r;

             }
             else
             {
                 var r = Request.CreateResponse(HttpStatusCode.Forbidden, new
                 {
                     success = false,
                     error_type = token,
                     error_message = "Invalid or expired token"
                 });

                 return r;
             }
        
        }
       
        // Obtiene todas las vacunas del expediente
         [HttpGet]
         public HttpResponseMessage vaccines(string token)
         { 
             int i = GetToken(token);
             var vac = from v in _Db.Vacunas
                       where v.idpaciente == i
                       select v;
             if (i > 0)
             {

                 List<vaccines> ls = getVaccines();
                 // asigno la fecha de la vacuna
                 foreach( Vacunas vcs in vac.ToList())
                 {
                    foreach( vaccines vc in ls)
                    {   foreach( doses ds in vc.doses)
                       {
                           if (ds.id.ToString() == vcs.codigo)
                           {
                               ds.date = vcs.Fecha.Value.ToString("dd/MMM/yyyy");
                           }
                       }
                    
                    }
                 }

                 var va = new
                 {
                     success = true,
                     total = ls.Count(),
                     timestamp = 0,
                     vaccines = ls


                 };

                 var r = Request.CreateResponse(HttpStatusCode.OK, va);

                 return r;


             }
             else
             { 
              var r = Request.CreateResponse(HttpStatusCode.Forbidden, new
                 {
                     success = false,
                     error_type = token,
                     error_message = "Invalid or expired token"
                 });

                 return r;
             }
         
         }

         // Actuliza la vacuna del expediente
        [HttpPost] 
         public HttpResponseMessage vaccines(string token, doses  id)
         {

             savelog("vaccines: " + id.id + " fecha: " + id.date  );

             int i = GetToken(token);
             string vcod = id.id.ToString();
            
             if (i > 0)
             
             {
             
                 try {
                 var vac = from v in _Db.Vacunas
                           where v.idpaciente == i && v.codigo == vcod
                           select v;

                 if (vac.Count() > 0)
                 {
                     vac.First().Fecha = new DateTime(int.Parse(id.date.Substring(0,4)),int.Parse(id.date.Substring(5,2)), int.Parse(id.date.Substring(8,2)));

                 }
                 else
                 {
                    Vacunas vacu = new Models.Vacunas();
                    _Db.Vacunas.AddObject(vacu);
                    vacu.Fecha = new DateTime(int.Parse(id.date.Substring(0,4)),int.Parse(id.date.Substring(5,2)),int.Parse(id.date.Substring(8,2)));
                    vacu.idpaciente = i;
                    vacu.codigo = id.id.ToString();
                 
                 }
                 _Db.SaveChanges();

                 var va = new { success = true };

                 var r = Request.CreateResponse(HttpStatusCode.OK, va);

                 return r;
                 }
                catch (Exception ex)
                 {
                     var re = Request.CreateResponse(HttpStatusCode.BadRequest, new
                     {
                         success = false,
                         error_type = token,
                         error_message = ex.Message
                     });

                     return re;
                }


             }
             else
             {
                 var r = Request.CreateResponse(HttpStatusCode.Forbidden, new
                 {
                     success = false,
                     error_type = token,
                     error_message = "Invalid or expired token"
                 });

                 return r;
             }

         }

        //GET Images Gallery
        [HttpGet]
        public HttpResponseMessage gallery(string token)
         {

             int i = GetToken(token);
           

             if (i > 0)
             {
                 string[] strFiles;
                 string sName;
                
                    int  pPunto;
                    int pDash;

                 List<images> lsG = new List<Models.images>();
                 List<images> lsD = new List<Models.images>();
                 List<images> lsP = new List<Models.images>();

                List<images> lsE = new List<Models.images>();
                List<images> lsM = new List<Models.images>();
                List<images> lsV = new List<Models.images>();

                images im;
                 string pathimages;
                 string spathimages = System.Configuration.ConfigurationManager.AppSettings["vPathImges"].ToString();
                 pathimages = System.Configuration.ConfigurationManager.AppSettings["PathImges"].ToString();
                 strFiles =Directory.GetFiles(pathimages + i.ToString() + "\\");
                 //Galeria
                foreach(string strF in strFiles)
                     {                
                        pPunto = strF.LastIndexOf(".");
                        pDash = strF.LastIndexOf("\\");
                        if (pPunto - pDash - 1 > 0)
                            sName = strF.Substring(pDash + 1, pPunto - pDash - 1);
                        else
                            sName = "";
                        if (  strF.IndexOf(".jpg") > 0)
                        {
                            im = new images();
                            lsG.Add(im);                       
                            im.title = sName;
                            im.url =  i.ToString() + "/" + sName + ".jpg";
                            im.url = spathimages + im.url.Replace("//", "/");
                        }
                     }
               //  Recetas
              if (Directory.Exists(pathimages + i.ToString() + "\\imgRec\\"))
             {
                strFiles = Directory.GetFiles(pathimages + i.ToString() + "\\imgRec\\");
                foreach (string strF in strFiles)
                {
                    pPunto = strF.LastIndexOf(".");
                    pDash = strF.LastIndexOf("\\");                   
                    if (pPunto - pDash - 1 > 0)
                        sName = strF.Substring(pDash + 1, pPunto - pDash - 1);
                    else
                        sName = "";
                    if (strF.IndexOf(".jpg") > 0)
                    {
                        im = new images();
                        lsP.Add(im);
                        im.title = sName;
                        im.url = i.ToString() + "/imgRec/" + sName + ".jpg";
                        im.url = spathimages + im.url.Replace("//", "/");
                    }
                }
            }
                 //Diagnosticos
            if (Directory.Exists(pathimages + i.ToString() + "\\imgDiag\\"))
            {
                strFiles = Directory.GetFiles(pathimages + i.ToString() + "\\imgDiag\\");
                foreach (string strF in strFiles)
                {
                    pPunto = strF.LastIndexOf(".");
                    pDash = strF.LastIndexOf("\\");                  
                    if (pPunto - pDash - 1 > 0)
                        sName = strF.Substring(pDash + 1, pPunto - pDash - 1);
                    else
                        sName = "";
                    if (strF.IndexOf(".jpg") > 0)
                    {
                        im = new images();
                        lsD.Add(im);
                        im.title = sName;
                        im.url = i.ToString() + "/imgDiag/" + sName + ".jpg";
                        im.url = spathimages + im.url.Replace("//", "/");
                    }
                }
            }

                //Estudios
                if (Directory.Exists(pathimages + i.ToString() + "\\imgEst\\"))
                {
                    strFiles = Directory.GetFiles(pathimages + i.ToString() + "\\imgEst\\");
                    foreach (string strF in strFiles)
                    {
                        pPunto = strF.LastIndexOf(".");
                        pDash = strF.LastIndexOf("\\");
                        if (pPunto - pDash - 1 > 0)
                            sName = strF.Substring(pDash + 1, pPunto - pDash - 1);
                        else
                            sName = "";
                        if (strF.IndexOf(".jpg") > 0)
                        {
                            im = new images();
                            lsE.Add(im);
                            im.title = sName;
                            im.url = i.ToString() + "/imgEst/" + sName + ".jpg";
                            im.url = spathimages + im.url.Replace("//", "/");
                        }
                    }
                }
                // Medicamentos
                if (Directory.Exists(pathimages + i.ToString() + "\\imgMed\\"))
                {
                    strFiles = Directory.GetFiles(pathimages + i.ToString() + "\\imgMed\\");
                    foreach (string strF in strFiles)
                    {
                        pPunto = strF.LastIndexOf(".");
                        pDash = strF.LastIndexOf("\\");
                        if (pPunto - pDash - 1 > 0)
                            sName = strF.Substring(pDash + 1, pPunto - pDash - 1);
                        else
                            sName = "";
                        if (strF.IndexOf(".jpg") > 0)
                        {
                            im = new images();
                            lsM.Add(im);
                            im.title = sName;
                            im.url = i.ToString() + "/imgMed/" + sName + ".jpg";
                            im.url = spathimages + im.url.Replace("//", "/");
                        }
                    }
                }
                // Vacunas
                if (Directory.Exists(pathimages + i.ToString() + "\\imgVac\\"))
                {
                    strFiles = Directory.GetFiles(pathimages + i.ToString() + "\\imgVac\\");
                    foreach (string strF in strFiles)
                    {
                        pPunto = strF.LastIndexOf(".");
                        pDash = strF.LastIndexOf("\\");
                        if (pPunto - pDash - 1 > 0)
                            sName = strF.Substring(pDash + 1, pPunto - pDash - 1);
                        else
                            sName = "";
                        if (strF.IndexOf(".jpg") > 0)
                        {
                            im = new images();
                            lsV.Add(im);
                            im.title = sName;
                            im.url = i.ToString() + "/imgVac/" + sName + ".jpg";
                            im.url = spathimages + im.url.Replace("//", "/");
                        }
                    }
                }
                //


                // galeria normal
                List<galleries> pics  = new List<galleries>();
            galleries g = new galleries();
            g.name = "pictures";
            g.pictures = lsG;
            pics.Add(g);

            // Recetas
             g = new galleries();
             g.name = "prescriptions";
            g.pictures = lsP;
            pics.Add(g);

            // Diagnosticos
             g = new galleries();
             g.name = "diagnostics";
            g.pictures = lsD;
            pics.Add(g);

                // Estudios
                g = new galleries();
                g.name = "estudies";
                g.pictures = lsE;
                pics.Add(g);

                // Medicamentos
                g = new galleries();
                g.name = "medicines";
                g.pictures = lsM;
                pics.Add(g);

                // Vacunas
                g = new galleries();
                g.name = "vaccines";
                g.pictures = lsV;
                pics.Add(g);

                var va = new
            {
                success = true,
                galleries = pics // new {
                //    diagnostics = lsD,
                //    pictures = lsG,
                //    prescriptions = lsP
                //}

            };
                 var r = Request.CreateResponse(HttpStatusCode.OK, va);
                 return r;
             }
             else
             {
                 var r = Request.CreateResponse(HttpStatusCode.Forbidden, new
                 {
                     success = false,
                     error_type = "token",
                     error_message = "Invalid or expired token"
                 });

                 return r;
             }

         }


        //Post Images Gallery
        public HttpResponseMessage gallery(string token, Models.images im)
        {

            int i = GetToken(token);
            string pathimages;
               
            pathimages = System.Configuration.ConfigurationManager.AppSettings["PathImges"].ToString();

            if (i > 0)
            {

                string nombreimg=im.title;
                string strimage = im.image;
               System.Drawing.Image img = Base64ToImage(strimage);
                int idemeci = i;
                string pathemeci = pathimages + idemeci.ToString();
                switch (im.category)
                {
                    case "diagnostics": pathemeci += "\\imgDiag\\"; break;
                    case "prescriptions": pathemeci += "\\imgRec\\"; break;

                    case "estudies": pathemeci += "\\imgEst\\"; break;
                    case "medicines": pathemeci += "\\imgMed\\"; break;
                    case "vaccines": pathemeci += "\\imgVac\\"; break;

                }
                nombreimg = im.title;
                //pathemeci = System.Web.HttpContext.Current.Server.MapPath(pathemeci);
                nombreimg = System.Text.RegularExpressions.Regex.Replace(nombreimg, @"[^\w\.@-]", "_") + ".jpg";

                if (!(System.IO.Directory.Exists(pathemeci)))
                {
                    System.IO.Directory.CreateDirectory(pathemeci);
                }
                if (!System.IO.File.Exists(pathemeci + "/" + nombreimg))
                {
                    pathemeci = pathemeci + "\\" + nombreimg;
                }
                else
                {
                    if (!System.IO.File.Exists(pathemeci + "/2" + nombreimg))
                    {
                        pathemeci = pathemeci + "/2" + nombreimg;
                    }
                    else
                    {
                        if (!System.IO.File.Exists(pathemeci + "/3" + nombreimg))
                        {
                            pathemeci = pathemeci + "/3" + nombreimg;
                        }
                        else
                        {
                            pathemeci = pathemeci + "/4" + nombreimg;
                        }
                    }
                }

                if (img != null)
                {
                    try
                    {
                        img.Save(pathemeci);

                        var va = new { success = true };

                        var r = Request.CreateResponse(HttpStatusCode.OK, va);

                        return r;

                    }
                    catch (Exception ex)
                    { // mandar error

                        var r = Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, new
                        {
                            success = false,
                            error_type = "token",
                            error_message = "Invalid or Image no Support " + ex.Message
                        });
                        return r;

                    }
                }
                else
                {
                    var r = Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, new
                    {
                        success = false,
                        error_type = "token",
                        error_message = "Invalid or Image no Support"
                    });
                    return r;
                }


                

            }
            else
            {
                var r = Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    success = false,
                    error_type = "token",
                    error_message = "Invalid or expired token"
                });

                return r;
            }

        }

        //Get Profile Paciente
        [HttpGet]
        public HttpResponseMessage profile(string token)
        {


            int i = GetToken(token);

            if (i > 0)
            {
                var pac = (from p in _Db.Paciente
                          join re in _Db.Registro on p.IdRegistro equals re.idRegistro
                           where p.idPaciente == i
                           select new {re.Nombre, re.Apellido, p.FechaNacimiento,p.Sexo, re.Domicilio, re.Colonia,re.CodigoPostal, re.IdPais,
                            re.Telefono,re.TelefonoCel,re.Emails}).FirstOrDefault();


                int meses = ((DateTime.Now.Year - pac.FechaNacimiento.Value.Year) * 12) + DateTime.Now.Month - pac.FechaNacimiento.Value.Month;
                          
                int PacAnos = (int)(meses / 12);
                int PacMeses =  meses % 12;
                Profiler prof = new Profiler();

                prof.generals.full_name = pac.Nombre + " " + pac.Apellido;
                            prof.generals.birthday = pac.FechaNacimiento.Value.ToString("yyyy/MM/dd");
                            prof.generals.sex = pac.Sexo =="F"? "Femenino":"Masculino";
                            prof.generals.life_era = PacAnos.ToString() + " Años " + PacMeses.ToString() + " Meses ";

                
                prof.address.addres = pac.Domicilio;
                prof.address.colony = pac.Colonia;
                prof.address.zipcode = pac.CodigoPostal;
                prof.address.country = pac.IdPais;
                
                prof.contacts.phones = pac.Telefono;
                prof.contacts.office = "";
                prof.contacts.mobile = pac.TelefonoCel;
                prof.contacts.email = pac.Emails;

                var response = new
                {
                    success = true,
                    profile = new {
                        
                        generals = prof.generals,
                        address = new {
                                address=prof.address.addres,
                                colony= prof.address.colony,
                                zipcode = prof.address.zipcode,
                                country= prof.address.country
                            },
                       contacts = prof.contacts   
    
                        }
                };

                var r = Request.CreateResponse(HttpStatusCode.OK, response);

                return r;
            }
            else
            {
                var r = Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    success = false,
                    error_type = "token",
                    error_message = "Invalid or expired token"
                });

                return r;

            }

        }

        // Post Profide Paciente
        [HttpPost]
        public HttpResponseMessage profile(string token, Profiler prof)
        {
            savelog("profile: " + prof.generals.full_name + " " +
              prof.generals.birthday + " " +

               
              prof.generals.sex +  " " + 

                prof.address.addres + " " + 
               prof.address.colony + " " + 
                 prof.address.zipcode + " " +
                prof.address.country + " " +

                 prof.contacts.phones + " " +
               prof.contacts.mobile + " " +
               prof.contacts.email + " " + prof.contacts.office); 

            
            int i = GetToken(token);

            if (i > 0)
            {
                var pac = (from p in _Db.Paciente
                           where p.idPaciente == i
                           select p).FirstOrDefault();
                
                int idreg = pac.IdRegistro;

                var reg = ( from re  in _Db.Registro 
                            where re.idRegistro ==idreg select re).FirstOrDefault();


                if (prof.generals.full_name !=null) reg.Nombre = prof.generals.full_name;
                if (prof.generals.birthday != null) pac.FechaNacimiento = new DateTime(int.Parse(prof.generals.birthday.Substring(0,4)),
                                                        int.Parse(prof.generals.birthday.Substring(5, 2)),
                                                        int.Parse(prof.generals.birthday.Substring(8, 2)));


                if (prof.generals.sex != null) pac.Sexo = prof.generals.sex = (pac.Sexo ==  "Femenino" || pac.Sexo == "F") ? "F" : "M"; 
                
                if (prof.generals.scholarship != null) pac.Escolaridad = prof.generals.scholarship;

               

                if (prof.address.addres != null) reg.Domicilio = prof.address.addres;
                if (prof.address.colony != null) reg.Colonia = prof.address.colony;
                if (prof.address.zipcode != null) reg.CodigoPostal = prof.address.zipcode;
                if (prof.address.country != null) reg.IdPais = prof.address.country;

                if (prof.contacts.phones != null) reg.Telefono = prof.contacts.phones;
                if (prof.contacts.mobile != null) reg.TelefonoCel = prof.contacts.mobile;
                if (prof.contacts.office != null) pac.telefonooficina = prof.contacts.office;
                if (prof.contacts.email != null) reg.Emails = prof.contacts.email;

                _Db.SaveChanges();

                var response = new
                {
                    success = true,
                   
                };

                var r = Request.CreateResponse(HttpStatusCode.OK, response);

                return r;
            }
            else
            {
                var r = Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    success = false,
                    error_type = "token",
                    error_message = "Invalid or expired token"
                });

                return r;

            }

        }

        ///////////////////////////////////////////////////
        /// Metodos Privados
        //get token
        /// <summary>
        ///  para sacar el token con id paciente //////////
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private int GetToken(string token)
        {

            if (System.Web.HttpContext.Current.Cache[token] == null)
                return 0;
            else
                return int.Parse(System.Web.HttpContext.Current.Cache[token].ToString());

        }
        
        // get vaccins
        private List<vaccines> getVaccines()
        {
            List<vaccines> ls = new List<Models.vaccines>();
            //vacuna 1
            vaccines v = new vaccines();
            ls.Add(v);
            doses d = new doses();
            v.code = "1";
            v.name = "BCG";
            v.order=1;
            v.prevents ="Tuberculosis";
            v.doses = new List<doses>();
            v.doses.Add(d);
            d.id = 1;
            d.name = "única";
            d.when ="Al Nacer";

             //vacuna 2
            v = new vaccines();
            ls.Add(v);
            d = new doses();
            v.code = "2";
            v.name = "HB";
            v.order=2;
            v.prevents ="Hepatitis B";
            v.doses = new List<doses>();
            v.doses.Add(d);
            d.id = 2;
            d.name = "Primera Al Nacer";
            d.when ="Al Nacer";

            // vacuna 3
             d = new doses();
            v.doses.Add(d);
            d.id =3;
            d.name = "Hepatitis B";
            d.when = "Segunda 2 Meses";

             // vacuna 4
             d = new doses();
            v.doses.Add(d);
            d.id =4;
            d.name = "Hepatitis B";
            d.when = "Tercera 6 Meses";

            
             //vacuna 5
            v = new vaccines();
            ls.Add(v);
            d = new doses();
            v.code = "3";
            v.name = "PENTAVALENTE";
            v.order=3;
            v.prevents ="Difteria, Tosferina, Tétanos, Poliovirus Inactivado, Haemophilus tipo b";
            v.doses = new List<doses>();
            v.doses.Add(d);
            d.id = 5;
            d.name = "PENTAVALENTE";
            d.when ="Primera 2 Meses";

              // vacuna 6
             d = new doses();
            v.doses.Add(d);
            d.id =6;
            d.name = "DPT + IPV + Hib";
            d.when = "Segunda 4 Meses";

            // vacuna 7
             d = new doses();
            v.doses.Add(d);
            d.id =7;
            d.name = "DPT + IPV + Hib";
            d.when = "Tercera 6 Meses";

             //vacuna 8
            v = new vaccines();
            ls.Add(v);
            d = new doses();
            v.code = "4";
            v.name = "RV";
            v.order=4;
            v.prevents ="Rotavirus";
            v.doses = new List<doses>();
            v.doses.Add(d);
            d.id = 8;
            d.name = "RV";
            d.when ="Primera 2 Meses";

             // vacuna 9
             d = new doses();
            v.doses.Add(d);
            d.id =9;
            d.name = "RV";
            d.when = "Segunda 4 Meses";

             // vacuna 10
             d = new doses();
            v.doses.Add(d);
            d.id =10;
            d.name = "RV";
            d.when = "Tercera 6 Meses";
		
		 //vacuna 11
            v = new vaccines();
            ls.Add(v);
            d = new doses();
            v.code = "11";
            v.name = "Neumococo";
            v.order=11;
            v.prevents ="Neumococo";
            v.doses = new List<doses>();
            v.doses.Add(d);
            d.id = 11;
            d.name = "Neumococo";
            d.when ="Primera 2 Meses";

            // vacuna 12
             d = new doses();
            v.doses.Add(d);
            d.id =12;
            d.name = "RV";
            d.when = "Segunda 4 Meses";

             // vacuna 13
             d = new doses();
            v.doses.Add(d);
            d.id =13;
            d.name = "Neumococo";
            d.when = "Tercera 6 Meses";

             // vacuna 14
             d = new doses();
            v.doses.Add(d);
            d.id =14;
            d.name = "Neumococo";
            d.when = "Refuerzo 15 Meses";
            
            //vacuna 15
            v = new vaccines();
            ls.Add(v);
            d = new doses();
            v.code = "15";
            v.name = "HA";
            v.order=15;
            v.prevents ="Hepatitis A";
            v.doses = new List<doses>();
            v.doses.Add(d);
            d.id = 15;
            d.name = "Hepatitis A";
            d.when ="Primera 1 Año";

             // vacuna 16
             d = new doses();
            v.doses.Add(d);
            d.id =16;
            d.name = "Hepatitis A";
            d.when = "Segunda 18 Meses";

            //vacuna 17
            v = new vaccines();
            ls.Add(v);
            d = new doses();
            v.code = "17";
            v.name = "Varicela";
            v.order=17;
            v.prevents ="Varicela";
            v.doses = new List<doses>();
            v.doses.Add(d);
            d.id = 17;
            d.name = "Varicela";
            d.when ="Primera 1 Año";

             // vacuna 18
             d = new doses();
            v.doses.Add(d);
            d.id =18;
            d.name = "Varicela";
            d.when = "Segunda 13 Años";

             //vacuna 19
            v = new vaccines();
            ls.Add(v);
            d = new doses();
            v.code = "19";
            v.name = "Triple Viral";
            v.order=19;
            v.prevents ="Sarampión, Parotiditis y Rubéola";
            v.doses = new List<doses>();
            v.doses.Add(d);
            d.id = 19;
            d.name = "Triple Viral";
            d.when ="Primera 1 Año";

             // vacuna 20
             d = new doses();
            v.doses.Add(d);
            d.id =20;
            d.name = "Triple Viral";
            d.when = "Refuerzo 6 Años";


             //vacuna 21
            v = new vaccines();
            ls.Add(v);
            d = new doses();
            v.code = "21";
            v.name = "VPH";
            v.order=21;
            v.prevents ="Virus del Papiloma Humano";
            v.doses = new List<doses>();
            v.doses.Add(d);
            d.id = 21;
            d.name = "Virus del Papiloma Humano";
            d.when ="Primera 9 Años";

             // vacuna 22
             d = new doses();
            v.doses.Add(d);
            d.id =22;
            d.name = "Virus del Papiloma Humano";
            d.when = "Segunda 2 Meses Después";

             // vacuna 23
             d = new doses();
            v.doses.Add(d);
            d.id =23;
            d.name = "Virus del Papiloma Humano";
            d.when = "Tercera 6 Meses Después";


               //vacuna 24
            v = new vaccines();
            ls.Add(v);
            d = new doses();
            v.code = "24";
            v.name = "Gripe";
            v.order=24;
            v.prevents ="Influenza";
            v.doses = new List<doses>();
            v.doses.Add(d);
            d.id = 24;
            d.name = "Gripe";
            d.when ="Primera 6 Años";

             // vacuna 25
             d = new doses();
            v.doses.Add(d);
            d.id =25;
            d.name = "Gripe";
            d.when = "Segunda 7 Meses";

             // vacuna 26
             d = new doses();
            v.doses.Add(d);
            d.id =26;
            d.name = "Gripe";
            d.when = "Refuerzo Anual";

            //Otra
            return ls;
        }


        // leer base64
        public System.Drawing.Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);

            System.IO.MemoryStream ms = new System.IO.MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            return image;
        }
    
        private void savelog(string strlog)
        {
            try {
               using (System.IO.StreamWriter file = new System.IO.StreamWriter(System.Configuration.ConfigurationManager.AppSettings["RutaLogs"], true))
                {
                    file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + " " +   strlog + "\r\n");
                }
            
            }
            catch
            { 
            
            }
        }

        //quita numeros
        string formateaCadena(string cadena)
        {
            string strCad = "";
            foreach (string c in cadena.Replace("\r\n", "\r").Split(new string[] { "\r" }, StringSplitOptions.None))
            {
                if (c.Length > 2)
                    strCad += c.Substring(2) + "<BR/>";
            }
            return strCad;
        }

    }
}
