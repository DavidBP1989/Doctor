using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Configuration;
using EmeciDoctoresV2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Objects;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;
using System.Data.Objects.SqlClient;

namespace EmeciDoctoresV2.Controllers
{
    public class MedicoController : Controller
    {
        public MedicoController()
          {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-MX");

            }
        //
        // GET: /Login/
      emeciEntities  _Db = new emeciEntities();


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        //recoradar pwd
        public ActionResult RecordarPwd()
        {
            return View();
        }


        public ActionResult pacientes()
        {
            if (Session["idmedico"] == null) return Redirect("~/medico/login");

            int idmed = (int)Session["idmedico"];

            string notar = Session["notarjeta"].ToString();

            var con = from p in _Db.Paciente 
                      join co in _Db.Consultas on p.idPaciente equals co.idpaciente 
                      where co.idmedico == idmed 
                      group co by co.idpaciente into g
                      select new { idpaciente = g.Key, fecha = g.Max(t => t.Fecha)};

            

            var med = (from r in _Db.Registro
                      join p in _Db.Paciente on r.idRegistro equals p.IdRegistro
                      join co in con on p.idPaciente equals co.idpaciente
                    //  where r.Emeci.IndexOf(notar) >= 0
                       orderby r.Nombre
                       select new { r.Emeci, nombreapellido = r.Nombre + " " + r.Apellido, fecha = co.fecha.Value, p.idPaciente });


            var np = from pn in _Db.Consultas where pn.idmedico == idmed select pn.idpaciente;

            var med2 = (from r2 in _Db.Registro
                        join p2 in _Db.Paciente on r2.idRegistro equals p2.IdRegistro
                        where r2.Emeci.StartsWith(notar) && ! np.Contains(p2.idPaciente)
                        select new { r2.Emeci, nombreapellido =  r2.Nombre + " " + r2.Apellido, fecha = DateTime.Now, p2.idPaciente }
                           );

            var lista =  med.Union(med2).ToList().OrderBy(t=> t.nombreapellido);


            var newresult = lista.Select(T => new PatientList
            (
                  T.nombreapellido.ToUpper(),
                            T.fecha.ToString("yyyy-MMM-dd"),
                            T.idPaciente.ToString()
            ));


            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            ViewBag.jsoncons = serializer.Serialize(newresult.ToList().ConvertAll(d => d.NombreApellido));

            ViewBag.jsoncons2 = serializer.Serialize(newresult);



            ViewBag.PatientsRestult = newresult;

           

            getMedico();

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginMedico lm)
        {
            var med = from r in _Db.Registro
                      join m in _Db.Medico on r.idRegistro equals m.IdRegistro
                      where lm.UserName == r.Emeci && r.clave == lm.Password && r.Status =="V"
                      select new { m.Idmedico, r.Nombre, r.Apellido };
            if (med.ToList().Count > 0)
            {
                Session["idmedico"] = med.ToList()[0].Idmedico;
                Session["password"] = lm.Password;
                Session["notarjeta"] = lm.UserName;
                return Redirect("~/Medico/pacientes");
            }
            else
            {
                ModelState.AddModelError("", "Tarjeta o contraseña incorrecta.");
            }
            return View();
        }


        [HttpPost]
        public ActionResult RecordarPwd(RecordarPwd lm)
        {
            var med = from r in _Db.Registro
                      join m in _Db.Medico on r.idRegistro equals m.IdRegistro
                      where lm.UserName == r.Emeci  && r.Status == "V"
                      select new { m.Idmedico, r.Nombre, r.Apellido,r.clave,r.Emails };
            if (med.ToList().Count > 0)
            {  
                    MailMessage eM = new MailMessage();
                    eM.Priority = MailPriority.Normal;
                    eM.IsBodyHtml = false;
                    eM.Subject = "Recordar Contraseña Emeci";
                    eM.To.Add(med.First().Emails);
                    eM.From = new MailAddress("info@emeci.com");

                    eM.Body = new string('*', 100) + "\r\n";
                    eM.Body += "Clave:" + med.First().clave + "\r\n";                  
                   
                    eM.Body += new string('*', 100);
                    Boolean bnd = false;
                    try
                    {

                        SmtpClient client = new SmtpClient();
                        client.Host = ConfigurationManager.AppSettings["Host"];
                        client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;
                        string m = ConfigurationManager.AppSettings["MailFrom"];
                        string p = ConfigurationManager.AppSettings["passwordMail"];
                        client.Credentials = new System.Net.NetworkCredential(m, p);

                        client.Send(eM);

                        bnd = true;
                    }
                    catch (Exception ex)
                    {
                        Response.Write(ex.Message);
                    }
                    if (bnd)
                        // Response.Redirect("exito.aspx");
                        return Redirect("~/Medico/Login");
                                       
                }
            else
            {
                ModelState.AddModelError("", "Tarjeta o Dato incorrecto.");
            }
            return View();
        }

        //David
        public void getMargins()
        {
            if (Session["idmedico"] != null)
            {
                int idDoctor = (int)Session["idmedico"];

                var doctor = (from d in _Db.Medico
                              where d.Idmedico == idDoctor
                              select d).ToList();

                if (doctor.Count > 0)
                {
                    var doc = doctor.First();
                    ViewBag.recetatop = doc.recetatop;
                    ViewBag.recetabottom = doc.recetabottom;
                    ViewBag.recetaright = doc.recetaright;
                    ViewBag.recetaleft = doc.recetaleft;
                }
            }
        }



       // trae la conuslta la primera vez
        public ActionResult Consulta(int idpac, bool fromGine = false)
        {  // Paciente

            if (Session["idmedico"] == null) return Redirect("~/medico/login") ;
            else
            {
               if (!fromGine)
                {
                    /*,_,*/
                    var patient = (from p in _Db.Paciente
                                   join r in _Db.Registro on p.IdRegistro equals r.idRegistro
                                   where p.idPaciente == idpac
                                   select new { p.FechaNacimiento, p.Sexo }).First();

                    if (patient.FechaNacimiento.HasValue && !string.IsNullOrEmpty(patient.Sexo))
                    {
                        var years = (DateTime.Now.Year - patient.FechaNacimiento.Value.Year);
                        if (years >= 12 && patient.Sexo == "F")
                        {
                            return RedirectToAction("ConsultaGinecologica", "medico", new { idPac = idpac });
                        }
                        else ViewBag.sexo = patient.Sexo;
                    }
                }
            }


            getMargins();
            var pac = from r in _Db.Registro
                      join p in _Db.Paciente on r.idRegistro equals p.IdRegistro
                      where p.idPaciente == idpac
                      select new {sexo=p.Sexo,  nombre = r.Nombre + " " + r.Apellido, p.FechaNacimiento, p.grupoRH, p.AlergiaMedicina, p.AlergiaOtros, p.Patologia,r.Emeci,r.FechaExpiracion };

            int meses = ((DateTime.Now.Year - pac.First().FechaNacimiento.Value.Year) * 12) + DateTime.Now.Month - pac.First().FechaNacimiento.Value.Month;

            var alg = from pat in _Db.Patologias
                      where pat.idpaciente == idpac && pat.Categoria == 5
                      select pat;


            ViewBag.Sexo = pac.First().sexo;
            ViewBag.idmed = Session["idmedico"];
            ViewBag.Pacnombre = pac.First().nombre;
           if(pac.First().FechaNacimiento != null) ViewBag.PacFechanacimiento = pac.First().FechaNacimiento.Value.ToString("dd/MMM/yyyy");
           
            ViewBag.PacAños = (int)(meses / 12);
            ViewBag.PacMeses =  meses % 12;
           if (pac.First().grupoRH != null) ViewBag.PacgrupoRH = pac.First().grupoRH;
            
            if (pac.First().FechaExpiracion != null) ViewBag.Vence = pac.First().FechaExpiracion.Value.ToString("dd/MMM/yyyy");

            if (pac.First().Patologia != null)
                ViewBag.PacPatologia = pac.First().Patologia;
            else ViewBag.PacPatologia = "";
            if (pac.First().AlergiaOtros != null)
                ViewBag.PacAlergiaOtros = pac.First().AlergiaOtros;
            else ViewBag.PacAlergiaOtros = "";

            if (alg.Count() > 0)
            {
                string al;
                al = "";
                foreach (var a in alg.ToList())
                {
                    al += a.Alergeno + "\r\n";
                }
                ViewBag.PacAlergiaMedicina = al;
            }
            else
                if (pac.First().AlergiaMedicina != null)
                    ViewBag.PacAlergiaMedicina = pac.First().AlergiaMedicina;
                else ViewBag.PacAlergiaMedicina = "";

            // Consultas
            var cons = from c in _Db.Consultas                       
                       where c.idpaciente == idpac 
                       orderby c.Fecha descending
                       select c;
            EmeciDoctoresV2.Models.Consultas c1;
            
            ViewBag.pacConsultas = cons.ToList();

            if (cons.Count() > 0)
            {
                c1 = cons.First();
                c1.Altura = cons.First().Altura == null ? 0 : cons.First().Altura;
            }
            else
            {
                c1 = new Consultas();
                c1.Altura = 0;
                c1.Peso = 0;
            }

            ViewBag.Cons1 = c1;
           
            string emeci = pac.First().Emeci;
            //tarjeta
            var tar = from tr in _Db.DatosTarjeta
                      where tr.noTarjeta == emeci
                      select new { tr.Dato,tr.Coordenada };
            
            ViewBag.NoTarjeta =emeci;
            ViewBag.TarjetaCord = tar.First().Coordenada;
            ViewBag.TarjetaDato = tar.First().Dato;

            // recetas
            var rec = from r in _Db.Recetas                       
                       where r.idpaciente == idpac && r.idpaciente == idpac
                      orderby r.Fecha descending, r.idconsulta descending
                       select r;
            if (rec.Count() > 0)
            {
                ViewBag.recetas = formateaCadena(rec.First().Lineas);//.Replace("\r\n", "\r").Split(new string[] { "\r" }, StringSplitOptions.None);
            }
            else ViewBag.recetas = "";

            // diagnosticos
            var diag = from d in _Db.Diagnosticos
                      where d.idpaciente == idpac && d.idpaciente == idpac
                      orderby d.Fecha descending ,d.idconsulta descending
                      select d;

            string strdiagExc = "";
            if (diag.Count() > 0)
            {
               
              strdiagExc = formateaCadena(diag.First().Lineas);//diag.First().Lineas.Replace("\r\n","\r").Split(new string[] { "\r" }, StringSplitOptions.None);
                
               
            }
            else ViewBag.Diagnosticos = "";

            var diagExc = (from d in _Db.DiagnosticosExc
                           where d.idconsulta == c1.idconsulta
                           select d).ToList();
            foreach (var de in diagExc)
            {
                strdiagExc += de.lineas + "<BR/>";
            }
            ViewBag.Diagnosticos = strdiagExc;


            //Estudios Lab
            var estLab = from el in _Db.EstudiosLab
                      where el.idpaciente == idpac
                         orderby el.Fecha descending, el.idconsulta descending
                      select el;

            if (estLab.Count() > 0)
                ViewBag.estLab = formateaCadena(estLab.First().Lineas);//.Replace("\r\n", "\r").Split(new string[] { "\r" }, StringSplitOptions.None);
            else
                ViewBag.estLab = "";

            // Estudios Gab
            var estGab = from eG in _Db.EstudiosGab
                         where eG.idpaciente == idpac
                         orderby eG.Fecha descending , eG.idconsulta descending
                         select eG;

            if (estGab.Count() > 0)
                ViewBag.estGab = formateaCadena(estGab.First().Lineas);//.Replace("\r\n", "\r").Split(new string[] { "\r" }, StringSplitOptions.None);
            else
                ViewBag.estGab = "";


            getMedico();


            ViewBag.idpac = idpac;

            var catestud = from c in _Db.CatCategoEstudios  
                        select c;

            var estud = from e in _Db.CatEstudios
                        select e;

            ViewBag.catEstudios = catestud.ToList();
            ViewBag.estudios = estud.ToList();

            //Patologias


            return View();

        }

