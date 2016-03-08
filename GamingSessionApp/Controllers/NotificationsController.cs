using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using GamingSessionApp.BusinessLogic;

namespace GamingSessionApp.Controllers
{
    public class NotificationsController : BaseController
    {
        private readonly NotificationLogic _notificationLogic;

        public NotificationsController(NotificationLogic notificationLogic)
        {
            _notificationLogic = notificationLogic;
        }

        [HttpGet]
        public async Task<ActionResult> ViewAll(int page = 1)
        {
            var model = await _notificationLogic.GetAllForUser(UserId, page);

            return View(model);
        }

        [HttpGet]
        public async Task<PartialViewResult> GetNotifications()
        {
            if (Request.IsAjaxRequest())
            {
                var model = await _notificationLogic.GetNotifications(UserId);

                if (model != null)
                    return PartialView("_UserNotifications", model);
            }
            return null;
        }

        [HttpPost]
        public async Task UpdateNotifications(List<Guid> ids)
        {
            if (ids == null) return;

            if (Request.IsAjaxRequest())
            {
                await _notificationLogic.UpdateNotifications(UserId, ids);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _notificationLogic.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}