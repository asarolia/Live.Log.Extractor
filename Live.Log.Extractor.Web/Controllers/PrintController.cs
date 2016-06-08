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
    using System.Xml.Linq;
    using System.Xml;

    [AjaxSessionAttribute(SessionKey = sessionKey)]
    public class PrintController : BaseController
    {
        /// <summary>
        /// Print view model.
        /// </summary>
        PrintViewModel vm;

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            vm = new PrintViewModel();
            return View(vm);
        }

        /// <summary>
        /// Gets the PrintIO rows from policy number.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <param name="Region">The region.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPrintIORowsFromPolicyNumber(string Input, string Region)
        {
            this.vm = new PrintViewModel();

            vm.ExceedRegion.CurrentRegion = Region;
            vm.PolicyNumber = Input;
            vm.PolicyID = this.GetPolicyIDFromPolicyNumber();
            if (!string.IsNullOrEmpty(vm.PolicyID))
            {
                List<PrintIORow> printIORow = this.CreatePrintIORows();
                if (printIORow != null && printIORow.Count > 0)
                {
                    var formattedData = new { PrintRows = printIORow };
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
        /// Gets the PrintIO rows from polic ID.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <param name="Region">The region.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPrintIORowsFromPolicID(string Input, string Region)
        {
            this.vm = new PrintViewModel();

            vm.ExceedRegion.CurrentRegion = Region;
            vm.PolicyID = Input;

            this.logDataModel.PrintKeys  = this.CreatePrintIORows();

            if (this.logDataModel.PrintKeys != null && this.logDataModel.PrintKeys.Count > 0)
            {
                var formattedData = new { PrintRows = this.logDataModel.PrintKeys };
                return Json(formattedData);
            }
            else
            {
                return this.DataNotFound("No Data found for this Policy ID.");
            }
        }

        /// <summary>
        /// Gets the PRINT message.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <param name="Region">The region.</param>
        /// <param name="IsPolicyID">if set to <c>true</c> [is policy ID].</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPrintMessage(string Input, string Region, bool IsPolicyID)
        {
            this.vm = new PrintViewModel();
            if (string.IsNullOrEmpty(Input) || !ModelState.IsValid)
            {
                return View(vm);
            }

            vm.EddKey = Input;
            Mapper.MapPrintKeyToDataModel(vm, this.logDataModel);
            string PrintMessage = this.CreatePrintMessage();
            //var formattedData = new { PrintMessage = PrintMessage.Replace("|", "|\n") };                        
            //var formattedData = new { PrintMessage = XElement.Parse(PrintMessage).ToString() };
            try
            {
                var formattedData = new { PrintMessage = XDocument.Parse(PrintMessage).ToString() };
                return Json(formattedData);            
            }
            catch (System.Xml.XmlException ex)
            {
                var formattedData = new { PrintMessage = PrintMessage.Replace("|", "|\n") };
                return Json(formattedData);                    
            }
            //return Json(formattedData);            
                                    
        }

        /// <summary>
        /// Downloads the EDD message.
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //public FileResult DownloadEDDMessage()
        //{
        //    string ImpersonatedUser = ConfigurationManager.AppSettings.Get("User");
        //    string ImpersonatedUserPassword = ConfigurationManager.AppSettings.Get("Password");
        //    string ImpersonatedUserDomain = ConfigurationManager.AppSettings.Get("Domain");

        //    string clientPath = ConfigurationManager.AppSettings.Get("TargetFolder")
        //            + HttpContext.Request.UserHostAddress;
        //    string storagePath = clientPath + "\\EDDMessages";
        //    string zipTo = storagePath + ".zip";
        //    string downloadName = this.logDataModel.PolicyId + ".zip";

        //    using (UserImpersonation user = new UserImpersonation(ImpersonatedUser, ImpersonatedUserDomain, ImpersonatedUserPassword))
        //    {
        //        if (user.ImpersonateValidUser())
        //        {
        //            HelperClass.CreateDirectory(clientPath);
        //            HelperClass.CreateDirectory(storagePath);

        //            foreach (PrintIORow row in this.logDataModel.EddKeys)
        //            {
        //                this.logDataModel.EddKey = string.Format("{0}-{1}", row.TRANSACTION_TIME, row.IO_CHAR);
        //                string EDDMessage = this.CreateEDDMessage().Replace("|", "|\r\n");
        //                string filename = ConfigurationManager.AppSettings.Get("TargetFolder")
        //                    + HttpContext.Request.UserHostAddress
        //                    + string.Format("\\EDDMessages\\{0} {1}-{2}-{3}.txt", this.logDataModel.PolicyId,
        //                    row.TRANSACTION_TIME.Replace("/", "-").Replace(":", "-"),
        //                    row.MSG_ID, string.Equals(row.IO_CHAR, "I") ? "Input" : "Output");

        //                using (StreamWriter writer = new StreamWriter(filename, false))
        //                {
        //                    writer.WriteLine(EDDMessage);
        //                    writer.Close();
        //                }
        //            }



        //            using (ZipFile zip = new ZipFile())
        //            {
        //                zip.AddDirectory(storagePath);
        //                zip.Save(zipTo);
        //            }
        //        }
        //    }
        //    return File(zipTo, "application/zip", downloadName);
        //}

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPrintMessageColAndTab()
        {
            this.vm = new PrintViewModel();
            vm.EddKey = this.logDataModel.EddKey;
            this.GetLOBCode();

            if (string.Equals(vm.MasterCompanyNbr, "02"))
            {
                return this.DataNotFound("Unable to find mapping for IB Policy.");
            }
            List<string> splitePrintMessage = this.CreatePrintMessage().Split('|').ToList();
            this.GetPolarisIds();
            List<DataRow> polarisData = this.GetPolirisData();
            string printMessageWithTableColumn = string.Empty;

            splitePrintMessage.ForEach(
                message =>
                {
                    string strPrintMessageRow = string.Format("<span class='printMessage'>{0}</span>", message);
                    polarisData.ForEach(dr =>
                    {
                        if (message.Contains(dr.Field<string>("POLARIS_ID")))
                        {
                            strPrintMessageRow += string.Format("<span class='eddTabCol'><span class='eddTable'>{0}</span><span class='eddColumn'>{1}</span></span>", dr.Field<string>("TABLE_NAME").Trim(), dr.Field<string>("COLUMN_NAME").Trim());
                        }
                    });
                    printMessageWithTableColumn += "<span class='eddRow'><span class='eddMTC'>" + strPrintMessageRow + "</span></span>";
                });

            var formattedData = new { PrintMessage = printMessageWithTableColumn };
            return Json(formattedData);
        }

        /// <summary>
        /// Creates the PrintIO rows.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns>Print IO Rows</returns>
        private List<PrintIORow> CreatePrintIORows()
        {
            List<PrintIORow> printIORow = null;
            //vm.SqlQuery = ConfigurationManager.AppSettings["TXAMDETQuery"];
            vm.SqlQuery = ConfigurationManager.AppSettings["PrintQuery"];
            Mapper.MapPrintInitialDataToDataModel(vm, this.logDataModel);
            GetExceedData.GetDetailsPrint(this.logDataModel);
            if (this.logDataModel.dataSet.Tables["Formatted_POL_BIL_MQ_AUDIT_V"] != null && this.logDataModel.dataSet.Tables["Formatted_POL_BIL_MQ_AUDIT_V"].AsEnumerable() != null)
            {
                //printIORow = new List<PrintIORow>();
                //var formattedPOL_BIL_MQ_AUDIT_V = this.logDataModel.dataSet.Tables["Formatted_POL_BIL_MQ_AUDIT_V"].AsEnumerable();
                //var keyData = (from row in formattedPOL_BIL_MQ_AUDIT_V select new { MSG_TS = row.Field<string>("MSG_TS"), MSG_ID = row.Field<string>("MSG_ID"), TRS_ID = row.Field<string>("TRS_ID") }).ToList();
                //string Message_ID = string.Empty;
                //keyData.ForEach(row =>
                //{
                //    Message_ID = HelperClass.GetExceedTransaction(row.MSG_ID);
                //    PrintIORow.Add(new PrintIORow { MSG_TS = row.MSG_TS, MSG_ID = Message_ID, TRS_ID = row.TRS_ID });
                //});

                printIORow = new List<PrintIORow>();
                var formattedPOL_BIL_MQ_AUDIT_V = this.logDataModel.dataSet.Tables["Formatted_POL_BIL_MQ_AUDIT_V"].AsEnumerable();
                var keyData = (from row in formattedPOL_BIL_MQ_AUDIT_V select new { MSG_TS = row.Field<string>("MSG_TS") }).ToList();
                //string Message_ID = string.Empty;
                keyData.ForEach(row =>
                {
                   // Message_ID = HelperClass.GetExceedTransaction(row.MSG_ID);
                    printIORow.Add(new PrintIORow { MSG_TS = row.MSG_TS });
                });
            }
            return printIORow;


            //List<PrintIORow> printIORow = null;
            //vm.SqlQuery = ConfigurationManager.AppSettings["TXAMDETQuery"];
            //Mapper.MapEDDInitialDataToDataModel(vm, this.logDataModel);
            //GetExceedData.GetEddDetails(this.logDataModel);
            //if (this.logDataModel.dataSet.Tables["Formatted_TXAMDET"] != null && this.logDataModel.dataSet.Tables["Formatted_TXAMDET"].AsEnumerable() != null)
            //{
            //    printIORow = new List<EDDIORow>();
            //    var formattedTXAMDET = this.logDataModel.dataSet.Tables["Formatted_TXAMDET"].AsEnumerable();
            //    var keyData = (from row in formattedTXAMDET select new { TRANSACTION_TIME = row.Field<string>("TRANSACTION_TIME"), MSG_ID = row.Field<string>("MSG_ID"), IO_CHAR = row.Field<string>("IO_CHAR") }).ToList();
            //    string Message_ID = string.Empty;
            //    keyData.ForEach(row =>
            //    {
            //        Message_ID = HelperClass.GetExceedTransaction(row.MSG_ID);
            //        eDDIORow.Add(new EDDIORow { TRANSACTION_TIME = row.TRANSACTION_TIME, MSG_ID = Message_ID, IO_CHAR = row.IO_CHAR });
            //    });
            //}
            //return eDDIORow;
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
            Mapper.MapPrintInitialDataToDataModel(vm, this.logDataModel);
            GetExceedData.GetPolicyTabDetails(this.logDataModel);

            if (this.logDataModel.dataSet.Tables["POLICY_TAB"] != null)
            {
                var PolicyTabDetails = this.logDataModel.dataSet.Tables["POLICY_TAB"].AsEnumerable();
                PolicyId = (from row in PolicyTabDetails select row.Field<string>("POLICY_ID")).FirstOrDefault().Trim();
            }
            return PolicyId;
        }

        /// <summary>
        /// Creates the Print message.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
        private string CreatePrintMessage()
        {
            string[] arrKey = this.logDataModel.EddKey.Split('-');
            var formattedPOL_BIL_MQ_AUDIT_V = this.logDataModel.dataSet.Tables["Formatted_POL_BIL_MQ_AUDIT_V"].AsEnumerable();
            //string EDDMessage = (from row in formattedPOL_BIL_MQ_AUDIT_V
            //                     where (string.Equals(row.Field<string>("MSG_TS").Trim(), arrKey[0].Trim())
            //                     && string.Equals(arrKey[1].Trim(), row.Field<string>("TRS_ID").Trim()))
            //                     select row.Field<string>("MSG_REPORT")).FirstOrDefault().ToString();

            string PrintMessage = (from row in formattedPOL_BIL_MQ_AUDIT_V
                                 where (string.Equals(row.Field<string>("MSG_TS").Trim(), arrKey[0].Trim())
                                 )
                                 select row.Field<string>("MSG_REPORT")).FirstOrDefault().ToString();
            return PrintMessage;

            //string[] arrKey = this.logDataModel.EddKey.Split('-');
            //var formattedTXAMDET = this.logDataModel.dataSet.Tables["Formatted_TXAMDET"].AsEnumerable();
            //string EDDMessage = (from row in formattedTXAMDET
            //                     where (string.Equals(row.Field<string>("TRANSACTION_TIME").Trim(), arrKey[0].Trim())
            //                     && string.Equals(arrKey[1].Trim(), row.Field<string>("IO_CHAR").Trim()))
            //                     select row.Field<string>("MSG_SECTION")).FirstOrDefault().ToString();
            //return EDDMessage;
        }

        /// <summary>
        /// Gets the polaris ids.
        /// </summary>
        /// <param name="Message">The message.</param>
        /// <returns></returns>
        private void GetPolarisIds()
        {
            List<string> splitePrintMessage = this.CreatePrintMessage().Split('|').ToList();
            this.vm.PolarisIds = new List<string>();
            splitePrintMessage.ForEach(
                message =>
                {
                    List<string> splitPrintRow = message.Split(';').ToList();
                    if (splitPrintRow != null && splitPrintRow.Count > 1)
                    {
                        if (!vm.PolarisIds.Contains(splitPrintRow[1]))
                        {
                            vm.PolarisIds.Add(splitPrintRow[1]);
                        }
                    }
                });
            Mapper.MapPrintPolarisIDsToDataModel(this.vm, this.logDataModel);
        }

        /// <summary>
        /// Gets the LOB code.
        /// </summary>
        public void GetLOBCode()
        {
            string PolicyId = string.Empty;
            if (this.logDataModel.dataSet.Tables["POLICY_TAB"] != null)
            {
                var PolicyTabQuery = this.logDataModel.dataSet.Tables["POLICY_TAB"].AsEnumerable();
                PolicyId = (from row in PolicyTabQuery select row.Field<string>("POLICY_ID")).FirstOrDefault().Trim();
            }

            if (this.logDataModel.dataSet.Tables["POLICY_TAB"] == null || !string.Equals(this.logDataModel.PolicyId, PolicyId))
            {
                vm.SqlQuery = ConfigurationManager.AppSettings["LOBCDQuery"];
                Mapper.MapPrintLOBCDQueryToDataModel(vm, this.logDataModel);
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
            Mapper.MapPrintRatingQueryToDataModel(vm, this.logDataModel);
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
