using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Araye.Code.Mvc;

namespace Araye.Code.Security
{
    public static class Extensions
    {
        public static bool ActionAuthorized(this HtmlHelper htmlHelper, string actionName, string controllerName)
        {
            ControllerBase controllerBase = string.IsNullOrEmpty(controllerName) ? htmlHelper.ViewContext.Controller : htmlHelper.GetControllerByName(controllerName);
            ControllerContext controllerContext = new ControllerContext(htmlHelper.ViewContext.RequestContext, controllerBase);
            ControllerDescriptor controllerDescriptor = new ReflectedControllerDescriptor(controllerContext.Controller.GetType());
            ActionDescriptor actionDescriptor = controllerDescriptor.FindAction(controllerContext, actionName);

            if (actionDescriptor == null)
                return false;

            FilterInfo filters = new FilterInfo(FilterProviders.Providers.GetFilters(controllerContext, actionDescriptor));

            AuthorizationContext authorizationContext = new AuthorizationContext(controllerContext, actionDescriptor);
            foreach (IAuthorizationFilter authorizationFilter in filters.AuthorizationFilters)
            {
                authorizationFilter.OnAuthorization(authorizationContext);
                if (authorizationContext.Result != null)
                    return false;
            }
            return true;
        }

        public static bool ActionAuthorized(this HtmlHelper htmlHelper, string actionName, string controllerName, RouteData routeData)
        {
            ControllerBase controllerBase = string.IsNullOrEmpty(controllerName) ? htmlHelper.ViewContext.Controller : htmlHelper.GetControllerByName(controllerName);
            ControllerContext controllerContext = new ControllerContext(htmlHelper.ViewContext.HttpContext, routeData, controllerBase);
            ControllerDescriptor controllerDescriptor = new ReflectedControllerDescriptor(controllerContext.Controller.GetType());
            ActionDescriptor actionDescriptor = controllerDescriptor.FindAction(controllerContext, actionName);

            if (actionDescriptor == null)
                return false;

            FilterInfo filters = new FilterInfo(FilterProviders.Providers.GetFilters(controllerContext, actionDescriptor));

            AuthorizationContext authorizationContext = new AuthorizationContext(controllerContext, actionDescriptor);
            foreach (IAuthorizationFilter authorizationFilter in filters.AuthorizationFilters)
            {
                authorizationFilter.OnAuthorization(authorizationContext);
                if (authorizationContext.Result != null)
                    return false;
            }
            return true;
        }


        public static MvcHtmlString ActionLinkAuthorized(this HtmlHelper htmlHelper, string linkText, string actionName, bool showActionLinkAsDisabled = false)
        {
            return htmlHelper.ActionLinkAuthorized(linkText, actionName, null, new RouteValueDictionary(), new RouteValueDictionary(), showActionLinkAsDisabled);
        }

        public static MvcHtmlString ActionLinkAuthorized(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, bool showActionLinkAsDisabled = false)
        {
            return htmlHelper.ActionLinkAuthorized(linkText, actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary(), showActionLinkAsDisabled);
        }

        public static MvcHtmlString ActionLinkAuthorized(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, bool showActionLinkAsDisabled = false)
        {
            return htmlHelper.ActionLinkAuthorized(linkText, actionName, controllerName, new RouteValueDictionary(), new RouteValueDictionary(), showActionLinkAsDisabled);
        }

        public static MvcHtmlString ActionLinkAuthorized(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues, bool showActionLinkAsDisabled = false)
        {
            return htmlHelper.ActionLinkAuthorized(linkText, actionName, null, routeValues, new RouteValueDictionary(), showActionLinkAsDisabled);
        }

        public static MvcHtmlString ActionLinkAuthorized(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, object htmlAttributes, bool showActionLinkAsDisabled = false)
        {
            return htmlHelper.ActionLinkAuthorized(linkText, actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlAttributes), showActionLinkAsDisabled);
        }

        public static MvcHtmlString ActionLinkAuthorized(this HtmlHelper htmlHelper, string linkText, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, bool showActionLinkAsDisabled = false)
        {
            return htmlHelper.ActionLinkAuthorized(linkText, actionName, null, routeValues, htmlAttributes, showActionLinkAsDisabled);
        }

        public static MvcHtmlString ActionLinkAuthorized(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes, bool showActionLinkAsDisabled = false)
        {
            return htmlHelper.ActionLinkAuthorized(linkText, actionName, controllerName, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlAttributes), showActionLinkAsDisabled);
        }


        public static string SHA1(this string data)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
            }
        }

        public static string GetIdentityProperty(this IIdentity identity,string propertyName)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(propertyName);
            return (!string.IsNullOrEmpty(claim.Value)) ? claim.Value : string.Empty;
        }

        /// <summary>
        /// returns user's name and family from custom claims named 'Name' & 'Family'
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string GetNameFamily(this IIdentity identity)
        {
            var nameClaim = ((ClaimsIdentity)identity).FindFirst("Name");
            var familyClaim = ((ClaimsIdentity)identity).FindFirst("Family");
            return (!string.IsNullOrEmpty(nameClaim.Value) && !string.IsNullOrEmpty(nameClaim.Value)) ? $"{nameClaim.Value} {familyClaim.Value}" : identity.Name;
        }

    }
}
