using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Web;
using Live.Log.Extractor.IndexerService.Abstract;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Configuration;
using Live.Log.Extractor.Domain;
using Ionic.Zip;

namespace Live.Log.Extractor.IndexerService.Infrastructure
{
    public class XmlProcessor
    {
        public const string ProductionServers = "ProductionServers";

        public Product ReadProduct(ProductType productType)
        {
            ProductDirector productDirector = new ProductDirector();
            IBuildProduct buildProduct = new BuildProduct(productType);
            
            productDirector.Construct(buildProduct);
            Product product = buildProduct.GetProduct();
            return product;
        }

        /// <summary>
        /// Reads the file.
        /// </summary>
        /// <param name="productType">Type of the product.</param>
        /// <returns></returns>
        public HashSet<string> ReadFile(Product product, string filename)
        {
            HashSet<string> set = new HashSet<string>();
            
            string line;
                if (System.IO.File.Exists(filename))
                {
                    StreamReader file = null;
                    try
                    {
                        file = new StreamReader(filename);
                        while ((line = file.ReadLine()) != null)
                        {
                            foreach (string s in FindMatchingElements(product, line))
                            {
                                set.Add(s);
                            }
                        }
                    }

                    finally
                    {
                        if (file != null)
                        {
                            file.Close();
                            File.Delete(filename);
                        }
                    }
                }
            return set;
        }

        /// <summary>
        /// Finds the matching elements.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        private IEnumerable<string> FindMatchingElements(Product product, string line)
        {
            foreach (string expression in product.RegularExpressions)
            {
                Regex regex = new Regex(expression);
                MatchCollection matches = regex.Matches(line);
                foreach (var match in matches)
                {
                    yield return match.ToString();
                }
            }
        }

        /// <summary>
        /// Copies the files.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="model">The model.</param>
        /// <param name="targetDirectory">The target directory.</param>
        /// <returns></returns>
        public IEnumerable<string> CopyFiles(string date, ProductType productType)
        {
            List<string> productionServers = GetConfiguration(ProductionServers, productType.ToString()).Split(',').ToList();
                
            foreach (string i in productionServers)
            {
                string file = GetConfiguration(productType.ToString()) + ParseToFileName(date);
                string fileName = GetFileName(file, ".txt.zip");
                string sourceFile1 = GetConfiguration("NewProductionLogsFolder1") + i + @"\" + fileName;
                string sourceFile2 = GetConfiguration("NewProductionLogsFolder2") + i + @"\" + fileName;
                string targetFile = GetConfiguration("DecompressedFolder") + @"\" + i + "-" + fileName;
                CopyFile(sourceFile1, targetFile);
                CopyFile(sourceFile2, targetFile);
                yield return targetFile;
            }
        }

        /// <summary>
        /// Copy File delegate.
        /// </summary>
        Action<string, string> CopyFile = (source, target) =>
        {
            if (System.IO.File.Exists(source) && !File.Exists(target))
            {
                System.IO.File.Copy(source, target);
            }
        };

        /// <summary>
        /// Delegate to return file name.
        /// </summary>
        Func<string, string, string> GetFileName = (fileName, extension) => fileName + extension;

        /// Gets the configuration.
        /// </summary>
        /// <param name="appVariable">The app variable.</param>
        /// <returns></returns>
        private string GetConfiguration(string appVariable, string optional = "")
        {
            return ConfigurationManager.AppSettings.Get(appVariable + optional);
        }

        /// <summary>
        /// Decompresses the file.
        /// </summary>
        /// <param name="fi">The fi.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public string DecompressFile(FileInfo fi)
        {
            string result = string.Empty;
            string decompressedFileName = fi.FullName.Replace(".zip", string.Empty);

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
                        /*using (GZipStream Decompress = new GZipStream(inFile, CompressionMode.Decompress))
                        {
                            Decompress.CopyTo(outFile);
                            result = outFile.Name;
                        }*/
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
                }
                fi.Delete();
            }
            return result;
        }

        /// <summary>
        /// Parses the name of to file.
        /// </summary>
        /// <param name="strDate">The STR date.</param>
        /// <returns></returns>
        public static string ParseToFileName(string strDate)
        {
            StringBuilder result = new StringBuilder();
            DateTime dateTime = DateTime.Parse(strDate);
            result.Append(dateTime.Year.ToString());

            result.Append(dateTime.Month < 10 ? "0" + dateTime.Month.ToString() : dateTime.Month.ToString());
            result.Append(dateTime.Day < 10 ? "0" + dateTime.Day.ToString() : dateTime.Day.ToString());

            return result.ToString();
        }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="deleteFrom">The delete from.</param>
        /// <param name="server">The server.</param>
        /// <returns></returns>
        public string GetFilePath(Product product, DateTime deleteFrom, string server)
        {
            string file = GetConfiguration(product.ProductName.ToString()) + ParseToFileName(deleteFrom.ToShortDateString());
            string fileName = GetFileName(file, ".txt");
            string targetFile = GetConfiguration("DecompressedFolder") + @"\" + server + "-" + fileName;
            return targetFile;
        }
    }
}
