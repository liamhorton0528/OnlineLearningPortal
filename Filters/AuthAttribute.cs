using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineLearningPortal.Filters
{
    public class AuthAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public AuthAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = HttpContext.Current.Session;

            if (session["UserId"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        {"controller", "Login"},
                        { "action", "Index" }
                    }
                );
                return;
            }

            if (_roles.Length > 0)
            {
                string userRole = session["UserRole"] as string;

                if(string.IsNullOrEmpty(userRole) || !_roles.Contains(userRole, StringComparer.OrdinalIgnoreCase))
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary
                        {
                            { "controller", "Home" },
                            { "action", "AccessDenied" }
                        }
                    );
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}