        private void getMedico()
        {

            // medico
            int idmed = (int)Session["idmedico"];
            string stremeci  ="";
            var med = from r in _Db.Registro
                      join m in _Db.Medico on r.idRegistro equals m.IdRegistro
                      where m.Idmedico == idmed
                      select new { nombre = r.Nombre + " " + r.Apellido, r.Emeci, m.recetatop, m.recetabottom, m.recetaleft, m.recetaright, m.consultabottom
                      , m.consultaleft, m.consultaright, m.consultatop};

            stremeci =  med.First().Emeci;
            ViewBag.Mednombre = med.First().nombre;
            ViewBag.MedEmeci =stremeci;
            ViewBag.Emeci1 = stremeci.Split('-')[0];
            ViewBag.Emeci2 = stremeci.Split('-')[1];
            ViewBag.Emeci3 = maxEmeci(stremeci);


            /// margenes de receta //
             ViewBag.recetatop =  med.First().recetatop ;
             ViewBag.recetabottom =  med.First().recetabottom ;
             ViewBag.recetaleft = med.First().recetaleft;
             ViewBag.recetaright = med.First().recetaright;

             ViewBag.consultatop = med.First().consultatop;
             ViewBag.consultabottom = med.First().consultabottom;
             ViewBag.consultaleft = med.First().consultaleft;
             ViewBag.consultaright = med.First().consultaright; 

            /// /////////////////////
        }

        private string maxEmeci(string eme)
        {
            
            var mt = (from R in _Db.Registro
                     where R.Emeci.StartsWith(eme) == true && R.Tipo=="P"
                      orderby R.Emeci descending
                     select new {R.Emeci});
            int semeci = 0;
            if (mt.ToList().Count()>0)
              semeci = int.Parse(mt.First().Emeci.Split('-')[2]);
            semeci += 1;
            return semeci.ToString("000#");
        }


        // trae la conuslta la primera vez
        [HttpPost]
        public ActionResult Consulta(int idpac,string peso, string talla, string indicemasa, string temperatura, string tensionArterial, string tensionArterialB,string perimetroCef,
            string frecCard, string frecResp, string motivo, string expFisica, string MedPrev, string Observa, string tratamiento, string diagnostico, string estudioslab,
            string estudiosgab, string Pronostico , string alergiasmed,  
                        string alergiasotros,
                        string PacPatologia, string diagnosticosExc)
        {      
      
            //paciente 
            var paci = (from pa in _Db.Paciente
                       where pa.idPaciente == idpac
                       select pa).ToList();

            paci[0].AlergiaMedicina = alergiasmed;
            paci[0].AlergiaOtros = alergiasotros;
            paci[0].Patologia = PacPatologia;
            _Db.SaveChanges();

            //consulta
            DateTime f = DateTime.Now;
            EmeciDoctoresV2.Models.Consultas c1 = new Consultas();
            c1.Altura = float.Parse(talla);
            c1.Cabeza = float.Parse(perimetroCef);
            c1.Fecha = f;
            c1.FrecuenciaCardiaca = int.Parse(frecCard);
            c1.TensionArterial =  int.Parse(tensionArterial);
            c1.TensionArterialB = int.Parse(tensionArterialB);
            c1.FrecuenciaRespiratoria = int.Parse(frecResp);
            c1.idmedico = (int)Session["idmedico"];
            c1.idpaciente = idpac;
            c1.MedidasPreventivas = MedPrev;
            c1.motivo = motivo;
            c1.observaciones = Observa;
            c1.perimetroCefalico = float.Parse(perimetroCef);
            c1.Peso = float.Parse(peso);
            c1.SignosSintomas1 = expFisica;
            c1.Temperatura = float.Parse(temperatura);

            c1.Pronostico = Pronostico;
            
            _Db.Consultas.AddObject(c1);
            _Db.SaveChanges();

            // recetas
            EmeciDoctoresV2.Models.Recetas r = new Recetas();
            r.idconsulta = c1.idconsulta;
            r.idmedico = c1.idmedico;
            r.idpaciente = c1.idpaciente;
            r.Fecha = f;

            string strRec = "";
            int contrec = 0;
            string[] estrec = tratamiento.Split(new Char[] { '|' });
            
            foreach (string sr in estrec)
            {
                if (sr.Trim() != "")
                {
                    contrec += 1;
                    strRec += contrec.ToString("0#") + sr + "\r\n";
                }
            }

            r.Lineas = strRec;


           // r.Lineas = tratamiento.Replace("|", " \r");
            _Db.Recetas.AddObject(r);

            
            // diagnosticos
            EmeciDoctoresV2.Models.Diagnosticos d= new  Diagnosticos();
            d.idconsulta = c1.idconsulta;
            d.idmedico = c1.idmedico;
            d.idpaciente = idpac;
            d.Fecha = f;
            //d.Lineas = diagnostico.Replace("|", " \r");

            string strdiag = "";
            int contdiag = 0;
            string[] estdiag = diagnostico.Split(new Char[] { '|' });

            foreach (string sd in estdiag)
            {
                if (sd.Trim() != "")
                {
                    contdiag += 1;
                    strdiag += contdiag.ToString("0#") + sd + "\r\n";
                }
            }

            d.Lineas = strdiag;

            _Db.Diagnosticos.AddObject(d);


            //// Estudios Lab
            EmeciDoctoresV2.Models.EstudiosLab el = new EstudiosLab();
            el.idconsulta = c1.idconsulta;
            el.idmedico = c1.idmedico;
            el.idpaciente = idpac;
            el.Fecha = f;

            string strEs = "";
            int conta=0;
            string[] est = estudioslab.Split(new Char[] { '|' });

            foreach (string s in est)
            {
                if (s.Trim() != "")
                {
                    conta += 1;
                    strEs += conta.ToString("0#") + s + ":\r\n";
                }
            }
            
            el.Lineas = strEs;
            _Db.EstudiosLab.AddObject(el);
            
            //// Estudios Gab
            EmeciDoctoresV2.Models.EstudiosGab eg = new EstudiosGab();
            eg.idconsulta = c1.idconsulta;
            eg.idmedico = c1.idmedico;
            eg.idpaciente = idpac;
            conta = 0;
             est = estudiosgab.Split(new Char[] { '|' });
             strEs = "";
            foreach (string s in est)
            {
                if (s.Trim() != "")
                {
                    conta += 1;
                    strEs += conta.ToString("0#") + s + ":\r\n" ;
                }
            }

            eg.Lineas = strEs;
            eg.Fecha = f;
            
            _Db.EstudiosGab.AddObject(eg);

            // diagnosticos exlusivos
           if (diagnosticosExc != null) est = diagnosticosExc.Split(new Char[] { '|' });
            EmeciDoctoresV2.Models.DiagnosticosExc DE = new DiagnosticosExc();
            foreach (string s in est)
            {
                if (s.Trim() != "")
                {
                    DE = new DiagnosticosExc();
                    DE.idconsulta = c1.idconsulta;
                    DE.lineas = s;
                    
                    _Db.DiagnosticosExc.AddObject(DE);

                }
            }


            _Db.SaveChanges();

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string jsoncons = serializer.Serialize(new { status = "OK" });
            return Content(jsoncons, "application/json");

        }











        
        /*
            david
            inicio consultas ginecologica y obstetrica
        */
        public void InformacionPaciente(int idPac)
        {
            ViewBag.idPac = idPac;
            var pac = (from r in _Db.Registro
                       join p in _Db.Paciente on r.idRegistro equals p.IdRegistro
                       where p.idPaciente == idPac
                       select new {
                           nombre = r.Nombre + " " + r.Apellido,
                           p.FechaNacimiento,
                           p.grupoRH,
                           p.AlergiaMedicina,
                           p.AlergiaOtros,
                           p.Patologia,
                           r.Emeci,
                           r.FechaExpiracion,
                           p.Sexo
                       }).First();
            if (pac != null)
            {
                int months =
                    ((DateTime.Now.Year - pac.FechaNacimiento.Value.Year) * 12) +
                    DateTime.Now.Month - pac.FechaNacimiento.Value.Month;

                ViewBag.Pacnombre = pac.nombre;
                ViewBag.pacFechanacimiento = pac.FechaNacimiento.Value.ToString("dd/MMM/yyyy");
                ViewBag.PacAños = (int)(months / 12);
                ViewBag.PacMeses = months % 12;
                ViewBag.sexPatient = pac.Sexo;
                ViewBag.PacgrupoRH = pac.grupoRH;

                if (pac.FechaExpiracion.HasValue)
                    ViewBag.Vence = pac.FechaExpiracion.Value.ToString("dd/MMM/yyyy");
                ViewBag.PacPatologia = pac.Patologia ?? "";
                ViewBag.PacAlergiaOtros = pac.AlergiaOtros ?? "";
            }

            var alg = from pat in _Db.Patologias
                      where pat.idpaciente == idPac && pat.Categoria == 5
                      select pat;
            if (alg.Count() > 0)
            {
                string al = "";
                foreach (var a in alg.ToList())
                    al += a.Alergeno + "\r\n";
                ViewBag.PacAlergiaMedicina = al;
            }
            else ViewBag.PacAleriaMedicina = pac.AlergiaMedicina ?? "";

            //Informacion del medico
            if (Session["idmedico"] != null)
            {
                int idDoctor = (int)Session["idmedico"];
                var doctor = (from r in _Db.Registro
                              join m in _Db.Medico on r.idRegistro equals m.IdRegistro
                              where m.Idmedico == idDoctor
                              select r).First();

                ViewBag.Mednombre = doctor.Nombre + " " + doctor.Apellido;
                ViewBag.MedEmeci = doctor.Emeci;
            }
        }

        

