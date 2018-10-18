using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyVeggieStore
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute("Account", "Account/{action}/{name}", new { controller = "Account", action = "Index", name = UrlParameter.Optional }, new[] { "MyVeggieStore.Controllers" });

            routes.MapRoute("Store", "Store/{action}/{name}", new { controller = "Store", action = "Index", name = UrlParameter.Optional }, new[] { "MyVeggieStore.Controllers" });

            routes.MapRoute("tblLocations", "tblLocations/{action}/{name}", new { controller = "tblLocations", action = "Index", name = UrlParameter.Optional }, new[] { "MyVeggieStore.Controllers" });

            routes.MapRoute("PagesMenuPartial", "Pages/PagesMenuPartial", new { controller = "Pages", action = "PagesMenuPartial" },
                new[] { "MyVeggieStore.Controllers" });

            routes.MapRoute("SidebarPartial", "Pages/SidebarPartial", new { controller = "Pages", action = "SidebarPartial" },
                 new[] { "MyVeggieStore.Controllers" });

            routes.MapRoute("EditSidebar", "Pages/EditSidebar", new { controller = "Pages", action = "EditSidebar" },
                new[] { "MyVeggieStore.Controllers" });

            routes.MapRoute("Pages", "{page}", new { controller = "Pages", action = "Index" },
                new[] { "MyVeggieStore.Controllers" });

            routes.MapRoute("Default", "", new { controller = "Pages", action = "Index" },
                new[] { "MyVeggieStore.Controllers" });

             //routes.MapRoute(
             //   name: "Default",
             //   url: "{controller}/{action}/{id}",
             //   defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
           // );
        }
    }
}
