using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
