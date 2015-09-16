using System.Web.Mvc;
using GamingSessionApp.BusinessLogic;
using Microsoft.Practices.Unity;
using Unity.Mvc3;
using GamingSessionApp.Controllers;

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

            container.RegisterType<SessionLogic>();          

            return container;
        }
    }
}