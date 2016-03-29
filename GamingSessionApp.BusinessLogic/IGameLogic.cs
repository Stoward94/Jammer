using GamingSessionApp.Models;
using System;
using System.Threading.Tasks;

namespace GamingSessionApp.BusinessLogic
{
    public interface IGameLogic : IDisposable
    {
        Task<Game> ExistingGame(int? igdbId, string gameTitle);
    }
}