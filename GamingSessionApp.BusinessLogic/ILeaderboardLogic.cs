using System;
using System.Threading.Tasks;
using GamingSessionApp.ViewModels.Leaderboard;
using GamingSessionApp.ViewModels.Shared;

namespace GamingSessionApp.BusinessLogic
{
    public interface ILeaderboardLogic : IDisposable
    {
        Task<UserLeaderboardViewModel> GetUserLeaderboard(string userId, int page);
    }
}
