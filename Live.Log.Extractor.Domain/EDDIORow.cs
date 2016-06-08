namespace Live.Log.Extractor.Domain
{
    public class EDDIORow
    {
        /// <summary>
        /// Gets or sets the TRANSACTIO n_ TIME.
        /// </summary>
        /// <value>
        /// The TRANSACTIO n_ TIME.
        /// </value>
        public string TRANSACTION_TIME { get; set; }

        /// <summary>
        /// Gets or sets the MS g_ ID.
        /// </summary>
        /// <value>
        /// The MS g_ ID.
        /// </value>
        public string MSG_ID { get; set; }

        /// <summary>
        /// Gets or sets the I o_ CHAR.
        /// </summary>
        /// <value>
        /// The I o_ CHAR.
        /// </value>
        public string IO_CHAR { get; set; }

        public string MSG_TS { get; set; }

        public string TRS_ID { get; set; }
    }
}