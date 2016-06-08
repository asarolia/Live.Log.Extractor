namespace Live.Log.Extractor.Web.Controllers
{
    using System.Web.Mvc;
    using Live.Log.Extractor.Web.Helper.Mappers;
    using Live.Log.Extractor.Web.Models;

    public class CartController : BaseController
    {
        /// <summary>
        /// Summaries this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Summary()
        {
            CartViewModel vm = new CartViewModel();
            Mapper.MapDataModelToCartViewModel(logDataModel, vm);
            return View(vm);
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Login");
        }

    }
}
