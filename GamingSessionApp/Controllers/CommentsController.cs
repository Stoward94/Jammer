using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GamingSessionApp.BusinessLogic;
using GamingSessionApp.ViewModels.SessionComments;

namespace GamingSessionApp.Controllers
{
    public class CommentsController : BaseController
    {
        private readonly SessionCommentLogic _commentLogic;

        public CommentsController(SessionCommentLogic commentLogic)
        {
            _commentLogic = commentLogic;
        }

        [HttpPost]
        public async Task<ActionResult> Post(PostCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "You cannot post nothing! Get typing");
            }

            try
            {
                var comment = await _commentLogic.AddSessionComment(model, UserId);
                return PartialView("_Comment", comment);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError,
                    "Unable to post your comment at this time.Please try again later");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _commentLogic.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}