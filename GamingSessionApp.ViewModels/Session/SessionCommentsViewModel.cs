using System;
using System.Collections.Generic;

namespace GamingSessionApp.ViewModels.Session
{
    public class SessionCommentsViewModel
    {
        public Guid SessionId { get; set; }

        //Can the user post a comment
        public bool CanPost { get; set; }

        //List of comments
        public List<CommentViewModel> Comments { get; set; }
    }

    public class CommentViewModel
    {
        //Comment order no.
        public int CommentNo { get; set; }

        //Author display name
        public string Author { get; set; }

        //Author thumbnail url
        public string ThumbnailUrl { get; set; }

        //Author kudos
        public string Kudos { get; set; }

        //Comment body (text)
        public string Body { get; set; }

        //Comment posted date
        public DateTime CreatedDate { get; set; }

        //Friendly display date
        public string CreatedDisplayDate { get; set; }
    }
}
