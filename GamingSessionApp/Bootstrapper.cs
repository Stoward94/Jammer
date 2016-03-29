using System.Web.Mvc;
using GamingSessionApp.BusinessLogic;
using Microsoft.Practices.Unity;
using Unity.Mvc3;
using GamingSessionApp.Controllers;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using Microsoft.Owin.Security;
using System.Web;

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

            container.RegisterType<ApplicationDbContext>(new PerRequestLifetimeManager());
            container.RegisterType<UnitOfWork>(new PerRequestLifetimeManager());

            container.RegisterType<IAuthenticationManager>(new InjectionFactory(o => HttpContext.Current.GetOwinContext().Authentication));
            
            container.RegisterType<ManageController>(new InjectionConstructor());

            container.RegisterType<SessionLogic>();
            container.RegisterType<SessionDetailsVmLogic>();
            container.RegisterType<HomeLogic>();

            container.RegisterType<IUserLogic, UserLogic>();
            container.RegisterType<IFeedbackLogic, FeedbackLogic>();
            container.RegisterType<INotificationLogic, NotificationLogic>();
            container.RegisterType<IMessageLogic, MessageLogic>();
            container.RegisterType<IHomeLogic, HomeLogic>();
            container.RegisterType<IProfileLogic, ProfileLogic>();
            container.RegisterType<ISessionCommentLogic, SessionCommentLogic>();
            container.RegisterType<ISessionLogic, SessionLogic>();
            container.RegisterType<IUserPreferencesLogic, UserPreferencesLogic>();

            return container;
        }
    }
}