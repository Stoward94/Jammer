using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using IgdbApi;
using IgdbApi.Models;

namespace GamingSessionApp.Controllers
{
    public class IGDBController : Controller
    {
        private readonly IIgdbGamesApi _gamesApi;

        public IGDBController()
        {
            _gamesApi = new IgdbApi.IgdbApi("K5hOq_p3hvaQ7IyBpw5CD_Z6EJw-rCuiKsYWVwrRS1U");
        }

        // Search IGDB Games
        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> Search(string q)
        {

           var games = await _gamesApi.SearchGamesAsync(q);

            var model = new List<object>();
            foreach (var g in games)
            {
                model.Add(new {
                    id = g.Id,
                    label = g.Name,
                    date = g.ReleaseDate?.ToShortDateString() ?? "Unknown"
                });
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> Game(int id)
        {
            var game = await _gamesApi.GetGameAsync(id);

            var model = new
            {
                name = game.Name,
                cover = game.Cover
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Nothing to dispose atm
            }
            base.Dispose(disposing);
        }
    }
}