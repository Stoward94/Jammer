using System;
using System.Threading.Tasks;
using GamingSessionApp.ViewModels.Feedback;
using System.Collections.Generic;

namespace GamingSessionApp.BusinessLogic
{
    public interface IFeedbackLogic : IDisposable
    {
        Task<CreateFeedbackViewModel> SubmitFeedbackViewModel(Guid sessionId, string userId);
        Task<ValidationResult> SubmitFeedback(CreateFeedbackViewModel model, string userId);
        Task<SessionFeedbackViewModel> GetSessionFeedback(Guid sessionId, string userId);
        Task<List<ReceivedFeedbackViewModel>> GetReceivedFeedback(string userId);
    }
}
