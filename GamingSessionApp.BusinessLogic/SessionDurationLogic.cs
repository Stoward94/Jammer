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
    public class SessionDurationLogic : BaseLogic, IBusinessLogic<SessionDuration>
    {
        private readonly GenericRepository<SessionDuration> _sessionDurationRepo;

        public SessionDurationLogic()
        {
            _sessionDurationRepo = UoW.Repository<SessionDuration>();
        }

        public async Task<List<SessionDuration>> GetAll()
        {
            return await _sessionDurationRepo.Get().ToListAsync();
        }

        public SessionDuration GetById(object id)
        {
            throw new NotImplementedException();
        }

        public Task<SessionDuration> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }
    }
}
