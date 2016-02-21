using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.Models
{
    public class UserProfile
    {
        public UserProfile()
        {
            About = string.Empty;
            Website = string.Empty;
            XboxUrl = string.Empty;
            PlayStationUrl = string.Empty;
            SteamUrl = string.Empty;
        }

        [Key, ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required, MaxLength(256), Index(IsClustered = false, IsUnique = true)]
        public string DisplayName { get; set; }

        [Required]
        public string ThumbnailUrl { get; set; }

        public string About { get; set; }

        public string Website { get; set; }

        public string XboxGamertag { get; set; }
        public string XboxUrl { get; set; }

        public string PlayStationNetwork { get; set; }
        public string PlayStationUrl { get; set; }

        public string SteamName { get; set; }
        public string SteamUrl { get; set; }

        //Users kudos
        public Kudos Kudos { get; set; }

        public UserPreferences Preferences { get; set; }

        public ICollection<UserFriend> Friends { get; set; }

        public ICollection<UserNotification> Notifications { get; set; }

        public ICollection<UserMessage> Messages { get; set; }

        public ICollection<Session> Sessions { get; set; }

        public ICollection<SessionFeedback> Feedback { get; set; }

    }
}
