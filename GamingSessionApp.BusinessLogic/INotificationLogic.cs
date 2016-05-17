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
        Task AddUserLeftNotification(Session session, string refereeId);
        Task AddUserKickedNotification(Session session, string userId);

        void AddCommentNotification(ICollection<UserProfile> members, Guid sessionId, SessionComment comment);

        Task UpdateNotifications(string userId, List<Guid> ids);
    }
}