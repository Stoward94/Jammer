using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.ViewModels.Home
{
    public class SearchResultsViewModel
    {
        //Sessions that match the search criteria
        public List<SessionSearchResult> Sessions { get; set;  }

        //Users that match the search criteria
        public List<UserSearchResult> Users { get; set; }

        //Do we have any search results?
        public bool AnyResults => Sessions.Any() || Users.Any();

        public bool IsUtcTime { get; set; }
    }

    public class SessionSearchResult
    {
        public Guid Id { get; set; }

        public int PlatformId { get; set; }
        public string Platform { get; set; }

        public int TypeId { get; set; }
        public string Type { get; set; }

        public string Game { get; set; }

        public DateTime ScheduledStart { get; set; }
        public string DisplayScheduledStart { get; set; }
    }


    public class UserSearchResult
    {
        public string ThumbnailUrl { get; set; }

        public string Username { get; set; }

        public string Kudos { get; set; }
    }
}
