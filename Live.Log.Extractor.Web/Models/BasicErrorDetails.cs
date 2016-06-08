namespace Live.Log.Extractor.Web.Models
{
    public class BasicErrorDetails
    {
        /// <summary>
        /// Gets or sets the no of instances.
        /// </summary>
        /// <value>
        /// The no of instances.
        /// </value>
        public int NoOfInstances { get; set; }

        /// <summary>
        /// Gets or sets the earliest repoted.
        /// </summary>
        /// <value>
        /// The earliest repoted.
        /// </value>
        public string EarliestRepoted { get; set; }

        /// <summary>
        /// Gets or sets the latest reported.
        /// </summary>
        /// <value>
        /// The latest reported.
        /// </value>
        public string LatestReported { get; set; }
    }
}