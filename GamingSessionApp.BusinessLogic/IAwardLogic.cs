using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamingSessionApp.ViewModels.Awards;

namespace GamingSessionApp.BusinessLogic
{
    public interface IAwardLogic : IDisposable
    {
        Task<UserAwardsViewModel> GetUserAwards(string userId);
    }
}
