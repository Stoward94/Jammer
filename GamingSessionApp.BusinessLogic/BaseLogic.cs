using System;
using System.IO;
using System.Threading.Tasks;
using Elmah;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GamingSessionApp.BusinessLogic
{
    public class BaseLogic : IDisposable
    {

        protected UnitOfWork UoW;
        public string UserId { protected get; set; }

        private UserManager<ApplicationUser> _userManager;
        private ApplicationUser _applicationUser;
        private TimeZoneInfo _userTimeZone;

        protected ValidationResult VResult;

        protected BaseLogic()
        {
            VResult = new ValidationResult();
        }

        public UserManager<ApplicationUser> UserManager
        {
            get { return _userManager ?? (_userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()))); }
            set { _userManager = value; }
        }

        protected ApplicationUser CurrentUser
        {
            get { return _applicationUser ?? (_applicationUser = UserManager.FindById(UserId)); }
            set { _applicationUser = value; }
        }

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

        protected TimeZoneInfo GetUserTimeZone()
        {
            if (_userTimeZone != null) return _userTimeZone;

            if (CurrentUser != null) return _userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(CurrentUser.TimeZoneId);

            //Else return UTC time zone (non-registered users)
            return _userTimeZone = TimeZoneInfo.Utc;
        }

        protected string GetImageUrl(string profileImageUrl, string targetFolder)
        {
            //Get the full image rather than the thumbnail
            string fileName = Path.GetFileName(profileImageUrl);
            return$"/Images/{targetFolder}/{fileName}";
        }

        /// <summary>
        /// Creates X,000 to k shorthand
        /// </summary>
        /// <param name="kudos"></param>
        /// <returns></returns>
        protected string TrimKudos(string kudos)
        {
            if (kudos.Length <= 3)
                return kudos;

            int i = int.Parse(kudos);
            return ((double)i / 1000).ToString("0.#k");
        }

        #region Dispose

        public void Dispose()
        {
            UoW?.Dispose();
            _userManager?.Dispose();
        }

        #endregion
    }

    public class ValidationResult
    {
        public ValidationResult()
        {
            Success = true;
        }

        public string Error { get; set; }

        public bool Success { get; set; }

        public object Data { get; set; }

        public ValidationResult AddError(string error)
        {
            Error = error;
            Success = false;

            return this;
        }

    }
}