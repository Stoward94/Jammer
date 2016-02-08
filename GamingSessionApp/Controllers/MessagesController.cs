using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GamingSessionApp.BusinessLogic;

namespace GamingSessionApp.Controllers
{
    [Authorize]
    public class MessagesController : BaseController
    {
        private readonly MessageLogic _messageLogic;

        public MessagesController(MessageLogic messageLogic)
        {
            _messageLogic = messageLogic;
        }

        [HttpGet]
        public async Task<ActionResult> Inbox()
        {
            var model = await _messageLogic.GetUsersMessages(UserId);
            
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _messageLogic.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}