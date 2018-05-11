using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace EmeciDoctoresV2
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //   routeTemplate: "api/{controller}/{action}"//,
            //   // defaults: new { id = RouteParameter.Optional }
            //);

           // config.Routes.MapHttpRoute(
           //    name: "Api3",
           //    routeTemplate: "api/{controller}/{action}/{token}"
           //     // , defaults: new { token = RouteParameter.Optional }
           //);
            config.Routes.MapHttpRoute(
            name: "Api4",
            routeTemplate: "api/{controller}/{action}/{token}/{id}"
          , defaults: new { token = RouteParameter.Optional, id = RouteParameter.Optional }
        );

          //  config.Routes.MapHttpRoute(
          //    name: "Api2",
          //    routeTemplate: "api/{controller}/{action}/{id}"
          //        , defaults: new { id = RouteParameter.Optional }
          //);

         //   config.Routes.MapHttpRoute(
         //    name: "Api",
         //    routeTemplate: "api/{controller}/{action}"
         //  //, defaults: new { id = RouteParameter.Optional }
         //);

           

          

          

             
        }
    }
}
