namespace Live.Log.Extractor.Web.Helper
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Text;
    using System.IO;

    public static class HelperClass
    {
        /// <summary>
        /// Replaces the key.
        /// </summary>
        /// <param name="strUrl">The STR URL.</param>
        /// <param name="key">The replace key.</param>
        /// <param name="value">The value.</param>
        /// <returns>Replaced string for DB2 Connect</returns>
        public static string ReplaceKey(string strUrl, string key, string value)
        {
            Contract.Requires(!string.IsNullOrEmpty(key));
            Contract.Requires(!string.IsNullOrEmpty(value));

            return strUrl.Replace("{" + key + "}", value);
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
        /// Function to conver date to UK format.
        /// </summary>
        public static Func<int, int, string> convertToUKFormat = (year,month) => year + "-" + (month < 10 ? "0" + month.ToString() : month.ToString())  + "-" +"01-00.00.00.000000";

        /// <summary>
        /// Get max time stamp
        /// </summary>
        public static Func<int, int, int, string> getMaxTimeStamp = (year, month, day) => year + "-" + (month < 10 ? "0" + month.ToString() : month.ToString()) + "-" + (day < 10 ? "0" + day.ToString() : day.ToString()) + "-23.59.59.999999";

        /// <summary>
        /// Creates the union query.
        /// </summary>
        /// <param name="queryStructure">The query structure.</param>
        /// <returns>Union query</returns>
        internal static string CreateUnionQuery(string queryStructure)
        {
            string unionQuery = string.Empty;
            
            for (int i = 0; i >= -5; i--)
            { 
                string query = ReplaceKey(queryStructure ,"month", DateTime.Now.AddMonths(i).ToString("MMMM"));    
                query = ReplaceKey(query, "MinFailTs", convertToUKFormat(DateTime.Now.AddMonths(i).Year, DateTime.Now.AddMonths(i).Month));
                query = ReplaceKey(query, "MaxFailTs", getMaxTimeStamp(DateTime.Now.AddMonths(i).Year, DateTime.Now.AddMonths(i).Month, DateTime.DaysInMonth(DateTime.Now.AddMonths(i).Year, DateTime.Now.AddMonths(i).Month)));
                if (i != -5)
                {
                    unionQuery = unionQuery + query + " Union ";
                }
                else
                {
                    unionQuery = unionQuery + query;
                }
            }
            
            return unionQuery;
        }

        /// <summary>
        /// Gets the exceed transaction.
        /// </summary>
        /// <param name="messageId">The message id.</param>
        /// <returns>Exceed Transaction</returns>
        internal static string GetExceedTransaction(string messageId)
        {
            if (messageId.Contains("NBQ"))
            {
                return "New Business";
            }
            
            if (messageId.Contains("MTA"))
            {
                return "Amendment";
            }

            if (messageId.Contains("REN"))
            { 
                return "Renewal"; 
            }

            if (messageId.Contains("CAN"))
            {
                return "Cancellation";
            }

            return "Other";
        }

        /// <summary>
        /// Create client Directory.
        /// </summary>
        public static Action<string> CreateDirectory = (filePath) =>
        {
            if (Directory.Exists(filePath))
            {
                Directory.Delete(filePath, true);
            }
            Directory.CreateDirectory(filePath);
        };
    }
}