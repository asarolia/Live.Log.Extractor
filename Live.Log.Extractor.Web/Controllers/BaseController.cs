namespace Live.Log.Extractor.Web.Controllers
{
    using System;
    using System.Globalization;
    using System.Web.Mvc;
    using Live.Log.Extractor.Domain;

    public class BaseController: Controller
    {
        /// <summary>
        /// Session Key
        /// </summary>
        public const string sessionKey = "LogDataModel";

        /// <summary>
        /// Function to conver string to datetime.
        /// </summary>
        public Func<string, DateTime> convertToDateTime = x => Convert.ToDateTime(x, CultureInfo.CreateSpecificCulture("en-GB"));

        /// <summary>
        /// Log Data Model
        /// </summary>
        public LogDataModel logDataModel = new LogDataModel();

        /// <summary>
        /// Datas the not found.
        /// </summary>
        /// <returns></returns>
        protected JsonResult DataNotFound(string Message)
        {
            var formattedData = new { dataNotFound = Message };
            return Json(formattedData);
        }

        /// <summary>
        /// Called before the action method is invoked.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            this.logDataModel = Session[sessionKey] != null ? (LogDataModel)Session[sessionKey] : this.logDataModel;
        }

        /// <summary>
        /// Called before the action result that is returned by an action method is executed.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action result</param>
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
            Session[sessionKey] = this.logDataModel;
        }

        /// <summary>
        /// Called when an unhandled exception occurs in the action.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnException(ExceptionContext filterContext)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(filterContext.Exception);
            base.OnException(filterContext);
        }
    }
}
