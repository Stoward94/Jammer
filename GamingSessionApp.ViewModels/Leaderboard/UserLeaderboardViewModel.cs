using System.Collections.Generic;
using GamingSessionApp.ViewModels.Shared;

namespace GamingSessionApp.ViewModels.Leaderboard
{
    public class UserLeaderboardViewModel
    {
        public Pagination Pagination { get; set; }

        public UserListItem CurrentUser { get; set; }

        public List<UserListItem> Users { get; set; }
        public List<UserListItem> Friends { get; set; }
        
    }

    public class UserListItem
    {
        public string Username { get; set; }

        public string ThumbnailUrl { get; set; }

        public string Kudos { get; set; }

        public int Rank { get; set; }
    }
}
