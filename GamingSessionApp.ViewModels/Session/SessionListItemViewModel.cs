using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.ViewModels.Session
{
    public class AllSessionsViewModel
    {
        public string StartDisplayDate { get; set; }
        
        public string EndDisplayDate { get; set; }

        public int TotalSessions { get; set; }

        public List<SessionDateGroup> Groups { get; set; }
    }

    public class SessionDateGroup
    {
        public DateTime ScheduledDate { get; set; }
        public string ScheduledDisplayDate { get; set; }

        public List<SessionListItemViewModel> Sessions { get; set; }
    }

    public class SessionListItemViewModel
    {
        public Guid Id { get; set; }

        //Creator Name
        public string Creator { get; set; }

        public string Game { get; set; }

        //User-friendly date
        public DateTime ScheduledTime { get; set; }
        public string ScheduledDisplayTime { get; set; }

        //What's the status of the session
        public string Status { get; set; }
        public string StatusDescription { get; set; }

        //Which platform is this session for?
        public string Platform { get; set; }
        public int PlatformId { get; set; }

        //Type of session this session is 'Boosting/Co-op etc.
        public string Type { get; set; }
        public int TypeId { get; set; }

        //# of gamers needed for the session
        public int RequiredCount { get; set; }

        //# of gamers that have joined the session
        public int MembersCount { get; set; }

        //Summary of the session
        public string Summary { get; set; }

        //Expected duration of the session
        public string Duration { get; set; }
    }
}
