﻿using InventorySolution.Classes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace InventorySolution
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        void Application_EndRequest(object sender, EventArgs e)
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var loginPage = urlHelper.Action("Logon", "Account");
            var registerPage = urlHelper.Action("Register", "Account");

            if (Context.Request.Path != loginPage && Context.Request.Path != registerPage)
            {
                var logCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                
                if (Request.IsAuthenticated)
                {

                }
                else
                {
                    //redirect catre login daca nu este autentificat 
                    Response.Redirect(loginPage);
                }
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // Use LocalDB for Entity Framework by default
            Database.DefaultConnectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\v11.0; Integrated Security=True; MultipleActiveResultSets=True");

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BundleConfiguration.RegisterBundles(BundleTable.Bundles);
        }
    }
}