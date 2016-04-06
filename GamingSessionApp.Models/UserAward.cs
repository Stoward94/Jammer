using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamingSessionApp.Models
{
    public class UserAward
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("User")]
        public string UserId { get; set; }
        public UserProfile User { get; set; }

        [Required, ForeignKey("Award")]
        public int AwardId { get; set; }
        public Award Award { get; set; }

        [Required]
        public DateTime DateAwarded { get; set; }
    }
}
