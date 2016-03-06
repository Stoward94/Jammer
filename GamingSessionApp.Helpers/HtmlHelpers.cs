using System.Web.Mvc;

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
    }
}
