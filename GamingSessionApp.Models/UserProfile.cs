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
        [Key, ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string ThumbnailUrl { get; set; }

        public string About { get; set; }

        //Users kudos
        public Kudos Kudos { get; set; }

        public ICollection<UserFriend> Friends { get; set; }

        public ICollection<UserNotification> Notifications { get; set; }

        public ICollection<UserMessage> Messages { get; set; }
    }
}
