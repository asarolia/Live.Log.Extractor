namespace Live.Log.Extractor.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web;

    public class PrintViewModel
    {
          /// <summary>
        /// Initializes a new instance of the <see cref="PrintViewModel"/> class.
        /// </summary>
        public PrintViewModel()
        {
            this.ExceedRegion = new ExceedRegion();
        }

        /// <summary>
        /// Gets or sets the policy number.
        /// </summary>
        /// <value>
        /// The policy number.
        /// </value>
        [DisplayName("Policy Number")]
        public string PolicyNumber { get; set; }

        /// <summary>
        /// Gets or sets the policy ID.
        /// </summary>
        /// <value>
        /// The policy ID.
        /// </value>
        [DisplayName("Policy ID")]
        public string PolicyID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is policy number.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is policy number; otherwise, <c>false</c>.
        /// </value>
        public bool IsPolicyID { get; set; }

        /// <summary>
        /// Gets or sets the exceed region.
        /// </summary>
        /// <value>
        /// The exceed region.
        /// </value>
        public ExceedRegion ExceedRegion { get; set; }

        /// <summary>
        /// Gets or sets the txamdet SQL query.
        /// </summary>
        /// <value>
        /// The txamdet SQL query.
        /// </value>
        public string SqlQuery { get; set; }

        /// <summary>
        /// Gets or sets the edd key.
        /// </summary>
        /// <value>
        /// The edd key.
        /// </value>
        public string EddKey { get; set; }

        /// <summary>
        /// Gets or sets the polaris ids.
        /// </summary>
        /// <value>
        /// The polaris ids.
        /// </value>
        public List<string> PolarisIds { get; set; }

        /// <summary>
        /// Gets or sets the LOBCD.
        /// </summary>
        /// <value>
        /// The LOBCD.
        /// </value>
        public string LOBCD { get; set; }

        /// <summary>
        /// Gets or sets the master company NBR.
        /// </summary>
        /// <value>
        /// The master company NBR.
        /// </value>
        public string MasterCompanyNbr { get; set; }
    }
}