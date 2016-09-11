using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Movies.CustomHelpers
{
    public static class CustomHtmlHelpers
    {
        public static IHtmlString CustomLabel(this HtmlHelper helper, string id, string value)
        {
            string element = string.Format("<label id='{0}' value='{1}'>{1}</label>", id, value);
            return new MvcHtmlString(element);
        }
    }
}