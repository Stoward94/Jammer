using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.ViewModels.Session
{
    public class SessionListItemViewModel
    {
        public Guid Id { get; set; }

        //Creator Name
        public string Creator { get; set; }

        //User-friendly date
        public string ScheduledDate { get; set; }

        //What's the status of the session
        public string Status { get; set; }

        //Which platform is this session for?
        public string Platform { get; set; }

        //Type of session this session is 'Boosting/Co-op etc.
        public string Type { get; set; }

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
