namespace Live.Log.Extractor.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Web.Mvc;
    using Live.Log.Extractor.Domain.ServiceHelper;
    using Live.Log.Extractor.Web.Helper.Attributes;
    using Live.Log.Extractor.Web.Helper.Mappers;
    using Live.Log.Extractor.Web.Models;

    [AjaxSessionAttribute(SessionKey = sessionKey)]
    public class ExceedController : BaseController
    {
        /// <summary>
        /// Hal Error Detail View Model
        /// </summary>
        HalErrorDetailViewModel vm;

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            vm = new HalErrorDetailViewModel();
            return View(vm);
        }

        /// <summary>
        /// Gets the details.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns>Error details</returns>
        public ActionResult GetTopTenErrorCode(string Region)
        {
            this.vm = new HalErrorDetailViewModel();

            this.vm.ExceedRegion.CurrentRegion = Region;

            List<ErrorCodeCount> errorCodeCount = this.CreateTopTenErrorCode();
            if (errorCodeCount != null && errorCodeCount.Count > 0)
            {
                var formattedData =
                new
                {
                    ErrorCodeCount = errorCodeCount
                };
                return Json(formattedData);
            }
            else
            {
                return this.DataNotFound("Sorry, No data found for you request.");
            }
        }

        /// <summary>
        /// Gets the details.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns>Error details</returns>
        public ActionResult GetDetails(string ErrorCode, string Region)
        {
            this.vm = new HalErrorDetailViewModel();
            
            if (string.IsNullOrEmpty(ErrorCode) || string.IsNullOrEmpty(Region))
            {
                return View(vm);
            }
            this.vm.ErrorCode = ErrorCode;
            this.vm.ExceedRegion.CurrentRegion = Region;

            BasicErrorDetails basicErrorDetails = this.GetBasicErrorDetails();
            if (basicErrorDetails != null && !string.IsNullOrEmpty(basicErrorDetails.LatestReported) && !string.IsNullOrEmpty(basicErrorDetails.EarliestRepoted))
            {
                var formattedData =
                new
                {
                    ErrorCode = vm.ErrorCode,
                    NoOfInstances = basicErrorDetails.NoOfInstances,
                    EarliestRepoted = basicErrorDetails.EarliestRepoted,
                    LatestReported = basicErrorDetails.LatestReported
                };
                return Json(formattedData);
            }
            else
            {
                return this.DataNotFound("Sorry, No data found for you request.");
            }
        }

        /// <summary>
        /// Gets the more details.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMoreDetails(string ErrorCode, string Region)
        {
            this.vm = new HalErrorDetailViewModel();
            this.vm.ErrorCode = ErrorCode;
            this.vm.ExceedRegion.CurrentRegion = Region;

            ErrorDetails errorDetails = this.GetDetailErrorDetails();

            if (errorDetails != null)
            {
                var formattedData =
                new
                {
                    FailedProgrammeName = errorDetails.FailedProgrammeName,
                    FailedParagraphName = errorDetails.FailedParagraphName,
                    ErrorAdditionalText = errorDetails.ErrorAdditionalText,
                    SQLCode = errorDetails.SQLCode,
                    ErrorCmtText = errorDetails.ErrorCmtText,
                    FailedUOWName = errorDetails.FailedUOWName,
                    FailedLocationName = errorDetails.FailedLocationName,
                    SQLErroeMCText = errorDetails.SQLErroeMCText,
                    UserId = errorDetails.UserId,
                    FailedKeyText = errorDetails.FailedKeyText,
                    ACYText = errorDetails.ACYText,
                    PriorityCd = errorDetails.PriorityCd,
                    ErrorText = errorDetails.ErrorText
                };
                return Json(formattedData);
            }
            else
            {
                return this.DataNotFound("Sorry, No data found for you request.");
            }
        }

        /// <summary>
        /// Gets the details.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
        public ActionResult GetStats(string ErrorCode, string Region)
        {
            this.vm = new HalErrorDetailViewModel();
            vm.ExceedRegion.CurrentRegion = Region;
            vm.ErrorCode = ErrorCode;
            
            List<string> Last10Occurances = this.GetLastTenTimeStamps();
            List<MonthCount> monthCount = this.GetSixMonthCount();
            BasicErrorDetails basicErrorDetails = this.GetBasicErrorDetails();
            if(Last10Occurances != null && Last10Occurances.Count > 0 && basicErrorDetails != null)
            {
                var formattedData =
                    new
                    {
                        ErrorCode = vm.ErrorCode,
                        NoOfInstances = basicErrorDetails.NoOfInstances,
                        EarliestRepoted = basicErrorDetails.EarliestRepoted,
                        LatestReported = basicErrorDetails.LatestReported,
                        Last10Occurances = Last10Occurances,
                        LastSix = monthCount
                    };
                return Json(formattedData);
            }
            else
            {
               return this.DataNotFound("Sorry, No data found for you request.");
            }
        }

        /// <summary>
        /// Gets the basic error details.
        /// </summary>
        /// <returns></returns>
        private BasicErrorDetails GetBasicErrorDetails()
        {
            BasicErrorDetails basicErrorDetails = null;
            vm.DetailSqlQuery = ConfigurationManager.AppSettings["HALDTLQuery"];
            Mapper.MapErrorDetailViewModelToDataModel(this.vm, this.logDataModel);
            GetExceedData.GetErrorDetails(this.logDataModel);
            if (this.logDataModel.dataSet.Tables["HAL_ERR_LOG_DTL"] != null)
            {
                var HalErrorLogTable = this.logDataModel.dataSet.Tables["HAL_ERR_LOG_DTL"].AsEnumerable();
                basicErrorDetails = new BasicErrorDetails();
                basicErrorDetails.NoOfInstances = (from row in HalErrorLogTable select Int32.Parse(row.Field<string>("COUNTER"))).FirstOrDefault();
                basicErrorDetails.EarliestRepoted = (from row in HalErrorLogTable select row.Field<string>("MIN_FAIL_TS")).FirstOrDefault();
                basicErrorDetails.LatestReported = (from row in HalErrorLogTable select row.Field<string>("MAX_FAIL_TS")).FirstOrDefault();
            }
            return basicErrorDetails;
        }

        /// <summary>
        /// Gets the basic error details.
        /// </summary>
        /// <returns></returns>
        private List<ErrorCodeCount> CreateTopTenErrorCode()
        {
            List<ErrorCodeCount> errorCodeCount = null;
            vm.DetailSqlQuery = ConfigurationManager.AppSettings["TopTenErrorCodeQuery"];
            Mapper.MapTopTenErrorCodeQueryToDataModel(this.vm, this.logDataModel);
            GetExceedData.GetErrorDetails(this.logDataModel);
            if (this.logDataModel.dataSet.Tables["HAL_ERR_LOG_DTL"] != null)
            {
                var HalErrorLogTable = this.logDataModel.dataSet.Tables["HAL_ERR_LOG_DTL"].AsEnumerable();
                var errorCount = (from row in HalErrorLogTable select new { errCount = Int32.Parse(row.Field<string>("COUNTER")), errorCode = row.Field<string>("ERRORCODE") }).ToList();
                errorCodeCount = new List<ErrorCodeCount>();
                errorCount.ForEach(error => errorCodeCount.Add(new ErrorCodeCount { ErrorCode = error.errorCode, Count = error.errCount }));
            }
            return errorCodeCount;
        }

        /// <summary>
        /// Details the error details.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
        private ErrorDetails GetDetailErrorDetails()
        {
            vm.DetailSqlQuery = ConfigurationManager.AppSettings["HALFAILQuery"];
            Mapper.MapErrorDetailViewModelToDataModel(this.vm, this.logDataModel);
            GetExceedData.GetErrorFailed(this.logDataModel);
            ErrorDetails errorDetails = null;
            if (this.logDataModel.dataSet.Tables["HAL_ERR_LOG_FAIL"] != null)
            {
                var HalErrorLogFailTable = this.logDataModel.dataSet.Tables["HAL_ERR_LOG_FAIL"].AsEnumerable();
                errorDetails = new ErrorDetails();
                errorDetails.FailedProgrammeName = (from row in HalErrorLogFailTable select row.Field<string>("HELF_FAIL_PGM_NM")).FirstOrDefault();
                errorDetails.FailedParagraphName = (from row in HalErrorLogFailTable select row.Field<string>("HELF_FAIL_PARA_NM")).FirstOrDefault();
                errorDetails.ErrorAdditionalText = (from row in HalErrorLogFailTable select row.Field<string>("HELF_ERR_ADD_TXT")).FirstOrDefault();
                errorDetails.SQLCode = (from row in HalErrorLogFailTable select row.Field<string>("SQL_CODE")).FirstOrDefault();
                errorDetails.ErrorCmtText = (from row in HalErrorLogFailTable select row.Field<string>("HELF_ERR_CMT_TXT")).FirstOrDefault();
                errorDetails.FailedUOWName = (from row in HalErrorLogFailTable select row.Field<string>("HELF_FAIL_UOW_NM")).FirstOrDefault();
                errorDetails.FailedLocationName = (from row in HalErrorLogFailTable select row.Field<string>("HELF_FAIL_LOC_NM")).FirstOrDefault();
                errorDetails.SQLErroeMCText = (from row in HalErrorLogFailTable select row.Field<string>("HELF_SQLERRMC_TXT")).FirstOrDefault();
                errorDetails.UserId = (from row in HalErrorLogFailTable select row.Field<string>("USERID")).FirstOrDefault();
                errorDetails.FailedKeyText = (from row in HalErrorLogFailTable select row.Field<string>("HELF_FAIL_KEY_TXT")).FirstOrDefault();
                errorDetails.ACYText = (from row in HalErrorLogFailTable select row.Field<string>("HELF_ACY_TXT")).FirstOrDefault();
                errorDetails.PriorityCd = (from row in HalErrorLogFailTable select row.Field<string>("HELF_PRIORITY_CD")).FirstOrDefault();
                errorDetails.ErrorText = (from row in HalErrorLogFailTable select row.Field<string>("HELF_ERR_TXT")).FirstOrDefault();
            }
            return errorDetails;
        }

        /// <summary>
        /// Gets the last ten time stamps.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
        private List<string> GetLastTenTimeStamps()
        {
            List<string> Last10Occurances = null;
            vm.DetailSqlQuery = ConfigurationManager.AppSettings["LastTenQuery"];

            Mapper.MapErrorDetailViewModelToDataModel(this.vm, this.logDataModel);
            GetExceedData.GetErrorDetails(this.logDataModel);
            if (this.logDataModel.dataSet.Tables["HAL_ERR_LOG_DTL"] != null)
            {
                Last10Occurances = new List<string>();
                var HalErrorLogTable = this.logDataModel.dataSet.Tables["HAL_ERR_LOG_DTL"].AsEnumerable();
                List<DateTime> dateTime = (from row in HalErrorLogTable select convertToDateTime(row.Field<string>("FAIL_TS"))).ToList();
                dateTime.ForEach(dt => Last10Occurances.Add(dt.ToString()));
            }
            return Last10Occurances;
        }

        /// <summary>
        /// Gets the six month count.
        /// </summary>
        /// <param name="vm">The vm.</param>
        /// <returns></returns>
        private List<MonthCount> GetSixMonthCount()
        { 
            List<MonthCount> monthCount = new List<MonthCount>();
            vm.DetailSqlQuery = ConfigurationManager.AppSettings["ViewStatQuery"];
            Mapper.MapViewStatToDataModel(this.vm, this.logDataModel);
            GetExceedData.GetErrorDetails(this.logDataModel);
            if (this.logDataModel.dataSet.Tables["HAL_ERR_LOG_DTL"] != null)
            {
                var HalErrorLogTable = this.logDataModel.dataSet.Tables["HAL_ERR_LOG_DTL"].AsEnumerable();
                var perMonthCount = (from row in HalErrorLogTable select new { errCount = Int32.Parse(row.Field<string>("COUNTER")), Month = row.Field<string>("MONTH") }).ToList();
                for (int i = 0; i >= -5; i--)
                {
                    string month = DateTime.Now.AddMonths(i).ToString("MMMM");
                    int errorCount = perMonthCount.Find(x => string.Equals(x.Month, month)).errCount;
                    monthCount.Add(new MonthCount
                    {
                        Count = errorCount,
                        Month = month
                    });
                }
            }
            return monthCount;
        }
    }
}
