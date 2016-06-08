namespace Live.Log.Extractor.Domain
{
    using System.Collections.Generic;
    using System.Data;
    
    public class LogDataModel
    {
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
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the policy id.
        /// </summary>
        /// <value>
        /// The policy id.
        /// </value>
        public string PolicyId { get; set; }

        /// <summary>
        /// Gets or sets the policy number.
        /// </summary>
        /// <value>
        /// The policy number.
        /// </value>
        public string PolicyNumber { get; set; }

        /// <summary>
        /// Gets or sets the data set.
        /// </summary>
        /// <value>
        /// The data set.
        /// </value>
        public DataSet dataSet { get; set; }

        /// <summary>
        /// Gets or sets the edd key.
        /// </summary>
        /// <value>
        /// The edd key.
        /// </value>
        public string EddKey { get; set; }

        /// <summary>
        /// Gets or sets the edd keys.
        /// </summary>
        /// <value>
        /// The edd keys.
        /// </value>
        public List<EDDIORow> EddKeys { get; set; }

        /// <summary>
        /// Gets or sets the Print keys.
        /// </summary>
        /// <value>
        /// The Print keys.
        /// </value>
        public List<PrintIORow> PrintKeys { get; set; }

        /// <summary>
        /// Gets or sets the polaris ids.
        /// </summary>
        /// <value>
        /// The polaris ids.
        /// </value>
        public string PolarisIds { get; set; }

        /// <summary>
        /// Gets or sets the LOBCD.
        /// </summary>
        /// <value>
        /// The LOBCD.
        /// </value>
        public string LOBCD { get; set; }

        /// <summary>
        /// Gets or sets the echannel logs.
        /// </summary>
        /// <value>
        /// The echannel logs.
        /// </value>
        public List<EchannelLog> EchannelLogs { get; set; }
    }
}