        [HttpGet]
        public ActionResult ConsultaGinecologica(int idPac)
        {
            if (Session["idmedico"] == null) return Redirect("~/medico/login");
            InformacionPaciente(idPac);
            ViewBag.IdPaciente = idPac;

            //Todas las consultas
            var cnsult = (from c in _Db.Consultas
                          join g in _Db.ConsultaGinecologa
                          on c.idconsulta equals g.idconsulta into joined
                          from j in joined.DefaultIfEmpty()
                          where j != null && c.idpaciente == idPac
                          orderby c.Fecha descending
                          select new
                          {
                              gine = j,
                              c.Fecha,
                              c.Peso,
                              c.Altura,
                              c.Temperatura,
                              c.TensionArterial,
                              c.TensionArterialB,
                              c.motivo
                          }).ToList();

            List<SelectListItem> iConsultation = new List<SelectListItem>();
            foreach (var i in cnsult)
            {

                iConsultation.Add(new SelectListItem
                { Text = i.Fecha.Value.ToString("dddd dd MMMM yyyy, H:m"), Value = i.gine.idconsulta.ToString() });
            }

            ViewBag.consultation = iConsultation;
            ViewBag.consultationName = iConsultation.Count > 0 ? iConsultation[0].Text : "No hay consultas anteriores";

            //Mostrar ultima consulta
            var paciente = (from p in _Db.Paciente
                            where p.idPaciente == idPac
                            select p).First();

            bool xcons = cnsult.Count > 0 ? true : false;

            int idreg = 0;

            float peso = 0;
            float altura = 0;
            float masa = 0;

            idreg = paciente.IdRegistro;

            var reg = (from r in _Db.Registro where r.idRegistro==idreg select r).First();
            
            if (xcons && (cnsult[0].Peso.HasValue && cnsult[0].Altura.HasValue))
            {
                peso = cnsult[0].Peso.Value;
                altura = cnsult[0].Altura.Value;
                masa = peso / (altura * altura);
            }
            ViewBag.peso = peso > 0 ? peso.ToString() : "&nbsp;";
            ViewBag.talla = altura > 0 ? altura.ToString() : "&nbsp;";
            ViewBag.masa = masa > 0 ? Math.Round(masa, 2).ToString() : "&nbsp;";
            ViewBag.edadMenarca = paciente.EdadMenarca;
            ViewBag.temperatura = xcons ? (cnsult[0].Temperatura.HasValue ? cnsult[0].Temperatura.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.sistolica = xcons ? (cnsult[0].TensionArterial.HasValue ? cnsult[0].TensionArterial.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.distolica = xcons ? (cnsult[0].TensionArterialB.HasValue ? cnsult[0].TensionArterialB.Value.ToString() : "&nbsp;") : "&nbsp;";
            if (xcons && cnsult[0].gine.FechaUltimaMestruacion.HasValue)
                ViewBag.fechaM = cnsult[0].gine.FechaUltimaMestruacion.Value.ToString("dddd dd MMMM yyyy");
            else ViewBag.fechaM = "&nbsp;";
            ViewBag.gestas = xcons ? (cnsult[0].gine.Gestas.HasValue ? cnsult[0].gine.Gestas.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.paragestas = xcons ? (cnsult[0].gine.ParaGestas.HasValue ? cnsult[0].gine.ParaGestas.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.cesarea = xcons ? (cnsult[0].gine.Cesareas.HasValue ? cnsult[0].gine.Cesareas.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.aborto = xcons ? (cnsult[0].gine.abortos.HasValue ? cnsult[0].gine.abortos.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.nacido = xcons ? (cnsult[0].gine.RecienNacidosVivos.HasValue ? cnsult[0].gine.RecienNacidosVivos.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.mortinato = xcons ? (cnsult[0].gine.mortinatos.HasValue ? cnsult[0].gine.mortinatos.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.edadSexual = xcons ? (cnsult[0].gine.EdadInicioVidaSexual.HasValue ? cnsult[0].gine.EdadInicioVidaSexual.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.menacma = xcons ? cnsult[0].gine.menacma : "&nbsp;";
            ViewBag.oligomenorrea = xcons ? (cnsult[0].gine.oligonorrea.HasValue ? cnsult[0].gine.oligonorrea.Value : false) : false;
            ViewBag.proiomenorrea = xcons ? (cnsult[0].gine.Proiomenorrea.HasValue ? cnsult[0].gine.Proiomenorrea.Value : false) : false;
            ViewBag.hipermenorrea = xcons ? (cnsult[0].gine.Hipermenorrea.HasValue ? cnsult[0].gine.Hipermenorrea.Value : false) : false;
            ViewBag.dismenorrea = xcons ? (cnsult[0].gine.Dismenorrea.HasValue ? cnsult[0].gine.Dismenorrea.Value : false) : false;
            ViewBag.dispareunia = xcons ? (cnsult[0].gine.Dispareunia.HasValue ? cnsult[0].gine.Dispareunia.Value : false) : false;
            ViewBag.leucorrea = xcons ? (cnsult[0].gine.Leucorrea.HasValue ? cnsult[0].gine.Leucorrea.Value : false) : false;
            ViewBag.lactorrea = xcons ? (cnsult[0].gine.Lactorrea.HasValue ? cnsult[0].gine.Lactorrea.Value : false) : false;
            ViewBag.amenorrea = xcons ? (cnsult[0].gine.Amenorrea.HasValue ? cnsult[0].gine.Amenorrea.Value : false) : false;
            ViewBag.metrorragia = xcons ? (cnsult[0].gine.Metrorragia.HasValue ? cnsult[0].gine.Metrorragia.Value : false) : false;
            ViewBag.otros = xcons ? (cnsult[0].gine.Otros.HasValue ? cnsult[0].gine.Otros.Value : false) : false;
            ViewBag.reason = xcons ? cnsult[0].gine.OtrosEspecifique : "&nbsp;";
            ViewBag.tienePareja = xcons ? (cnsult[0].gine.TienePareja.HasValue ? cnsult[0].gine.TienePareja.Value : false) : false;
            ViewBag.nombrePareja = xcons ? cnsult[0].gine.nombrePareja : "&nbsp;";
            ViewBag.sexoPareja = xcons ? cnsult[0].gine.SexoPareja : "M";
            ViewBag.civilPareja = xcons ? cnsult[0].gine.EstadoCivilPareja : "0";
            ViewBag.gruporhPareja = xcons ? cnsult[0].gine.GrupoRHPareja : "&nbsp;";
            if (xcons && cnsult[0].gine.FechaNacimientoPareja.HasValue)
                ViewBag.fechaNPareja = cnsult[0].gine.FechaNacimientoPareja.Value.ToString("dddd dd MMMM yyyy");
            else ViewBag.fechaNPareja = "&nbsp;";
            ViewBag.edadPareja = xcons ? cnsult[0].gine.edadPareja : "&nbsp;";
            ViewBag.ocupacionPareja = xcons ? cnsult[0].gine.OcupacionPareja : "&nbsp;";
            ViewBag.telPareja = xcons ? cnsult[0].gine.TelefonoPareja : "&nbsp;";
            ViewBag.motivo = xcons ? cnsult[0].motivo : "&nbsp;";
            ViewBag.showBtnObstetrica = true;

            ViewBag.notarjeta = reg.Emeci;
            buscadatostarjeta(reg.Emeci);
            return PartialView();
        }

        // para sacar las coordenadas del sistema //
        void buscadatostarjeta(string emeci)
        {
            var tar = from tr in _Db.DatosTarjeta
                      where tr.noTarjeta == emeci
                      select new { tr.Dato, tr.Coordenada };

            ViewBag.NoTarjeta = emeci;
            ViewBag.TarjetaCord = tar.First().Coordenada;
            ViewBag.TarjetaDato = tar.First().Dato;

        }


        [HttpPost]
        public ActionResult ConsultaGinecologicaPorId(int idPac, int idquery)
        {
            var cnsult = (from c in _Db.Consultas
                          join g in _Db.ConsultaGinecologa
                          on c.idconsulta equals g.idconsulta into joined
                          from j in joined.DefaultIfEmpty()
                          where j != null && c.idpaciente == idPac
                          orderby c.Fecha descending
                          select new
                          {
                              gine = j,
                              c.Fecha,
                              c.Peso,
                              c.Altura,
                              c.Temperatura,
                              c.TensionArterial,
                              c.TensionArterialB,
                              c.motivo
                          }).ToList();

            var paciente = (from p in _Db.Paciente
                            where p.idPaciente == idPac
                            select p).First();

            string fechaUltimaMenstruacion = "&nbsp;";
            string fechaNacimientoPareja = "&nbsp;";
            if (cnsult.Count > 0)
            {
                cnsult = cnsult.Where(x => x.gine.idconsulta == idquery).ToList();
                if (cnsult[0].gine.FechaUltimaMestruacion.HasValue)
                    fechaUltimaMenstruacion = cnsult[0].gine.FechaUltimaMestruacion.Value.ToString("dddd dd MMMM yyyy");
                if (cnsult[0].gine.FechaNacimientoPareja.HasValue)
                    fechaNacimientoPareja = cnsult[0].gine.FechaNacimientoPareja.Value.ToString("dddd dd MMMM yyyy");
            }
;
            return Json(new { consulta = cnsult, paciente = paciente, fUM = fechaUltimaMenstruacion, fNP = fechaNacimientoPareja }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ConsultaGinecologica
            (int idpac, float? peso, float? talla,
            int? edadMenarca, float? temperatura, int? sistolica, int? diastolica,
            string fechaMenstruacion, byte? gestas, byte? paragestas, byte? cesarea,
            byte? aborto, byte? recienNacido, byte? mortinato, byte? edadSexual,
            string menacma,
            bool oligomenorrea, bool proimenorrea, bool hipermenorrea, bool dismenorrea,
            bool dispareunia, bool leucorrea, bool lactorrea, bool amenorrea,
            bool metrorragia, bool otros, string explique,
            bool pareja,
            string nombrePareja, string sexoPareja, string estadoCivilPareja, string grupoRHPareja,
            string fechaPareja, string edadPareja, string ocupacionPareja, string telefonoPareja,
            string motivoConsulta)
        {
            string error = "";
            bool isAdd = false;
            try
            {
                var paciente = (from p in _Db.Paciente
                                where p.idPaciente == idpac
                                select p).First();

                paciente.EdadMenarca = edadMenarca;
                _Db.SaveChanges();

                Consultas consulta = new Consultas();
                consulta.idpaciente = idpac;
                consulta.Peso = peso;
                consulta.Altura = talla;
                consulta.Temperatura = temperatura;
                consulta.TensionArterial = sistolica;
                consulta.TensionArterialB = diastolica;
                consulta.motivo = motivoConsulta;
                consulta.Fecha = DateTime.Now;
                _Db.Consultas.AddObject(consulta);
                _Db.SaveChanges();

                ConsultaGinecologa gine = new ConsultaGinecologa();
                gine.idconsulta = consulta.idconsulta;
                gine.FechaUltimaMestruacion =
                    string.IsNullOrEmpty(fechaMenstruacion) ? (DateTime?)null : Convert.ToDateTime(fechaMenstruacion);
                gine.Gestas = gestas;
                gine.ParaGestas = paragestas;
                gine.Cesareas = cesarea;
                gine.abortos = aborto;
                gine.RecienNacidosVivos = recienNacido;
                gine.mortinatos = mortinato;
                gine.EdadInicioVidaSexual = edadSexual;
                gine.menacma = menacma;

                gine.oligonorrea = oligomenorrea;
                gine.Proiomenorrea = proimenorrea;
                gine.Hipermenorrea = hipermenorrea;
                gine.Dismenorrea = dismenorrea;
                gine.Dispareunia = dispareunia;
                gine.Leucorrea = leucorrea;
                gine.Lactorrea = lactorrea;
                gine.Amenorrea = amenorrea;
                gine.Metrorragia = metrorragia;
                gine.Otros = otros;
                gine.OtrosEspecifique = explique;

                gine.TienePareja = pareja;
                if (pareja)
                {
                    gine.nombrePareja = nombrePareja;
                    gine.edadPareja = edadPareja;
                    gine.SexoPareja = sexoPareja;
                    gine.EstadoCivilPareja = estadoCivilPareja;
                    gine.GrupoRHPareja = grupoRHPareja;
                    gine.FechaNacimientoPareja =
                        string.IsNullOrEmpty(fechaPareja) ? (DateTime?)null : Convert.ToDateTime(fechaPareja);
                    gine.OcupacionPareja = ocupacionPareja;
                    gine.TelefonoPareja = telefonoPareja;
                }

                _Db.ConsultaGinecologa.AddObject(gine);
                _Db.SaveChanges();
                isAdd = true;
            }
            catch (Exception ex) {
                isAdd = false;
                error = ex.Message;
            }

            return Json(new { status = isAdd.ToString().ToLower(), error = error }, JsonRequestBehavior.AllowGet);
        }

        private string NumBoolean(bool input) => input ? "1" : "0";



        [HttpGet]
        public ActionResult ConsultaObstetrica(int idPac)
        {
            if (Session["idmedico"] == null) return Redirect("~/medico/login");
            InformacionPaciente(idPac);
            ViewBag.IdPaciente = idPac;

            //Todas las consultas
            var cnsult = (from c in _Db.Consultas
                          join g in _Db.ConsultaObstetrica
                          on c.idconsulta equals g.idconsulta into joined
                          from j in joined.DefaultIfEmpty()
                          where j != null && c.idpaciente == idPac
                          orderby c.Fecha descending
                          select new
                          {
                              obst = j,
                              c.Fecha,
                              c.Peso,
                              c.Altura,
                              c.Temperatura,
                              c.TensionArterial,
                              c.TensionArterialB,
                              c.motivo
                          }).ToList();

            List<SelectListItem> iConsultation = new List<SelectListItem>();
            foreach (var i in cnsult)
            {

                iConsultation.Add(new SelectListItem
                { Text = i.Fecha.Value.ToString("dddd dd MMMM yyyy, H:m"), Value = i.obst.idconsulta.ToString() });
            }

            ViewBag.consultation = iConsultation;
            ViewBag.consultationName = iConsultation.Count > 0 ? iConsultation[0].Text : "No hay consultas anteriores";

            //Mostrar ultima consulta
            var paciente = (from p in _Db.Paciente
                            where p.idPaciente == idPac
                            select p).First();

            bool xcons = cnsult.Count > 0 ? true : false;

            float peso = 0;
            float altura = 0;
            float masa = 0;
            if (xcons && cnsult[0].Peso.HasValue) peso = cnsult[0].Peso.Value;
            if (xcons && cnsult[0].Altura.HasValue) altura = cnsult[0].Altura.Value;
            if (peso > 0 && altura > 0)
            {
                masa = peso / (altura * altura);
            }

            int idreg = paciente.IdRegistro;
            var reg = (from r in _Db.Registro where r.idRegistro == idreg select r).First();

            buscadatostarjeta(reg.Emeci);

            ViewBag.peso = peso > 0 ? peso.ToString() : "&nbsp;";
            ViewBag.talla = altura > 0 ? altura.ToString() : "&nbsp;";
            ViewBag.masa = masa > 0 ? Math.Round(masa, 2).ToString() : "&nbsp;";
            ViewBag.noEmbarazo = xcons ? (cnsult[0].obst.noembarazo.HasValue ? cnsult[0].obst.noembarazo.Value : 1) : 1;
            ViewBag.activaSex = xcons ? (cnsult[0].obst.activaSexualmente.HasValue ? cnsult[0].obst.activaSexualmente.Value : false) : false;
            ViewBag.temperatura = xcons ? (cnsult[0].Temperatura.HasValue ? cnsult[0].Temperatura.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.sistolica = xcons ? (cnsult[0].TensionArterial.HasValue ? cnsult[0].TensionArterial.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.distolica = xcons ? (cnsult[0].TensionArterialB.HasValue ? cnsult[0].TensionArterialB.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.abortos = xcons ? (cnsult[0].obst.abortos.HasValue ? cnsult[0].obst.abortos.Value : 0) : 0;
            ViewBag.fechaUltParto = xcons ?
                (cnsult[0].obst.FechaUltmoParto.HasValue ? cnsult[0].obst.FechaUltmoParto.Value.ToString("dddd dd MMMM yyyy") : "&nbsp;") : "&nbsp;";
            ViewBag.fechaUltMenstruacion = xcons ?
                (cnsult[0].obst.PrimerDiaUltimaMestruacuion.HasValue ? cnsult[0].obst.PrimerDiaUltimaMestruacuion.Value.ToString("dddd dd MMMM yyyy") : "&nbsp;") : "&nbsp;";
            ViewBag.toxemias = xcons ? (cnsult[0].obst.ToxemiasPrevias.HasValue ? cnsult[0].obst.ToxemiasPrevias.Value : 0) : 0;
            ViewBag.espToxemias = xcons ? (cnsult[0].obst.EspecifiqueToxemias) : "&nbsp;";
            ViewBag.partos = xcons ? (cnsult[0].obst.Partos.HasValue ? cnsult[0].obst.Partos.Value : 0) : 0;
            ViewBag.tipoDistocia = xcons ? (cnsult[0].obst.TipoDistocia.HasValue ? cnsult[0].obst.TipoDistocia.Value : 0) : 0;
            ViewBag.espTipoDistocia = xcons ? (cnsult[0].obst.EspecifiqueTipoDistocia == null ? "&nbsp;" : cnsult[0].obst.EspecifiqueTipoDistocia) : "&nbsp;";
            ViewBag.motivoDistocia = xcons ? (cnsult[0].obst.MotivoDistocia.HasValue ? cnsult[0].obst.MotivoDistocia.Value : 0) : 0;
            ViewBag.espMotivoDistocia = xcons ? (cnsult[0].obst.EspecifiqueMotivoDistocia == null ? "&nbsp;" : cnsult[0].obst.EspecifiqueMotivoDistocia) : "&nbsp;";
            ViewBag.cesareasPrevias = xcons ? (cnsult[0].obst.CesareasPrevia.HasValue ? cnsult[0].obst.CesareasPrevia.Value : 0) : 0;
            ViewBag.forceps = xcons ? (cnsult[0].obst.UsoDeForceps.HasValue ? cnsult[0].obst.UsoDeForceps.Value : 0) : 0;
            ViewBag.mortinatos = xcons ? (cnsult[0].obst.Motinatos.HasValue ? cnsult[0].obst.Motinatos.Value : 0) : 0;
            ViewBag.rnVivos = xcons ? (cnsult[0].obst.RMVivos.HasValue ? cnsult[0].obst.RMVivos.Value : 0) : 0;
            ViewBag.ectopico = xcons ? (cnsult[0].obst.EmbarazoEtopicos.HasValue ? cnsult[0].obst.EmbarazoEtopicos.Value : 0) : 0;
            ViewBag.espEctopico = xcons ? (cnsult[0].obst.EmbrazoEtopicoExplique == null ? "" : cnsult[0].obst.EmbrazoEtopicoExplique) : "";
            ViewBag.complicacionesEmbarazo = xcons ? (cnsult[0].obst.EmbrazosComplicadosPrevios.HasValue ? cnsult[0].obst.EmbrazosComplicadosPrevios.Value : 0) : 0;
            ViewBag.espComplicacionesEmbarazo = xcons ? (cnsult[0].obst.EmbarazosComplicadosExplique == null ? "" : cnsult[0].obst.EmbarazosComplicadosExplique) : "";
            ViewBag.perinatales = xcons ? (cnsult[0].obst.NoComplicacionesPertinales.HasValue ? cnsult[0].obst.NoComplicacionesPertinales.Value : 0) : 0;
            ViewBag.espPerinatales = xcons ? (cnsult[0].obst.ComplicacionesPerinatalesExplique == null ? "" : cnsult[0].obst.ComplicacionesPerinatalesExplique) : "";
            ViewBag.anormales = xcons ? (cnsult[0].obst.NoEmbrazosAnormales.HasValue ? cnsult[0].obst.NoEmbrazosAnormales.Value : 0) : 0;
            ViewBag.espAnormales = xcons ? (cnsult[0].obst.EmbarazosAnormalesExplique == null ? "" : cnsult[0].obst.EmbarazosAnormalesExplique) : "";
            ViewBag.observaciones = xcons ? cnsult[0].obst.Observaciones : "";

            //control de embarazo

            ViewBag.showBtnObstetrica = false;

            ViewBag.gestacion = Gestacion(xcons ? cnsult[0].obst.PrimerDiaUltimaMestruacuion : null);
            ViewBag.fu = xcons ? (cnsult[0].obst.FU.HasValue ? cnsult[0].obst.FU.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.fcf = xcons ? (cnsult[0].obst.FCF.HasValue ? cnsult[0].obst.FCF.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.cc = xcons ? (cnsult[0].obst.CC.HasValue ? cnsult[0].obst.CC.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.ca = xcons ? (cnsult[0].obst.CA.HasValue ? cnsult[0].obst.CA.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.lf = xcons ? (cnsult[0].obst.LF.HasValue ? cnsult[0].obst.LF.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.dbp = xcons ? (cnsult[0].obst.DSP.HasValue ? cnsult[0].obst.DSP.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.posicion = xcons ? (cnsult[0].obst.Posicion == null ? "&nbsp;" : cnsult[0].obst.Posicion) : "&nbsp;";
            ViewBag.presentacion = xcons ? (cnsult[0].obst.Presentacion == null ? "&nbsp;" : cnsult[0].obst.Presentacion) : "&nbsp;";
            ViewBag.situacion = xcons ? (cnsult[0].obst.siuacuion == null ? "&nbsp;" : cnsult[0].obst.siuacuion) : "&nbsp;";
            ViewBag.actitud = xcons ? (cnsult[0].obst.Actitud == null ? "&nbsp;" : cnsult[0].obst.Actitud) : "&nbsp;";
            ViewBag.fetales = xcons ? (cnsult[0].obst.MovimientosFetales == null ? "&nbsp;" : cnsult[0].obst.MovimientosFetales) : "&nbsp;";
            ViewBag.pesoProducto = xcons ? (cnsult[0].obst.PesoAproxProducto.HasValue ? cnsult[0].obst.PesoAproxProducto.Value.ToString() : "&nbsp;") : "&nbsp;";

            float pesoMadre = 0;
            if (xcons && cnsult[0].obst.PesoAproxProducto.HasValue) 
            if (xcons && (cnsult[0].Peso.HasValue && cnsult[0].obst.PesoAproxProducto.HasValue))
            {
                pesoMadre = peso - cnsult[0].obst.PesoAproxProducto.Value;
            }
            ViewBag.pesoMadre = pesoMadre > 0 ? Math.Round(pesoMadre, 2).ToString() : "&nbsp;";
            ViewBag.ta = xcons ? (cnsult[0].obst.TA.HasValue ? cnsult[0].obst.TA.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.fcm = xcons ? (cnsult[0].obst.FCM.HasValue ? cnsult[0].obst.FCM.Value.ToString() : "&nbsp;") : "&nbsp;";
            ViewBag.edema = xcons ? (cnsult[0].obst.Edema == null ? "&nbsp;" : cnsult[0].obst.Edema) : "&nbsp;";
            ViewBag.seHizoUS = xcons ? (cnsult[0].obst.SeHizoUf.HasValue ? cnsult[0].obst.SeHizoUf.Value : false) : false;
            ViewBag.ultrasonido = xcons ? cnsult[0].obst.ultrasonido : "";
            ViewBag.motivoConsulta = xcons ? cnsult[0].motivo : "";
            ViewBag.exploracion = xcons ? cnsult[0].obst.exploracionFisica  : "";

            return PartialView();
        }


        private string Gestacion(DateTime? LastMenstruation)
        {
            string gest = "&nbsp;";
            if (LastMenstruation.HasValue)
            {
                double diffDays = (DateTime.Now.Date - LastMenstruation.Value).TotalDays;

                int weeks = 0;
                int days = 0;
                if (diffDays > 0)
                {
                    int rank = 6;
                    bool pending = false;
                    for (int i = 0; i < diffDays; i++)
                    {
                        if (i == rank)
                        {
                            weeks++;
                            rank = rank + 7;
                            pending = false;
                        }
                        else pending = true;
                    }

                    if (pending)
                    {
                        if (rank > 6)
                        {
                            for (int b = (weeks * 7); b <= diffDays; b++) days++;
                        }
                        else days = (int)diffDays;
                    }
                }

                gest = weeks.ToString() + " / " + days.ToString();
            }

            return gest;
        }


        [HttpPost]
        public ActionResult ConsultaObstetricaPorId(int idPac, int idquery)
        {
            var cnsult = (from c in _Db.Consultas
                          join g in _Db.ConsultaObstetrica
                          on c.idconsulta equals g.idconsulta into joined
                          from j in joined.DefaultIfEmpty()
                          where j != null && c.idpaciente == idPac
                          orderby c.Fecha descending
                          select new
                          {
                              obst = j,
                              c.Fecha,
                              c.Peso,
                              c.Altura,
                              c.Temperatura,
                              c.TensionArterial,
                              c.TensionArterialB,
                              c.motivo
                          }).ToList();

            var paciente = (from p in _Db.Paciente
                            where p.idPaciente == idPac
                            select p).First();

            string fechaUltimoParto = "&nbsp;";
            string ultimaMenstruacion = "&nbsp;";
            string gest = "&nbsp;";
            if (cnsult.Count > 0)
            {
                cnsult = cnsult.Where(x => x.obst.idconsulta == idquery).ToList();
                if (cnsult[0].obst.FechaUltmoParto.HasValue)
                    fechaUltimoParto = cnsult[0].obst.FechaUltmoParto.Value.ToString("dddd dd MMMM yyyy");
                if (cnsult[0].obst.PrimerDiaUltimaMestruacuion.HasValue)
                    ultimaMenstruacion = cnsult[0].obst.PrimerDiaUltimaMestruacuion.Value.ToString("dddd dd MMMM yyyy");
                gest = Gestacion(cnsult[0].obst.PrimerDiaUltimaMestruacuion);
            }

            return Json(new { consulta = cnsult, fUP = fechaUltimoParto, fUM = ultimaMenstruacion, gestacion = gest}, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ConsultaObstetrica(int idpac, byte noEmbarazo, float? peso, float? talla, 
            bool activaSexualmente, float? temperatura, int? sistolica, 
            int? diastolica, byte abortos, string fechaUltimoParto, string ultimaMenstruacion, 
            byte toxemias, string EspToxemias, byte partos, byte? tipoDistocia, string EspTipoDistocia,
            byte? motivoDistocia, string EspMotivoDistocia, byte cesareas, byte forceps, byte mortinatos,
            byte rnvivos, byte? EmbarazosEctopicos, string EspEmbarazosEctopicos, byte? complicacionEmbarazo,
            string EspComplicacionEmbarazo, byte? perinatales, string EspPerinatales, byte? anembrionicos,
            string EspAnembrionicos, string observaciones, byte? fu, byte? fcf, byte? cc, byte? ca, byte? lf,
            byte? dbp, string posicion, string presentacion, string situacion, string actitud, string fetales,
            byte? pesoProducto, byte? ta, byte? fcm, string edema, bool hizoUS,
            string ultrasonido, string motivoConsulta, string exploracion)
        {
            string error = "";
            bool isAdd = false;
            try
            {
                Consultas consulta = new Consultas();
                consulta.idpaciente = idpac;
                consulta.Peso = peso;
                consulta.Altura = talla;
                consulta.Temperatura = temperatura;
                consulta.TensionArterial = sistolica;
                consulta.TensionArterialB = diastolica;
                consulta.motivo = motivoConsulta;
                consulta.Fecha = DateTime.Now;
                _Db.Consultas.AddObject(consulta);
                _Db.SaveChanges();

                ConsultaObstetrica obst = new ConsultaObstetrica();
                obst.idconsulta = consulta.idconsulta;
                obst.noembarazo = noEmbarazo;
                obst.activaSexualmente = activaSexualmente;
                obst.abortos = abortos;
                obst.FechaUltmoParto =
                    string.IsNullOrEmpty(fechaUltimoParto) ? (DateTime?)null : Convert.ToDateTime(fechaUltimoParto);
                obst.PrimerDiaUltimaMestruacuion =
                    string.IsNullOrEmpty(ultimaMenstruacion) ? (DateTime?)null : Convert.ToDateTime(ultimaMenstruacion);
                obst.ToxemiasPrevias = toxemias;
                obst.EspecifiqueToxemias = EspToxemias == "" ? null : EspToxemias;
                obst.Partos = partos;
                obst.TipoDistocia = tipoDistocia;
                obst.EspecifiqueTipoDistocia = EspTipoDistocia == "" ? null : EspTipoDistocia;
                obst.MotivoDistocia = motivoDistocia;
                obst.EspecifiqueMotivoDistocia = EspMotivoDistocia == "" ? null : EspMotivoDistocia;
                obst.CesareasPrevia = cesareas;
                obst.UsoDeForceps = forceps;
                obst.Motinatos = mortinatos;
                obst.RMVivos = rnvivos;
                obst.EmbarazoEtopicos = EmbarazosEctopicos;
                obst.EmbrazoEtopicoExplique = EspEmbarazosEctopicos == "" ? null : EspEmbarazosEctopicos;
                obst.EmbrazosComplicadosPrevios = complicacionEmbarazo;
                obst.EmbarazosComplicadosExplique = EspComplicacionEmbarazo == "" ? null : EspComplicacionEmbarazo;
                obst.NoComplicacionesPertinales = perinatales;
                obst.ComplicacionesPerinatalesExplique = EspPerinatales == "" ? null : EspPerinatales;
                obst.NoEmbrazosAnormales = anembrionicos;
                obst.EmbarazosAnormalesExplique = EspAnembrionicos == "" ? null : EspAnembrionicos;

                //control de embarazo
                obst.FU = fu;
                obst.FCF = fcf;
                obst.CC = cc;
                obst.CA = ca;
                obst.LF = lf;
                obst.DSP = dbp;
                obst.Posicion = posicion == "" ? null : posicion;
                obst.Presentacion = presentacion == "" ? null : presentacion;
                obst.siuacuion = situacion == "" ? null : situacion;
                obst.Actitud = actitud == "" ? null : actitud;
                obst.MovimientosFetales = fetales == "" ? null : fetales;
                obst.PesoAproxProducto = pesoProducto;
                obst.TA = ta;
                obst.FCM = fcm;
                obst.Edema = edema == "" ? null : edema;
                obst.SeHizoUf = hizoUS;
                obst.ultrasonido = ultrasonido;
                obst.exploracionFisica = exploracion;
                obst.Observaciones = observaciones;

                _Db.ConsultaObstetrica.AddObject(obst);
                _Db.SaveChanges();
                isAdd = true;


            }
            catch (Exception ex) {
                error = ex.Message;
                isAdd = false;
            }

            return Json(new { status = isAdd.ToString().ToLower(), error = error }, JsonRequestBehavior.AllowGet);
        }

        private bool CheckElement(string input) => input == "1" ? true : false;
        /*
            david
            fin consultas ginecologica y obstetrica
        */









        // formatea a texto la consulta

        string formateaCadena(string cadena)
        {
            string strCad="";
            foreach(string c in cadena.Replace("\r\n","\r").Split(new string[] { "\r" }, StringSplitOptions.None))
            {
                if (c.Length>2)
                    strCad += c.Substring(2) + "<BR/>";
            }
            return strCad;
        }

        // Trae consulta aterior
        public ActionResult ConsultaById(int idcons)
        {
            int idpac=0;
            DateTime Fecha;
            // Consultas
            var cons = from c in _Db.Consultas
                       where c.idconsulta == idcons
                       orderby c.Fecha descending
                       select c;

            
            EmeciDoctoresV2.Models.Consultas co = cons.First();

            
            idpac = (int)co.idpaciente;
            Fecha = co.Fecha.Value.Date;

            // recetas
            var rec = from r in _Db.Recetas
                      where r.idpaciente == idpac && r.idconsulta == idcons
                      orderby r.Fecha descending, r.idconsulta descending
                      select r;
            if (rec.Count() == 0)
            {
                rec = from r in _Db.Recetas
                      where r.idpaciente == idpac && EntityFunctions.TruncateTime(r.Fecha) == EntityFunctions.TruncateTime(Fecha)
                      orderby r.Fecha descending, r.idconsulta descending
                      select r;            
            }
            string strRecetas = "";
            if (rec.Count() > 0)
                strRecetas = rec.First().Lineas;
            else {                 // && EntityFunctions.TruncateTime(d.Fecha)== EntityFunctions.TruncateTime(Fecha)
            }

            // diagnosticos
            var diag = from d in _Db.Diagnosticos
                       where d.idpaciente == idpac && d.idconsulta ==idcons
                       orderby d.Fecha descending, d.idconsulta descending
                       select d;

            if (diag.Count()==0)
            {
                diag = from d in _Db.Diagnosticos
                       where d.idpaciente == idpac && EntityFunctions.TruncateTime(d.Fecha) == EntityFunctions.TruncateTime(Fecha)
                       orderby d.Fecha descending, d.idconsulta descending
                       select d;
            }
            string strDiag = "";

            if (diag.Count() > 0) strDiag = diag.First().Lineas;
            else {
                //EntityFunctions.TruncateTime(d.Fecha)== EntityFunctions.TruncateTime(Fecha)
            }

            //diagnosticos Exlusivos
            string strdiagExc = "<BR/>";
            var diagExc = (from d in _Db.DiagnosticosExc
                           where d.idconsulta == idcons
                           select d).ToList();
            foreach (var de in diagExc)
            {
                strdiagExc += de.lineas + "<BR/>";

            }

            //Estudios Lab
            var estLab = from el in _Db.EstudiosLab
                         where el.idpaciente == idpac && el.idconsulta ==idcons
                         orderby el.Fecha descending,el.idconsulta descending
                         select el;

            if (estLab.Count() == 0)
            {
                estLab = from el in _Db.EstudiosLab
                         where el.idpaciente == idpac && EntityFunctions.TruncateTime(el.Fecha) == EntityFunctions.TruncateTime(Fecha)
                         orderby el.Fecha descending, el.idconsulta descending
                         select el;
            }

            string strEstLab = "";
            if (estLab.Count() > 0) strEstLab = estLab.First().Lineas;
            else {
                //EntityFunctions.TruncateTime(el.Fecha) == EntityFunctions.TruncateTime(Fecha)s
            }

            // Estudios Gab
            var estGab = from eG in _Db.EstudiosGab
                         where eG.idpaciente == idpac && idcons == eG.idconsulta
                         orderby eG.Fecha descending, eG.idconsulta descending
                         select eG;
            if (estGab.Count()==0)
            {
                estGab = from eG in _Db.EstudiosGab
                         where eG.idpaciente == idpac && EntityFunctions.TruncateTime(eG.Fecha) == EntityFunctions.TruncateTime(Fecha)
                         orderby eG.Fecha descending, eG.idconsulta descending
                         select eG;
            }
            string strEstGab="";
            if (estGab.Count() > 0)
            {
                strEstGab = estGab.First().Lineas.Replace("\r\n", "\r");
                // strEstGabList = strEstGab.Split("\r");
            }
            else { 
                
            }
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string jsoncons = serializer.Serialize(cons.ToList().Select(
                d => new{
                    fecha = d.Fecha.Value.ToLongDateString(),
                    altura = d.Altura,
                    cabeza = d.Cabeza,
                    frecuenciacardiaca = d.FrecuenciaCardiaca,
                    peso = d.Peso,
                    frecuenciarespiratoria=  d.FrecuenciaRespiratoria,
                    medidaspreventivas = d.MedidasPreventivas,
                    motivo = d.motivo,
                    observaciones = d.observaciones,
                    perimetrocefalico = d.perimetroCefalico,
                    sintomas = d.SignosSintomas1 +d.SignosSintomas2 + d.SignosSintomas3,
                    temperatura = d.Temperatura,
                    tensionarterial = d.TensionArterial, 
                    tensionarterialb = d.TensionArterialB,
                    receta = formateaCadena(strRecetas),
                    diagnostico = formateaCadena(strDiag) + strdiagExc,
                    EstLaboratorio= formateaCadena(strEstLab),
                    EstGabinete = formateaCadena(strEstGab)//.Split("\r")
                    
                    
                }
                ));
            return Content(jsoncons, "application/json");

           
        }

        public ActionResult salir()
        {
            Session.Clear();
            Session.Abandon();
            return Redirect("Login");

        }

        [HttpGet]
        public ActionResult AgregarPaciente()
        {
            if (Session["idmedico"] == null) return Redirect("~/medico/login");

            Random random = new Random();
            int randomNumber = random.Next(1, 10);
            char randomLetter = (char)random.Next(65, 74);
            ViewBag.cord = randomLetter.ToString() + randomNumber.ToString();
            ViewBag.idmed = Session["idmedico"].ToString();

            if (Session["password"] != null) ViewBag.pass = Session["password"].ToString();
            getMedico();
           


            return View();

        }

        [HttpPost]
        public ActionResult AgregarPaciente(string emeci1, string emeci2, string emeci3, string coordenada, string valor)
        {
            ViewBag.cord = coordenada;

            string emeci = emeci1 + "-" + emeci2 + "-" + emeci3;
            var re = (from r in _Db.Registro
                     join p in _Db.Paciente on r.idRegistro equals p.IdRegistro
                     where emeci == r.Emeci && r.Tipo == "P"
                     select  p);

   

            var dt = (from da in _Db.DatosTarjeta
                      where da.Coordenada == coordenada && da.Dato == valor && da.noTarjeta == emeci
                      select da);
            getMedico();
            if (re.ToList().Count>0  && dt.ToList().Count >0)
               return Redirect("~/Medico/Consulta?idpac=" + re.First().idPaciente.ToString());
            else
            {
                ModelState.AddModelError("", "Tarjeta y Coordenada Invalidos.");
                return View();
            }
        }

        [HttpPost]
        public ActionResult AgregarNuevoPaciente(string emeci1, string emeci2, string emeci3 , string Nombre,  string Apellidos,  string nombremadre,
            string NombrePadre, string Telefono, string Email, string Fechanac1, string Fechanac2, string Fechanac3,string Alergia, string pwd)
        {
            if (emeci1 == "" || pwd == "")
            {
                ModelState.AddModelError("", "Tarjeta y Coordenada son requeridos.");
                return View();
            }

            string emeci = emeci1 + "-"+ emeci2 + "-" + emeci3;

            var re =  from r in _Db.Registro
                    where emeci == r.Emeci && r.Tipo=="P"
                    select r;

           DateTime fn =  new DateTime(int.Parse(Fechanac3),int.Parse(Fechanac2),int.Parse(Fechanac1));

            EmeciDoctoresV2.Models.Registro reg;
            EmeciDoctoresV2.Models.Paciente pac;
            int cnt=re.ToList().Count;
            if ( cnt > 0)
            {
                reg = re.First();
                pac = (from p in _Db.Paciente  
                        where p.IdRegistro == reg.idRegistro
                        select p).First();

            }
            else
            {

                string nm = Nombre;

                var res = from rg in _Db.Registro
                          join pc in _Db.Paciente on rg.idRegistro equals pc.IdRegistro
                          where rg.Nombre == nm && rg.Tipo == "P" && rg.Apellido == Apellidos
                            && EntityFunctions.TruncateTime(pc.FechaNacimiento) == EntityFunctions.TruncateTime(fn)
                         select rg;

                if (res.ToList().Count > 0)
                {

                    var ser = new System.Web.Script.Serialization.JavaScriptSerializer();
                    string jsonc = ser.Serialize(new { idpac =0, ex ="El registro del paciente ya existe con el numero: " + res.FirstOrDefault().Emeci });
                    return Content(jsonc, "application/json");

                
                }


                reg = new EmeciDoctoresV2.Models.Registro();
                pac = new EmeciDoctoresV2.Models.Paciente();
                
                _Db.Registro.AddObject(reg);

                
            }

              reg.Apellido = Apellidos;
                reg.Colonia = "" ;

                reg.Domicilio = "";
                reg.Emails = Email;
                reg.FechaRegistro = DateTime.Now.Date;


                reg.Emeci = emeci;

                reg.idCiudad = 0;
                reg.idEstado = "BS";
                reg.IdPais = "MX";

                reg.IdUsuario = null;

                reg.Nombre = Nombre;
                reg.OtraCiudad = "";
                reg.Status = "V";
                reg.Telefono = Telefono ;
                reg.TelefonoCel = ""; 
                reg.Tipo = "P";

                reg.CodigoPostal = "";
                reg.NoExt = "";
                reg.clave = pwd;
            // agregarle un mes a la fecha

            reg.FechaRegistro = DateTime.Now.Date;
            reg.FechaExpiracion = DateTime.Now.AddMonths(1).Date;
                //david
               // pac.NombreMadre = nombremadre;
               // pac.NombrePadre = NombrePadre;
                
                pac.Escolaridad = "";
                pac.Sexo = "M";

                pac.AbortosPrevios = 0 ;
                pac.FechaNacimiento = fn;
                pac.grupoRH = "";
                pac.HospitalNacer = "";
                pac.LugarNacer = "";
                pac.PerCefalicoNacer = 0; 
                pac.PesoNacer = 0;
                pac.TallaNacer = 0;
                pac.AlergiaMedicina = Alergia;
                
                if (cnt>0)
                    _Db.SaveChanges();
                else
                    {
                    _Db.SaveChanges();
                    pac.IdRegistro = reg.idRegistro;
                    _Db.Paciente.AddObject(pac);
                    _Db.SaveChanges();
                    }
              var tar = from dt in _Db.DatosTarjeta 
                         where dt.noTarjeta == emeci
                         select dt;
              string strex = "";
                if (tar.Count()==0  )
                {

                    //New
                    var doc = new Document();
                    MemoryStream memory = new MemoryStream();
                    PdfWriter pdf = PdfWriter.GetInstance(doc, memory);

                    doc.Open();
                    BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                    Font fbold = new Font(bfTimes, 8, Font.NORMAL, Color.BLACK);

                    PdfPTable pdfTble = new PdfPTable(2);
                    pdfTble.TotalWidth = 500f;
                    pdfTble.LockedWidth = true;
                    float[] widths = new float[] { 2.8f, 2.1f };
                    pdfTble.SetWidths(widths);
                    pdfTble.HorizontalAlignment = 1;
                    pdfTble.DefaultCell.Border = Rectangle.NO_BORDER;

                    Image jpg = Image.GetInstance(new PDF().Img_access(Server.MapPath("~/imgAccess.jpg"), emeci));
                    jpg.ScaleAbsolute(258f, 153f);

                    PdfPCell cellImg = new PdfPCell(jpg);
                    cellImg.HorizontalAlignment = PdfPCell.ALIGN_MIDDLE;
                    cellImg.Border = Rectangle.NO_BORDER;
                    pdfTble.AddCell(cellImg);

                    PdfPTable position = new PdfPTable(11);
                    position.HorizontalAlignment = 1;
                    position.TotalWidth = 265f;
                    position.LockedWidth = true;
                    PdfPCell cellPosition = new PdfPCell(new Phrase("Posiciones de Acceso Seguro"));
                    cellPosition.BackgroundColor = new Color(162, 212, 255);
                    cellPosition.HorizontalAlignment = 1;
                    cellPosition.Colspan = 11;
                    cellPosition.Border = Rectangle.NO_BORDER;
                    position.AddCell(cellPosition);

                    string[] arrABC = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
                    for (int i = 0; i <= 10; i++)
                    {
                        position.DefaultCell.BackgroundColor = new Color(255, 255, 255);
                        position.DefaultCell.Padding = 2f;
                        position.DefaultCell.Border = Rectangle.NO_BORDER;
                        if (i == 0) position.AddCell("");
                        else position.AddCell(new Phrase(arrABC[i - 1], fbold));
                    }

                    bool color = true;
                    int count = 0;
                    Random random = new Random();
                    EmeciDoctoresV2.Models.DatosTarjeta ndt;
                    for (int i = 1; i <= 10; i++)
                    {
                        count++;
                        if (color)
                        {
                            position.DefaultCell.BackgroundColor = new Color(162, 212, 255);
                            color = false;
                        }
                        else { position.DefaultCell.BackgroundColor = new Color(255, 255, 255); color = true; }

                        position.AddCell(new Phrase(count.ToString(), fbold));
                        string letra;
                        for (var j = 0; j <= 9; j++)
                        {
                            letra = arrABC[j];
                            ndt = new DatosTarjeta();
                            ndt.noTarjeta = emeci;
                            ndt.Dato = random.Next(0, 999).ToString("00#");
                            ndt.Coordenada = string.Concat(letra, i);

                            position.AddCell(new Phrase(ndt.Dato, fbold));

                            _Db.DatosTarjeta.AddObject(ndt);
                        }
                    }

                    pdfTble.AddCell(position);
                    doc.Add(pdfTble);

                    pdf.CloseStream = false;
                    doc.Close();
                    memory.Position = 0;
                    //Fin New

                    //var doc = new Document();
                    //MemoryStream memory = new MemoryStream();
                    //PdfWriter writer = PdfWriter.GetInstance(doc, memory);

                    //doc.Open();

                    //BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                    //Font Fbold = new Font(bfTimes, 12, Font.BOLD, Color.BLACK);
                    //Font FboldRed = new Font(bfTimes, 12, Font.BOLD, Color.RED);
                    //Font FundRed = new Font(bfTimes, 13, Font.UNDERLINE, Color.RED);
                    //Font FundBlue = new Font(bfTimes, 12, Font.UNDERLINE, Color.BLUE);
                    //Font FunBlueBig = new Font(bfTimes, 15, Font.UNDERLINE, Color.BLUE);
                    //Font Fbig = new Font(bfTimes, 15, Font.BOLD, Color.BLACK);
                    //----------------
                    //string img = Server.MapPath("~" + "/img/emeci.png");
                    //Image _img = Image.GetInstance(img);
                    //_img.ScaleToFit(120f, 120f);
                    //_img.SpacingAfter = 1f;
                    //_img.SpacingBefore = 2f;
                    //_img.Alignment = Element.ALIGN_LEFT;
                    //doc.Add(_img);
                    //-----------------
                    //Paragraph pg;
                    //Anchor ancla;
                    //----------------
                    //pg = new Paragraph("ESTA ES SU MEMBRECÍA EMECI \n \n", Fbold);
                    //pg.Alignment = Element.ALIGN_RIGHT;
                    //doc.Add(pg);
                    //-----------------
                    //pg = new Paragraph("Debe Imprimir y Resguardar su Tarjeta EMECI, ya que es la única llave de acceso al expediente de su hijo(a)", Fbold);
                    //pg.Alignment = Element.ALIGN_JUSTIFIED;
                    //pg.Add("\n Nota: Recuerde es de suma importancia reguardar bien esta tarjeta ya que es responsabilidad de usted el uso adecuado y conﬁdencial que se le de, ya que posee información de Salud.");
                    //doc.Add(pg);
                    //doc.Add(new Phrase(Environment.NewLine));
                    //-----------------
                    //pg = new Paragraph("Para su Activación debe hacer un pago promocional por su Médico Pediatra de $100.00 pesos", FundRed);
                    //pg.Alignment = Element.ALIGN_CENTER;
                    //pg.SpacingAfter = 2f;
                    //doc.Add(pg);
                    //pg = new Paragraph("Metodos de Pago:", Fbold);
                    //pg.Alignment = Element.ALIGN_CENTER;
                    //pg.SpacingAfter = 1f;
                    //doc.Add(pg);
                    //doc.Add(new Phrase(Environment.NewLine));
                    //-----------------
                    //pg = new Paragraph("1)  Transferencia Bancaria: ", Fbold);
                    //ancla = new Anchor("www.emeci.com", FundBlue);
                    //ancla.Reference = "http://www.emeci.com";
                    //pg.Add(ancla);
                    //doc.Add(pg);
                    //pg = new Paragraph("2)  Deposito a Banco: ", Fbold);
                    //pg.Add(ancla);
                    //doc.Add(pg);
                    //pg = new Paragraph("3)  Pago por PayPal: ", Fbold);
                    //ancla = new Anchor("www.paypal.com", FundBlue);
                    //ancla.Reference = "http://paypal.com";
                    //pg.Add(ancla);
                    //doc.Add(pg);
                    //pg = new Paragraph("4)  Pago por OXXO: ", Fbold);
                    //ancla = new Anchor("www.oxxo.com", FundBlue);
                    //ancla.Reference = "http://oxxo.com";
                    //pg.Add(ancla);
                    //doc.Add(pg);
                    //pg = new Paragraph("5)  Compra de Ficha de Saldo en Famarcia Banavides: ", Fbold);
                    //ancla = new Anchor("www.benavides.com", FundBlue);
                    //ancla.Reference = "http://www.benavides.com";
                    //pg.Add(ancla);
                    //doc.Add(pg);

                    //doc.Add(new Phrase(Environment.NewLine));
                    ////-----------------
                    //pg = new Paragraph("Recuerde si el pago no se efectúa en las primeras 48 hrs esta promoción vencerá y tendrá que pagar $250.00 pesos por anualidad del servicio", FboldRed);
                    //pg.Alignment = Element.ALIGN_JUSTIFIED;
                    //doc.Add(pg);
                    ////-----------------
                    //doc.Add(new Phrase(Environment.NewLine));
                    //pg = new Paragraph("BENEFICIOS DE EMECI: ", Fbig);
                    //pg.Alignment = Element.ALIGN_CENTER;
                    //ancla = new Anchor("www.emeci.com", FunBlueBig);
                    //ancla.Reference = "http://www.emeci.com";
                    //pg.Add(ancla);
                    //doc.Add(pg);
                    //doc.Add(new Phrase(Environment.NewLine));
                    ////-----------------
                    //pg = new Paragraph("Asimismo usted puede descargar el Manual de Usuario en la siguiente liga: \n", Fbold);
                    //pg.Alignment = Element.ALIGN_CENTER;
                    //ancla = new Anchor("www.emeci.com", FundBlue);
                    //ancla.Reference = "http://www.emeci.com";
                    //pg.Add(ancla);
                    //doc.Add(pg);
                    ////-----------------
                    //doc.Add(new Phrase(Environment.NewLine));
                    //doc.Add(new Phrase(Environment.NewLine));
                    //pg = new Paragraph("Para mayor información visitar ", Fbold);
                    //pg.Add(ancla);
                    //pg.Add(" o enviar un correo a ");
                    //ancla = new Anchor("info@emeci.com", FundBlue);
                    //ancla.Reference = "info@emeci.com";
                    //pg.Add(ancla);
                    //doc.Add(pg);
                    //doc.Add(new Phrase(Environment.NewLine));
                    ////-----------------
                    //PdfPTable table = new PdfPTable(11);
                    
                    //PdfPCell cell = new PdfPCell(new Paragraph("Posiciones de Acceso Seguro"));
                    //cell.BackgroundColor = new Color(162,212,255);
                    //cell.HorizontalAlignment = 1;
                    //cell.Colspan = 11;
                    //table.AddCell(cell);
                    //table.DefaultCell.HorizontalAlignment = 1;

                    //int cont = 0;
                    //string[]  arrABC  = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J"};
                    //string letra;
                    //EmeciDoctoresV2.Models.DatosTarjeta ndt;
                    //Random random;
                    ////genero valores de tarjeta
                    //for (int b = 0; b <= 10; b++) {
                    //    table.DefaultCell.BackgroundColor = new Color(255, 255, 255);
                    //    table.DefaultCell.Padding = 4f;
                    //    if (b == 0) table.AddCell("");
                    //    else table.AddCell(arrABC[b - 1]); //Agregamos letras
                    //}

                    //Boolean color = true;
                    //random = new Random();
                    //for(int i = 1 ; i <= 10; i++){   
                    //    cont += 1;
                       
                    //    if (color)
                    //    {
                    //        table.DefaultCell.BackgroundColor = new Color(162, 212, 255);
                    //        color = false;
                    //    }
                    //    else
                    //    {
                    //        table.DefaultCell.BackgroundColor = new Color(255, 255, 255);
                    //        color = true;
                    //    }
                    //    table.AddCell(cont.ToString());

                    //    for( int j  = 0; j <= 9;j++){
                    //        letra = arrABC[j];
                    //        //if (j == 0){
                               
                    //        //}

                    //        //if (j > 0)
                    //        //{
                    //            ndt = new DatosTarjeta();
                    //            ndt.noTarjeta = emeci;

                    //            ndt.Dato = random.Next(0, 999).ToString("00#");

                    //            ndt.Coordenada = string.Concat(letra, i);
                               
                    //            table.AddCell(ndt.Dato);

                    //            _Db.DatosTarjeta.AddObject(ndt);

                    //        //}

                    //    }
                    //}

                    //doc.Add(table);
                    //writer.CloseStream = false;
                    //doc.Close();
                    //memory.Position = 0;
                    //guardo la tarjeta
                    _Db.SaveChanges();

                    string line= "";
                    try
                    {
                        StreamReader sr = new StreamReader(Server.MapPath("~/BienvenidaEMECI2.html"));
                        line = sr.ReadToEnd();
                        sr.Close();
                        sr.Dispose();
                    }
                    catch (Exception e)
                    {

                    }

                    MailMessage mail = getMailMessage(Email);
                    mail.Body =  line;
                    mail.Subject = "Bienvenido a EMECI " + emeci ;
                    mail.IsBodyHtml = true;
                    
                    //agregar PDF
                    mail.Attachments.Add(new Attachment(memory, "PosicionesDeAcceso.pdf"));
                    SmtpClient client = getSmtpClient();
                   
                    try
                    {
                        client.Send(mail);
                    }
                    catch (SmtpException exSmtp)
                    {
                        strex = exSmtp.Message;
                    }
                }
                
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string jsoncons = serializer.Serialize(new { idpac = pac.idPaciente , ex = strex});
                return Content(jsoncons, "application/json");

        }

        private string getStates() {
            var Est = from e in _Db.Estados
                      select new { e.idEstado, e.Nombre };

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = string.Empty;
            json = serializer.Serialize(Est.ToList());
            return json;
        }

        private string getCitys() {
            var Cit = from c in _Db.Ciudades
                      select new { c.Nombre, c.idciudad };

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = string.Empty;
            json = serializer.Serialize(Cit.ToList());
            return json;
        }

        public ActionResult RegistroMedico() {
            
            @ViewBag.Estados = getStates();
            @ViewBag.Ciudades = getCitys();
            return View();
        }

        private SmtpClient getSmtpClient() {

            SmtpClient client = new SmtpClient();
            client.Host = ConfigurationManager.AppSettings["Host"];
            client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            string m = ConfigurationManager.AppSettings["MailFrom"];
            string p = ConfigurationManager.AppSettings["passwordMail"];
            client.Credentials = new System.Net.NetworkCredential(m, p);
            return client;
        }

        private MailMessage getMailMessage(string to) {

            MailMessage mail = new MailMessage();
            string[] manyEmails = to.Split(',');
            for (int b = 0; b < manyEmails.Length; b++) {
                mail.To.Add(manyEmails[b].Trim());
            }
            mail.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
            mail.To.Add(mail.From.ToString());
            return mail;
        }

        public string SaveRegistroMedico(string name, string lastname, string rfc, string cedula, string nossa, string cmcp, string hospitalresidencia, string uniEspecialidad,
            string cedulaEspecialidad, string subespecialidad, string cedulasubespecialidad, string colegioespecialidad, string nombreagrupacion, string noexterior, string CP,
            string domicilio, string colonia, string estado, string Ciudad, string domicilioconsultorio, string tel, string telC, string celular, string fax, string email, string tipopediatra, string modify)
        {

            String[] data = new string[]{name, lastname, rfc, cedula, nossa, cmcp, 
                hospitalresidencia, uniEspecialidad, cedulaEspecialidad, 
                colegioespecialidad, nombreagrupacion, noexterior, CP, 
                domicilio, colonia, estado, Ciudad, domicilioconsultorio, tel, telC, celular,
                fax, email};
             
            if (Convert.ToBoolean(modify)) { 
               
                int idMedico = (int)Session["idmedico"];

                Medico med = (from m in _Db.Medico
                             where m.Idmedico == idMedico
                             select m).FirstOrDefault();

                med.HospitalResidenciaPediatra = hospitalresidencia;
                med.CedulaProfesional = cedula;
                med.CedulaEspecialidad = cedulaEspecialidad;
                med.CertCMCP = cmcp;
                med.AgrupacionLocal = colegioespecialidad;
                med.AgrupacionNacional = nombreagrupacion;
                med.NoRegSSA = nossa;
                med.RFC = rfc;
                med.TelefonoConsultorio = telC;
                med.TelefonoFax = fax;
                med.UniversidadEspecialidad = uniEspecialidad;
                med.DomicilioConsultorio = domicilioconsultorio;
                //med.PediatraPlus = Convert.ToByte(tipopediatra);

                Registro reg = (from r in _Db.Registro
                               where r.idRegistro == med.IdRegistro
                               select r).FirstOrDefault();

                reg.NoExt = noexterior;
                reg.Nombre = name;
                //reg.idEstado = estado;
                //reg.idCiudad = Convert.ToInt32(Ciudad);
                reg.Telefono = tel;
                reg.TelefonoCel = celular;
                reg.Domicilio = domicilio;
                reg.Colonia = colonia;
                reg.CodigoPostal = CP;
                reg.Apellido = lastname;
                reg.Emails = email;

                _Db.SaveChanges();
            }

            if (Convert.ToBoolean(modify) == false) {

                Session["RegistroMedico"] = data;
                
                MailMessage mail = getMailMessage(email);
                mail.Subject = "Registro Medico";
                mail.Body = RenderPartialViewToString("emailRegistro", "");
                mail.IsBodyHtml = true;
                
                SmtpClient client = getSmtpClient();
                try
                {
                    client.Send(mail);
                }
                catch (SmtpException exSmtp)
                {
                    var ex = exSmtp.Message;
                    return exSmtp.Message + " " + exSmtp.StackTrace;
                }
            }
            return "";
        }

        public ActionResult emailRegistro() {
            

            return View();
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                if (Session["RegistroMedico"] != null)
                {

                    string[] datos = Session["RegistroMedico"] as string[];
                    viewContext.ViewBag.Nombre = datos[0];
                    viewContext.ViewBag.Apellido = datos[1];
                    viewContext.ViewBag.Rfc = datos[2];
                    viewContext.ViewBag.Cedula = datos[3];
                    viewContext.ViewBag.Nossa = datos[4];
                    viewContext.ViewBag.cmcp = datos[5];
                    viewContext.ViewBag.hospitalResidencia = datos[6];
                    viewContext.ViewBag.uniEspecialidad = datos[7];
                    viewContext.ViewBag.cedulaEspecialidad = datos[8];
                    viewContext.ViewBag.colegioEspecialidad = datos[9];
                    viewContext.ViewBag.Agrupacion = datos[10];
                    viewContext.ViewBag.noExterior = datos[11];
                    viewContext.ViewBag.CP = datos[12];
                    viewContext.ViewBag.Domicilio = datos[13];
                    viewContext.ViewBag.Colonia = datos[14];
                    viewContext.ViewBag.Estado = datos[15];
                    viewContext.ViewBag.Ciudad = datos[16];
                    viewContext.ViewBag.domicioConsultorio = datos[17];
                    viewContext.ViewBag.tel = datos[18];
                    viewContext.ViewBag.telc = datos[19];
                    viewContext.ViewBag.celular = datos[20];
                    viewContext.ViewBag.fax = datos[21];
                    viewContext.ViewBag.Email = datos[22];

                }

                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult MisDatosMedico()
        {
            if (Session["idmedico"] == null) return Redirect("~/medico/login");

            int id = (int)Session["idmedico"];

            Medico Med = (from m in _Db.Medico
                         where m.Idmedico == id
                         select m).FirstOrDefault();

            Registro Reg = (from r in _Db.Registro
                            where r.idRegistro == Med.IdRegistro
                            select r).FirstOrDefault();

            Estados Est = (from e in _Db.Estados
                           where e.idEstado == Reg.idEstado
                           select e).FirstOrDefault();

            Ciudades Ciu = (from c in _Db.Ciudades
                            where c.idciudad == Reg.idCiudad
                            select c).FirstOrDefault();

            ViewBag.name = Reg.Nombre;
            ViewBag.lastname = Reg.Apellido;
            ViewBag.rfc = Med.RFC;
            ViewBag.cedulaE = Med.CedulaEspecialidad;
            ViewBag.cedulaP = Med.CedulaProfesional;
            ViewBag.ssa = Med.NoRegSSA;
            ViewBag.cmcp = Med.CertCMCP;
            ViewBag.hospital = Med.HospitalResidenciaPediatra;
            ViewBag.uni = Med.UniversidadEspecialidad;
            ViewBag.address = Reg.Domicilio;
            ViewBag.noExt = Reg.NoExt;
            ViewBag.cp = Reg.CodigoPostal;
            ViewBag.colony = Reg.Colonia;
            ViewBag.addressConsul = Med.DomicilioConsultorio;
            ViewBag.phoneConsul = Med.TelefonoConsultorio;
            ViewBag.phone = Reg.Telefono;
            ViewBag.cellphone = Reg.TelefonoCel;
            ViewBag.fax = Med.TelefonoFax;
            ViewBag.email = Reg.Emails;
            ViewBag.state = Est.Nombre;
            try
            {
            ViewBag.city = Ciu.Nombre;
            }
            catch 
                {
                }

            
            ViewBag.colegioEstatal = Med.AgrupacionLocal;
            ViewBag.AgrupacionEstatal = Med.AgrupacionNacional;
            try
            {
                ViewBag.listStates = getStates();
            }
            catch { }

            try
            {
                ViewBag.listCitys = getCitys();
            }
            catch { }

            return View();
        }

        [HttpPost]
        public ActionResult RegistroMedico(string cedula, string nombre, string apellido)
        {

            return View();
        }

        [HttpPost]
        public ActionResult ChangePwd(string currentpwd, string newpwd )
        {
             int idmed = (int)Session["idmedico"];
             string jsoncons;
             
            var dr = (from r in _Db.Registro
                     join m in _Db.Medico on r.idRegistro equals m.IdRegistro
                     where m.Idmedico ==idmed select r).ToList();
            
            if (dr.Count>0)
            {
                if (dr.First().clave == currentpwd)
                {
                    dr[0].clave = newpwd;
                    _Db.SaveChanges();
                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    jsoncons = serializer.Serialize(
                    new
                    {
                        success = "OK"
                    }
                     );
                }
                else
                {
                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    jsoncons = serializer.Serialize(
                     new
                     {
                         success = "Error",
                         message = "Contraseña no coincide"
                     }
                      );
                }    
            }
            else
            {
               var  serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
               jsoncons = serializer.Serialize(
                new{
                    success="Error",
                    message="No se encontro médico"
                  }
                 );
            }     

            return Content(jsoncons, "application/json");
            
        }

    }
}
