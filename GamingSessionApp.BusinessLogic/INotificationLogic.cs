using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamingSessionApp.BusinessLogic
{
    public interface INotificationLogic : IDisposable
    {
        Task<List<UserNotificationViewModel>> GetNotifications(string userId);
        Task<AllNotificationsViewModel> GetAllForUser(string userId, int page);

        Task AddUserJoinedNotification(Session session, string refereeId);
        void AddCommentNotification(ICollection<UserProfile> members, string authorId, Guid sessionId, int commentId);

        Task UpdateNotifications(string userId, List<Guid> ids);
    }
}