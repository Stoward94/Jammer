using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
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

        #region All Sessions
        
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            //If we have a user then pass the Id
            _sessionLogic.UserId = UserId;

            List<Session> sessions = await _sessionLogic.GetAll();
            return View(sessions);
        }

        #endregion
        
        #region Create Session

        // GET: Sessions/Create
        [HttpGet]
        [Authorize]
        public async Task<ViewResult> Create()
        {
            var viewModel = new CreateSessionVM {CreatorId = UserId};
            viewModel = await _sessionLogic.PrepareCreateSessionVM(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateSessionVM viewModel)
        {
            if (ModelState.IsValid)
            {
                //Pass the view model to the create logic
                if (await _sessionLogic.CreateSession(viewModel))
                    return RedirectToAction("Index");
            }

            viewModel = await _sessionLogic.PrepareCreateSessionVM(viewModel);
            return View(viewModel);
        }

        #endregion

        #region View Session

        // GET: Sessions/Details/{Guid}
        [HttpGet]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //If we have a user then pass the Id
            _sessionLogic.UserId = UserId;

            var viewModel = await _sessionLogic.PrepareViewSessionVM(id.Value);

            if (viewModel == null) return HttpNotFound();

            return View(viewModel);
        }

        #endregion

        #region Edit Session

        [HttpGet]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (!id.HasValue) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var viewModel = await _sessionLogic.EditSessionVM(id.Value);

            if (viewModel == null) return HttpNotFound();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditSessionVM viewModel)
        {
            if (viewModel.SessionId == Guid.Empty) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //Try and update the record
            if (await _sessionLogic.EditSession(viewModel))
            {
                //Redirect on success
                return RedirectToAction("Index");
            }

            viewModel = await _sessionLogic.PrepareEditSessionVM(viewModel);
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
