using System.Data.Entity;
using System.Web.Mvc;
using GamingSessionApp.BusinessLogic;
using Microsoft.Practices.Unity;
using Unity.Mvc3;
using GamingSessionApp.Controllers;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GamingSessionApp
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            //User Manager / DB contect
            container.RegisterType<UserManager<ApplicationUser>>(new HierarchicalLifetimeManager());
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(new HierarchicalLifetimeManager());
            container.RegisterType<DbContext, ApplicationDbContext>(new HierarchicalLifetimeManager());

            container.RegisterType<AccountController>(new InjectionConstructor());

            container.RegisterType<SessionLogic>();
            container.RegisterType<HomeLogic>();

            return container;
        }
    }
}