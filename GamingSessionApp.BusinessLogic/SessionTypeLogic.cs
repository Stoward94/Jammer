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
    public class SessionTypeLogic : BaseLogic
    {
        private readonly GenericRepository<SessionType> _typeRepo;

        public SessionTypeLogic()
        {
            _typeRepo = UoW.Repository<SessionType>();
        }
        
        public async Task<SelectList> GetTypeSelectList()
        {
            List<SessionType> typeList = await _typeRepo.Get().ToListAsync();

            return new SelectList(typeList, "Id", "Name");
        }
    }
}
