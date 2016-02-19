using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Session;

namespace GamingSessionApp.BusinessLogic
{
    public class SessionDetailsVmLogic : SessionLogic
    {
        //Session Repository
        private readonly GenericRepository<Session> _sessionRepo;

        public SessionDetailsVmLogic()
        {
            _sessionRepo = UoW.Repository<Session>();
        }

        /// <summary>
        /// Prepares the view model used to view a session ready to be passed the view
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<SessionDetailsVM> PrepareViewSessionVm(Guid sessionId, string userId)
        {
            try
            {
                UserId = userId;

                //Load the session from the db
                SessionDetailsVM model = await _sessionRepo.Get(x => x.Id == sessionId)
                    .Select(x => new SessionDetailsVM
                    {
                        Id = x.Id,
                        CreatorId = x.CreatorId,
                        CreatorName = x.Creator.DisplayName,
                        CreatedDate = x.CreatedDate,
                        Duration = x.Duration.Duration,
                        MembersRequired = x.MembersRequired,
                        IsPublic = x.Settings.IsPublic,
                        Information = x.Information,
                        Platform = x.Platform.Name,
                        ScheduledDate = x.ScheduledDate,
                        Members = x.Members.Select(m => new SessionMemberViewModel
                        {
                            UserId = m.UserId,
                            DisplayName = m.DisplayName,
                            Kudos = m.Kudos.Points
                        }).ToList(),
                        Messages = x.Messages.ToList(),
                        MembersCount = x.Members.Count(),
                        Status = x.Status.Name,
                        Type = x.Type.Name,
                        Active = x.Active
                    })
                    .FirstOrDefaultAsync();

                if (model == null) return null;

                //Mark the correct member as host
                model.Members.First(m => m.UserId == model.CreatorId).IsHost = true;

                //Convert the DateTimes to the users time zone
                model.CreatedDate = model.CreatedDate.ToTimeZoneTime(GetUserTimeZone());
                model.ScheduledDate = model.ScheduledDate.ToTimeZoneTime(GetUserTimeZone());

                //Is the session joinable? (has room / already joined)
                model.CanJoin = ShowCanJoinBtn(model);

                //Is the session leaveable? (already joined && not creators)
                model.CanLeave = ShowCanLeaveBtn(model);

                //Can this user post on this session?
                model.CanPost = ShowPostCommentSection(model);

                return model;
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
        /// <param name="model"></param>
        /// <returns></returns>
        private bool ShowCanJoinBtn(SessionDetailsVM model)
        {
            //Allow anonymous users to see the button
            if (UserId == null) return true; 

            bool canJoin = model.Active; //Active session
            canJoin = canJoin && model.MembersCount < model.MembersRequired;//Free space(s)?
            canJoin = canJoin && model.Members.All(x => x.UserId != UserId);//Not already a member

            return canJoin;
        }

        /// <summary>
        /// Evaluates whether or not to show the 'leave' button on the view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool ShowCanLeaveBtn(SessionDetailsVM model)
        {
            bool canLeave = model.Active;
            canLeave = canLeave && UserId != null; //Anonymous user?
            canLeave = canLeave && model.CreatorId != UserId; //If not owner
            canLeave = canLeave && model.Members.Any(x => x.UserId == UserId);
            return canLeave;

        }

        /// <summary>
        /// Evaluates whether or not to show the post comment section on the view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool ShowPostCommentSection(SessionDetailsVM model)
        {
            bool canPost = UserId != null; //Anonymous user?
            canPost = canPost && model.Members.Any(x => x.UserId == UserId); //Is member

            return canPost;
        }
    }
}
