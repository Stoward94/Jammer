using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Profile;

namespace GamingSessionApp.BusinessLogic
{
    public class ProfileLogic : BaseLogic
    {
        private readonly GenericRepository<UserProfile> _profileRepo;

        public ProfileLogic()
        {
            _profileRepo = UoW.Repository<UserProfile>();
        }

        public async Task<UserProfileViewModel> GetMyProfile(string userId)
        {
            try
            {
                UserProfileViewModel profile = await _profileRepo.Get(x => x.UserId == userId)
                    .Select(x => new UserProfileViewModel
                    {
                        DisplayName = x.DisplayName,
                        KudosValue = x.User.Kudos.Points,
                        ProfileImageUrl = x.ThumbnailUrl,
                    })
                    .FirstOrDefaultAsync();

                return profile;
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }
    }
}
