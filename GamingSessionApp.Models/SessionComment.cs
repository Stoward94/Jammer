using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingSessionApp.Models
{
    public class SessionComment
    {
        public SessionComment()
        {
            CreatedDate = DateTime.UtcNow;
        }

        public int Id { get; set; }

        //Foreign Key reference to its owning session
        [Required, ForeignKey("Session")]
        public Guid SessionId { get; set; }
        public virtual Session Session { get; set; }

        //Foreign Key reference to the type of comment
        [Required, ForeignKey("CommentType")]
        public int CommentTypeId { get; set; }
        public SessionCommentType CommentType { get; set; }

        [Required, ForeignKey("Author")]
        public string AuthorId { get; set; }
        public UserProfile Author { get; set; }

        //When was the message added
        [Required, DisplayFormat(DataFormatString = "{0:dd/MM/yy HH:mm}")]
        public DateTime CreatedDate { get; set; }

        //What is the message number
        //[Required]
        //public int CommentNo { get; set; }

        //What does the message say
        [Required, StringLength(2000), MaxLength(2000)]
        public string Body { get; set; }
    }
}
