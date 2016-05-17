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
            Invitation = 5,
            PlayerKicked = 6
        }

        public enum SessionStatusEnum
        {
            Open = 1,
            Full = 2,
            InProgress = 3,
            Completed = 4
        }

        public enum UserNotificationTypeEnum
        {
            PlayerJoined = 1,
            PlayerLeft = 2,
            KudosAdded = 3,
            Information = 4,
            Invitation = 5,
            Comment = 6,
            PlayerKicked = 7
        }

        public enum PlatformEnum
        {
            Windows = 1,
            Xbox360 = 2,
            XboxOne = 3,
            Ps2 = 4,
            Ps3 = 5,
            Ps4 = 6,
            Wii = 7,
            WiiU = 8,
            iOS = 9,
            Android = 10,
        }

        public enum SessionTypeEnum
        {
            Boosting = 1,
            Coop = 2,
            Competitive = 3,
            ClanMatch = 4,
            CasualPlay = 5,
            Achievement = 6
        }

        public enum AwardGroupsEnum
        {
            Created = 1,
            Completed = 2,
            Donation = 3,
            Rating = 4,
            Kudos = 5,
            Unique = 6
        }

        public enum AwardLevelsEnum
        {
            Beginner = 1,
            Novice = 2,
            Intermediate = 3,
            Advanced = 4,
            Expert = 5
        }
    }
}
