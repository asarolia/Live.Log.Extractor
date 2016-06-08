namespace Live.Log.Extractor.Web.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class HalErrorDetailViewModel
    {
        public HalErrorDetailViewModel()
        {
            this.ExceedRegion = new ExceedRegion();
        }
        
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        [DisplayName("Exceed error code")]
        [Required (ErrorMessage = "Please enter error code.")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the detail SQL query.
        /// </summary>
        /// <value>
        /// The detail SQL query.
        /// </value>
        public string DetailSqlQuery { get; set; }

        /// <summary>
        /// Gets or sets the exceed region.
        /// </summary>
        /// <value>
        /// The exceed region.
        /// </value>
        public ExceedRegion ExceedRegion { get; set; }

        /// <summary>
        /// Gets or sets the last10 occurances.
        /// </summary>
        /// <value>
        /// The last10 occurances.
        /// </value>
        public List<string> Last10Occurances { get; set; }

        /// <summary>
        /// Gets or sets the six month count.
        /// </summary>
        /// <value>
        /// The six month count.
        /// </value>
        public List<MonthCount> SixMonthCount { get; set; } 
    }
}