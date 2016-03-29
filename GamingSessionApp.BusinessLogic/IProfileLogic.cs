using Elmah.ContentSyndication;
using GamingSessionApp.ViewModels.Profile;
using GamingSessionApp.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace GamingSessionApp.BusinessLogic
{
    public interface IProfileLogic : IDisposable
    {
        Task<UserProfileViewModel> GetMyProfile(string userId);
        Task<UserProfileViewModel> GetUserProfile(string userName);
        UserMenuViewModel GetUserMenuInformation(string userId);
        Task<object> GetUsersJson(string q);
        Task<ValidationResult> ProcessImageUpload(HttpPostedFileBase file, string userId);
        Task<EditProfileViewModel> GetEditProfileModel(string userId);
        Task<ValidationResult> EditProfile(EditProfileViewModel model, string userId);
        Task<List<FriendListItem>> GetUsersFriends(string userId);
        Task<ValidationResult> AddFriend(string username, string userId);
    }
}