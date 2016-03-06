using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GamingSessionApp.ViewModels.SessionComments
{
    public class PostCommentViewModel
    {
        [Required]
        public Guid SessionId { get; set; }

        [Required, AllowHtml]
        public string Comment { get; set; }
    }
}