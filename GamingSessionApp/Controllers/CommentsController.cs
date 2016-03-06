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
            if (ModelState.IsValid)
            {
                int commentId = await _commentLogic.AddSessionComment(model, UserId);

                if (commentId != 0)
                {
                    var newComment = await _commentLogic.LoadComment(commentId, UserId);
                    if (newComment != null)
                    {
                        return PartialView("_Comment", newComment);
                    }
                }
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Unable to post your comment at this time.Please try again later");
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "You cannot post nothing! Get typing");
        }
    }
}