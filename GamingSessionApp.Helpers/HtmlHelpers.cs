using System;
using System.Text;
using System.Web.Mvc;
using GamingSessionApp.ViewModels.Notifications;

namespace GamingSessionApp.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString SessionTypeIcon(this HtmlHelper helper, string type)
        {
            var builder = new TagBuilder("img");

            switch (type)
            {
                case "Achievement hunting":
                    builder.MergeAttribute("src", "/Media/achievement-icon.png");
                    break;
                case "Boosting":
                    builder.MergeAttribute("src", "/Media/boosting-icon.png");
                    break;
                case "Competitive":
                    builder.MergeAttribute("src", "/Media/competitive-icon.png");
                    break;
                case "Co-op":
                    builder.MergeAttribute("src", "/Media/coop-icon.png");
                    break;
            }


            builder.MergeAttribute("alt", type);
            builder.MergeAttribute("title", type);
            builder.MergeAttribute("class", "img-circle");
            builder.MergeAttribute("width", "48");
            builder.MergeAttribute("height", "48");

            // Render tag
            return new MvcHtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString SuccessMessage(this HtmlHelper helper, string message)
        {
            if (string.IsNullOrEmpty(message))
                return null;

            var span = new TagBuilder("span");
            span.MergeAttribute("aria-hidden", "true");
            span.SetInnerText("×");

            var button = new TagBuilder("button");
            button.MergeAttribute("type", "button");
            button.MergeAttribute("class", "close");
            button.MergeAttribute("data-dismiss", "alert");
            button.MergeAttribute("aria-label", "Close");
            button.InnerHtml = span.ToString();

            var div = new TagBuilder("div");
            div.MergeAttribute("class", "alert alert-success alert-dismissible");
            div.MergeAttribute("role", "alert");
            div.InnerHtml = button + message;

            // Render tag
            return new MvcHtmlString(div.ToString());
        }

        public static MvcHtmlString NotificationTypeIcon(this HtmlHelper helper, int typeId)
        {
            //<img src="~/Media/user-joined-icon.png" width="36" height="36"/>
            var img = new TagBuilder("img");

            switch (typeId)
            {
                case 1: //PlayerJoined
                    img.MergeAttribute("src", "/Media/user-joined-icon.png");
                    img.MergeAttribute("alt", "Joined icon");
                    break;
                case 2: //PlayerLeft
                    img.MergeAttribute("src", "/Media/user-left-icon.png");
                    img.MergeAttribute("alt", "Left icon");
                    break;
                case 3: //KudosAdded
                    img.MergeAttribute("src", "/Media/kudos-added-icon.png");
                    img.MergeAttribute("alt", "Kudos awarded icon");
                    break;
                case 4: //Information
                    img.MergeAttribute("src", "/Media/info-icon.png");
                    img.MergeAttribute("alt", "Info icon");
                    break;
                case 5: //Invitation
                    img.MergeAttribute("src", "/Media/invitation-icon.png");
                    img.MergeAttribute("alt", "Invite icon");
                    break;
                case 6: //Comment
                    img.MergeAttribute("src", "/Media/comment-icon.png");
                    img.MergeAttribute("alt", "Comment icon");
                    break;
            }

            img.MergeAttribute("width", "36");
            img.MergeAttribute("height", "36");

            // Render tag
            return new MvcHtmlString(img.ToString(TagRenderMode.SelfClosing));
        }

        public static string NotificationTypeLink(this UrlHelper helper, UserNotificationViewModel n)
        {
            string link = string.Empty;

            switch (n.TypeId)
            {
                case 1: //PlayerJoined
                    return helper.Action("Details", "Sessions", new { id = n.SessionId });
                case 2: //PlayerLeft
                    return helper.Action("Details", "Sessions", new { id = n.SessionId });
                case 3: //KudosAdded
                    //TODO - Add Kudos item link
                    break;
                case 4: //Information
                   //TODO - Add info link
                    break;
                case 5: //Invitation
                    return helper.Action("Details", "Sessions", new { id = n.SessionId });
                case 6: //Comment
                    //TODO - Add comment to link
                    break;
            }

            return link;
        }

        public static MvcHtmlString PaginationFull(this HtmlHelper helper, int pageSize, int pageNo, int totalCount)
        {
            //If we are on page 1 of 1 return (no control is needed);
            if (pageNo == 1 && totalCount <= pageSize)
            {
                return null;
            }

            var nav = new TagBuilder("nav");
            var ul = new TagBuilder("ul");

            var start = new TagBuilder("li");
            var end = new TagBuilder("li");

            ul.AddCssClass("pagination");

            start.InnerHtml = "<a href='#' aria-label='Previous'><span aria-hidden='true'>&laquo;</span></a>";
            end.InnerHtml = "<a href='#' aria-label='Next'><span aria-hidden='true'>&raquo;</span></a>";

            int pagesRounded = (int)Math.Ceiling((double)totalCount / (double)pageSize);

            StringBuilder numbersHtml = new StringBuilder();

            for (int i = 1; i <= pagesRounded; i++)
            {
                var li = new TagBuilder("li");
                var a = new TagBuilder("a");

                if (i == pageNo)
                    li.AddCssClass("active");

                a.Attributes.Add("href", "/Notifications/ViewAll?page=" + i);
                a.InnerHtml = i.ToString();

                li.InnerHtml = a.ToString();

                numbersHtml.Append(li);
            }

            //Disable start/end btn's
            if (pageNo == 1)
                start.AddCssClass("disabled");
            else if (pageNo == pagesRounded)
                end.AddCssClass("disabled");

            //Build ul tag
            ul.InnerHtml = start + numbersHtml.ToString() + end;

            //Build nav tag
            nav.InnerHtml = ul.ToString();

            return new MvcHtmlString(nav.ToString());
        }

        public static MvcHtmlString InfoPopUp(this HtmlHelper helper, string title, string body, string position)
        {
            var span = new TagBuilder("span");
            span.MergeAttribute("data-toggle", "popover");
            span.MergeAttribute("title", title);
            span.MergeAttribute("data-content", body);
            span.MergeAttribute("data-placement", position);

            var i = new TagBuilder("i");
            i.AddCssClass("fa fa-question-circle info-icon");

            span.InnerHtml = i.ToString();

            return new MvcHtmlString(span.ToString());
        }
    }
}
