using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Elmah;
using GamingSessionApp.DataAccess;

namespace GamingSessionApp.BusinessLogic
{
    public class BaseLogic
    {
        protected readonly UnitOfWork UoW = new UnitOfWork();

        protected void SaveChanges()
        {
            UoW.Save();
        }

        protected async Task<bool> SaveChangesAsync()
        {
            return await UoW.SaveAsync();
        }

        /// <summary>
        /// Used to log an error using elmah
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message">Optional custom message to be added to the exception</param>
        protected void LogError(Exception ex, string message = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var newEx = new Exception(message, ex);
                ErrorSignal.FromCurrentContext().Raise(newEx);

            }
            else
                ErrorSignal.FromCurrentContext().Raise(ex);
        }
        
        public void Dispose()
        {
            UoW.Dispose();
        }
    }
}