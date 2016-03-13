using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.Models
{
    public class SessionGoal
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Session")]
        public Guid SessionId { get; set; }
        public Session Session { get; set; }

        [Required]
        public string Goal { get; set; }
    }
}
