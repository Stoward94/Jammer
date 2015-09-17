using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GamingSessionApp.BusinessLogic;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Session;

namespace GamingSessionApp.Controllers
{
    public class SessionsController : BaseController
    {
        private readonly SessionLogic _sessionLogic;

        public SessionsController(SessionLogic sessionLogic)
        {
            _sessionLogic = sessionLogic;
        }

        // GET: Sessions
        public async Task<ActionResult> Index()
        {
            List<Session> sessions = await _sessionLogic.GetAll();
            return View(sessions);
        }
        
        #region Create Session

        // GET: Sessions/Create
        [HttpGet]
        [Authorize]
        public async Task<ViewResult> Create()
        {
            var viewModel = new CreateSessionViewModel {CreatorId = UserId};
            viewModel = await _sessionLogic.PrepareCreateSessionViewModel(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateSessionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                //Pass the view model to the create logic
                if (await _sessionLogic.CreateSession(viewModel))
                    return RedirectToAction("Index");
            }

            viewModel = await _sessionLogic.PrepareCreateSessionViewModel(viewModel);
            return View(viewModel);
        }

        #endregion

        #region View Session

        // GET: Sessions/Details/{Guid}
        [HttpGet]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var viewModel = await _sessionLogic.PrepareViewSessionViewModel(id.Value);

            if (viewModel == null) return HttpNotFound();

            return View(viewModel);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _sessionLogic.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
