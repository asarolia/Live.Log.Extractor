namespace Live.Log.Extractor.Domain.ServiceHelper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;

    public static class GetExceedData
    {
        /// <summary>
        /// Function to conver string to datetime.
        /// </summary>
        private static Func<string, DateTime> convertToDateTime = x => Convert.ToDateTime(x);

        /// <summary>
        /// Verifies the login.
        /// </summary>
        /// <param name="dataModel">The data model.</param>
        /// <returns></returns>
        public static bool VerifyLogin(LogDataModel dataModel)
        {
            DBConnect dbConnect = new DBConnect();
            PopulateConnectionObject(dataModel, dbConnect);
            
            if (dataModel.dataSet == null)
            {
                dataModel.dataSet = new DataSet();
            }

            DataTable dataTable = dbConnect.GetData();

            if (dataTable == null)
            {
                return false;
            }
            else 
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the error details.
        /// </summary>
        /// <param name="dataModel">The data model.</param>
        public static void GetErrorDetails(LogDataModel dataModel)
        {
            DBConnect dbConnect = new DBConnect();
            PopulateConnectionObject(dataModel, dbConnect);

            if (dataModel.dataSet == null)
            {
                dataModel.dataSet = new DataSet();
            }
            
            if (dataModel.dataSet.Tables["HAL_ERR_LOG_DTL"] != null)
            {
                dataModel.dataSet.Tables.Remove("HAL_ERR_LOG_DTL");
            }
            DataTable dataTable = dbConnect.GetData();

            if (dataTable != null)
            {
                dataModel.dataSet.Tables.Add(dataTable);
            }
        }

        /// <summary>
        /// Gets the error failed.
        /// </summary>
        /// <param name="dataModel">The data model.</param>
        public static void GetErrorFailed(LogDataModel dataModel)
        {
            DBConnect dbConnect = new DBConnect();
            PopulateConnectionObject(dataModel, dbConnect);

            if (dataModel.dataSet == null)
            {
                dataModel.dataSet = new DataSet();
            }

            if (dataModel.dataSet.Tables["HAL_ERR_LOG_FAIL"] != null)
            {
                dataModel.dataSet.Tables.Remove("HAL_ERR_LOG_FAIL");
            }

            DataTable dataTable = dbConnect.GetData();

            if (dataTable != null)
            {
                dataModel.dataSet.Tables.Add(dataTable);
            }
        }

        /// <summary>
        /// Gets the edd details.
        /// </summary>
        /// <param name="logDataModel">The log data model.</param>
        public static void GetDetailsPrint(LogDataModel dataModel)
        {
            DBConnect dbConnect = new DBConnect();
            PopulateConnectionObject(dataModel, dbConnect);
            DataTable formattedDataTable = null;
            if (dataModel.dataSet == null)
            {
                dataModel.dataSet = new DataSet();
            }

            if (dataModel.dataSet.Tables["Formatted_POL_BIL_MQ_AUDIT_V"] != null)
            {
                dataModel.dataSet.Tables.Remove("Formatted_POL_BIL_MQ_AUDIT_V");
            }

            DataTable dataTable = dbConnect.GetData();

            if (dataTable != null)
            {
                formattedDataTable = FormatEDDMessagePrint(dataTable);    
            }

            if (formattedDataTable != null)
            {
                dataModel.dataSet.Tables.Add(formattedDataTable);
            }
                        
        }

        public static void GetEddDetails(LogDataModel dataModel)
        {

            DBConnect dbConnect = new DBConnect();
            PopulateConnectionObject(dataModel, dbConnect);
            DataTable formattedDataTable = null;
            if (dataModel.dataSet == null)
            {
                dataModel.dataSet = new DataSet();
            }

            if (dataModel.dataSet.Tables["Formatted_TXAMDET"] != null)
            {
                dataModel.dataSet.Tables.Remove("Formatted_TXAMDET");
            }

            DataTable dataTable = dbConnect.GetData();

            if (dataTable != null)
            {
                formattedDataTable = FormatEDDMessage(dataTable);
            }

            if (formattedDataTable != null)
            {
                dataModel.dataSet.Tables.Add(formattedDataTable);
            }
        }

        /// <summary>
        /// Gets the polaris ids.
        /// </summary>
        /// <param name="logDataModel">The log data model.</param>
        public static void GetPolarisIds(LogDataModel dataModel)
        {
            DBConnect dbConnect = new DBConnect();
            PopulateConnectionObject(dataModel, dbConnect);
            
            if (dataModel.dataSet == null)
            {
                dataModel.dataSet = new DataSet();
            }

            if (dataModel.dataSet.Tables["RATING_PARAMETERS"] != null)
            {
                dataModel.dataSet.Tables.Remove("RATING_PARAMETERS");
            }

            DataTable dataTable = dbConnect.GetData();

            if (dataTable != null)
            {
                dataModel.dataSet.Tables.Add(dataTable);
            }
        }

        /// <summary>
        /// Gets the LOB cd.
        /// </summary>
        /// <param name="dataModel">The data model.</param>
        public static void GetPolicyTabDetails(LogDataModel dataModel)
        {
            DBConnect dbConnect = new DBConnect();
            PopulateConnectionObject(dataModel, dbConnect);

            if (dataModel.dataSet == null)
            {
                dataModel.dataSet = new DataSet();
            }

            if (dataModel.dataSet.Tables["POLICY_TAB"] != null)
            {
                dataModel.dataSet.Tables.Remove("POLICY_TAB");
            }

            DataTable dataTable = dbConnect.GetData();

            if (dataTable != null)
            {
                dataModel.dataSet.Tables.Add(dataTable);
            }
        }

        /// <summary>
        /// Formats the EDD message.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <returns></returns>
        private static DataTable FormatEDDMessagePrint(DataTable dataTable)
        {
            Contract.Requires(dataTable != null);

            DataTable formattedDataTable = new DataTable("Formatted_POL_BIL_MQ_AUDIT_V");
            
            //formattedDataTable.Columns.Add("MSG_ID");
            formattedDataTable.Columns.Add("MSG_TS");           
            formattedDataTable.Columns.Add("MSG_REPORT");

            var POL_BIL_MQ_AUDIT_V = dataTable.AsEnumerable();
            //var timeIOCharKey = (from row in POL_BIL_MQ_AUDIT_V
            //                     where (!row.Field<string>("MSG_ID").Trim().Contains("\0")) 
            //                     select new { MSG_TS = row.Field<string>("MSG_TS"),
            //                        MSG_ID = row.Field<string>("MSG_ID"), 
            //                        TRS_ID = row.Field<string>("TRS_ID") })
            //                        .Distinct();
            //foreach (var key in timeIOCharKey)
            //{
            //    StringBuilder sb = new StringBuilder();
            //    List<System.Data.DataRow> keyData = (from row in POL_BIL_MQ_AUDIT_V 
            //                   where (string.Equals(row.Field<string>("MSG_TS").Trim(), key.MSG_TS.Trim())
            //                   && string.Equals(key.TRS_ID.Trim(), row.Field<string>("TRS_ID").Trim()))
            //                      select row).ToList();
            //    keyData.ForEach(row => sb.Append(row.Field<string>("MSG_REPORT").Trim()));
            //    DataRow dataRow = formattedDataTable.NewRow();
            //    dataRow["MSG_TS"] = key.MSG_TS.Trim();
            //    dataRow["MSG_ID"] = key.MSG_ID.Trim();
            //    dataRow["TRS_ID"] = key.TRS_ID.Trim();
            //    dataRow["MSG_REPORT"] = sb.ToString();
            //    formattedDataTable.Rows.Add(dataRow); 

            var timeIOCharKey = (from row in POL_BIL_MQ_AUDIT_V
                                 
                                 select new
                                 {
                                     //MSG_ID = row.Field<string>("MSG_ID"),
                                     MSG_TS = row.Field<string>("MSG_TS"),                                     
                                     
                                 })
                                    .Distinct();
            foreach (var key in timeIOCharKey)
            {
                StringBuilder sb = new StringBuilder();
                List<System.Data.DataRow> keyData = (from row in POL_BIL_MQ_AUDIT_V
                                                     where (string.Equals(row.Field<string>("MSG_TS").Trim(), key.MSG_TS.Trim())
                                                     )
                                                     select row).ToList();
                keyData.ForEach(row => sb.Append(row.Field<string>("MSG_REPORT").Trim()));
                DataRow dataRow = formattedDataTable.NewRow();
                dataRow["MSG_TS"] = key.MSG_TS.Trim();                
                dataRow["MSG_REPORT"] = sb.ToString();
                formattedDataTable.Rows.Add(dataRow); 
            }
            
            Contract.Ensures(formattedDataTable != null);
            return formattedDataTable;
                       
        }

        private static DataTable FormatEDDMessage(DataTable dataTable)
        {
            Contract.Requires(dataTable != null);

            DataTable formattedDataTable = new DataTable("Formatted_TXAMDET");
            formattedDataTable.Columns.Add("TRANSACTION_TIME");
            formattedDataTable.Columns.Add("MSG_ID");
            formattedDataTable.Columns.Add("IO_CHAR");
            formattedDataTable.Columns.Add("MSG_SECTION");

            var Txamdet = dataTable.AsEnumerable();
            var timeIOCharKey = (from row in Txamdet
                                 where (!row.Field<string>("MSG_ID").Trim().Contains("\0"))
                                 select new
                                 {
                                     TRANSACTION_TIME = row.Field<string>("TRANSACTION_TIME"),
                                     MSG_ID = row.Field<string>("MSG_ID"),
                                     IO_CHAR = row.Field<string>("IO_CHAR")
                                 })
                                    .Distinct();
            foreach (var key in timeIOCharKey)
            {
                StringBuilder sb = new StringBuilder();
                List<System.Data.DataRow> keyData = (from row in Txamdet
                                                     where (string.Equals(row.Field<string>("TRANSACTION_TIME").Trim(), key.TRANSACTION_TIME.Trim())
                                                     && string.Equals(key.IO_CHAR.Trim(), row.Field<string>("IO_CHAR").Trim()))
                                                     select row).ToList();
                keyData.ForEach(row => sb.Append(row.Field<string>("MSG_SECTION").Trim()));
                DataRow dataRow = formattedDataTable.NewRow();
                dataRow["TRANSACTION_TIME"] = key.TRANSACTION_TIME.Trim();
                dataRow["MSG_ID"] = key.MSG_ID.Trim();
                dataRow["IO_CHAR"] = key.IO_CHAR.Trim();
                dataRow["MSG_SECTION"] = sb.ToString();
                formattedDataTable.Rows.Add(dataRow);
            }

            Contract.Ensures(formattedDataTable != null);
            return formattedDataTable;


        }
        /// <summary>
        /// Populates the connection object.
        /// </summary>
        /// <param name="dataModel">The data model.</param>
        /// <param name="dbConnect">The db connect.</param>
        private static void PopulateConnectionObject(LogDataModel dataModel, DBConnect dbConnect)
        {
            dbConnect.UserId = dataModel.UserId;
            dbConnect.Password = dataModel.Password;
            dbConnect.Region = dataModel.Region;
            dbConnect.SqlQuery = dataModel.SqlQuery;
        }

    }
}
