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
        private SessionLogic _sessionLogic;

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

        // GET: Sessions/Details/5
        public async Task<ActionResult> Details(int id = 0)
        {
            if (id == 0) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Session session = await _sessionLogic.GetByIdAsync(id);

            if (session == null) return HttpNotFound();

            return View(session);
        }

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
