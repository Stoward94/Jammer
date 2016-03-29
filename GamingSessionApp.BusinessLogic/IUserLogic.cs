using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Account;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GamingSessionApp.BusinessLogic
{
    public interface IUserLogic : IDisposable
    {
        ApplicationUser GetUser(string username);
        Task<EditAccountViewModel> GetEditAccountModel(string userId);
        SelectList GetTimeZonesList();
        Task<ValidationResult> EditAccount(EditAccountViewModel model, string userId);

    }
}
