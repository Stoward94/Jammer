using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamingSessionApp.ViewModels.Session;

namespace GamingSessionApp.ViewModels.Home
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            NewSessions = new List<SessionListItemViewModel>();
        }

        public KudosLeaderboard KudosLeaderboard { get; set; }

        public List<NewestUsers> NewUsers { get; set; }

        public List<SessionListItemViewModel> NewSessions { get; set; }

    }

    public class KudosLeaderboard
    {
        public List<UserSearchResult> Users { get; set; }
    }

    public class NewestUsers
    {
        public string ThumbnailUrl { get; set; }

        public string Username { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy}")]
        public DateTime Registered { get; set; }

    }
}
