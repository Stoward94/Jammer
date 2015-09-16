using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GamingSessionApp.DataAccess;

namespace GamingSessionApp.BusinessLogic
{
    public class BaseLogic
    {
        protected UnitOfWork UoW = new UnitOfWork();

        protected void SaveChanges()
        {
            UoW.Save();
        }

        protected async Task<bool> SaveChangesAsync()
        {
            return await UoW.SaveAsync();
        }
        
        public void Dispose()
        {
            UoW.Dispose();
        }
    }
}