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
    public class UserLogic : BaseLogic
    {
        private readonly GenericRepository<ApplicationUser> _userRepo;

        public UserLogic()
        {
            _userRepo = UoW.Repository<ApplicationUser>();
        }

        public ApplicationUser GetUser(string userName)
        {
            return _userRepo.Get(x => x.UserName == userName)
                .Include(x => x.Profile)
                .FirstOrDefault();
        }


    }
}
