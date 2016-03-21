using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.ViewModels.Session
{
    public class SessionMemberViewModel
    {
        public string UserId { get; set; }

        public string ThumbnailUrl { get; set; }
        
        public string DisplayName { get; set; }

        public string Kudos { get; set; }

        public bool IsHost { get; set; }
    }
}
