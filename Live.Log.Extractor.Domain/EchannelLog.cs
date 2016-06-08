namespace Live.Log.Extractor.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Diagnostics.Contracts;

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
        /// Request text.
        /// </summary>
        private const string requestText = "--Request";

        /// <summary>
        /// End of Transaction logged text.
        /// </summary>
        private const string endOfLogText = "End of Transaction logged";

        /// <summary>
        /// Initializes a new instance of the <see cref="EchannelLog"/> class.
        /// </summary>
        /// <param name="logText">The log text.</param>
        public EchannelLog(string logText, string fileName)
        {
            this.LogBlock = logText;
            this.IP = this.GetText("IP     :");
            this.URL = this.GetText("URL    :");
            this.ResponseTime = this.GetText("ResponseTime:");
            this.GetRequestResponseText();
            this.LogDate = this.GetLogDate(fileName);
        }

        /// <summary>
        /// Gets the log date.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Log Date</returns>
        private DateTime GetLogDate(string fileName)
        {
            Contract.Requires(!string.IsNullOrEmpty(fileName));
            string dateString = fileName.Split('_')[1].Split('.')[0];
            return GetLogDateFromString(dateString);
        }

        /// <summary>
        /// Converts Datestring to DateTime
        /// </summary>
        private Func<string, DateTime> GetLogDateFromString = (dateString) => new DateTime(Int32.Parse(dateString.Substring(0,4)), Int32.Parse(dateString.Substring(4,2)), Int32.Parse(dateString.Substring(6,2))) ; 

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
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public DateTime LogDate { get; set; }

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
        /// Gets or sets the IP.
        /// </summary>
        /// <value>
        /// The IP.
        /// </value>
        public string IP { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string URL { get; set; }

        /// <summary>
        /// Gets the response time.
        /// </summary>
        public string ResponseTime { get; set; }

        /// <summary>
        /// Gets the request.
        /// </summary>
        public string Request { get; set; } 

        /// <summary>
        /// Gets the response.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Gets the first line.
        /// </summary>
        private string FirstLine 
        { 
            get 
            {
                return this.logSplit[0];
            }
        }

        /// <summary>
        /// Gets or sets the log block.
        /// </summary>
        /// <value>
        /// The log block.
        /// </value>
        private string LogBlock { get; set; }

        /// <summary>
        /// Log Split.
        /// </summary>
        private List<string> logSplit;

        /// <summary>
        /// Gets or sets the index of the request.
        /// </summary>
        /// <value>
        /// The index of the request.
        /// </value>
        private int requestIndex { get ; set; }

        /// <summary>
        /// Gets the log split.
        /// </summary>
        private List<string> LogSplit
        { 
            get 
            {
                if (!(this.logSplit != null && this.logSplit.Count > 0))
                {
                    this.logSplit = this.LogBlock.Split('\r').ToList();
                }
                return this.logSplit;
            }
        }

        /// <summary>
        /// Gets the request response text.
        /// </summary>
        private void GetRequestResponseText()
        {
            for (int count = this.requestIndex; !this.LogSplit[count].Contains(endOfLogText); count++)
            {
                if (this.LogSplit[count].Contains(requestText))
                {
                    if (count + 1 < this.LogSplit.Count && !string.IsNullOrEmpty(this.LogSplit[count + 1]))
                    {
                        this.Request = this.LogSplit[count + 1].Replace("\n", string.Empty).Trim();
                    }
                    else
                    {
                        this.Request = string.Empty;
                    }

                    if (count + 3 < this.LogSplit.Count && this.LogSplit[count + 3] != null && !string.IsNullOrEmpty(this.LogSplit[count + 3]))
                    {
                        this.Response = this.LogSplit[count + 3].Replace("\n", string.Empty).Trim();
                    }
                    else 
                    {
                        this.Response = string.Empty;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private string GetText(string text)
        {
            int count = 0;
            for (; !this.LogSplit[count].Contains(requestText); count++)
            {
                if (this.LogSplit[count].Contains(text))
                {
                    if (this.LogSplit[count].Contains(requestText))
                    {
                        this.requestIndex = count;
                    }
                    else
                    {
                        if (this.requestIndex == 0 && this.LogSplit[count + 1].Contains(requestText))
                        {
                            this.requestIndex = count + 1;
                        }
                    }
                    
                    return this.LogSplit[count].Replace("\n", string.Empty).Trim();
                }
            }

            return string.Empty;
        }
    }
}