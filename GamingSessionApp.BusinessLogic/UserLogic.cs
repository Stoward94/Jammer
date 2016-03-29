using System;
using System.Web.Mvc;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Account;
using Microsoft.AspNet.Identity;

namespace GamingSessionApp.BusinessLogic
{
    public class UserLogic : BaseLogic, IUserLogic
    {
        private readonly GenericRepository<ApplicationUser> _userRepo;

        public UserLogic(UnitOfWork uow)
        {
            UoW = uow;
            _userRepo = UoW.Repository<ApplicationUser>();
        }

        public ApplicationUser GetUser(string userName)
        {
            return _userRepo.Get(x => x.UserName == userName)
                .Include(x => x.Profile)
                .FirstOrDefault();
        }

        public async Task<EditAccountViewModel> GetEditAccountModel(string userId)
        {
            try
            {
                EditAccountViewModel model = await _userRepo.Get(x => x.Id == userId)
                    .Select(x => new EditAccountViewModel
                    {
                        Email = x.Email,
                        TimeZoneId = x.TimeZoneId
                    })
                    .FirstOrDefaultAsync();

                //Get times zones for select list
                model.TimeZones = GetTimeZonesList();

                return model;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to get edit account view model for user : " + userId);
                return null;
            }
        }

        public SelectList GetTimeZonesList()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();

            return new SelectList(timeZones, "Id", "DisplayName");
        }

        public async Task<ValidationResult> EditAccount(EditAccountViewModel model, string userId)
        {
            try
            {
                //Load the account object from the db
                ApplicationUser user = await _userRepo.Get(x => x.Id == userId)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return VResult.AddError("Unable to update your account details. Please try again later.");

                //Security check
                if(model.Password != model.ConfirmPassword)
                    return VResult.AddError("Your passwords do not match. Please check your passwords and try again.");

                //Update identity password if needed
                if (!string.IsNullOrEmpty(model.Password))
                {
                    if(string.IsNullOrEmpty(model.CurrentPassword))
                        return VResult.AddError("You need to enter your current password before you can change it.");

                    IdentityResult r = await UserManager.ChangePasswordAsync(userId, model.CurrentPassword, model.Password);

                    //If failed
                    if(!r.Succeeded)
                        return VResult.AddError("Unable to change your password. Please check your current password and try again.");
                }

                //Now update fields
                if(user.Email != model.Email)
                { 
                    user.Email = model.Email;
                    user.EmailConfirmed = false;
                }

                user.TimeZoneId = model.TimeZoneId;

                _userRepo.Update(user);
                await SaveChangesAsync();

                return VResult;
            }
            catch (Exception ex)
            {
                LogError(ex, "Error updating the account details for user : " + userId);
                return
                    VResult.AddError("An error occurred when trying to update your account details. Please try again later.");
            }
        }
    }
}
