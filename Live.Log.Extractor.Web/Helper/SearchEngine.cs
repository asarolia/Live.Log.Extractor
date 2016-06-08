namespace Live.Log.Extractor.Web.Helper
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using Live.Log.Extractor.Domain;
    using Live.Log.Extractor.Web.Models;
    using SignalR;
    using SignalR.Hosting.AspNet;
    using SignalR.Hubs;
    using SignalR.Infrastructure;
    using Ionic.Zip;

    /// <summary>
    /// Search Engine class
    /// </summary>
    public class SearchEngine : ISearchEngine
    {

        /// <summary>
        /// Begin text.
        /// </summary>
        private const string serviveText = "-----Begin of ";

        /// <summary>
        /// Time Stamp Text.
        /// </summary>
        private const string timeStampText = " - Transaction logged at ";

        /// <summary>
        /// Session Text.
        /// </summary>
        private const string sessionText = "-----  for session ";

        /// <summary>
        /// Searches Log Sequentially
        /// </summary>
        /// <param name="model"></param>
        /// <param name="productionServers"></param>
        public void SearchInProductionServersSequentially(IndexModel model, List<string> productionLogFiles)
        {
            string result = string.Empty;
            Stopwatch watch = new Stopwatch();

            // Delete Existing files in the directory
            string targetDirectory = ConfigurationManager.AppSettings.Get("TargetFolder") + model.ClientIP + @"\";
            CreateDirectory(targetDirectory);
            watch.Restart();

            productionLogFiles.ForEach(targetFile =>
            {
                List<EchannelLog> logs = SearchInFile(DecompressFile(CopyFiles(targetFile, model), model), model);
                if (logs.Count > 0)
                {
                    logs.ForEach(log => model.Results.Add(log));
                }
            });

            if (model.Results != null && model.Results.Count > 0)
            {
                model.Results = (from finalLogs in model.Results
                                orderby finalLogs.LogDate, finalLogs.TimeStamp
                                select finalLogs).ToList();
            }
        }

        /// <summary>
        /// Copies the files.
        /// </summary>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        private string CopyFiles(string sourceFile, IndexModel model)
        {
            string productionLogsFolder = ConfigurationManager.AppSettings.Get("ProductionLogsFolder");
            string productionLogsFolder1 = ConfigurationManager.AppSettings.Get("NewProductionLogsFolder1");
            string productionLogsFolder2 = ConfigurationManager.AppSettings.Get("NewProductionLogsFolder2");
            string targetFolder = ConfigurationManager.AppSettings.Get("TargetFolder") + model.ClientIP;
            string targetFile = string.Empty;

            if (sourceFile.Contains(productionLogsFolder))
            {
                string serverName = sourceFile.Replace(productionLogsFolder, string.Empty).Split('\\')[0];
                string fileName = sourceFile.Replace(productionLogsFolder, string.Empty).Split('\\')[1];
                CreateDirectory(targetFolder);
                targetFile = targetFolder + @"\" + serverName + fileName;
                string decompressedTargetFile = targetFile.Replace(".gz", string.Empty);
                if (System.IO.File.Exists(sourceFile) && !System.IO.File.Exists(decompressedTargetFile)) // Do not Copy if file Exists
                {
                    if (model.EnableLogging)
                    {
                        this.BrodCast(model.ConnectionId, string.Format("Copying file {0} from server {1}", fileName, serverName));
                    }
                    System.IO.File.Copy(sourceFile, targetFile);
                }
            }

            if (sourceFile.Contains(productionLogsFolder1))
            {
                string serverName = sourceFile.Replace(productionLogsFolder1, string.Empty).Split('\\')[0];
                string fileName = sourceFile.Replace(productionLogsFolder1, string.Empty).Split('\\')[1];
                CreateDirectory(targetFolder);
                targetFile = targetFolder + @"\" + serverName + fileName;
                string decompressedTargetFile = targetFile.Replace(".zip", string.Empty);
                if (System.IO.File.Exists(sourceFile) && !System.IO.File.Exists(decompressedTargetFile)) // Do not Copy if file Exists
                {
                    if (model.EnableLogging)
                    {
                        this.BrodCast(model.ConnectionId, string.Format("Copying file {0} from server {1}", fileName, serverName));
                    }
                    System.IO.File.Copy(sourceFile, targetFile);
                }
            }

            if (sourceFile.Contains(productionLogsFolder2))
            {
                string serverName = sourceFile.Replace(productionLogsFolder2, string.Empty).Split('\\')[0];
                string fileName = sourceFile.Replace(productionLogsFolder2, string.Empty).Split('\\')[1];
                CreateDirectory(targetFolder);
                targetFile = targetFolder + @"\" + serverName + fileName;
                string decompressedTargetFile = targetFile.Replace(".zip", string.Empty);
                if (System.IO.File.Exists(sourceFile) && !System.IO.File.Exists(decompressedTargetFile)) // Do not Copy if file Exists
                {
                    if (model.EnableLogging)
                    {
                        this.BrodCast(model.ConnectionId, string.Format("Copying file {0} from server {1}", fileName, serverName));
                    }
                    System.IO.File.Copy(sourceFile, targetFile);
                }
            }
            
            return targetFile;
        }

        /// <summary>
        /// Searches Log using Producer Consumer pattern
        /// </summary>
        /// <param name="model"></param>
        /// <param name="productionServers"></param>
        public void SearchInProductionServersParallely(IndexModel model, List<string> productionServers)
        {
            int processorCount = Environment.ProcessorCount;
            SearchInProductionServersSequentially(model, productionServers);
        }

        /// <summary>
        /// Writes to file.
        /// </summary>
        /// <param name="concurrentQueue">The concurrent queue.</param>
        /// <param name="model">The model.</param>
        public void WriteToFile(List<EchannelLog> concurrentQueue, IndexModel model)
        {
            try
            {
                model.FilePath = ConfigurationManager.AppSettings.Get("TargetFolder") + model.ClientIP + @"\result.txt";
                if (System.IO.File.Exists(model.FilePath))
                {
                    System.IO.File.Delete(model.FilePath);
                }
                StreamWriter writer = new StreamWriter(model.FilePath);
                foreach (EchannelLog echannelLog in concurrentQueue)
                {
                    writer.WriteLine(string.Format(serviveText + "{0}" + timeStampText + "{1}" + sessionText + "{2}", echannelLog.ServiceName, echannelLog.TimeStamp.ToString(), echannelLog.SessionID));
                    if (!string.IsNullOrEmpty(echannelLog.IP))
                    {
                        writer.WriteLine(echannelLog.IP);
                    }
                    
                    writer.WriteLine(echannelLog.URL);
                    writer.WriteLine(echannelLog.ResponseTime);
                    writer.WriteLine("     --------Request------------------");
                    writer.WriteLine(echannelLog.Request);
                    writer.WriteLine("     --------Response------------------");
                    writer.WriteLine(echannelLog.Response);
                    writer.WriteLine("-----End of Transaction logged ---------------------------------");
                }
                writer.Close();
            }
            catch(Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
        }

        /// <summary>
        /// Searches the in file.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        private List<EchannelLog> SearchInFile(string fileName, IndexModel model)
        {

            List<EchannelLog> EchannelLogs = new List<EchannelLog>();
            
            if (string.IsNullOrEmpty(fileName))
            {
                return EchannelLogs;
            }

            FileInfo item = new FileInfo(fileName);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string line;
            string filename = item.FullName;
            string searchtext = model.SearchText;
            string searchStartText = model.SearchStartText;
            string searchEndText = model.SearchEndText;
            string newline = Environment.NewLine;
            StringBuilder result = new StringBuilder();
            StringBuilder temp = new StringBuilder();
            bool match = false;

            if (string.IsNullOrEmpty(model.SearchStartText))
            {
                searchStartText = searchtext;
            }
            if (string.IsNullOrEmpty(model.SearchEndText))
            {
                searchEndText = searchtext;
            }
            
            if (model.EnableLogging)
            {
                this.BrodCast(model.ConnectionId, "Searching file : " + item.Name);
            }

            if (File.Exists(filename))
            {
                StreamReader file = null;
                try
                {
                    file = new StreamReader(filename);
                    while ((line = file.ReadLine()) != null)
                    {
                        if (line.IndexOf(searchStartText, StringComparison.Ordinal) != -1)
                        {
                            temp.Append(line + newline);
                            if (line.IndexOf(searchtext, StringComparison.Ordinal) != -1)
                            {
                                match = true;
                            }

                            do
                            {
                                line = file.ReadLine();
                                temp.Append(line + newline);
                                
                                if (!string.IsNullOrEmpty(line) && line.IndexOf(searchtext, StringComparison.Ordinal) != -1)
                                {
                                    match = true;
                                }

                            } while (!string.IsNullOrEmpty(line) && line.IndexOf(searchEndText, StringComparison.Ordinal) == -1);
                        }
                        
                        if (match)
                        {
                            result.Append(temp.ToString());
                            EchannelLogs.Add(new EchannelLog(temp.ToString(), fileName));
                        }

                        match = false;
                        temp.Clear();
                    }
                }

                finally
                {
                    if (file != null)
                    {
                        file.Close();
                        File.Delete(fileName);
                    }
                }
            }
            sw.Stop();
            Trace.WriteLine(string.Format("{0}: {1}", item.FullName, sw.ElapsedMilliseconds));
            return EchannelLogs;
        }

        /// <summary>
        /// Copies the files.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="model">The model.</param>
        /// <param name="targetDirectory">The target directory.</param>
        /// <returns></returns>
        private IEnumerable<string> CopyFiles(List<string> productionServers, IndexModel model, string targetDirectory)
        {
            string dirPath = ConfigurationManager.AppSettings.Get("ProductionLogsFolder");
            string fileName = ConfigurationManager.AppSettings.Get(model.AppName) + HelperClass.ParseToFileName(model.Date) + ".txt.gz";
            string decompressedFileName = ConfigurationManager.AppSettings.Get(model.AppName) + HelperClass.ParseToFileName(model.Date) + ".txt";
            
            foreach (string productionServer in productionServers)
            {
                if (IndexModel.CancelToken.IsCancellationRequested)
                {
                    yield break;
                }

                string sourceFile = dirPath + productionServer + @"\" + fileName;
                string targetFile = targetDirectory + productionServer + fileName;
                string decompressedTargetFile = targetDirectory + productionServer + decompressedFileName;

                if (System.IO.File.Exists(sourceFile) && !System.IO.File.Exists(decompressedTargetFile)) // Do not Copy if file Exists
                {
                    if (model.EnableLogging)
                    {
                        this.BrodCast(model.ConnectionId, string.Format("Copying File : {0} - {1}", productionServer, fileName));
                    }
                    System.IO.File.Copy(sourceFile, targetFile);
                }
                    yield return targetFile;
            }
        }

        /// <summary>
        /// Decompresses the file.
        /// </summary>
        /// <param name="fi">The fi.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        private string DecompressFile(string fileName, IndexModel model)
        {
            FileInfo fi = new FileInfo(fileName);
            string result = string.Empty;
            string decompressedFileName = string.Empty;

            if (fi.FullName.Contains(".gz"))
            {
                decompressedFileName = fi.FullName.Replace(".gz", string.Empty);
            }
            else if (fi.FullName.Contains(".zip"))
            {
                decompressedFileName = fi.FullName.Replace(".zip", string.Empty);
            }
            
            if (System.IO.File.Exists(decompressedFileName))
            {
                return decompressedFileName;
            }

            if (System.IO.File.Exists(fi.FullName))
            {
                using (FileStream inFile = fi.OpenRead())
                {
                    string curFile = fi.FullName;
                    string origName = curFile.Remove(curFile.Length - fi.Extension.Length);
                    
                        using (FileStream outFile = System.IO.File.Create(origName))
                        {
                            if (fi.FullName.Contains(".gz"))
                            {
                                using (GZipStream Decompress = new GZipStream(inFile,
                                        CompressionMode.Decompress))
                                {
                                    Decompress.CopyTo(outFile);
                                    result = outFile.Name;
                                    
                                }
                            }
                            else if (fi.FullName.Contains(".zip"))
                            {
                                using (ZipFile zip = ZipFile.Read(curFile))
                                {
                                    foreach (ZipEntry entity in zip)
                                    {
                                        if (entity.FileName.Contains(".txt"))
                                        {
                                            entity.Extract(outFile);
                                            result = outFile.Name;
                                        }
                                    }
                                }
                            }

                            if (model.EnableLogging)
                            {

                                this.BrodCast(model.ConnectionId, "Decompressing File :" + fi.Name);
                            }
                        }
                }
                fi.Delete();
            }
            return result;
        }

        /// <summary>
        /// Brodcast the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void BrodCast(string connectionId, string message)
        {
            IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            dynamic clients = connectionManager.GetClients<Chat>();
            clients.addMessage(connectionId, message);
        }

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        private void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Gets the file list.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<string> GetFileList(List<IndexerService.IndexInformation> indexInfo, IndexModel model)
        {
            List<string> files = new List<string>();
            string dirPath = ConfigurationManager.AppSettings.Get("ProductionLogsFolder");
            string dirPath1 = ConfigurationManager.AppSettings.Get("NewProductionLogsFolder1");
            string dirPath2 = ConfigurationManager.AppSettings.Get("NewProductionLogsFolder2");
            string fileName = ConfigurationManager.AppSettings.Get(model.AppName);
            string targetDirectory = ConfigurationManager.AppSettings.Get("TargetFolder");
            string extension =".txt.gz";
            string zipExtension = ".txt.zip";

            indexInfo.ForEach(info =>
            {
                string server = info.FilePath.Replace(targetDirectory, string.Empty).Split('-')[0];
                string date = HelperClass.ParseToFileName(info.Date);
                if (DateTime.Parse(info.Date).CompareTo(DateTime.Parse(ConfigurationManager.AppSettings.Get("ServerChangeDate"))) < 0)
                {
                    files.Add(string.Format(@"{0}{1}\{2}{3}{4}", dirPath, server, fileName, date, extension));
                }
                else 
                {
                    files.Add(string.Format(@"{0}{1}\{2}{3}{4}", dirPath1, server, fileName, date, zipExtension));
                    files.Add(string.Format(@"{0}{1}\{2}{3}{4}", dirPath2, server, fileName, date, zipExtension));
                }
            });
                        
            return files;
        }

        /// <summary>
        /// Gets the file list.
        /// </summary>
        /// <param name="productionServers">The production servers.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public List<string> GetFileList(List<string> productionServers, IndexModel model)
        {
            List<string> files = new List<string>();
            string dirPath = ConfigurationManager.AppSettings.Get("ProductionLogsFolder");
            string fileName = ConfigurationManager.AppSettings.Get(model.AppName) + HelperClass.ParseToFileName(model.Date) + ".txt.gz";
            productionServers.ForEach(server =>files.Add(string.Format(@"{0}{1}\{2}", dirPath, server, fileName)));
            return files;
        }

        
    }
}