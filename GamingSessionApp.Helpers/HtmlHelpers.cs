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

            var builder = new TagBuilder("div");
            
            builder.MergeAttribute("class", "alert alert-success");
            builder.MergeAttribute("role", "alert");
            builder.SetInnerText(message);

            // Render tag
            return new MvcHtmlString(builder.ToString());
        }
    }
}
