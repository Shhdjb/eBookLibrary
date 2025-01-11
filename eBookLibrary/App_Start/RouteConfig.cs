using System.Web.Mvc;
using System.Web.Routing;

namespace eBookLibrary
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Ignore routes for resource files
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Admin-specific routes
            routes.MapRoute(
              name: "AdminManageCatalog",
              url: "Admin/ManageCatalog",
              defaults: new { controller = "Admin", action = "ManageCatalog" }
            );




            routes.MapRoute(
                name: "AdminEditBook",
                url: "Admin/EditBook/{id}",
                defaults: new { controller = "Admin", action = "EditBook", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "AdminDeleteBook",
                url: "Admin/DeleteBook/{id}",
                defaults: new { controller = "Admin", action = "DeleteBook", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "Admin",
                url: "Admin/{action}/{id}",
                defaults: new { controller = "Admin", action = "BookCatalog", id = UrlParameter.Optional }
            );

            // Default route
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
