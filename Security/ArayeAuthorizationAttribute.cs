using System;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Araye.Code.Security
{
    public class ArayeAuthorizationAttribute : FilterAttribute, IAuthorizationFilter 
    {
        void IAuthorizationFilter.OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof (AllowAnonymousAttribute), false).Any())
                return;
            if (HttpContext.Current.User != null && HttpContext.Current.User.IsInRole(Consts.WebAdminRoleName)) 
                return;
            var atts = filterContext.ActionDescriptor.GetCustomAttributes(false).Where(ac => ac.GetType() == typeof(ArayeActionPermissionAttribute)).Select(a => (ArayeActionPermissionAttribute)a);
            var permission = false;
            foreach (var att in atts)
            {
                permission = HasPermission(att.ActionId.ToLower().Trim());
                if (permission)
                    break;
            }

            if (!permission && atts.Any())

            {
                filterContext.Result = new HttpUnauthorizedResult("You don't have permission");
            }
        }

        private static bool HasPermission(string actionId)
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            var accessClaim = identity.FindFirst(Consts.PermissionAccessClaimTitle);
            if (accessClaim == null || accessClaim.Value.Length <= 0) return false;
            return accessClaim.Value.Split(',').Contains(actionId);
        }
    }


}
