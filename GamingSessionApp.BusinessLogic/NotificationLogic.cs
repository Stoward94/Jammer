using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
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

        public async Task AddUserJoinedNotification(Session session, ApplicationUser user)
        {
            try
            {
                UserNotification notification = new UserNotification
                {
                    SessionId = session.Id,
                    RecipientId = session.CreatorId,
                    TypeId = (int) UserNotificationTypeEnum.PlayerJoined,
                    Body = $"{user.UserName} has joined the session: {session.Type.Name}"
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

        //Adds notification, but first checks against users preferences. 
        private void AddNotification(UserPreferences prefs, UserNotification notification)
        {
            if (prefs == null)
                throw new Exception("User preferences must be included");

            //If the user doesn't want notifications, just return
            if (prefs.ReceiveNotifications == false) return;

            //Else add the notification
            _notificationRepo.Insert(notification);
        } 

        public async Task<List<UserNotification>> GetNotifications(string userId)
        {
            try
            {
                UserId = userId;

                List<UserNotification> model = await _notificationRepo.Get(x => x.RecipientId == userId)
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(10)
                    .ToListAsync();

                //Convert times to local timezone
                foreach (var m in model)
                {
                    m.CreatedDate = m.CreatedDate.ToTimeZoneTime(GetUserTimeZone());
                }

                return model;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to fetch users notification: UserId = " + userId);
                return null;
            }
        }

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

    }
}
