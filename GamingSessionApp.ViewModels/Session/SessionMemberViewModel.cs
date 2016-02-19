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

        public bool IsHost { get; set; }

        public string DisplayName { get; set; }

        public int Kudos { get; set; }
    }
}
