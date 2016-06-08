namespace Live.Log.Extractor.IndexerService
{
    using System;
    using Live.Log.Extractor.IndexerService.Infrastructure;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Xml.Linq;
    using System.Linq;
    using Live.Log.Extractor.Domain;
    
    /// <summary>
    /// IndexingService class
    /// </summary>
    public class IndexingService : IService
    {
        public const string IndexFolder = "IndexFolder";
        public const string DecompressedFolder = "DecompressedFolder";
        public const string ProductionServers = "ProductionServers";

        /// <summary>
        /// Indexes the files.
        /// </summary>
        /// <param name="servers">The servers.</param>
        /// <returns></returns>
        public bool CreateIndexFiles(ProductType productType)
        {
            string ImpersonatedUser = ConfigurationManager.AppSettings.Get("User");
            string ImpersonatedUserPassword = ConfigurationManager.AppSettings.Get("Password");
            string ImpersonatedUserDomain = ConfigurationManager.AppSettings.Get("Domain");
            using (UserImpersonation user = new UserImpersonation(ImpersonatedUser, ImpersonatedUserDomain, ImpersonatedUserPassword))
            {
                if (user.ImpersonateValidUser())
                {
                    XmlProcessor processor = new XmlProcessor();
                    Product product = processor.ReadProduct(productType);

                    if (product.LastIndexedDate.HasValue && DateTime.Compare(product.LastIndexedDate.Value, DateTime.Today) == 0)
                    {
                        return true;
                    }

                    DateTime startDate = product.LastIndexedDate ?? product.IndexStartDate;
                    DateTime endDate = DateTime.Today.AddDays(-1);

                    string targetDirectory = ConfigurationManager.AppSettings.Get("DecompressedFolder");
                    string indexLocation = ConfigurationManager.AppSettings.Get("IndexFolder") + productType.ToString();
                    ClearLogDecompress(targetDirectory);
                    DeleteEarlierIndexes(product, processor);
                    
                    while (startDate <= endDate)
                    {
                        foreach (string file in processor.CopyFiles(startDate.ToShortDateString(), productType))
                        {
                            string fileName;
                            FileInfo fi = new DirectoryInfo(targetDirectory).GetFiles("*.zip").FirstOrDefault();

                            fileName = fi != null ? processor.DecompressFile(fi) : string.Empty;
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                LuceneIndexer li = new LuceneIndexer();
                                HashSet<string> set = processor.ReadFile(product, fileName);
                                li.IndexFile(indexLocation, fileName, startDate.ToShortDateString(), set);
                            }
                        }

                        startDate = startDate.AddDays(1);
                        this.UpdateProductDate(product, startDate, "LastIndexedDate");

                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Deletes the earlier indexes.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="processor">The processor.</param>
        private void DeleteEarlierIndexes(Product product, XmlProcessor processor)
        {
            DateTime deleteFrom = product.IndexStartDate;
            DateTime deleteUntil = DateTime.Today.AddDays(-31);
            string indexLocation = ConfigurationManager.AppSettings.Get("IndexFolder") + product.ProductName.ToString();
            List<string> docList=new List<string>();
            LuceneIndexer li = new LuceneIndexer();
            while (deleteFrom < deleteUntil)
            {
                foreach (string server in ConfigurationManager.AppSettings.Get(ProductionServers + product.ProductName.ToString()).Split(','))
                {
                    docList.Add(processor.GetFilePath(product, deleteFrom, server));
                }
                deleteFrom = deleteFrom.AddDays(1);
            }
            li.DeleteIndex(indexLocation, docList);
            UpdateProductDate(product, deleteUntil, "IndexStartDate");
        }

        /// <summary>
        /// Clears the log decompress.
        /// </summary>
        /// <param name="targetDirectory">The target directory.</param>
        private void ClearLogDecompress(string targetDirectory)
        {
            if (Directory.Exists(targetDirectory))
            {
                Directory.Delete(targetDirectory, true);
            }

            Directory.CreateDirectory(targetDirectory);
        }

        /// <summary>
        /// Updates the end date.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="date">The start date.</param>
        /// <param name="elementName">Name of the element.</param>
        private void UpdateProductDate(Product product, DateTime date, string elementName)
        {
            string path = System.Configuration.ConfigurationManager.AppSettings.Get("ResourcePath");
            XDocument doc = XDocument.Load(path);
            XElement node = doc.Root.Elements("Product").FirstOrDefault(x => string.Equals(x.Attribute("value").Value, product.ProductName.ToString()));
            node.Element(elementName).SetAttributeValue("value", date.ToShortDateString());
            doc.Save(path);
        }

        /// <summary>
        /// Creates the file list.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        private List<string> CreateFileList(Product product, string date)
        {
            List<string> filePaths = new List<string>(); 
            string decompressedFolder = ConfigurationManager.AppSettings.Get(DecompressedFolder + product.ProductName.ToString());
            foreach (string fullFilePath in Directory.GetFiles(decompressedFolder))
            {
                filePaths.Add(fullFilePath);
            }

            return filePaths;
        }

        /// <summary>
        /// Gets the lastindexed date.
        /// </summary>
        /// <returns></returns>
        public List<IndexInformation> SearchIndex(ProductType productType, string searchText)
        {
            LuceneIndexer li = new LuceneIndexer();
            string indexFolder = ConfigurationManager.AppSettings.Get(IndexFolder) + productType.ToString();
            return li.SearchIndex(indexFolder, searchText);
        }
    }
}
