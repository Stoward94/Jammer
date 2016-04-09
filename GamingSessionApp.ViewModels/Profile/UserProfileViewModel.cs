using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GamingSessionApp.ViewModels.Awards;
using GamingSessionApp.ViewModels.Session;

namespace GamingSessionApp.ViewModels.Profile
{
    public class UserProfileViewModel
    {
        public string DisplayName { get; set; }

        public string About { get; set; }
        
        public string Kudos { get; set; }

        public double Rating { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy}")]
        public DateTime Registered { get; set; }

        public DateTime LastSignIn { get; set; }
        public string DisplayLastSignIn { get; set; }

        public string ProfileImageUrl { get; set; }

        public UserSocialLinks Social { get; set; }

        public List<AwardViewModel> Awards { get; set; }

        //Awards counts
        public int BeginnerCount { get; set; }
        public int NoviceCount { get; set; }
        public int IntermediateCount { get; set; }
        public int AdvancedCount { get; set; }
        public int ExpertCount { get; set; }

        //Is the user viewing the profile friends with the profile owner?
        public bool IsFriend { get; set; }

        public UserSessionStatistics Statistics { get; set; }

        public List<UserFriendViewModel> Friends{ get; set; }

        public List<SessionListItemViewModel> Sessions { get; set; }

        public List<SessionListItemViewModel> FriendsSessions { get; set; }

        public List<Models.KudosHistory> KudosHistory { get; set; }
    }

    public class UserSocialLinks
    {
        public string Xbox { get; set; }
        public string XboxUrl { get; set; }

        public string PlayStation { get; set; }
        public string PlayStationUrl { get; set; }

        public string Steam { get; set; }
        public string SteamUrl { get; set; }

        public string Twitter { get; set; }

        public string Facebook { get; set; }

        public string Twitch { get; set; }
    }

    public class UserSessionStatistics
    {
        public int CompletedSessions { get; set; }

        public List<UserPlatformStatistic> Platforms { get; set; }
    }

    public class UserPlatformStatistic
    {
        public string Platform { get; set; }

        public int PlatformId { get; set; }

        public int CompletedCount { get; set; }
    }
}
