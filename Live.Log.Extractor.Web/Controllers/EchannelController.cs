namespace Live.Log.Extractor.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web.Mvc;
    using Live.Log.Extractor.Domain;
    using Live.Log.Extractor.Web.Helper;
    using Live.Log.Extractor.Web.Helper.Attributes;
    using Live.Log.Extractor.Web.Models;
    using Live.Log.Extractor.IndexerService;

    /// <summary>
    /// MainController class
    /// </summary>
    public class EchannelController : BaseController
    {
        /// <summary>
        /// ISearch Engine instance.
        /// </summary>
        private SearchEngine engine;

        /// <summary>
        /// Gets the engine.
        /// </summary>
        private SearchEngine Engine
        {
            get
            {
                if (engine == null)
                {
                    engine = new SearchEngine();
                }

                return engine;
            }
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Message = "Search Production Logs";
            IndexModel model = new IndexModel
            {
                SearchStartText = ConfigurationManager.AppSettings.Get("SearchStartText"),
                SearchEndText = ConfigurationManager.AppSettings.Get("SearchEndText"),
                IsProduction = true,
                Sequential = false,
                Date = DateTime.Today.AddDays(-1).ToShortDateString(),
                ResultMessage=string.Empty,
                ConnectionId = Guid.NewGuid().ToString() 
            };
           
            return View(model);
        }

        /// <summary>
        /// Indexes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        [PerformanceMonitor]
        public ActionResult Index_Search(IndexModel model, string Find, string Stop)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(Find))
                {
                    IndexModel.Cancel = new CancellationTokenSource();
                    IndexModel.CancelToken = IndexModel.Cancel.Token;
                    IndexModel.ErrorOrAbort = false;
                    model.EnableLogging = bool.Parse(ConfigurationManager.AppSettings.Get("Enablelogging"));
                    model.ImpersonatedUser = ConfigurationManager.AppSettings.Get("User");
                    model.ImpersonatedUserPassword = ConfigurationManager.AppSettings.Get("Password");
                    model.ImpersonatedUserDomain = ConfigurationManager.AppSettings.Get("Domain");
                    model.SearchLocation = ConfigurationManager.AppSettings.Get("ProductionLogsFolder");
                    List<string> productionServers = ConfigurationManager.AppSettings.Get("ProductionServers"+model.AppName).Split(new char[] { ',' }, StringSplitOptions.None).ToList<string>();
                    List<IndexerService.IndexInformation> files = new List<IndexInformation>();
                    model.ClientIP = HttpContext.Request.UserHostAddress;
                    string targetDirectory = ConfigurationManager.AppSettings.Get("TargetFolder");
                    List<string> productionLogFiles;
                    
                    if (model.IsProduction)
                    {
                        using (UserImpersonation user = new UserImpersonation(model.ImpersonatedUser, model.ImpersonatedUserDomain, model.ImpersonatedUserPassword))
                            {
                                if (user.ImpersonateValidUser())
                                {
                                    try
                                    {
                                        ServiceClient client = new ServiceClient();
                                        Engine.BrodCast(model.ConnectionId, "Creating Index.");
                                        if (client.CreateIndexFiles(model.AppName.GetEnumForValue<ProductType>()))
                                        {
                                            Engine.BrodCast(model.ConnectionId, "Search Started.");
                                            foreach (IndexerService.IndexInformation indexInformation in client.SearchIndex(model.AppName.GetEnumForValue<ProductType>(), model.SearchText))
                                            {
                                                files.Add(indexInformation);
                                            }
                                        }

                                        Engine.BrodCast(model.ConnectionId, string.Format("Found in server files: {0}",files.Count));

                                        if (files.Count > 0)
                                        {
                                            productionLogFiles = Engine.GetFileList(files, model);
                                            Engine.SearchInProductionServersSequentially(model, productionLogFiles);
                                        }
                                        else
                                        {
                                            model.ResultMessage = "No result found for this request.";
                                            //Engine.BrodCast(model.ConnectionId, model.ProcessCompleteMessage);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (model.EnableLogging)
                                        {
                                            Engine.BrodCast(model.ConnectionId, ex.Message);
                                        }
                                        IndexModel.ErrorOrAbort = true;
                                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                                        model.ResultMessage = "Bad bad Server....Error Occurred, Search Aborted";
                                    }
                                    finally
                                    {
                                        if (IndexModel.CancelToken.IsCancellationRequested)
                                        {
                                            Engine.BrodCast(model.ConnectionId, "Search Aborted");
                                        }
                                        Engine.WriteToFile(model.Results, model);
                                    }
                                }
                        }
                    }
                }
                else
                {
                    IndexModel.Cancel.Cancel();
                    IndexModel.ErrorOrAbort = true;
                    model.ResultMessage = "Search Aborted";
                    Engine.BrodCast(model.ConnectionId, model.ProcessCompleteMessage);
                }
            }
            Engine.BrodCast(model.ConnectionId, model.ProcessCompleteMessage);
            if (model.Results != null && model.Results.Count > 0)
            {
                this.logDataModel.EchannelLogs = model.Results;
            }
            return PartialView("Results", model);
        }

        /// <summary>
        /// Gets the request response.
        /// </summary>
        /// <param name="RequestName">Name of the request.</param>
        /// <param name="TimeStamp">The time stamp.</param>
        /// <param name="Session">The session.</param>
        /// <returns></returns>
        public ActionResult GetRequestResponse(string RequestName, string LogDate, string TimeStamp, string SessionKey)
        {
            if (this.logDataModel.EchannelLogs != null && this.logDataModel.EchannelLogs.Count > 0)
            {
                EchannelLog eChannelLog = this.logDataModel.EchannelLogs.FirstOrDefault(x => string.Equals(x.ServiceName, RequestName.Trim()) && string.Equals(x.LogDate.ToShortDateString(), LogDate.Trim()) && string.Equals(x.TimeStamp.ToString(), TimeStamp.Trim()) && string.Equals(x.SessionID, SessionKey.Trim()));
                
                if (eChannelLog != null && Request.IsAjaxRequest())
                {
                    string pattern = ">\\s*<";
                    string replacement = ">\n<";
                    Regex rgx = new Regex(pattern);

                    var formattedData =
                        new
                        {
                            RqMessage = rgx.Replace(eChannelLog.Request, replacement),
                            RsMessage = rgx.Replace(eChannelLog.Response, replacement)
                        };
                    return Json(formattedData);
                }
            }
            return null;
        }

        /// <summary>
        /// Annuals the report.
        /// </summary>
        /// <returns></returns>
        public FileResult Download()
        {
            string filename = ConfigurationManager.AppSettings.Get("TargetFolder") + HttpContext.Request.UserHostAddress + @"\result.txt";
            string contentType = "plain/txt";
            string downloadName = "Result.txt";
            return File(filename, contentType, downloadName);
        }

        /// <summary>
        /// Deletes the extra files.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="productionServers">The production servers.</param>
        private void DeleteExtraFiles(IndexModel model, List<string> productionServers)
        {
            System.Diagnostics.Stopwatch Watch = new System.Diagnostics.Stopwatch();
            Watch.Start();
            string targetDirectory = ConfigurationManager.AppSettings.Get("TargetFolder")+ @"\"+ model.ClientIP ;
            
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            
            List<string> files = Directory.GetFiles(targetDirectory).ToList();
            List<string> requiredFiles = new List<string>();
            foreach (string server in productionServers)
            {
                string decompressedFileName = ConfigurationManager.AppSettings.Get(model.AppName) + HelperClass.ParseToFileName(model.Date) + ".txt";
                string decompressedTargetFile = targetDirectory + server + decompressedFileName;
                requiredFiles.Add(decompressedTargetFile);
            }

            foreach (string file in files)
            {
                if (!requiredFiles.Contains(file))
                {
                    System.IO.File.Delete(file);
                }
            }
            Watch.Stop();
            System.Diagnostics.Trace.Write(Watch.ElapsedMilliseconds);
            return;
        }
       
    }
}
