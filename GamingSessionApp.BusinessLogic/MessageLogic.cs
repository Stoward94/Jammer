using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Inbox;
using GamingSessionApp.ViewModels.Shared;

namespace GamingSessionApp.BusinessLogic
{
    public class MessageLogic : BaseLogic, IMessageLogic
    {
        private readonly GenericRepository<UserMessage> _messageRepo;

        public MessageLogic(UnitOfWork uow)
        {
            UoW = uow;
            _messageRepo = UoW.Repository<UserMessage>();
        }

        public async Task<UserMessagesViewModel> GetUsersMessages(string userId, int page)
        {
            try
            {
                //Update base logic reference
                UserId = userId;

                int pageSize = 10;
                int skip = (page - 1) * pageSize;

                UserMessagesViewModel userMessages = await _messageRepo.Get(x => x.RecipientId == userId)
                    .GroupBy(x => x.RecipientId)
                    .Select(x => new UserMessagesViewModel
                    {
                        Pagination = new Pagination
                        {
                            TotalCount = x.Count(),
                            PageNo = page,
                            PageSize = pageSize
                        },

                        Messages = x.Select(m => new MessageViewModel
                        {
                            Id = m.Id,
                            Read = m.Read,
                            SenderImageUrl = m.Sender.ThumbnailUrl,
                            SenderName = m.Sender.DisplayName,
                            SentDate = m.CreatedDate,
                            Subject = m.Subject
                        })
                        .OrderByDescending(m => m.SentDate)
                        .Skip(skip)
                        .Take(pageSize)
                        .ToList()
                    })
                    .FirstOrDefaultAsync();

                //If no messages create default
                if (userMessages == null)
                    userMessages = new UserMessagesViewModel();

                DateTime now = DateTime.UtcNow.ToTimeZoneTime(GetUserTimeZone());

                //Convert the times to the users time zone
                foreach (var m in userMessages.Messages)
                {
                    m.SentDate = m.SentDate.ToTimeZoneTime(GetUserTimeZone());
                    m.SentDisplayDate = m.SentDate.ToMinsAgoTime(now);
                }

                return userMessages;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to get messages for user: " + userId);
                throw;
            }
        }

        public async Task<OutboxMessageViewModel> GetUserOutbox(string userId, int page)
        {
            try
            {
                UserId = userId;

                int pageSize = 10;
                int skip = (page - 1) * pageSize;

                OutboxMessageViewModel sentMessages = await _messageRepo.Get(x => x.SenderId == userId)
                    .GroupBy(x => x.SenderId)
                    .Select(x => new OutboxMessageViewModel
                    {
                        Pagination = new Pagination
                        {
                            TotalCount = x.Count(),
                            PageNo = page,
                            PageSize = pageSize
                        },

                        Messages = x.Select(m => new SentMessageViewModel
                        {
                            Id = m.Id,
                            RecipientImageUrl = m.Recipient.ThumbnailUrl,
                            RecipientName = m.Recipient.DisplayName,
                            SentDate = m.CreatedDate,
                            Subject = m.Subject
                        })
                        .OrderByDescending(m => m.SentDate)
                        .Skip(skip)
                        .Take(pageSize)
                        .ToList()
                    })
                    .FirstOrDefaultAsync();

                DateTime now = DateTime.UtcNow.ToTimeZoneTime(GetUserTimeZone());
                
                //Convert the times to the users time zone
                foreach (var m in sentMessages.Messages)
                {
                    m.SentDate = m.SentDate.ToTimeZoneTime(GetUserTimeZone());
                    m.SentDisplayDate = m.SentDate.ToMinsAgoTime(now);
                }

                return sentMessages;

            }
            catch (Exception ex)
            {
                LogError(ex, "Error getting outbox for user : " + userId);
                return null;
            }
        }

        public async Task<ViewMessageViewModel> ViewMessage(Guid id, string userId)
        {
            try
            {
                UserId = userId;

                //Load the message
                ViewMessageViewModel message = await _messageRepo.Get(x => x.Id == id)
                    .Where(x => x.RecipientId == userId)
                    .Select(x => new ViewMessageViewModel
                    {
                        ImageUrl = x.Sender.ThumbnailUrl,
                        SenderName = x.Sender.DisplayName,
                        Kudos = x.Sender.Kudos.Points.ToString(),
                        SentDate = x.CreatedDate,
                        Subject = x.Subject,
                        Body = x.Body,
                        Read = x.Read

                    })
                    .FirstOrDefaultAsync();

                if (message == null) return null;

                //Now we need to update and save the message as being read
                //If the message hasn't been read
                if (message.Read == false)
                {
                    var model = await _messageRepo.Get(x => x.Id == id)
                        .FirstAsync();

                    model.Read = true;

                    _messageRepo.Update(model);
                    await SaveChangesAsync();
                }

                DateTime now = DateTime.UtcNow.ToTimeZoneTime(GetUserTimeZone());

                //Get the large thumbnail image
                message.ImageUrl = GetImageUrl(message.ImageUrl, "180x180");

                //Convert date to local timezone
                message.SentDate = message.SentDate.ToTimeZoneTime(GetUserTimeZone());
                message.SentDisplayDate = message.SentDate.ToMinsAgoTime(now);
                message.Kudos = TrimKudos(message.Kudos);

                message.CanReply = true;

                return message;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to get user message: " + id);
                throw;
            }
        }

