using System.Threading.Tasks;
using System.Web.Mvc;
using GamingSessionApp.BusinessLogic;
using GamingSessionApp.ViewModels.Profile;

namespace GamingSessionApp.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly ProfileLogic _profileLogic;

        public ProfileController(ProfileLogic profileLogic)
        {
            _profileLogic = profileLogic;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> MyProfile()
        {
            UserProfileViewModel model = await _profileLogic.GetMyProfile(UserId);

            return View(model);
        }

        //[HttpGet]
        //public ActionResult User(string displayName)
        //{


        //    return View();
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
              _profileLogic.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}