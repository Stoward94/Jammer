using GamingSessionApp.ViewModels.Session;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GamingSessionApp.BusinessLogic
{
    public interface ISessionLogic : IDisposable
    {
        Task<AllSessionsViewModel> GetSessions(string userId, SessionsFilter filter = null);
        Task<ValidationResult> CreateSession(CreateSessionVM model, string userId);
        Task<ValidationResult> EditSession(EditSessionVM viewModel, string userId);

        Task<CreateSessionVM> CreateSessionViewModel(CreateSessionVM viewModel, string userId);
        Task<EditSessionVM> PrepareEditSessionVM(EditSessionVM viewModel);
        Task<EditSessionVM> EditSessionVM(Guid sessionId, string userId);
        
        Task<SelectList> GetPlatformsList();
        Task<SelectList> GetTypesList();
   
        Task<ValidationResult> AddUserToSession(string userId, Guid sessionId);
        Task<ValidationResult> RemoveUserFromSession(string userId, Guid sessionId);
    }
}