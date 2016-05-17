using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Session;
using GamingSessionApp.ViewModels.SessionComments;
using static GamingSessionApp.BusinessLogic.SystemEnums;

namespace GamingSessionApp.BusinessLogic
{
    public class SessionCommentLogic : BaseLogic, ISessionCommentLogic
    {
        //Session Repository
        private readonly GenericRepository<SessionComment> _commentRepo;

        public SessionCommentLogic(UnitOfWork uow)
        {
            UoW = uow;
            _commentRepo = UoW.Repository<SessionComment>();
        }

        public async Task<List<SessionComment>> GetSessionComments(Guid sessionId)
        {
            var messages = await _commentRepo.Get(x => x.SessionId == sessionId)
                .OrderBy(x => x.CreatedDate)
                .ToListAsync();

            return messages;
        }

        public void AddSessionCreatedComment(Session session)
        {
            SessionComment comment = new SessionComment
            {
                AuthorId = session.CreatorId,
                CommentTypeId = (int) SessionMessageTypeEnum.System,
                Body = "New session created!"
            };

            session.Comments.Add(comment);
        }

        public async Task<CommentViewModel> AddSessionComment(PostCommentViewModel model, string userId)
        {
            try
            {
                //Create the comment
                SessionComment comment = new SessionComment()
                {
                    AuthorId = userId,
                    Body = model.Comment,
                    CommentTypeId = (int)SessionMessageTypeEnum.Comment,
                    SessionId = model.SessionId
                };

                _commentRepo.Insert(comment);
                //await SaveChangesAsync();

                //Notify members
                var members = await UoW.Repository<Session>().Get(x => x.Id == model.SessionId)
                    .Select(x => x.Members).FirstAsync();

                var nLogic = new NotificationLogic(UoW);
                nLogic.AddCommentNotification(members, model.SessionId, comment);

                await SaveChangesAsync();

                //Return commentId to load the new comment viewModel
                return await LoadComment(comment.Id, userId);
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to add a comment to sessionId : " + model.SessionId);
                throw;
            }
        }

        public SessionComment AddUserJoinedComment(Guid sessionId, string username, string userId)
        {
            try
            {
                SessionComment comment = new SessionComment()
                {
                    AuthorId = userId,
                    Body = $"{username} has joined the session",
                    CommentTypeId = (int)SessionMessageTypeEnum.PlayerJoined,
                    SessionId = sessionId
                };

                return comment;
            }
            catch (Exception)
            {
                //Let the caller catch the exception
                return null;
            }
        }

        public SessionComment AddUserLeftComment(Guid sessionId, string username, string userId)
        {
            try
            {
                SessionComment comment = new SessionComment()
                {
                    AuthorId = userId,
                    Body = $"{username} has left the session",
                    CommentTypeId = (int)SessionMessageTypeEnum.PlayerLeft,
                    SessionId = sessionId
                };

                return comment;
            }
            catch (Exception ex)
            {
                //Let the caller catch the exception
                LogError(ex, "Error adding user left comment. SessionId : " + sessionId + " UserId : " + userId);
                return null;
            }
        }

        public async Task<CommentViewModel> LoadComment(int commentId, string userId)
        {
            try
            {
                UserId = userId;

                CommentViewModel comment = await _commentRepo.Get(x => x.Id == commentId)
                    .Select(x => new CommentViewModel
                    {
                        Author = x.Author.DisplayName,
                        Body = x.Body,
                        CreatedDate = x.CreatedDate,
                        Kudos = x.Author.Kudos.Points.ToString(),
                        ThumbnailUrl = x.Author.ThumbnailUrl
                    })
                    .FirstOrDefaultAsync();

                if (comment == null)
                    throw new NullReferenceException("comment");

                //Convert comments times to time zone
                //Get the 48x48 thumbnail
                //Finally shorthand the kudos value
                DateTime now = DateTime.UtcNow.ToTimeZoneTime(GetUserTimeZone());
                comment.CreatedDate = comment.CreatedDate.ToTimeZoneTime(GetUserTimeZone());
                comment.CreatedDisplayDate = comment.CreatedDate.ToMinsAgoTime(now);
                comment.ThumbnailUrl = GetImageUrl(comment.ThumbnailUrl, "48x48");
                comment.Kudos = TrimKudos(comment.Kudos);

                return comment;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to load comment : " + commentId);
                throw;
            }
        }

        public SessionComment AddUserKickedComment(Guid sessionId, string username, string hostId)
        {
            try
            {
                SessionComment comment = new SessionComment()
                {
                    AuthorId = hostId,
                    Body = $"{username} has been kicked from the session",
                    CommentTypeId = (int)SessionMessageTypeEnum.PlayerKicked,
                    SessionId = sessionId
                };

                return comment;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable add user kicked comment for session: " + sessionId + " and username: " + username);
                return null;
            }
        }
    }
}
