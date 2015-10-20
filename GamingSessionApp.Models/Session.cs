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
            Messages = new List<SessionMessage>();
            SignedGamers = new List<ApplicationUser>();
            StatusId = 1; // Status = Recruiting
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        //Who is the creator of this session
        [Required]
        public string CreatorId { get; set; }
        public virtual ApplicationUser Creator { get; set; }

        //When the Session was created
        [Required]
        public DateTime CreatedDate { get; set; }

        //Date of when the session is scheduled for
        [Required]
        public DateTime ScheduledDate { get; set; }

        //What status is the session in
        public int StatusId { get; set; }
        public virtual SessionStatus Status { get; set; }

        //Which platform is this session for?
        [Required]
        public int PlatformId { get; set; }
        public virtual Platform Platform { get; set; }
        
        //Type of session this session is 'Boosting/Co-op etc.
        [Required]
        public int TypeId { get; set; }
        public virtual SessionType Type { get; set; }

        //# of gamers needed for the session
        [Required]
        public int GamersRequired { get; set; }

        //# of gamers registered for session
        public int SignedGamersCount => SignedGamers.Count;

        //Description of what the session is about
        [Required, StringLength(5000)]
        public string Information { get; set; }

        //Expected duration of the session
        [Required]
        public int DurationId { get; set; }
        public virtual SessionDuration Duration { get; set; }

        //Is the session active (not yet reached the scheduled start date)
        public bool Active { get; set; }

        //Property to the settings object
        public virtual SessionSettings Settings { get; set; }

        //Navigation property to the session messages
        public virtual ICollection<SessionMessage> Messages { get; set; }

        //A collection of the gamers signed to the session
        public virtual ICollection<ApplicationUser> SignedGamers { get; set; }
    }
}