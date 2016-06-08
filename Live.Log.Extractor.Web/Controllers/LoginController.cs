.namespace Live.Log.Extractor.Web.Controllers
{
    using System.Web.Mvc;
    using Live.Log.Extractor.Domain.ServiceHelper;
    using Live.Log.Extractor.Web.Helper.Attributes;
    using Live.Log.Extractor.Web.Helper.Mappers;
    using Live.Log.Extractor.Web.Models;

    public class LoginController : BaseController
    {
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            LoginDetailsViewModel vm = new LoginDetailsViewModel();
            this.logDataModel = null;
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(LoginDetailsViewModel vm)
        {
            Mapper.MapLoginDetaisToDataModel(vm, logDataModel);
            
            if (!GetExceedData.VerifyLogin(this.logDataModel))
            {
                TempData.Add("LoginStatus", "Login Failed");
                this.logDataModel = null;
                return View(vm);
            }
            
            return RedirectToAction("Welcome");
        }

        [AjaxSessionAttribute(SessionKey = sessionKey)]
        [HttpGet]        
        public ActionResult Welcome()
        {
            return View();
        }
    }
}
