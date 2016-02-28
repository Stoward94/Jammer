using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GamingSessionApp.BusinessLogic;
using GamingSessionApp.ViewModels.Feedback;

namespace GamingSessionApp.Controllers
{
    public class FeedbackController : BaseController
    {
        private readonly FeedbackLogic _feedbackLogic;

        public FeedbackController(FeedbackLogic feedbackLogic)
        {
            _feedbackLogic = feedbackLogic;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SessionFeedback(Guid sessionId)
        {
            SessionFeedbackViewModel model = await _feedbackLogic.GetSessionFeedback(sessionId, UserId);

            if (model == null)
            {
                ModelState.AddModelError("", "Unable to get session feedback. Please try again later.");
            }

            //Else return the details view
            return PartialView("_SessionFeedback", model);
        }

        [HttpGet]
        public async Task<ActionResult> Submit(Guid sessionId)
        {
            var model = await _feedbackLogic.SubmitFeedbackViewModel(sessionId, UserId);

            if (model == null) return HttpNotFound();

            //Can submit then return submit view
            if (model.CanSubmitFeedback)
                return View(model);

            //Else return the details view
            return PartialView("_View", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Submit(CreateFeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                ValidationResult result = await _feedbackLogic.SubmitFeedback(model, UserId);

                if (result.Success)
                {
                    ViewBag.SuccessMessage = "Your feedback has been saved.";
                }
                else
                {
                    //Something went wrong
                    ModelState.AddModelError("", result.Error);
                }
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _feedbackLogic.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}