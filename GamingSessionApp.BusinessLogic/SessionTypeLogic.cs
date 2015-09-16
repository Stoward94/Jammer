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
    public class SessionTypeLogic : BaseLogic, IBusinessLogic<SessionType>
    {
        private readonly GenericRepository<SessionType> _sessionTypeRepo;

        public SessionTypeLogic()
        {
            _sessionTypeRepo = UoW.Repository<SessionType>();
        }
        
        public async Task<List<SessionType>> GetAll()
        {
            return await _sessionTypeRepo.Get().ToListAsync();
        }

        public SessionType GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<SessionType> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
