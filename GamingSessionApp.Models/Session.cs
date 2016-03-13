using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamingSessionApp.Models
{
    public class Session
    {
        public Session()
        {
            CreatedDate = DateTime.UtcNow;
            Active = true;
            Comments = new List<SessionComment>();
            Members = new List<UserProfile>();
            StatusId = 1; // Status = Recruiting
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        //Who is the creator of this session
        [Required, ForeignKey("Creator")]
        public string CreatorId { get; set; }
        public UserProfile Creator { get; set; }

        [ForeignKey("Game")]
        public int GameId { get; set; }
        public Game Game { get; set; }

        //When the Session was created
        [Required]
        public DateTime CreatedDate { get; set; }

        //Date of when the session is scheduled for
        [Required]
        public DateTime ScheduledDate { get; set; }

        //Date/Time of expected session end time.
        [Required]
        public DateTime EndTime { get; set; }

        //What status is the session in
        public int StatusId { get; set; }
        public SessionStatus Status { get; set; }

        //Which platform is this session for?
        [Required]
        public int PlatformId { get; set; }
        public Platform Platform { get; set; }
        
        //Type of session this session is 'Boosting/Co-op etc.
        [Required]
        public int TypeId { get; set; }
        public SessionType Type { get; set; }

        //# of gamers needed for the session
        [Required]
        public int MembersRequired { get; set; }

        //# of gamers registered for session
        public int MembersCount => Members.Count;

        //Description of what the session is about
        [Required, StringLength(5000)]
        public string Information { get; set; }

        //Expected duration of the session
        [Required]
        public int DurationId { get; set; }
        public SessionDuration Duration { get; set; }

        //Is the session active (not yet reached the scheduled start date)
        public bool Active { get; set; }

        //Property to the settings object
        public SessionSettings Settings { get; set; }

        //Navigation property to the session messages
        public ICollection<SessionComment> Comments { get; set; }

        //A collection of the gamers signed to the session
        public ICollection<UserProfile> Members { get; set; }

        public ICollection<SessionFeedback> Feedback { get; set; }

        public ICollection<SessionGoal> Goals { get; set; }
    }
}