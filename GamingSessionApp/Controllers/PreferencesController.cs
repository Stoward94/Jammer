using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using GamingSessionApp.BusinessLogic;
using GamingSessionApp.ViewModels.Preferences;

namespace GamingSessionApp.Controllers
{
    public class PreferencesController : BaseController
    {

        private readonly UserPreferencesLogic _preferencesLogic;

        public PreferencesController(UserPreferencesLogic preferencesLogic)
        {
            _preferencesLogic = preferencesLogic;
        }

        [HttpGet]
        public async Task<ActionResult> Edit()
        {
            var model = await _preferencesLogic.GetEditPreferencesModel(UserId);

            if (model == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserPreferencesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ReminderTimes = await _preferencesLogic.GetReminderTimes();
                return View(model);
            }

            ValidationResult result = await _preferencesLogic.EditPreferences(model, UserId);

            if (result.Success)
            {
                ViewBag.SuccessMessage = "Your changes have been saved.";
            }
            else
            {
                ModelState.AddModelError("", result.Error);
            }

            model.ReminderTimes = await _preferencesLogic.GetReminderTimes();
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _preferencesLogic.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}