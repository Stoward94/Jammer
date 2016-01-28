using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamingSessionApp.Models
{
    public class KudosHistory
    {
        public KudosHistory()
        {
            DateAdded = DateTime.UtcNow;
        }

        public int Id { get; set; }

        [ForeignKey("Kudos")]
        public string KudosId { get; set; }
        public Kudos Kudos { get; set; }

        public int Points { get; set; }

        public DateTime DateAdded { get; set; }
    }
}