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

        public enum SessionStatusEnum
        {
            Recruiting = 1,
            FullyLoaded = 2,
            Jamming = 3,
            Retired = 4
        }

        public enum UserNotificationTypeEnum
        {
            PlayerJoined = 1,
            PlayerLeft = 2,
            KudosAdded = 3,
            Information = 4,
            Invitation = 5
        }
    }
}
