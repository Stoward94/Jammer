﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamingSessionApp.Models
{
    public class UserProfile
    {
        public UserProfile()
        {
            About = string.Empty;
            Website = string.Empty;
            Rating = 5;
        }

        [Key, ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required, MaxLength(256), Index(IsClustered = false, IsUnique = true)]
        public string DisplayName { get; set; }

        [Required]
        public string ThumbnailUrl { get; set; }

        public string About { get; set; }

        [Required]
        public double Rating { get; set; }

        public string Website { get; set; }

       //Users kudos
        public Kudos Kudos { get; set; }
        
        public UserPreferences Preferences { get; set; }

        public UserStatistics Statistics { get; set; }

        public UserSocial Social { get; set; }

        public ICollection<UserFriend> Friends { get; set; }

        public ICollection<UserNotification> Notifications { get; set; }

        public ICollection<UserMessage> Messages { get; set; }

        public ICollection<Session> Sessions { get; set; }

        public ICollection<SessionFeedback> Feedback { get; set; }

        public ICollection<UserAward> Awards { get; set; }
    }
}
