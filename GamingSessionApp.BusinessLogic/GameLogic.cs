using System;
using System.Data.Entity;
using System.Threading.Tasks;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;

namespace GamingSessionApp.BusinessLogic
{
    public class GameLogic : BaseLogic
    {
        private readonly GenericRepository<Game> _gameRepo;

        public GameLogic()
        {
            _gameRepo = UoW.Repository<Game>();
        }

        public async Task<Game> ExistingGame(int? igdbId, string gameTitle)
        {
            try
            {
                var game = await _gameRepo.Get(x => x.GameTitle == gameTitle).FirstOrDefaultAsync();

                if (game == null)
                    return null;
                
                //If the game title exists but doesn't have an igdbId
                //However now it does (say it's been added to IGDB now) 
                //We want to update our db copy to include the id
                if (game.IgdbGameId == null && igdbId.HasValue)
                {
                    //update id and let the caller save changes
                    game.IgdbGameId = igdbId.Value;
                    _gameRepo.Update(game);
                }

                return game;
            }
            catch (Exception ex)
            {
                LogError(ex, "Error checking for existing game");
                return null;
            }
        }
    }
}
