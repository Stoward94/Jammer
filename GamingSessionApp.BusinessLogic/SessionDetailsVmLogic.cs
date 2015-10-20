using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Session;

namespace GamingSessionApp.BusinessLogic
{
    public class SessionDetailsVmLogic : SessionLogic
    {
        /// <summary>
        /// Prepares the view model used to view a session ready to be passed the view
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public async Task<SessionDetailsVM> PrepareViewSessionVm(Guid sessionId)
        {
            try
            {
                //Load the session from the db
                Session model = await GetByIdAsync(sessionId);

                if (model == null) return null;

                //Convert the DateTimes to the users time zone
                ConvertSessionTimesToTimeZone(model);

                //Map the properties to the view model
                var viewModel = Mapper.Map<Session, SessionDetailsVM>(model);

                //Map messages to viewModel
                viewModel.Messages = model.Messages.ToList();

                //Is the session joinable? (has room / already joined)
                viewModel.CanJoin = ShowCanJoinBtn(model);

                //Is the session leaveable? (already joined && not creators)
                viewModel.CanLeave = ShowCanLeaveBtn(model);

                //Can this user post on this session?
                viewModel.CanPost = ShowPostCommentSection(model);

                return viewModel;
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        /// <summary>
        /// Evaluates whether or not to show the 'join' button on the view
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private bool ShowCanJoinBtn(Session session)
        {
            //Allow anonymous users to see the button
            if (CurrentUser == null) return true; 

            bool canJoin = session.Active;
            canJoin = canJoin && session.SignedGamersCount < session.GamersRequired;
            canJoin = canJoin && CurrentUser.Sessions.All(x => x.Id != session.Id);

            return canJoin;
        }

        /// <summary>
        /// Evaluates whether or not to show the 'leave' button on the view
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private bool ShowCanLeaveBtn(Session session)
        {
            bool canLeave = session.Active;
            canLeave = canLeave && CurrentUser != null; //Anonymous user?
            canLeave = canLeave && session.Creator != CurrentUser;
            canLeave = canLeave && CurrentUser.Sessions.Any(x => x.Id == session.Id);
            return canLeave;

        }

        /// <summary>
        /// Evaluates whether or not to show the post comment section on the view
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private bool ShowPostCommentSection(Session session)
        {
            bool canPost = CurrentUser != null; //Anonymous user?
            canPost = canPost && CurrentUser.Sessions.Any(x => x.Id == session.Id);

            return canPost;
        }
    }
}
