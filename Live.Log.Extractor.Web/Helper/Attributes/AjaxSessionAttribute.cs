namespace Live.Log.Extractor.Web.Helper.Attributes
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Live.Log.Extractor.Domain;

    /// <summary>
    /// Class for Ajax Session Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AjaxSessionAttribute : AuthorizeAttribute
    {
        public string SessionKey { get; set; }

        /// <summary>
        /// Processes HTTP requests that fail authorization.
        /// </summary>
        /// <param name="filterContext">Encapsulates the information for using <see cref="T:System.Web.Mvc.AuthorizeAttribute"/>. The <paramref name="filterContext"/> object contains the controller, HTTP context, request context, action result, and route data.</param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest() && (HttpContext.Current.Session[this.SessionKey] == null ||(HttpContext.Current.Session[this.SessionKey] != null && string.IsNullOrEmpty(((LogDataModel)HttpContext.Current.Session[this.SessionKey]).UserId))))
            {
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        SessionTimeout = "Session Expired, Redirecting to login page."
                    }
                };
            }
            else
            {
                if (HttpContext.Current.Session[this.SessionKey] == null || string.IsNullOrEmpty(((LogDataModel)HttpContext.Current.Session[this.SessionKey]).UserId))
                {
                    if (!filterContext.Controller.TempData.ContainsKey("LoginStatus"))
                    {
                        filterContext.Controller.TempData.Add("LoginStatus", "Session Expired, Redirected to login page.");
                    }
                    filterContext.Result = new RedirectToRouteResult
                        (
                            new RouteValueDictionary 
                            {
                                {"controller", "Login"},
                                {"action", "Index"}
                            }
                        );
                }
            }
        }
    }
}