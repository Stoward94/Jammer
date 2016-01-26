using System;

namespace GamingSessionApp.Models
{
    public class KudosHistory
    {
        public KudosHistory()
        {
            DateAdded = DateTime.UtcNow;
        }

        public int Id { get; set; }

        public int Points { get; set; }

        public DateTime DateAdded { get; set; }
    }
}