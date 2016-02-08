using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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
        [Authorize]
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
        [Authorize]
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