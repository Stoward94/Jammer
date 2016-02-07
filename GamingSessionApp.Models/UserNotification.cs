using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamingSessionApp.Models
{
    public class UserNotification
    {
        public UserNotification()
        {
            CreatedDate = DateTime.UtcNow;
            Read = false;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ForeignKey("Recipient")]
        public string RecipientId { get; set; }
        public UserProfile Recipient { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required]
        public string Body { get; set; }
        
        [ForeignKey("Type")]
        public int TypeId { get; set; }
        public UserNotificationType Type { get; set; }

        //Session the notification is referring to
        [ForeignKey("Session")]
        public Guid SessionId { get; set; }
        public Session Session { get; set; }

        //If the notification is referring to a comment on a session
        public int? MessageId { get; set; }

        public bool Read { get; set; }

    }
}
