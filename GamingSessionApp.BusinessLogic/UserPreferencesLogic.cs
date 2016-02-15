using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Preferences;

namespace GamingSessionApp.BusinessLogic
{
    public class UserPreferencesLogic : BaseLogic
    {
        private readonly GenericRepository<UserPreferences> _preferencesRepo;

        public UserPreferencesLogic()
        {
            _preferencesRepo = UoW.Repository<UserPreferences>();
        }

        public async Task<EditUserPreferencesViewModel> GetEditPreferencesModel(string userId)
        {
            try
            {
                EditUserPreferencesViewModel model = await _preferencesRepo.Get(x => x.ProfileId == userId)
                    .Select(x => new EditUserPreferencesViewModel
                    {
                        ReceiveEmail = x.ReceiveEmail,
                        ReceiveNotifications = x.ReceiveNotifications,
                        ReminderTimeId = x.ReminderTimeId
                    })
                    .FirstOrDefaultAsync();

                model.ReminderTimes = await GetReminderTimes();

                return model;
            }
            catch (Exception ex)
            {
                LogError(ex, "Error getting user preferences for user : " + userId);
                return null;
            }
        }

        public async Task<ValidationResult> EditPreferences(EditUserPreferencesViewModel model, string userId)
        {
            try
            {
                UserPreferences pref = await _preferencesRepo.Get(x => x.ProfileId == userId)
                    .FirstOrDefaultAsync();

                if (pref == null)
                    return VResult.AddError("Unable to update your preferences. Please try again later.");

                //Update values
                pref.ReceiveEmail = model.ReceiveEmail;
                pref.ReceiveNotifications = model.ReceiveNotifications;
                pref.ReminderTimeId = model.ReminderTimeId;

                //Save changes
                _preferencesRepo.Update(pref);
                await SaveChangesAsync();

                return VResult;
            }
            catch (Exception ex)
            {
                LogError(ex, "Error updating user preferences for user : " + userId);
                return VResult.AddError("An error occured while updating your preferences. Please try again later");
            }
        }

        public async Task<SelectList> GetReminderTimes()
        {
            var repo = UoW.Repository<EmailReminderTime>();

            var times = await repo.Get()
                .Select(x => new
                {
                    Id = x.Id,
                    Value = x.Duration
                })
                .ToListAsync();

            return new SelectList(times, "Id", "Value");
        }
    }
}