using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GamingSessionApp.BusinessLogic;
using GamingSessionApp.Migrations;
using GamingSessionApp.ViewModels.Home;

namespace GamingSessionApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomeLogic _homeLogic;

        public HomeController(HomeLogic homeLogic)
        {
            _homeLogic = homeLogic;
        }

        public async Task<ViewResult> Index()
        {
            HomeViewModel viewModel = new HomeViewModel
            {
                OpenSessions = await _homeLogic.GetOpenSessions(),
                NewSessions = await _homeLogic.GetNewSessions(),
                RecommendedSessions = await _homeLogic.GetNewSessions()


            };

            return View(viewModel);
        }

        public ActionResult Contact()
        {
            var configuration = new Configuration();
            var migrator = new DbMigrator(configuration);
            migrator.Update();

            return View();
        }
    }
}