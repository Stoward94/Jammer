using System.Collections.Generic;

namespace GamingSessionApp.ViewModels.Profile
{
    public class UserProfileViewModel
    {
        public string DisplayName { get; set; }

        public int KudosPoints { get; set; }

        public string ProfileImageUrl { get; set; }

        public List<UserFriendViewModel> Friends{ get; set; }

        public List<Models.Session> Sessions { get; set; }

        public List<Models.Session> FriendsSessions { get; set; }


    }
}
