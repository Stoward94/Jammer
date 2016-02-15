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
                        SenderImageUrl = x.Sender.ThumbnailUrl,
                        SenderName = x.Sender.DisplayName,
                        Subject = x.Subject,
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

        public async Task<List<OutboxMessageViewModel>> GetUserOutbox(string userId)
        {
            try
            {
                UserId = userId;

                List<OutboxMessageViewModel> messages = await _messageRepo.Get(x => x.SenderId == userId)
                    .Select(x => new OutboxMessageViewModel
                    {
                        Id = x.Id,
                        RecipientImageUrl = x.Recipient.ThumbnailUrl,
                        RecipientName = x.Recipient.DisplayName,
                        SentDate = x.CreatedDate,
                        Subject = x.Subject
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
                LogError(ex, "Error getting outbox for user : " + userId);
                return null;
            }
        }

        public async Task<ViewMessageViewModel> GetMessage(Guid id, string userId)
        {
            try
            {
                UserId = userId;

                //Load the message
                UserMessage model = await _messageRepo.Get(x => x.Id == id)
                    .Where(x => x.RecipientId == userId || x.SenderId == userId)
                    .Include(x => x.Sender)
                    .FirstOrDefaultAsync();

                if (model == null) return null;

                //Now we need to update and save the message as being read
                //If the message hasn't been read
                if (model.Read == false)
                {
                    model.Read = true;

                    _messageRepo.Update(model);
                    await SaveChangesAsync();
                }

                //Now map to view model
                ViewMessageViewModel viewModel = new ViewMessageViewModel
                {
                    ImageUrl = model.Sender.ThumbnailUrl,
                    SenderName = model.Sender.DisplayName,
                    SentDate = model.CreatedDate,
                    Subject = model.Subject,
                    Body = model.Body
                };

                //Convert date to local timezone
                viewModel.SentDate = viewModel.SentDate.ToTimeZoneTime(GetUserTimeZone());

                return viewModel;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to get user message: " + id);
                throw;
            }
        }

        public async Task<ViewMessageViewModel> GetSentMessage(Guid id, string userId)
        {
            try
            {
                UserId = userId;

                //Load the message
                ViewMessageViewModel model = await _messageRepo.Get(x => x.Id == id)
                    .Where(x => x.RecipientId == userId || x.SenderId == userId)
                    .Select(x => new ViewMessageViewModel
                    {
                        ImageUrl = x.Recipient.ThumbnailUrl,
                        SenderName = x.Recipient.DisplayName,
                        SentDate = x.CreatedDate,
                        Subject = x.Subject,
                        Body = x.Body
                    })
                    .FirstOrDefaultAsync();

                if (model == null) return null;

                //Convert date to local timezone
                model.SentDate = model.SentDate.ToTimeZoneTime(GetUserTimeZone());

                return model;
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

                    profileRepo.Update(r);
                }

                //Save changes
                await SaveChangesAsync();

                //Send out email!
                await EmailLogic.NewPrivateMessageEmail(recipients, CurrentUser.UserName, message);

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
