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
    public class PlatformLogic : BaseLogic, IBusinessLogic<Platform>
    {
        private readonly GenericRepository<Platform> _platformRepo;

        public PlatformLogic()
        {
            _platformRepo = UoW.Repository<Platform>();
        }

        public async Task<List<Platform>> GetAll()
        {
            return await _platformRepo.Get().ToListAsync();
        }

        public Platform GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Platform> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
