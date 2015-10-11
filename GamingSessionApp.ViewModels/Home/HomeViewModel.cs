using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.ViewModels.Home
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            OpenSessions = new List<SessionListItem>();
            NewSessions = new List<SessionListItem>();
            RecommendedSessions = new List<SessionListItem>();
        }
        public List<SessionListItem> OpenSessions { get; set; }
        public List<SessionListItem> NewSessions { get; set; }
        public List<SessionListItem> RecommendedSessions { get; set; }

    }
}
