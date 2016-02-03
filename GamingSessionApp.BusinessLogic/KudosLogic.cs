using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;

namespace GamingSessionApp.BusinessLogic
{
    public class KudosLogic : BaseLogic
    {
        //Repository
        private readonly GenericRepository<Kudos> _kudosRepo;

        public KudosLogic()
        {
            _kudosRepo = UoW.Repository<Kudos>();
        }

        public async Task<ValidationResult> AddKudosPoints(string userId, int value)
        {
            try
            {
                var userRepo = UoW.Repository<UserProfile>();

                //Load user from the db
                UserProfile user = await userRepo.Get(x => x.UserId == userId)
                    .Include(x => x.Kudos.History)
                    .FirstOrDefaultAsync();

                user.Kudos.Points += value;
                

                //Insert Kudos history record
                user.Kudos.History.Add(new KudosHistory { Points = value });

                userRepo.Update(user);
                await SaveChangesAsync();

                return VResult;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return VResult.AddError("Unable to add kudos to user");
            }
        }

        public async Task<List<Kudos>> KudosLeadboard()
        {
            try
            {
                List<Kudos> results = await _kudosRepo.Get()
                    .OrderByDescending(x => x.Points)
                    .Take(20)
                    .ToListAsync();

                return results;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to get Kudos leaderboards");
                return null;
            }
        }
    }
}
