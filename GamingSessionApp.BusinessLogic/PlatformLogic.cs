using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using System.Web.Mvc;

namespace GamingSessionApp.BusinessLogic
{
    public class PlatformLogic : BaseLogic
    {
        private readonly GenericRepository<Platform> _platformRepo;

        public PlatformLogic(UnitOfWork uow)
        {
            UoW = uow;
            _platformRepo = UoW.Repository<Platform>();
        }

        public async Task<SelectList> GetPlatformSelectList()
        {
            List<Platform> typeList = await _platformRepo.Get().ToListAsync();

            return new SelectList(typeList, "Id", "Name");
        }
    }
}
