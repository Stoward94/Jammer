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

        [HttpGet]
        public async Task<ActionResult> UserProfile(string userName)
        {
            UserProfileViewModel model = await _profileLogic.GetUserProfile(userName);

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
              _profileLogic.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public async Task<JsonResult> AddFriend(string userName)
        {
            if(userName == null)
                return Json(new { success = false, responseText = "No user provided" });

            _profileLogic.UserId = UserId;

            ValidationResult result = await _profileLogic.AddFriend(userName);

            //Return Success
            if (result.Success)
                return Json(new { success = true, responseText = "Friend added" });
            
            //Return error
            return Json(new { success = false, responseText = result.Error });
        }
    }
}