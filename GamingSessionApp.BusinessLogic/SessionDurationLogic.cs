using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;

namespace GamingSessionApp.BusinessLogic
{
    public class SessionDurationLogic : BaseLogic
    {
        private readonly GenericRepository<SessionDuration> _durationRepo;

        public SessionDurationLogic(UnitOfWork uow)
        {
            UoW = uow;
            _durationRepo = UoW.Repository<SessionDuration>();
        }

        public async Task<SelectList> GetDurationSelectList()
        {
            List<SessionDuration> durationList = await _durationRepo.Get().ToListAsync();

            return new SelectList(durationList, "Id", "Duration");
        } 
    }
}
