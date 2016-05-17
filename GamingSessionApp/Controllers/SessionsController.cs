using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using GamingSessionApp.BusinessLogic;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Session;
using GamingSessionApp.ViewModels.SessionComments;

namespace GamingSessionApp.Controllers
{
    public class SessionsController : BaseController
    {
        private readonly ISessionLogic _sessionLogic;
        private readonly SessionDetailsVmLogic _detailsVmLogic;

        public SessionsController(ISessionLogic sessionLogic, SessionDetailsVmLogic detailsVmLogic)
        {
            _sessionLogic = sessionLogic;
            _detailsVmLogic = detailsVmLogic;
        }

        #region All Sessions
        
        //[HttpGet, AllowAnonymous]
        //public async Task<ActionResult> Index(SessionsFilter filter)
        //{
        //    var sessions = await _sessionLogic.GetSessions(UserId, filter);
            
        //    //Filter options lists
        //    ViewData["PlatformsList"] = await _sessionLogic.GetPlatformsList();
        //    ViewData["TypesList"] = await _sessionLogic.GetTypesList();

        //    return View(sessions);
        //}

        [HttpGet, AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            var sessions = await _sessionLogic.GetSessions(UserId);

            ViewData["Page"] = 1;
            
            //Filter options lists
            ViewData["PlatformsList"] = await _sessionLogic.GetPlatformsList();
            ViewData["TypesList"] = await _sessionLogic.GetTypesList();

            return View(sessions);
        }

        [HttpGet, AllowAnonymous]
        public async Task<ActionResult> Filter(SessionsFilter filter)
        {
            var sessions = await _sessionLogic.GetSessions(UserId, filter);

            ViewData["Page"] = filter.Page;

            return PartialView("_AllSessions", sessions);
        }

        #endregion

        #region Create Session

        // GET: Sessions/Create
        [HttpGet]
        public async Task<ViewResult> Create()
        {
            var model = await _sessionLogic.CreateSessionViewModel(new CreateSessionVM(), UserId);

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

                if (result.Success)
                {
                    if (result.Data != null)
                        return RedirectToAction("Details", new {id = result.Data});

                    return RedirectToAction("Index");
                }

                //Something went wrong
                ModelState.AddModelError("", result.Error);
            }

            viewModel = await _sessionLogic.CreateSessionViewModel(viewModel, UserId);
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
        public async Task<ActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var viewModel = await _sessionLogic.EditSessionVM(id, UserId);

            if (viewModel == null) return HttpNotFound();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditSessionVM viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.SessionId == Guid.Empty) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                //Try and update the record
                ValidationResult result = await _sessionLogic.EditSession(viewModel, UserId);

                if (result.Success)
                {
                    //Redirect on success
                    return RedirectToAction("Details", new {id = viewModel.SessionId});
                }

                //Something went wrong
                ModelState.AddModelError("", result.Error);
            }

            viewModel = await _sessionLogic.PrepareEditSessionVM(viewModel);
            return View(viewModel);
        }

        #endregion

        
        public async Task<ActionResult> JoinSession(Guid sessionId)
        {
            if (sessionId == Guid.Empty) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            ValidationResult result = await _sessionLogic.AddUserToSession(UserId, sessionId);

            if (result.Success)
                return RedirectToAction("Details", new {id = sessionId});

            //Add error
            TempData["ErrorMsg"] = result.Error;
            return RedirectToAction("Details", new { id = sessionId });
        }

        public async Task<ActionResult> LeaveSession(Guid sessionId)
        {
            if (sessionId == Guid.Empty) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            ValidationResult result = await _sessionLogic.RemoveUserFromSession(UserId, sessionId);

            if (result.Success)
                return RedirectToAction("Details", new { id = sessionId });

            //Add error
            TempData["ErrorMsg"] = result.Error;
            return RedirectToAction("Details", new { id = sessionId });
        }

        public async Task<ActionResult> KickUser(string kickUserId, Guid sessionId)
        {
            if (sessionId == Guid.Empty || string.IsNullOrEmpty(kickUserId))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ValidationResult result = await _sessionLogic.KickUserFromSession(kickUserId, sessionId, UserId);
            
            //Add error
            if (!result.Success)
                TempData["ErrorMsg"] = result.Error;

            return RedirectToAction("Details", new { id = sessionId });
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
