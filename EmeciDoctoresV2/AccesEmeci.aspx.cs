using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using EmeciCommon;
using EmeciFacade;
namespace EmeciDoctoresV2
{
    public partial class AccesEmeci : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
                  DataSet   dsPac;
             
            string emeci= Request.QueryString["Emeci"].ToString();
            string opt= Request.QueryString["opt"].ToString();
            string posicion = Request.QueryString["posicion"].ToString();
            string dato = Request.QueryString["dato"].ToString();
              
             
            dsPac = (new clsFARegistro()).GetPacienteTarjetaEmeci(dato, emeci, posicion);
             
            
            int idpaciente=0;
            if( !(dsPac == null) && dsPac.Tables[0].Rows.Count > 0)

                idpaciente = (int)dsPac.Tables[0].Rows[0]["idpaciente"];
            else
            {
                Response.Write("error paciente o clave incorrecta");
                Response.End();
            }
            
            switch(opt)
            {
                case "E": Response.Redirect("../Expediente/generales.aspx?d=" + DateTime.Now.ToString()); break;
                case "V": Response.Redirect("../Expediente/expediente_vacunas.aspx?d=" + DateTime.Now.ToString()); break;
            }

           

            this.Session["PacEmeci"] = emeci;
            this.Session["idpaciente"] = dsPac.Tables[0].Rows[0]["idpaciente"];
            this.Session["idregistro"] = dsPac.Tables[0].Rows[0]["idregistro"];

        }
    }
}