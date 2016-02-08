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
    public class HomeController : BaseController
    {
        private readonly HomeLogic _homeLogic;

        public HomeController(HomeLogic homeLogic)
        {
            _homeLogic = homeLogic;
        }

        [HttpGet, AllowAnonymous]
        public async Task<ViewResult> Index()
        {
            _homeLogic.UserId = UserId;

            HomeViewModel viewModel = new HomeViewModel
            {
                OpenSessions = await _homeLogic.GetOpenSessions(),
                NewSessions = await _homeLogic.GetNewSessions(),
                RecommendedSessions = await _homeLogic.GetNewSessions()
            };

            return View(viewModel);
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            var configuration = new Configuration();
            var migrator = new DbMigrator(configuration);
            migrator.Update();

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _homeLogic.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}