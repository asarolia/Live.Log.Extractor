namespace Live.Log.Extractor.Web.Models
{
    /// <summary>
    /// Error Detail Class
    /// </summary>
    public class ErrorDetails
    {
        /// <summary>
        /// Gets or sets the name of the failed programme.
        /// </summary>
        /// <value>
        /// The name of the failed programme.
        /// </value>
        public string FailedProgrammeName { get; set; }

        /// <summary>
        /// Gets or sets the name of the failed paragraph.
        /// </summary>
        /// <value>
        /// The name of the failed paragraph.
        /// </value>
        public string FailedParagraphName { get; set; }

        /// <summary>
        /// Gets or sets the error additional text.
        /// </summary>
        /// <value>
        /// The error additional text.
        /// </value>
        public string ErrorAdditionalText { get; set; }

        /// <summary>
        /// Gets or sets the SQL code.
        /// </summary>
        /// <value>
        /// The SQL code.
        /// </value>
        public string SQLCode { get; set; }

        /// <summary>
        /// Gets or sets the error CMT text.
        /// </summary>
        /// <value>
        /// The error CMT text.
        /// </value>
        public string ErrorCmtText { get; set; }

        /// <summary>
        /// Gets or sets the name of the failed UOW.
        /// </summary>
        /// <value>
        /// The name of the failed UOW.
        /// </value>
        public string FailedUOWName { get; set; }

        /// <summary>
        /// Gets or sets the name of the failed location.
        /// </summary>
        /// <value>
        /// The name of the failed location.
        /// </value>
        public string FailedLocationName { get; set; }

        /// <summary>
        /// Gets or sets the SQL erroe MC text.
        /// </summary>
        /// <value>
        /// The SQL erroe MC text.
        /// </value>
        public string SQLErroeMCText { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the failed key text.
        /// </summary>
        /// <value>
        /// The failed key text.
        /// </value>
        public string FailedKeyText { get; set; }

        /// <summary>
        /// Gets or sets the ACY text.
        /// </summary>
        /// <value>
        /// The ACY text.
        /// </value>
        public string ACYText { get; set; }

        /// <summary>
        /// Gets or sets the priority cd.
        /// </summary>
        /// <value>
        /// The priority cd.
        /// </value>
        public string PriorityCd { get; set; }

        /// <summary>
        /// Gets or sets the error text.
        /// </summary>
        /// <value>
        /// The error text.
        /// </value>
        public string ErrorText { get; set; }
    }
}