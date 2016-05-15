using System;
using System.Text;
using System.Web.Mvc;
using GamingSessionApp.ViewModels.Notifications;
using GamingSessionApp.ViewModels.Shared;

namespace GamingSessionApp.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString SessionTypeIconSmall(this HtmlHelper helper, int typeId)
        {
            var div = new TagBuilder("div");
            div.AddCssClass("type-icon-sm");

            string typeName = string.Empty;

            switch (typeId)
            {
                case 1: //Boosting
                    div.AddCssClass("type-boosting-sm");
                    typeName = "Boosting";
                    break;
                case 2: //Co-op
                    div.AddCssClass("type-coop-sm");
                    typeName = "Co-op";
                    break;
                case 3: //Competitive
                    div.AddCssClass("type-competitive-sm");
                    typeName = "Competitive";
                    break;
                case 4: //Clan Battle
                    div.AddCssClass("type-clan-sm");
                    typeName = "Clan Battle";
                    break;
                case 5: //Casual
                    div.AddCssClass("type-casual-sm");
                    typeName = "Casual";
                    break;
                case 6: //Achievement Hunting
                    div.AddCssClass("type-achievement-sm");
                    typeName = "Achievement | Trophy Hunting";
                    break;
            }

            div.MergeAttribute("title", typeName);

            // Render tag
            return new MvcHtmlString(div.ToString());
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

        //Not using this at the moment, may use it in the future
        //public static MvcHtmlString NotificationTypeIcon(this HtmlHelper helper, int typeId)
        //{
        //    //<img src="~/Media/user-joined-icon.png" width="36" height="36"/>
        //    var img = new TagBuilder("img");

        //    switch (typeId)
        //    {
        //        case 1: //PlayerJoined
        //            img.MergeAttribute("src", "/Media/user-joined-icon.png");
        //            img.MergeAttribute("alt", "Joined icon");
        //            break;
        //        case 2: //PlayerLeft
        //            img.MergeAttribute("src", "/Media/user-left-icon.png");
        //            img.MergeAttribute("alt", "Left icon");
        //            break;
        //        case 3: //KudosAdded
        //            img.MergeAttribute("src", "/Media/kudos-added-icon.png");
        //            img.MergeAttribute("alt", "Kudos awarded icon");
        //            break;
        //        case 4: //Information
        //            img.MergeAttribute("src", "/Media/info-icon.png");
        //            img.MergeAttribute("alt", "Info icon");
        //            break;
        //        case 5: //Invitation
        //            img.MergeAttribute("src", "/Media/invitation-icon.png");
        //            img.MergeAttribute("alt", "Invite icon");
        //            break;
        //        case 6: //Comment
        //            img.MergeAttribute("src", "/Media/comment-icon.png");
        //            img.MergeAttribute("alt", "Comment icon");
        //            break;
        //    }

        //    img.MergeAttribute("width", "36");
        //    img.MergeAttribute("height", "36");

        //    // Render tag
        //    return new MvcHtmlString(img.ToString(TagRenderMode.SelfClosing));
        //}

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
                    return helper.Action("Details", "Sessions", new { id = n.SessionId }) + "#" + n.CommentId;
            }

            return link;
        }

        public static MvcHtmlString PaginationFull(this HtmlHelper helper, Pagination pagination, string url)
        {
            //If we are on page 1 of 1 return (no control is needed);
            if (pagination.PageNo == 1 && pagination.TotalCount <= pagination.PageSize)
            {
                return null;
            }

            var nav = new TagBuilder("nav");
            var ul = new TagBuilder("ul");

            var start = new TagBuilder("li");
            var end = new TagBuilder("li");

            ul.AddCssClass("pagination");

            //Total number of pages
            int pagesRounded = (int)Math.Ceiling((double)pagination.TotalCount / (double)pagination.PageSize);

            start.InnerHtml = "<a href='"+ url + "?page=1' aria-label='Previous'><span aria-hidden='true'>&laquo;</span></a>";
            end.InnerHtml = "<a href='" + url + "?page=" + pagesRounded + "' aria-label='Next'><span aria-hidden='true'>&raquo;</span></a>";


            StringBuilder numbersHtml = new StringBuilder();

            for (int i = 1; i <= pagesRounded; i++)
            {
                var li = new TagBuilder("li");
                var a = new TagBuilder("a");

                if (i == pagination.PageNo)
                    li.AddCssClass("active");

                a.Attributes.Add("href", url + "?page=" + i);
                a.InnerHtml = i.ToString();

                li.InnerHtml = a.ToString();

                numbersHtml.Append(li);
            }

            //Disable start/end btn's
            if (pagination.PageNo == 1)
                start.AddCssClass("disabled");
            else if (pagination.PageNo == pagesRounded)
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

        public static MvcHtmlString SessionStatusLabel(this HtmlHelper helper, string status, string description, string placement = "right")
        {
            var span = new TagBuilder("span");
            span.AddCssClass("label");
            span.MergeAttribute("data-toggle", "popover");
            span.MergeAttribute("title", status);
            span.MergeAttribute("data-content", description);
            span.MergeAttribute("data-placement", placement);

            switch (status)
            {
                case "Open":
                    span.AddCssClass("label-success");
                    break;
                case "Full":
                    span.AddCssClass("label-warning");
                    break;
                case "In Progress":
                    span.AddCssClass("label-info");
                    break;
                case "Completed":
                    span.AddCssClass("label-danger");
                    break;

            }

            span.SetInnerText(status);

            return new MvcHtmlString(span.ToString());
        }

        public static MvcHtmlString PlatformIcon(this HtmlHelper helper, int platformId, string platform)
        {
            var div = new TagBuilder("div");
            div.AddCssClass("platform-icon");
            div.MergeAttribute("title", platform);

            switch (platformId)
            {
                case 1: //Windows PC
                    div.AddCssClass("platform-pc");
                    break;
                case 2: //Xbox 360
                    div.AddCssClass("platform-xbox");
                    break;
                case 3: //Xbox One
                    div.AddCssClass("platform-xbox-one");
                    break;
                case 4: //PS2
                    div.AddCssClass("platform-ps2");
                    break;
                case 5: //PS3
                    div.AddCssClass("platform-ps3");
                    break;
                case 6: //PS4
                    div.AddCssClass("platform-ps4");
                    break;
                case 7: //Wii
                    div.AddCssClass("platform-wii");
                    break;
                case 8: //Wii U
                    div.AddCssClass("platform-wii-u");
                    break;
                case 9: //iOS
                    div.AddCssClass("platform-ios");
                    break;
                case 10: //Android
                    div.AddCssClass("platform-android");
                    break;
            }
            
            return new MvcHtmlString(div.ToString());
        }

        public static MvcHtmlString SessionTypeIcon(this HtmlHelper helper, int typeId, string type)
        {
            var div = new TagBuilder("div");
            div.AddCssClass("type-icon");
            div.MergeAttribute("title", type);

            switch (typeId)
            {
                case 1: //Boosting
                    div.AddCssClass("type-boosting");
                    break;
                case 2: //Co-op
                    div.AddCssClass("type-coop");
                    break;
                case 3: //Competitive
                    div.AddCssClass("type-competitive");
                    break;
                case 4: //Clan Battle
                    div.AddCssClass("type-clan");
                    break;
                case 5: //Casual
                    div.AddCssClass("type-casual");
                    break;
                case 6: //Achievement Hunting
                    div.AddCssClass("type-achievement");
                    break;
            }

            // Render tag
            return new MvcHtmlString(div.ToString());
        }

    }
}
