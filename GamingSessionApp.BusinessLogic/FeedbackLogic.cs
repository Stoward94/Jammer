using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Feedback;
using static GamingSessionApp.BusinessLogic.SystemEnums;

namespace GamingSessionApp.BusinessLogic
{
    public class FeedbackLogic : BaseLogic
    {
        private readonly GenericRepository<SessionFeedback> _feedbackRepo;

        public FeedbackLogic()
        {
            _feedbackRepo = UoW.Repository<SessionFeedback>();
        }

        public async Task<CreateFeedbackViewModel> SubmitFeedbackViewModel(Guid sessionId, string userId)
        {
            try
            {
                UserId = userId;

                //Find and load the session
                Session session = await UoW.Repository<Session>().Get(x => x.Id == sessionId)
                    .Include(x => x.Members)
                    .Include(x => x.Feedback)
                    .FirstOrDefaultAsync();

                if (session == null) return null;

                //Only members of the session should be able to access this page!
                if (session.Members.All(x => x.UserId != userId)) return null;

                var model = new CreateFeedbackViewModel { SessionId = session.Id };

                //Calculate session end date and time (+ 30 mins to start time)
                DateTime endDate = session.ScheduledDate.AddMinutes(30);
                endDate = endDate.ToTimeZoneTime(GetUserTimeZone());
                model.SessionEndDate = endDate.ToFullDateString();
                model.SessionEndTime = endDate.ToShortTimeString();

                //Check if feedback can be submitted
                DateTime now = DateTime.UtcNow.ToTimeZoneTime(GetUserTimeZone());

                //We are within 1 week of session complete
                if (now > endDate && now < endDate.AddDays(7))
                {
                    model.CanSubmitFeedback = true;
                }

                //Add each member as a feedback user
                foreach (var m in session.Members)
                {
                    //Ignore ourselves 
                    if (m.UserId == userId) continue;

                    var feedback = new UserFeedbackViewModel();

                    //Existing feedback?
                    var existing = session.Feedback.FirstOrDefault(x => x.UserId == m.UserId);

                    if (existing != null)
                    {
                        feedback.UserId = existing.UserId;
                        feedback.User = m.DisplayName;
                        feedback.UserThumbnail = m.ThumbnailUrl;
                        feedback.Comments = existing.Comments;
                        feedback.Rating = existing.Rating;
                    }
                    //Add new
                    else
                    {
                        feedback.UserId = m.UserId;
                        feedback.UserThumbnail = m.ThumbnailUrl;
                        feedback.User = m.DisplayName;
                    }

                    model.UsersFeeback.Add(feedback);
                }

                foreach (var u in model.UsersFeeback)
                {
                    //Get the 48x48 images instead.
                    u.UserThumbnail = GetImageUrl(u.UserThumbnail, "48x48");
                }

                return model;
            }
            catch (Exception ex)
            {
                LogError(ex, "Failed to get submit feedback view model for session: " + sessionId);
                throw;
            }
        }

        public async Task<ValidationResult> SubmitFeedback(CreateFeedbackViewModel model, string userId)
        {
            try
            {
                UserId = userId;

                //Load existing feedback if any
                List<SessionFeedback> feedbacks = await _feedbackRepo.Get(x => x.SessionId == model.SessionId)
                    .Where(x => x.OwnerId == userId)
                    .ToListAsync();

                bool isNewFeedback = false;

                foreach (var f in model.UsersFeeback)
                {
                    var existing = feedbacks.FirstOrDefault(x => x.UserId == f.UserId);

                    //Update existing?
                    if (existing != null)
                    {
                        existing.Comments = f.Comments;
                        existing.Rating = f.Rating;

                        _feedbackRepo.Update(existing);
                    }
                    //Add new
                    else
                    {
                        var newFeedback = new SessionFeedback
                        {
                            Comments = f.Comments,
                            OwnerId = userId,
                            Rating = f.Rating,
                            SessionId = model.SessionId,
                            UserId = f.UserId
                        };

                        _feedbackRepo.Insert(newFeedback);

                        isNewFeedback = true;
                    }
                }

                await SaveChangesAsync();

                //Reward user with kudos if new feedback
                if (isNewFeedback)
                {
                    KudosLogic kudos = new KudosLogic();
                    int kudosPoints = 5*model.UsersFeeback.Count;
                    await kudos.AddKudosPoints(userId, kudosPoints);
                }

                return VResult;
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error submitting feedback for session: {model.SessionId} for user : {userId} ");
                return VResult.AddError("Unable to update your feedback at this time. Please try again later.");
            }
        }

        public async Task<SessionFeedbackViewModel> GetSessionFeedback(Guid sessionId, string userId)
        {
            try
            {
                UserId = userId;

                var model = new SessionFeedbackViewModel { SessionId = sessionId };

                var scheduledDate = await UoW.Repository<Session>().Get(x => x.Id == sessionId)
                    .Select(x => x.ScheduledDate).FirstAsync();

                //Calculate session end date (+ 30 mins to start time)
                DateTime endDate = scheduledDate.AddMinutes(30);
                endDate = endDate.ToTimeZoneTime(GetUserTimeZone());

                //Check if feedback can be submitted
                DateTime now = DateTime.UtcNow.ToTimeZoneTime(GetUserTimeZone());

                //Has the session been completed?
                if (now < endDate)
                {
                    //If not return
                    model.CanSubmit = false;
                    return model;
                }

                //This means the session is complete
                model.SessionCompleted = true;

                //Are we in the time frame to submit feedback?
                if (now > endDate && now < endDate.AddDays(7))
                {
                    model.CanSubmit = true;
                }
                
                //Load session feedback from database (exclude this users feedback)
                //Group by the feedback owner, so we display all the feedback each user
                //has submitted.
                model.Providers = await _feedbackRepo.Get(x => x.SessionId == sessionId)
                    .GroupBy(g => g.Owner)
                    .Select(x => new FeedbackProviderViewModel
                    {
                        Provider = x.Key.DisplayName,
                        Feedback = x.Select(f => new UserFeedbackViewModel
                        {
                            User = f.User.DisplayName,
                            UserThumbnail = f.User.ThumbnailUrl,
                            Kudos = f.User.Kudos.Points.ToString(),
                            Comments = f.Comments,
                            Rating = f.Rating,
                            Submitted = f.CreatedDate
                        }).ToList()
                    })
                    .OrderBy(x => x.Provider)
                    .ToListAsync();

                //Has this user already submitted?
                if (string.IsNullOrEmpty(userId))
                {
                    model.CanSubmit = false;
                }


                //Convert the times to the users time zone
                //and truncate kudos value
                //and get the 48x48 thumbnail image
                foreach (var p in model.Providers)
                {
                    foreach (var f in p.Feedback)
                    {
                        f.Submitted = f.Submitted.ToTimeZoneTime(GetUserTimeZone());
                        f.UserThumbnail = GetImageUrl(f.UserThumbnail, "48x48");

                        if (f.Kudos.Length > 3)
                        {
                            int i = int.Parse(f.Kudos);
                            f.Kudos = ((double) i/1000).ToString("0.#k");
                        }
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to get session feedback for session : " + sessionId);
                return null;
            }
        }
    }
}
