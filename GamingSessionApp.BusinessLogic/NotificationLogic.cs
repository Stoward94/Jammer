using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Notifications;
using static GamingSessionApp.BusinessLogic.SystemEnums;

namespace GamingSessionApp.BusinessLogic
{
    public class NotificationLogic : BaseLogic
    {
        private readonly GenericRepository<UserNotification> _notificationRepo;

        public NotificationLogic()
        {
            _notificationRepo = UoW.Repository<UserNotification>();
        }

        #region Get Methods

        public async Task<List<UserNotificationViewModel>> GetNotifications(string userId)
        {
            try
            {
                UserId = userId;

                List<UserNotification> notifs = await _notificationRepo.Get(x => x.RecipientId == userId)
                    .OrderByDescending(x => x.CreatedDate)
                    .Include(x => x.Referee)
                    .Include(x => x.Session)
                    .Take(10)
                    .ToListAsync();

                var model = BuildNotifications(notifs);

                return model;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to fetch users notification: UserId = " + userId);
                return null;
            }
        }

        /// <summary>
        /// Get all notifications for a user (paged)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<AllNotificationsViewModel> GetAllForUser(string userId, int page)
        {
            try
            {
                UserId = userId;

                int pageSize = 10;
                int skip = (page - 1)*pageSize;

                List<UserNotification> notifs = await _notificationRepo.Get(x => x.RecipientId == userId)
                    .OrderByDescending(x => x.CreatedDate)
                    .Include(x => x.Referee)
                    .Include(x => x.Session)
                    .Skip(skip)//Skip for pagination
                    .Take(pageSize)
                    .ToListAsync();

                var notifications = BuildNotifications(notifs);

                var model = new AllNotificationsViewModel
                {
                    TotalCount = await _notificationRepo.Get(x => x.RecipientId == userId).CountAsync(),
                    PageNo = page,
                    PageSize = pageSize,
                    Notifications = notifications
                };

                //Build the 'showing x - y of z' display text 
                int startCount = skip + 1;
                int endCount = skip + notifs.Count;
                model.ShowingText = $"Showing {startCount} - {endCount} of {model.TotalCount}";


                //Mark Unread notifications as read
                bool updated = false;
                foreach (var n in notifs)
                {
                    if (n.Read == false)
                    {
                        n.Read = true;
                        _notificationRepo.Update(n);
                        updated = true;
                    }
                }

                //Only save if needed;
                if (updated) await SaveChangesAsync();

                return model;
            }
            catch (Exception ex)
            {
                LogError(ex, "Error when fetching all notifications for user :" + userId);
                throw;
            }
        }

        /// <summary>
        /// Dynamically constructs the notification(s) body and creates the view model objects
        /// </summary>
        /// <param name="notifs"></param>
        /// <returns></returns>
        private List<UserNotificationViewModel> BuildNotifications(List<UserNotification> notifs)
        {
            try
            {
                var model = new List<UserNotificationViewModel>();

                DateTime now = DateTime.UtcNow.ToTimeZoneTime(GetUserTimeZone());

                foreach (var n in notifs)
                {
                    //Convert times to local timezone
                    n.CreatedDate = n.CreatedDate.ToTimeZoneTime(GetUserTimeZone());

                    //Create the notif view model object
                    var notif = new UserNotificationViewModel
                    {
                        DisplayDate = n.CreatedDate.ToMinsAgoTime(now),
                        Id = n.Id,
                        Read = n.Read,
                        CommentId = n.CommentId,
                        SessionId = n.SessionId,
                        TypeId = n.TypeId,
                        SenderThumbnailUrl = n.Referee?.ThumbnailUrl
                    };

                    //Build the notif html body
                    switch (n.TypeId)
                    {
                        case (int)UserNotificationTypeEnum.Comment:
                            notif.Body = $"<b>{n.Referee.DisplayName}</b> has commented on the session: UPDATE ME!";
                            break;
                        case (int)UserNotificationTypeEnum.Information:
                            notif.Body = n.Body;
                            break;
                        case (int)UserNotificationTypeEnum.Invitation:
                            notif.Body = $"<b>{n.Referee.DisplayName}</b> has invited you to join their new session";
                            break;
                        case (int)UserNotificationTypeEnum.KudosAdded:
                            notif.Body = n.Body;
                            break;
                        case (int)UserNotificationTypeEnum.PlayerJoined:
                            notif.Body = $"<b>{n.Referee.DisplayName}</b> has joined your session: UPDATE ME!";
                            break;
                        case (int)UserNotificationTypeEnum.PlayerLeft:
                            notif.Body = $"<b>{n.Referee.DisplayName}</b> has left your session: UPDATE ME!";
                            break;
                    }

                    //add view model object to list
                    model.Add(notif);
                }

                return model;
            }
            catch (Exception)
            {

                throw;
            }
        }


