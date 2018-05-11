using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using EmeciDoctoresV2.Models;
namespace EmeciDoctoresV2
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
       
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

          
        }


        protected void Application_BeginRequest(object sender, EventArgs e)
        { 
            Log.write("Begin REQUEST: " + HttpContext.Current.Request.RawUrl);
            string urlapi = HttpContext.Current.Request.RawUrl.Split('?')[0].ToLower();
            Log.write("url api: " + urlapi);
            


        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            Log.write("auth  request: " + HttpContext.Current.Request.RawUrl);
            //if (HttpContext.Current.Request.RawUrl == "/apita/config")
            //{
            //    HttpContext.Current.RewritePath(HttpContext.Current.Request.RawUrl + "/index.ashx");
            //}

        }

        protected void Application_Error(object sender, EventArgs e)
        {

            Log.write("ERROR: ");
            string strParams="---Parameters: ";
            
            foreach(string nval in HttpContext.Current.Request.Form.AllKeys)
            {
                strParams += nval + "=" + HttpContext.Current.Request.Form[nval] + "&";
            }

            Log.write("App ERROR: " + HttpContext.Current.Server.GetLastError().Message + " " + HttpContext.Current.Server.GetLastError().StackTrace + ",  " + HttpContext.Current.Request.RawUrl + strParams);
        }
    }
}