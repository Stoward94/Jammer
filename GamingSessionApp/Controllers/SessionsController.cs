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
            var sessions = await _sessionLogic.GetSessions(UserId);
            return View(sessions);
        }

        #endregion
        
        #region Create Session

        // GET: Sessions/Create
        [HttpGet]
        public async Task<ViewResult> Create()
        {
            var model = await _sessionLogic.PrepareCreateSessionVM(new CreateSessionVM());

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateSessionVM viewModel)
        {
            if (ModelState.IsValid)
            {
                //Pass the view model to the create logic
                ValidationResult result = await _sessionLogic.CreateSession(viewModel, UserId);

                if(result.Success)
                    return RedirectToAction("Index");

                //Something went wrong
                ModelState.AddModelError("", result.Error);
            }

            viewModel = await _sessionLogic.PrepareCreateSessionVM(viewModel);
            return View(viewModel);
        }

        #endregion

        #region View Session

        // GET: Sessions/Details/{Guid}
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult> Details(Guid id)
        {
            if (id == Guid.Empty) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            var model = await _detailsVmLogic.PrepareViewSessionVm(id, UserId);

            if (model == null) return HttpNotFound();

            return View(model);
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
            if (!ModelState.IsValid)
            {
                viewModel = await _sessionLogic.PrepareEditSessionVM(viewModel);
                return View(viewModel);
            }

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

            ValidationResult result = await _sessionLogic.AddSessionComment(comment, sessionId, UserId);

            if(result.Success)
            {
                return RedirectToAction("Details", new { id = sessionId });
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        public async Task<ActionResult> JoinSession(Guid sessionId)
        {
            if (sessionId == Guid.Empty) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            ValidationResult result = await _sessionLogic.AddUserToSession(UserId, sessionId);

            if(result.Success)
                return RedirectToAction("Details", new {id = sessionId});

            return new HttpStatusCodeResult(HttpStatusCode.Conflict);
        }

        public async Task<ActionResult> LeaveSession(Guid sessionId)
        {
            if (sessionId == Guid.Empty) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            PassUserToLogic();

            if (await _sessionLogic.RemoveUserFromSession(UserId, sessionId))
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
