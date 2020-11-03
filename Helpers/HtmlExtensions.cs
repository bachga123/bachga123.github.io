using LoginWebsite.Models;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace LoginWebsite.Models
{
    public static class ImageHelper
    {
        public static MvcHtmlString Image(this HtmlHelper html, byte[] image)
        {
            var img = String.Format("data:image/png;base64,{0}", Convert.ToBase64String(image));
            return new MvcHtmlString("<img src='" + img + "' />");
        }
    }
}