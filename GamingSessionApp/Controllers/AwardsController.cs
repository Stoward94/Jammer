using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GamingSessionApp.BusinessLogic;

namespace GamingSessionApp.Controllers
{
    public class AwardsController : BaseController
    {
        private readonly IAwardLogic _awardLogic;

        public AwardsController(IAwardLogic awardLogic)
        {
            _awardLogic = awardLogic;
        }

        // GET: Awards
        public async Task<ActionResult> MyAwards()
        {
            var model = await _awardLogic.GetUserAwards(UserId);

            if (model == null) return HttpNotFound();

            return View(model);
        }
    }
}