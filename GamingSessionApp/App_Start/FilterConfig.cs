using System.Web;
using System.Web.Mvc;

namespace GamingSessionApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RequireHttpsAttribute()); //Https required for the whole site.
            filters.Add(new AuthorizeAttribute()); //Lock down all of the site so a white list of pages can be created
        }
    }
}
