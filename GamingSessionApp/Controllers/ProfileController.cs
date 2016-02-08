using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using GamingSessionApp.BusinessLogic;
using GamingSessionApp.ViewModels.Profile;
using Microsoft.AspNet.Identity;

namespace GamingSessionApp.Controllers
{
    [Route("Profile/[action]")]
    public class ProfileController : BaseController
    {
        private readonly ProfileLogic _profileLogic;

        public ProfileController(ProfileLogic profileLogic)
        {
            _profileLogic = profileLogic;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Profile/{userName}")]
        public async Task<ActionResult> UserProfile(string userName)
        {
            if(userName == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //Is my profile?
            if (userName == User.Identity.GetUserName())
            {
                UserProfileViewModel model = await _profileLogic.GetMyProfile(UserId);

                return View("MyProfile", model);
            }
            else
            {
                UserProfileViewModel model = await _profileLogic.GetUserProfile(userName);

                if (model == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                return View("UserProfile", model);
            }
        }

        [HttpPost]
        //[Route("Profile/AddFriend")]
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

        [HttpGet]
        public PartialViewResult UserMenu()
        {
            var model = _profileLogic.GetUserMenuInformation(UserId);

            return PartialView("_UserMenu", model);
        }

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