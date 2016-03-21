using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.ViewModels.Session
{
    public class SessionsFilter
    {
        //PlatformId
        public int? PlatformId { get; set; }

        //Game
        public string Game { get; set; }

        //TypeId
        public int? TypeId { get; set; }

        //My Sessions
        public bool MySessions { get; set; }

        //Sessions Im In
        public bool SessionsImIn { get; set; }

        //Free Spaces
        public bool FreeSpaces { get; set; }

        //Specific Date
        public string SpecificDate { get; set; }

        //Specific Time
        public DateTime SpecificTime { get; set; }

        //Page
        public int Page { get; set; }
    }
}