        public async Task<ViewMessageViewModel> ViewSentMessage(Guid id, string userId)
        {
            try
            {
                UserId = userId;

                //Load the message
                ViewMessageViewModel message = await _messageRepo.Get(x => x.Id == id)
                    .Where(x => x.SenderId == userId)
                    .Select(x => new ViewMessageViewModel
                    {
                        ImageUrl = x.Recipient.ThumbnailUrl,
                        SenderName = x.Recipient.DisplayName,
                        Kudos = x.Recipient.Kudos.Points.ToString(),
                        SentDate = x.CreatedDate,
                        Subject = x.Subject,
                        Body = x.Body,
                        Read = x.Read

                    })
                    .FirstOrDefaultAsync();

                if (message == null) return null;

                //Convert date to local timezone
                DateTime now = DateTime.UtcNow.ToTimeZoneTime(GetUserTimeZone());

                //Get the large thumbnail image
                message.ImageUrl = GetImageUrl(message.ImageUrl, "180x180");

                //Convert date to local timezone
                message.SentDate = message.SentDate.ToTimeZoneTime(GetUserTimeZone());
                message.SentDisplayDate = message.SentDate.ToMinsAgoTime(now);
                message.Kudos = TrimKudos(message.Kudos);

                //We can't reply to a message we've sent
                message.CanReply = false;

                return message;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to get user message: " + id);
                throw;
            }
        }

        public async Task<ValidationResult> DeleteMessage(Guid id, string userId)
        {
            try
            {
                //Load the message
                UserMessage message = await _messageRepo.GetByIdAsync(id);

                if (message.RecipientId != userId)
                    return VResult.AddError("You are unauthorised to delete this message");

                _messageRepo.Delete(message);
                await SaveChangesAsync();

                return VResult;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to delete message :" + id);
                return VResult.AddError("Unable to delete the message at this time. " +
                                        "Please try again later");
            }
        }

        public async Task<ValidationResult> CreateMessage(CreateMessageViewModel model, string userId)
        {
            try
            {
                UserId = userId;

                //First lets try and parse out our users list
                List<string> usernames = model.Recipients.Split(',').ToList();

                if (usernames.Count < 1)
                    return VResult.AddError("Unable to work out which users you have selected. Please try again later.");

                //Load the recipient users
                GenericRepository<UserProfile> profileRepo = UoW.Repository<UserProfile>();

                List<UserProfile> recipients = await profileRepo.Get(x => usernames.Contains(x.DisplayName))
                    .Include(x => x.User)
                    .Include(x => x.Preferences)
                    .ToListAsync();

                if (recipients == null || recipients.Count < 1)
                    return VResult.AddError("No user(s) where found to send your message. Please try again.");

                UserMessage message = new UserMessage();

                foreach (var r in recipients)
                {
                    //Now create the message
                    message = new UserMessage
                    {
                        Subject = model.Subject,
                        Body = model.Body,
                        SenderId = userId,
                        RecipientId = r.UserId
                    };

                    _messageRepo.Insert(message);
                }

                //Save changes
                await SaveChangesAsync();

                //Send out email!
                EmailLogic email = new EmailLogic();
                await email.NewPrivateMessageEmail(recipients, CurrentUser.UserName, message);

                return VResult;
            }
            catch (Exception ex)
            {
                LogError(ex, "Error creating message for userId : " + userId);
                return VResult.AddError("Unable to send the message at this time. Please try again later.");
            }
        }

        public async Task<ValidationResult> MarkRead(List<Guid> ids, string userId)
        {
            try
            {
                //Load the messages
                List<UserMessage> messages = await _messageRepo.Get(x => ids.Contains(x.Id))
                    .ToListAsync();

                if (messages == null || messages.Count < 1)
                    return VResult.AddError("Error finding the messages your trying to update. Please try again later");

                foreach (var m in messages)
                {
                    m.Read = true;
                    _messageRepo.Update(m);
                }

                await SaveChangesAsync();
                return VResult;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to mark message as read for user : " + userId);
                return VResult.AddError("Unable to update the message(s) at this time. Please try again later.");
            }
        }
    }
}
