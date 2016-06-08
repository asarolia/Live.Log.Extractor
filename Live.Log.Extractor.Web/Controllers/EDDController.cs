namespace Live.Log.Extractor.Web.Controllers
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Web.Mvc;
    using Live.Log.Extractor.Domain.ServiceHelper;
    using Live.Log.Extractor.Web.Helper.Attributes;
    using Live.Log.Extractor.Web.Helper.Mappers;
    using Live.Log.Extractor.Web.Models;
    using Live.Log.Extractor.Web.Helper;
    using Live.Log.Extractor.Domain;
    using System.IO;
    using Ionic.Zip;

    [AjaxSessionAttribute(SessionKey = sessionKey)]
    public class EDDController : BaseController
    {
        /// <summary>
        /// EDD view model.
        /// </summary>
        EDDViewModel vm;

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            vm = new EDDViewModel();   
            return View(vm);
        }

        /// <summary>
        /// Gets the EDDIO rows from policy number.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <param name="Region">The region.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetEDDIORowsFromPolicyNumber(string Input, string Region)
        {
            this.vm = new EDDViewModel();

            vm.ExceedRegion.CurrentRegion = Region;
            vm.PolicyNumber = Input;
            vm.PolicyID = this.GetPolicyIDFromPolicyNumber();
            if (!string.IsNullOrEmpty(vm.PolicyID))
            {
                List<EDDIORow> eDDIORows = this.CreateEDDIORows();
                if (eDDIORows != null && eDDIORows.Count > 0)
                {
                    var formattedData = new { EDDRows = eDDIORows };
                    return Json(formattedData);
                }
                else
                {
                    return this.DataNotFound("No Data found for this Policy Number.");
                }
            }
            else
            {
                return this.DataNotFound("No Data found for this Policy Number.");
            }
        }

        /// <summary>
        /// Gets the EDDIO rows from polic ID.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <param name="Region">The region.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetEDDIORowsFromPolicID(string Input, string Region)
        {
            this.vm = new EDDViewModel();

            vm.ExceedRegion.CurrentRegion = Region;
            vm.PolicyID = Input;

            this.logDataModel.EddKeys = this.CreateEDDIORows();

            if (this.logDataModel.EddKeys != null && this.logDataModel.EddKeys.Count > 0)
            {
                var formattedData = new { EDDRows = this.logDataModel.EddKeys };
                return Json(formattedData);
            }
            else
            {
                return this.DataNotFound("No Data found for this Policy ID.");
            }
        }

        /// <summary>
        /// Gets the EDD message.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <param name="Region">The region.</param>
        /// <param name="IsPolicyID">if set to <c>true</c> [is policy ID].</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetEDDMessage(string Input, string Region, bool IsPolicyID)
        {
            this.vm = new EDDViewModel();
            if (string.IsNullOrEmpty(Input) || !ModelState.IsValid)
            {
                return View(vm);
            }

            vm.EddKey = Input;
            Mapper.MapEDDKeyToDataModel(vm, this.logDataModel);
            string EDDMessage = this.CreateEDDMessage();
            var formattedData = new { EDDMessage = EDDMessage.Replace("|", "|\n") };
            return Json(formattedData);
        }

        /// <summary>
        /// Downloads the EDD message.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public FileResult DownloadEDDMessage()
        {
            string ImpersonatedUser = ConfigurationManager.AppSettings.Get("User");
            string ImpersonatedUserPassword = ConfigurationManager.AppSettings.Get("Password");
            string ImpersonatedUserDomain = ConfigurationManager.AppSettings.Get("Domain");

            string clientPath = ConfigurationManager.AppSettings.Get("TargetFolder")
                    + HttpContext.Request.UserHostAddress;
            string storagePath = clientPath + "\\EDDMessages";
            string zipTo = storagePath + ".zip";
            string downloadName = this.logDataModel.PolicyId + ".zip";

            using (UserImpersonation user = new UserImpersonation(ImpersonatedUser, ImpersonatedUserDomain, ImpersonatedUserPassword))
            {
                if (user.ImpersonateValidUser())
                {
                    HelperClass.CreateDirectory(clientPath);
                    HelperClass.CreateDirectory(storagePath);

                    foreach (EDDIORow row in this.logDataModel.EddKeys)
                    {
                        this.logDataModel.EddKey = string.Format("{0}-{1}", row.TRANSACTION_TIME, row.IO_CHAR);
                        string EDDMessage = this.CreateEDDMessage().Replace("|", "|\r\n");
                        string filename = ConfigurationManager.AppSettings.Get("TargetFolder")
                            + HttpContext.Request.UserHostAddress
                            + string.Format("\\EDDMessages\\{0} {1}-{2}-{3}.txt", this.logDataModel.PolicyId,
                            row.TRANSACTION_TIME.Replace("/", "-").Replace(":", "-"),
                            row.MSG_ID, string.Equals(row.IO_CHAR, "I") ? "Input" : "Output");

                        using (StreamWriter writer = new StreamWriter(filename, false))
                        {
                            writer.WriteLine(EDDMessage);
                            writer.Close();
                        }
                    }



                    using (ZipFile zip = new ZipFile())
                    {
                        zip.AddDirectory(storagePath);
                        zip.Save(zipTo);
                    }
                }
            }
            return File(zipTo, "application/zip", downloadName);
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetEDDMessageColAndTab()
        {
            this.vm = new EDDViewModel();
            vm.EddKey = this.logDataModel.EddKey;
            this.GetLOBCode();

            if (string.Equals(vm.MasterCompanyNbr, "02"))
            {
                return this.DataNotFound("Unable to find mapping for IB Policy.");
            }
            List<string> spliteEDDMessage = this.CreateEDDMessage().Split('|').ToList();
            this.GetPolarisIds();
            List<DataRow> polarisData = this.GetPolirisData();
            string eddMessageWithTableColumn = string.Empty;

            spliteEDDMessage.ForEach(
                message =>
                {
                    string strEDDMessageRow = string.Format("<span class='eddMessage'>{0}</span>", message);
                    polarisData.ForEach(dr =>
                    {
                        if (message.Contains(dr.Field<string>("POLARIS_ID")))
                        {
                            strEDDMessageRow += string.Format("<span class='eddTabCol'><span class='eddTable'>{0}</span><span class='eddColumn'>{1}</span></span>", dr.Field<string>("TABLE_NAME").Trim(), dr.Field<string>("COLUMN_NAME").Trim());
                        }
                    });
                    eddMessageWithTableColumn += "<span class='eddRow'><span class='eddMTC'>" + strEDDMessageRow + "</span></span>";
                });

                var formattedData = new { EDDMessage = eddMessageWithTableColumn };
                return Json(formattedData);
        }

        /// <summary>
        /// Creates the EDDIO rows.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns>EDD IO Rows</returns>
        private List<EDDIORow> CreateEDDIORows()
        {
            List<EDDIORow> eDDIORow = null;
            vm.SqlQuery = ConfigurationManager.AppSettings["TXAMDETQuery"];
            Mapper.MapEDDInitialDataToDataModel(vm, this.logDataModel);
            GetExceedData.GetEddDetails(this.logDataModel);
            if (this.logDataModel.dataSet.Tables["Formatted_TXAMDET"] != null && this.logDataModel.dataSet.Tables["Formatted_TXAMDET"].AsEnumerable() != null)
            {
                eDDIORow = new List<EDDIORow>();
                var formattedTXAMDET = this.logDataModel.dataSet.Tables["Formatted_TXAMDET"].AsEnumerable();
                var keyData = (from row in formattedTXAMDET select new { TRANSACTION_TIME = row.Field<string>("TRANSACTION_TIME"), MSG_ID = row.Field<string>("MSG_ID"), IO_CHAR = row.Field<string>("IO_CHAR") }).ToList();
                string Message_ID = string.Empty;
                keyData.ForEach(row => {
                    Message_ID = HelperClass.GetExceedTransaction(row.MSG_ID);
                    eDDIORow.Add(new EDDIORow { TRANSACTION_TIME = row.TRANSACTION_TIME, MSG_ID = Message_ID, IO_CHAR = row.IO_CHAR });
                });
            }
            return eDDIORow;
        }

        /// <summary>
        /// Gets the policy ID from policy number.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns>Returns Policy ID.</returns>
        private string GetPolicyIDFromPolicyNumber()
        {
            string PolicyId = string.Empty;
            vm.SqlQuery = ConfigurationManager.AppSettings["POLICYTABQuery"];
            Mapper.MapEDDInitialDataToDataModel(vm, this.logDataModel);
            GetExceedData.GetPolicyTabDetails(this.logDataModel);
            
            if (this.logDataModel.dataSet.Tables["POLICY_TAB"] != null)
            {
                var PolicyTabDetails = this.logDataModel.dataSet.Tables["POLICY_TAB"].AsEnumerable();
                PolicyId = (from row in PolicyTabDetails select row.Field<string>("POLICY_ID")).FirstOrDefault().Trim();
            }
            return PolicyId;
        }

        /// <summary>
        /// Creates the EDD message.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
        private string CreateEDDMessage()
        {
            string[] arrKey = this.logDataModel.EddKey.Split('-');
            var formattedTXAMDET = this.logDataModel.dataSet.Tables["Formatted_TXAMDET"].AsEnumerable();
            string EDDMessage = (from row in formattedTXAMDET
                                 where (string.Equals(row.Field<string>("TRANSACTION_TIME").Trim(), arrKey[0].Trim())
                                 && string.Equals(arrKey[1].Trim(), row.Field<string>("IO_CHAR").Trim()))
                                 select row.Field<string>("MSG_SECTION")).FirstOrDefault().ToString();
            return EDDMessage;
        }

        /// <summary>
        /// Gets the polaris ids.
        /// </summary>
        /// <param name="Message">The message.</param>
        /// <returns></returns>
        private void GetPolarisIds()
        {
            List<string> spliteEDDMessage = this.CreateEDDMessage().Split('|').ToList();
            this.vm.PolarisIds = new List<string>();
            spliteEDDMessage.ForEach(
                message =>
                {
                    List<string> splitEDDRow = message.Split(';').ToList();
                    if (splitEDDRow != null && splitEDDRow.Count > 1)
                    {
                        if (!vm.PolarisIds.Contains(splitEDDRow[1]))
                        {
                            vm.PolarisIds.Add(splitEDDRow[1]);
                        }
                    }
                });
            Mapper.MapEDDPolarisIDsToDataModel(this.vm, this.logDataModel);
        }

        /// <summary>
        /// Gets the LOB code.
        /// </summary>
        public void GetLOBCode()
        {
            string PolicyId=string.Empty;
            if (this.logDataModel.dataSet.Tables["POLICY_TAB"] != null)
            {
                var PolicyTabQuery = this.logDataModel.dataSet.Tables["POLICY_TAB"].AsEnumerable();
                PolicyId = (from row in PolicyTabQuery select row.Field<string>("POLICY_ID")).FirstOrDefault().Trim();
            }
            
            if (this.logDataModel.dataSet.Tables["POLICY_TAB"] == null || !string.Equals(this.logDataModel.PolicyId, PolicyId))
            {
                vm.SqlQuery = ConfigurationManager.AppSettings["LOBCDQuery"];
                Mapper.MapEDDLOBCDQueryToDataModel(vm, this.logDataModel);
                GetExceedData.GetPolicyTabDetails(this.logDataModel);
            }
            
            var PolicyTab = this.logDataModel.dataSet.Tables["POLICY_TAB"].AsEnumerable();
            vm.LOBCD = (from row in PolicyTab select row.Field<string>("LOB_CD")).FirstOrDefault().Trim();
            vm.MasterCompanyNbr = (from row in PolicyTab select row.Field<string>("MASTER_COMPANY_NBR")).FirstOrDefault().Trim();
            Mapper.MapLOBCode(vm, this.logDataModel);
        }

        /// <summary>
        /// Gets the poliris data.
        /// </summary>
        /// <returns></returns>
        public List<DataRow> GetPolirisData()
        {
            vm.SqlQuery = ConfigurationManager.AppSettings["RATEPARMQuery"];
            Mapper.MapEDDRatingQueryToDataModel(vm, this.logDataModel);
            GetExceedData.GetPolarisIds(this.logDataModel);
            List<DataRow> polarisData = null;
            if (this.logDataModel.dataSet.Tables["RATING_PARAMETERS"] != null)
            {
                polarisData = this.logDataModel.dataSet.Tables["RATING_PARAMETERS"].AsEnumerable().ToList();
            }
            return polarisData;
        }
    }
}
