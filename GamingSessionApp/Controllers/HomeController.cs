using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using System.Web.Mvc;
using GamingSessionApp.BusinessLogic;
using GamingSessionApp.Migrations;
using GamingSessionApp.ViewModels.Home;

namespace GamingSessionApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IHomeLogic _homeLogic;

        public HomeController(IHomeLogic homeLogic)
        {
            _homeLogic = homeLogic;
        }

        [HttpGet, AllowAnonymous]
        public async Task<ViewResult> Index()
        {
            HomeViewModel viewModel = await _homeLogic.GetHomeViewModel(UserId);
            
            return View(viewModel);
        }

        [HttpGet, AllowAnonymous]
        public async Task<PartialViewResult> Search(string term)
        {
            SearchResultsViewModel viewModel = await _homeLogic.GetSearchResults(term, UserId);

            return PartialView("_SearchResults", viewModel);
        }



        [AllowAnonymous]
        public ActionResult Contact()
        {
            var configuration = new Configuration();
            var migrator = new DbMigrator(configuration);
            migrator.Update();

            return View();
        }

        [AllowAnonymous]
        [Route("Beta/")]
        public ActionResult Beta()
        {
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