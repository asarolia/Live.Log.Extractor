namespace Live.Log.Extractor.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;

    /// <summary>
    /// Mainframe connection class.
    /// </summary>
    public class DBConnect
    {
        /// <summary>
        /// Post url format.
        /// </summary>
        private string postUrlFormat = "{DBShadow}?SQLQuery={strSQL}&Connection={strConn};UID={UserId};PWD={Password}";
        
        /// <summary>
        /// DB2 Shadow URL.
        /// </summary>
        private string dB2ShadowURL;

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the SQL query.
        /// </summary>
        /// <value>
        /// The SQL query.
        /// </value>
        public string SqlQuery { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>
        /// The region.
        /// </value>
        public string Region { get; set; }

        /// <summary>
        /// Gets the DSN.
        /// </summary>
        private string DSN 
        {   
            get 
            {
               if (string.Equals(this.Region.Substring(0, 3), "CIT") || string.Equals(this.Region.Substring(0, 3), "CID"))
               {
                   return ConfigurationManager.AppSettings["DSNDev"];
               }
                else if (string.Equals(this.Region.Substring(0, 3), "CIU"))
               {
                   return ConfigurationManager.AppSettings["DSNUAT"];
                }
               else if (string.Equals(this.Region.Substring(0, 3), "CIL"))
               {
                   return ConfigurationManager.AppSettings["DSNLive"];
               }
               else
               {
                   return string.Empty;
               }
            }
        }

        /// <summary>
        /// Gets or sets the post URL.
        /// </summary>
        /// <value>
        /// The post URL.
        /// </value>
        private string PostUrl { get; set; }

        /// <summary>
        /// Gets the D b2 shadow URL.
        /// </summary>
        private string DB2ShadowURL
        {
            get 
            {
                if (string.IsNullOrEmpty(this.dB2ShadowURL))
                {
                    this.dB2ShadowURL = ConfigurationManager.AppSettings["DB2ShadowURL"];
                }

                return this.dB2ShadowURL;
            }
        }

        /// <summary>
        /// Creates the post string.
        /// </summary>
        private void CreatePostString()
        {
            Contract.Requires(!string.IsNullOrEmpty(this.SqlQuery));
            Contract.Requires(!string.IsNullOrEmpty(this.DSN));
            Contract.Requires(!string.IsNullOrEmpty(this.UserId));
            Contract.Requires(!string.IsNullOrEmpty(this.Password));

            this.PostUrl = this.postUrlFormat;
            this.PostUrl = this.ReplaceKey(this.PostUrl, "DBShadow", this.DB2ShadowURL);
            this.PostUrl = this.ReplaceKey(this.PostUrl, "strSQL", this.SqlQuery);
            this.PostUrl = this.ReplaceKey(this.PostUrl, "strConn", this.DSN);
            this.PostUrl = this.ReplaceKey(this.PostUrl, "UserId", this.UserId);
            this.PostUrl = this.ReplaceKey(this.PostUrl, "Password", this.Password);
        }

        /// <summary>
        /// Replaces the key.
        /// </summary>
        /// <param name="strUrl">The STR URL.</param>
        /// <param name="key">The replace key.</param>
        /// <param name="value">The value.</param>
        /// <returns>Replaced string for DB2 Connect</returns>
        private string ReplaceKey(string strUrl, string key, string value)
        {
            Contract.Requires(!string.IsNullOrEmpty(key));
            Contract.Requires(!string.IsNullOrEmpty(value));

            return strUrl.Replace("{" + key + "}", value);
        }

        /// <summary>
        /// Runs the query.
        /// </summary>
        /// <returns>
        /// Data got from mainframe.
        /// </returns>
        public string RunQuery()
        {
            this.CreatePostString();

            Random random = new Random();

            WebRequest webRequest = HttpWebRequest.Create(this.PostUrl);
            webRequest.Method = "GET";
            webRequest.Proxy = null;
            string result;
            WebResponse webResponse = null;

            try
            {
                webResponse = webRequest.GetResponse();
                StreamReader strResponse = new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.ASCII);
                result = strResponse.ReadToEnd();
                webResponse.Close();
            }
            catch (WebException e)
            {
                StreamReader strResponse = new StreamReader(e.Response.GetResponseStream(), System.Text.Encoding.ASCII);
                result = strResponse.ReadToEnd();
                throw new InvalidOperationException("web method failed " + e.Message + result);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("web method failed " + e.Message);
            }

            return result;
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <returns>
        /// Returns DB2 Table name.
        /// </returns>
        private string GetTableName()
        {
            Contract.Requires(!string.IsNullOrEmpty(this.SqlQuery));
            Contract.Assert(this.SqlQuery.IndexOf('.') != -1);

            return this.SqlQuery.Split('.')[1].Split(' ')[0];
        }

        /// <summary>
        /// Populates the data table.
        /// </summary>
        /// <returns>
        /// Poplulated data table.
        /// </returns>
        private DataTable PopulateDataTable()
        {
            string result = string.Empty;
            try
            {
                result = this.RunQuery();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }

            DataTable dataTable = new DataTable();
            List<string> rows = new List<string>();
            
            dataTable.TableName = this.GetTableName();
            rows.AddRange(result.Split('\r'));
            rows.RemoveAt(rows.Count - 1);

            if (rows.Count < 2)
            {
                return null;
            }
                
            this.CreateTableColumns(dataTable, rows[0]);

            rows.RemoveAt(0);

            foreach (string row in rows)
            {
                this.AddTableRow(dataTable, row);
            }

            return dataTable;
        }

        /// <summary>
        /// Creates the table columns.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <param name="tableColumns">The table columns.</param>
        private void CreateTableColumns(DataTable dataTable, string tableColumns)
        {
            List<string> columnsNames = new List<string>();
            columnsNames.AddRange(tableColumns.Split('\t'));
            foreach (string columnName in columnsNames)
            {
                dataTable.Columns.Add(columnName);
            }
        }

        /// <summary>
        /// Adds the table row.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="rowText">The row text.</param>
        private void AddTableRow(DataTable table, string rowText)
        {
            List<string> rowValue = new List<string>();
            rowValue.AddRange(rowText.Split('\t'));

            DataRow row = table.NewRow();

            for (int i = 0; i < table.Columns.Count; i++)
            {
                this.SetColumnValue(row, table.Columns[i], rowValue[i]);
            }

            table.Rows.Add(row);
        }

        /// <summary>
        /// Sets the column value.
        /// </summary>
        /// <param name="row">The data row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        private void SetColumnValue(DataRow row, DataColumn column, string value)
        {
            object result = null;
            Int32 int32;
            Int16 int16;
            float flt;
            Single single;

            switch (column.DataType.Name.ToLower())
            {
                case "string":
                    if (value.Length > 0)
                    { 
                        result = value; 
                    }
                    else if (!column.AllowDBNull)
                    {
                        result = value;
                    }

                    break;

                case "int32":
                    if (Int32.TryParse(value, out int32))
                    {
                        result = int32;
                    }

                    break;

                case "int16":
                    if (Int16.TryParse(value, out int16))
                    {
                        result = int16;
                    }

                    break;

                case "single":
                    if (Single.TryParse(value, out single))
                    {
                        result = single;
                    }

                    break;

                case "float":
                    if (float.TryParse(value, out flt))
                    {
                        result = flt;
                    }

                    break;
                default:
                    throw new InvalidOperationException("DataSetHelper: SetColumnValue - " + row.Table.TableName + "(" + column.ColumnName + ") data type inconsistant '" + column.DataType.Name.ToLower() + "'.");
            }

            try
            {
                if (result == null && column.AllowDBNull)
                {
                    row[column] = DBNull.Value;
                }
                else if (result != null)
                {
                    row[column] = result;
                }
                else
                {
                    throw new InvalidOperationException("DataSetHelper: SetColumnValue - " + row.Table.TableName + "(" + column.ColumnName + ") value '" + value + "' is inconsistant with data type '" + column.DataType.Name.ToLower() + "'.");
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("DataSetHelper: SetColumnValue - " + row.Table.TableName + "(" + column.ColumnName + ")  - " + e.Message);
            }
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>
        /// Populated dataset.
        /// </returns>
        public DataTable GetData()
        {
            DataTable dataTable = null;
            try
            {
                dataTable = this.PopulateDataTable();
            }
            catch (Exception e)
            {
                return dataTable;
            }

            return dataTable;
        }
    }
}
