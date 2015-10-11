using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.ViewModels.Home
{
    public class SessionListItem
    {
        public Guid SessionId { get; set; }

        public string Platform { get; set; }

        public string Type { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm | MMM dd, yyyy}")]
        public DateTime ScheduledDate { get; set; }

        public string GamerCount { get; set; }

        public string Summary { get; set; }
    }
}
