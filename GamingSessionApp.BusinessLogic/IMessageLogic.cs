using GamingSessionApp.ViewModels.Inbox;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamingSessionApp.BusinessLogic
{
    public interface IMessageLogic : IDisposable
    {
        Task<UserMessagesViewModel> GetUsersMessages(string userId, int page);
        Task<OutboxMessageViewModel> GetUserOutbox(string userId, int page);
        Task<ViewMessageViewModel> ViewMessage(Guid id, string userId);
        Task<ViewMessageViewModel> ViewSentMessage(Guid id, string userId);
        Task<ValidationResult> DeleteMessage(Guid id, string userId);
        Task<ValidationResult> CreateMessage(CreateMessageViewModel model, string userId);
        Task<ValidationResult> MarkRead(List<Guid> ids, string userId);
    }
}