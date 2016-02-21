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
        [Route("Submit/{sessionId}")]
        public async Task<ActionResult> Submit(Guid sessionId)
        {
            var model = await _feedbackLogic.SubmitFeedbackViewModel(sessionId, UserId);

            if (model == null) return HttpNotFound();

            return View(model);
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