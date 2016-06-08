using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace Live.Log.Extractor.Web.Models
{
    public class EchannelLog
    {
        /// <summary>
        /// Begin text.
        /// </summary>
        private const string serviveText = "-----Begin of ";

        /// <summary>
        /// Time Stamp Text.
        /// </summary>
        private const string timeStampText = " - Transaction logged at";

        /// <summary>
        /// Session Text.
        /// </summary>
        private const string sessionText = "-----  for session";

        /// <summary>
        /// Initializes a new instance of the <see cref="EchannelLog"/> class.
        /// </summary>
        /// <param name="logText">The log text.</param>
        public EchannelLog(string logText)
        {
            this.LogBlock = logText;
        }

        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        public string ServiceName 
        { 
            get 
            {
                return this.FirstLine.Replace(serviveText, string.Empty).Replace(timeStampText, string.Empty).Replace(sessionText, string.Empty).Split(' ')[0];
            }
        }

        /// <summary>
        /// Gets the time stamp.
        /// </summary>
        public TimeSpan TimeStamp 
        {
            get
            {
                return TimeSpan.ParseExact(this.FirstLine.Replace(serviveText, string.Empty).Replace(timeStampText, string.Empty).Replace(sessionText, string.Empty).Split(' ')[1], @"h\:mm\:ss\.fff", CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets the session ID.
        /// </summary>
        public string SessionID
        {
            get
            {
                return this.FirstLine.Replace(serviveText, string.Empty).Replace(timeStampText, string.Empty).Replace(sessionText, string.Empty).Split(' ')[2];
            }
        }

        /// <summary>
        /// Gets the request.
        /// </summary>
        public string Request 
        {
            get 
            {
                return this.LogBlock.Split('\r')[5].Replace("\n", string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        public string Response 
        {
            get
            {
                return this.LogBlock.Split('\r')[7].Replace("\n", string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets the first line.
        /// </summary>
        private string FirstLine 
        { 
            get 
            {
                return this.LogBlock.Split('\r')[0];
            }
        }

        /// <summary>
        /// Gets or sets the log block.
        /// </summary>
        /// <value>
        /// The log block.
        /// </value>
        private string LogBlock { get; set; }
    }
}