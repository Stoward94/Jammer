using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Session;
using GamingSessionApp.ViewModels.SessionComments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamingSessionApp.BusinessLogic
{
    public interface ISessionCommentLogic : IDisposable
    {
        Task<List<SessionComment>> GetSessionComments(Guid sessionId);
        void AddSessionCreatedComment(Session session);
        Task<CommentViewModel> AddSessionComment(PostCommentViewModel model, string userId);
        SessionComment AddUserJoinedComment(Guid sessionId, string username, string userId);
        SessionComment AddUserLeftComment(Guid sessionId, string username, string userId);
        Task<CommentViewModel> LoadComment(int commentId, string userId);
    }
}