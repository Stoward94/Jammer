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

                //Load the user to attach the notification
                UserProfile creator = await UoW.Repository<UserProfile>().Get(x => x.UserId == session.CreatorId)
                    .Include(x => x.Notifications)
                    .FirstOrDefaultAsync();

                if (creator == null) return;

                //Attach nofitication
                creator.Notifications.Add(notification);

                //Update db
                UoW.Repository<UserProfile>().Update(creator);
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
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
