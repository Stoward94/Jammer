using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.BusinessLogic
{
    public static class SystemEnums
    {
        public enum SessionMessageTypeEnum
        {
            System = 1,
            PlayerJoined = 2,
            PlayerLeft = 3,
            Comment = 4,
            Invitation = 5
        }
    }
}
