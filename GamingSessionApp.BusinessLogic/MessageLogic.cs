using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Inbox;

namespace GamingSessionApp.BusinessLogic
{
    public class MessageLogic : BaseLogic
    {
        private readonly GenericRepository<UserMessage> _messageRepo;

        public MessageLogic()
        {
            _messageRepo = UoW.Repository<UserMessage>();
        }

        public async Task<List<UserMessagesViewModel>> GetUsersMessages(string userId)
        {
            try
            {
                //Update base logic reference
                UserId = userId;

                List<UserMessagesViewModel> messages = await _messageRepo.Get(x => x.RecipientId == userId)
                    .Select(x => new UserMessagesViewModel
                    {
                        Id = x.Id,
                        SenderId = x.SenderId,
                        SenderName = x.Sender.DisplayName,
                        Subject = x.Subject,
                        Body = x.Body,
                        SentDate = x.CreatedDate,
                        Read = x.Read
                    })
                    .OrderByDescending(x => x.SentDate)
                    .ToListAsync();

                //Convert the times to the users time zone
                foreach (var m in messages)
                {
                    m.SentDate = m.SentDate.ToTimeZoneTime(GetUserTimeZone());
                }

                return messages;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to get messages for user: " + userId);
                throw;
            }
        }
    }
}
