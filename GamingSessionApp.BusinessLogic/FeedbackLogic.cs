using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                //Find and load the session
                Session session = await UoW.Repository<Session>().Get(x => x.Id == sessionId)
                    .Include(x => x.Members)
                    .Include(x => x.Feedback)
                    .FirstOrDefaultAsync();

                if (session == null) return null;

                //Only members of the session should be able to access this page!
                if (session.Members.All(x => x.UserId != userId)) return null;

                var model = new CreateFeedbackViewModel { SessionId = session.Id };

                //Check that the session has been completed.
                if (session.StatusId == (int) SessionStatusEnum.Retired)
                {
                    model.CanSubmitFeedback = true;
                }

                //Add each member as a feedback user
                foreach (var m in session.Members)
                {
                    //Ignore ourselves 
                    if (m.UserId == userId) continue;

                    var feedback = new UserFeedback();

                    //Existing feedback?
                    var existing = session.Feedback.FirstOrDefault(x => x.UserId == m.UserId);

                    if (existing != null)
                    {
                        feedback.UserId = existing.UserId;
                        feedback.Username = m.DisplayName;
                        feedback.Comments = existing.Comments;
                        feedback.Rating = existing.Rating;
                    }
                    //Add new
                    else
                    {
                        feedback.UserId = m.UserId;
                        feedback.Username = m.DisplayName;
                    }

                    model.UsersFeeback.Add(feedback);
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
                    }
                }

                await SaveChangesAsync();

                //TODO: Reward user with kudos
                

                return VResult;
            }
            catch (Exception ex)
            {
                LogError(ex, $"Error submitting feedback for session: {model.SessionId} for user : {userId} ");
                return VResult.AddError("Unable to update your feedback at this time. Please try again later.");
            }
        }
    }
}
