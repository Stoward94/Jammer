using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamingSessionApp.Models
{
    public class UserMessage
    {
        public UserMessage()
        {
            CreatedDate = DateTime.UtcNow;
            Read = false;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, ForeignKey("Recipient")]
        public string RecipientId { get; set; }
        public UserProfile Recipient { get; set; }

        [Required, ForeignKey("Sender")]
        public string SenderId { get; set; }
        public UserProfile Sender { get; set; }

        public DateTime CreatedDate { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        public bool Read { get; set; }

    }
}
