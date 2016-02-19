using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GamingSessionApp.ViewModels.Home;

namespace GamingSessionApp.ViewModels.Profile
{
    public class UserProfileViewModel
    {
        public string DisplayName { get; set; }

        public int KudosPoints { get; set; }

        public string ProfileImageUrl { get; set; }
        
        public string XboxUsername { get; set; }
        
        public string XboxUrl { get; set; }
        
        public string PsnUsername { get; set; }
        
        public string PsnUrl { get; set; }
        
        public string SteamUsername { get; set; }
        
        public string SteamUrl { get; set; }

        public List<UserFriendViewModel> Friends{ get; set; }

        public List<SessionListItem> Sessions { get; set; }

        public List<SessionListItem> FriendsSessions { get; set; }

        public List<Models.KudosHistory> KudosHistory { get; set; }


    }
}
