using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GamingSessionApp.BusinessLogic;
using GamingSessionApp.ViewModels.Inbox;

namespace GamingSessionApp.Controllers
{
    public class MessagesController : BaseController
    {
        private readonly MessageLogic _messageLogic;

        public MessagesController(MessageLogic messageLogic)
        {
            _messageLogic = messageLogic;
        }

        [HttpGet]
        public async Task<ActionResult> Inbox(int page = 1)
        {
            var model = await _messageLogic.GetUsersMessages(UserId, page);
            
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Outbox(int page = 1)
        {
            var model = await _messageLogic.GetUserOutbox(UserId, page);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> View(Guid id)
        {
            if (id == Guid.Empty) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ViewMessageViewModel model = await _messageLogic.ViewMessage(id, UserId);

            if(model == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View(model);
        }

        //View a message you have sent
        [HttpGet]
        public async Task<ActionResult> Sent(Guid id)
        {
            if (id == Guid.Empty) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ViewMessageViewModel model = await _messageLogic.ViewSentMessage(id, UserId);

            if (model == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View("View", model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new CreateMessageViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateMessageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ValidationResult result = await _messageLogic.CreateMessage(model, UserId);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Your message has been sent.";
                return RedirectToAction("Inbox", "Messages");
            }

            ModelState.AddModelError("", result.Error);
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> MarkRead(List<Guid> ids)
        {
            if(ids.Count < 1)
                return Json(new { success = false, responseText = "No id provided" });

            ValidationResult result = await _messageLogic.MarkRead(ids, UserId);

            if (result.Success)
            {
                return Json(new { success = true, responseText = "Message updated" });
            }

            return Json(new { success = false, responseText = result.Error });

        }

        [HttpDelete]
        public async Task<JsonResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return Json(new { success = false, responseText = "No id provided" });
            
            ValidationResult result = await _messageLogic.DeleteMessage(id, UserId);

            if (result.Success)
            {
                return Json(new { success = true, responseText = "Message deleted" });
            }

            return Json(new { success = false, responseText = result.Error });
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