using System;
using System.Data.Entity;
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

    }
}