        #endregion

        #region Create Notifications

        public async Task AddUserJoinedNotification(Session session, string refereeId)
        {
            try
            {
                UserNotification notification = new UserNotification
                {
                    SessionId = session.Id,
                    RecipientId = session.CreatorId,
                    TypeId = (int)UserNotificationTypeEnum.PlayerJoined,
                    RefereeId = refereeId,
                };

                //Load the preferences to check whether to add the notification
                UserPreferences preferences = await UoW.Repository<UserPreferences>()
                    .Get(x => x.ProfileId == session.CreatorId)
                    .FirstOrDefaultAsync();

                if (preferences == null) return;

                //Attach nofitication
                AddNotification(preferences, notification);

                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        internal async Task SessionInviteNotification(Session session, string creatorId, List<UserProfile> recipients)
        {
            try
            {
                foreach (var r in recipients)
                {
                    UserNotification notification = new UserNotification
                    {
                        SessionId = session.Id,
                        RecipientId = r.UserId,
                        TypeId = (int)UserNotificationTypeEnum.Invitation,
                        RefereeId = creatorId
                    };

                    //Attach nofitication
                    AddNotification(r.Preferences, notification);
                }

                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogError(ex, "Error creating session invite notification for session: " + session.Id);
            }
        }

        public async void AddCommentNotification(ICollection<UserProfile> members, string authorId, Guid sessionId, int commentId)
        {
            try
            {
                var commentAuthor = members.First(x => x.UserId == authorId);

                //Can't add notification, something is wrong
                if (commentAuthor == null) return;

                foreach (var m in members)
                {
                    //Ignore author
                    if (m.UserId == commentAuthor.UserId) continue;

                    //Create notification
                    UserNotification n = new UserNotification
                    {
                        Body = commentAuthor.DisplayName + " has commented on a session",
                        SessionId = sessionId,
                        CommentId = commentId,
                        RecipientId = m.UserId,
                        TypeId = (int)UserNotificationTypeEnum.Comment,
                        RefereeId = authorId
                    };

                    _notificationRepo.Insert(n);
                }

                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogError(ex, "Error creating comment notification for session: " + sessionId);
            }
        }

        //Adds notification, but first checks against users preferences. 
        private void AddNotification(UserPreferences prefs, UserNotification notification)
        {
            if (prefs == null)
                throw new Exception("User preferences cannot be null");

            //If the user doesn't want notifications, just return
            if (prefs.ReceiveNotifications == false) return;

            //Else add the notification
            _notificationRepo.Insert(notification);
        }

        #endregion

        #region Update Notifications
        
        public async Task UpdateNotifications(string userId, List<Guid> ids)
        {
            try
            {
                GenericRepository<UserNotification> notifRepo = UoW.Repository<UserNotification>();

                var nofitications = await notifRepo.Get(x => ids.Contains(x.Id))
                    .ToListAsync();

                foreach (var n in nofitications)
                {
                    n.Read = true;
                    notifRepo.Update(n);
                }

                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogError(ex, "Error updating nofitication");
            }
        }

        #endregion
    }
}
