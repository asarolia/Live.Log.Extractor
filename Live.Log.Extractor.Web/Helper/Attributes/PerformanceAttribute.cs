namespace Live.Log.Extractor.Web.Helper.Attributes
{
    using System;
    using System.Diagnostics;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class PerformanceMonitorAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The stop watch timer
        /// </summary>
        private readonly Stopwatch stopwatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceMonitorAttribute"/> class.
        /// </summary>
        public PerformanceMonitorAttribute()
        {
            this.stopwatch = new Stopwatch();
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.stopwatch.Restart();
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework after the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
                this.stopwatch.Stop();

                filterContext.Controller.ViewBag.PerformanceMonitor = stopwatch.Elapsed;         
        }
    }
}