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

        public async Task<ViewMessageViewModel> GetMessage(Guid id, string userId)
        {
            try
            {
                UserId = userId;

                //Load the message
                UserMessage model = await _messageRepo.Get(x => x.Id == id)
                    .Where(x => x.RecipientId == userId)
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
                    SenderImageUrl = model.Sender.ThumbnailUrl,
                    SenderName = model.Sender.DisplayName,
                    SentDate = model.CreatedDate,
                    Subject = model.Subject,
                    Body = model.Body,
                    Read = model.Read
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

        public Task<ValidationResult> CreateMessage(CreateMessageViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
