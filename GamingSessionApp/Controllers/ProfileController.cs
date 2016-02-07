using System;
using System.Collections.Generic;
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

        [HttpGet]
        [Authorize]
        public PartialViewResult UserMenu()
        {
            var model = _profileLogic.GetUserMenuInformation(UserId);

            return PartialView("_UserMenu", model);
        }

        [HttpGet]
        [Authorize]
        public async Task<PartialViewResult> GetNotifications()
        {
            if (Request.IsAjaxRequest())
            {
                var model = await _profileLogic.GetNotifications(UserId);

                if (model != null)
                    return PartialView("_UserNotifications", model);
            }
            return null;
        }

        [HttpPost]
        [Authorize]
        public async Task UpdateNotifications(List<Guid> ids)
        {
            if (ids == null) return;

            if (Request.IsAjaxRequest())
            {
                await _profileLogic.UpdateNotifications(UserId, ids);
            }
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