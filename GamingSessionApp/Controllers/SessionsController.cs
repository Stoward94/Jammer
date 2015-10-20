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
        private readonly SessionDetailsVmLogic _detailsVmLogic;

        public SessionsController(SessionLogic sessionLogic, SessionDetailsVmLogic detailsVmLogic)
        {
            _sessionLogic = sessionLogic;
            _detailsVmLogic = detailsVmLogic;
        }

        #region All Sessions
        
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            PassUserToLogic();

            List<Session> sessions = await _sessionLogic.GetAll();
            return View(sessions);
        }

        #endregion
        
        #region Create Session

        // GET: Sessions/Create
        [HttpGet]
        public async Task<ViewResult> Create()
        {
            var viewModel = new CreateSessionVM {CreatorId = UserId};
            viewModel = await _sessionLogic.PrepareCreateSessionVM(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateSessionVM viewModel)
        {
            if (ModelState.IsValid)
            {
                PassUserToLogic();

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
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            PassUserToLogic();

            var viewModel = await _detailsVmLogic.PrepareViewSessionVm(id.Value);

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

        public async Task<ActionResult> PostComment(string comment, Guid sessionId)
        {
            if(string.IsNullOrEmpty(comment) || sessionId == Guid.Empty)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            PassUserToLogic();

            if (await _sessionLogic.AddSessionComment(comment, sessionId))
            {
                return RedirectToAction("Details", new { id = sessionId });
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        public async Task<ActionResult> JoinSession(Guid sessionId)
        {
            if (sessionId == Guid.Empty) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            PassUserToLogic();

            if (await _sessionLogic.AddUserToSession(sessionId))
            {
                return RedirectToAction("Details", new {id = sessionId});
            }

            return new HttpStatusCodeResult(HttpStatusCode.Conflict);
        }

        public async Task<ActionResult> LeaveSession(Guid sessionId)
        {
            if (sessionId == Guid.Empty) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            PassUserToLogic();

            if (await _sessionLogic.RemoveUserFromSession(sessionId))
            {
                return RedirectToAction("Details", new { id = sessionId });
            }

            return new HttpStatusCodeResult(HttpStatusCode.Conflict);
        }

        /// <summary>
        /// Passes the current user to the session logic. (or null)
        /// </summary>
        private void PassUserToLogic()
        {
            //If we have a user then pass the Id
            _sessionLogic.UserId = UserId;
            _detailsVmLogic.UserId = UserId;
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _sessionLogic.Dispose();
                _detailsVmLogic.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
