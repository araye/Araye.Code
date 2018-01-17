using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Araye.Code.Mvc
{
    public static class Extensions
    {
        public static ControllerBase GetControllerByName(this HtmlHelper htmlHelper, string controllerName)
        {
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            IController controller = factory.CreateController(htmlHelper.ViewContext.RequestContext, controllerName);
            if (controller == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "The IControllerFactory '{0}' did not return a controller for the name '{1}'.", factory.GetType(), controllerName));
            }
            return (ControllerBase)controller;
        }

        public static MvcHtmlString ValidationSummaryBootstrap(this HtmlHelper helper, bool closeable)
        {
            # region Equivalent view markup
            // var errors = ViewData.ModelState.SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage));
            //
            // if (errors.Count() > 0)
            // {
            //     <div class="alert alert-error alert-block">
            //         <button type="button" class="close" data-dismiss="alert">&times;</button>
            //         <strong>Validation error</strong> - please fix the errors listed below and try again.
            //         <ul>
            //             @foreach (var error in errors)
            //             {
            //                 <li class="text-error">@error</li>
            //             }
            //         </ul>
            //     </div>
            // }
            # endregion

            var errors = helper.ViewContext.ViewData.ModelState.SelectMany(state => state.Value.Errors.Select(error => error.ErrorMessage));

            int errorCount = errors.Count();

            if (errorCount == 0)
            {
                return new MvcHtmlString(string.Empty);
            }

            var div = new TagBuilder("div");
            div.AddCssClass("alert");
            div.AddCssClass("alert-danger");

            string message;

            if (errorCount == 1)
            {
                message = errors.First();
            }
            else
            {
                message = "لطفاً خطاهای زیر را برطرف و مجدداً تلاش کنید";
                div.AddCssClass("alert-block");
            }

            if (closeable)
            {
                var button = new TagBuilder("button");
                button.AddCssClass("close");
                button.MergeAttribute("type", "button");
                button.MergeAttribute("data-dismiss", "alert");
                button.SetInnerText("x");
                div.InnerHtml += button.ToString();
            }

            div.InnerHtml += message;

            if (errorCount > 1)
            {
                var ul = new TagBuilder("ul");

                foreach (var error in errors)
                {
                    var li = new TagBuilder("li");
                    li.AddCssClass("text-error");
                    li.SetInnerText(error);
                    ul.InnerHtml += li.ToString();
                }

                div.InnerHtml += ul.ToString();
            }

            return new MvcHtmlString(div.ToString());
        }

        /// <summary>
        /// Overload allowing no arguments.
        /// </summary>
        public static MvcHtmlString ValidationSummaryBootstrap(this HtmlHelper helper)
        {
            return ValidationSummaryBootstrap(helper, true);
        }
    }
}
