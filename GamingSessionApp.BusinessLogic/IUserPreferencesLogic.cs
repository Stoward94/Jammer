using GamingSessionApp.ViewModels.Preferences;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GamingSessionApp.BusinessLogic
{
    public interface IUserPreferencesLogic : IDisposable
    {
        Task<EditUserPreferencesViewModel> GetEditPreferencesModel(string userId);
        Task<ValidationResult> EditPreferences(EditUserPreferencesViewModel model, string userId);
        Task<SelectList> GetReminderTimes();
    }
}