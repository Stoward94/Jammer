using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamingSessionApp.Models
{
    public class Kudos
    {
        public Kudos()
        {
            Points = 0;
        }

        [Key, ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public int Points { get; set; }

        public ICollection<KudosHistory> History { get; set; }
    }
}
