using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.Models
{
    public class SessionStatus
    {
        public int Id { get; set; }

        [Required]
        public string Status { get; set; }

        [Required, MaxLength(200)]
        public string Description { get; set; }
    }
}
