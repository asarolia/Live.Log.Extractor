namespace Live.Log.Extractor.IndexerService.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Class to expose the Exceed Code Attribute
    /// </summary>
    [Serializable]
    public class ProductCodeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the ExceedCodeAttribute class.
        /// </summary>
        /// <param name="code">The Exceed code.</param>
        public ProductCodeAttribute(params string[] code)
        {
            this.Codes = new List<string>();

            foreach (string s in code)
            {
                this.Codes.Add(s);
            }
        }

        /// <summary>
        /// Gets or sets the list of Exceed codes
        /// </summary>
        public List<string> Codes { get; set; }
    }
}
