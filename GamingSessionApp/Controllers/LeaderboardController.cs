using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GamingSessionApp.BusinessLogic;

namespace GamingSessionApp.Controllers
{
    [AllowAnonymous]
    public class LeaderboardController : BaseController
    {
        private readonly ILeaderboardLogic _leaderboardLogic;

        public LeaderboardController(ILeaderboardLogic leaderboardLogic)
        {
            _leaderboardLogic = leaderboardLogic;
        }

        [HttpGet]
        public async Task<ViewResult> Index(int page = 1)
        {
            var model = await _leaderboardLogic.GetUserLeaderboard(UserId, page);

            return View(model);
        }
        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _leaderboardLogic.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}