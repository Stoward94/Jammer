using GamingSessionApp.ViewModels.Home;
using System;
using System.Threading.Tasks;

namespace GamingSessionApp.BusinessLogic
{
    public interface IHomeLogic : IDisposable
    {
        Task<HomeViewModel> GetHomeViewModel(string userId);
    }
}