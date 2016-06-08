namespace Live.Log.Extractor.Web.Models
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Threading;
    using Live.Log.Extractor.Domain;

    /// <summary>
    /// Index Model
    /// </summary>
    public class IndexModel : IValidatableObject 
    {
        /// <summary>
        /// Gets or sets the name of the app.
        /// </summary>
        /// <value>
        /// The name of the app.
        /// </value>
        [DisplayName("Search for Application")]
        public string AppName { get; set; }

        /// <summary>
        /// Gets or sets the client IP.
        /// </summary>
        /// <value>
        /// The client IP.
        /// </value>
        public string ClientIP { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        [DisplayName("Search for Date (DD/MM/YYYY)")]
        [Required(ErrorMessage = "Please Enter")]
        public string Date { get; set; }

        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        /// <value>
        /// The search text.
        /// </value>
        [DisplayName("Search text block Containing")]
        [Required(ErrorMessage="Please select")]
        public string SearchText { get; set; }

        /// <summary>
        /// Gets or sets the search start text.
        /// </summary>
        /// <value>
        /// The search start text.
        /// </value>
        [DisplayName("Search text starting from")]
        public string SearchStartText { get; set; }

        /// <summary>
        /// Gets or sets the search end text.
        /// </summary>
        /// <value>
        /// The search end text.
        /// </value>
        [DisplayName("Search text Ending from")]
        public string SearchEndText { get; set; }

        /// <summary>
        /// Gets or sets the search location.
        /// </summary>
        /// <value>
        /// The search location.
        /// </value>
        [DisplayName("Search at Location")]
        public string SearchLocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is production.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is production; otherwise, <c>false</c>.
        /// </value>
        [DisplayName("Search in Production Servers")]
        public bool IsProduction { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        [DisplayName("Enter File Location to be searched")]
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        [DisplayName("Results")]
        public List<EchannelLog> Results { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IndexModel"/> is sequential.
        /// </summary>
        /// <value>
        ///   <c>true</c> if sequential; otherwise, <c>false</c>.
        /// </value>
        [DisplayName("Search Sequentially")]
        public bool Sequential { get; set; }

        /// <summary>
        /// Gets or sets the result file location.
        /// </summary>
        /// <value>
        /// The result file location.
        /// </value>
        public string ResultFileLocation { get; set; }

        /// <summary>
        /// Gets or sets the steps.
        /// </summary>
        /// <value>
        /// The steps.
        /// </value>
        public ConcurrentQueue<string> Steps { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable logging].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable logging]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableLogging { get; set; }

        /// <summary>
        /// Gets or sets the impersonated user.
        /// </summary>
        /// <value>
        /// The impersonated user.
        /// </value>
        public string ImpersonatedUser { get; set; }

        /// <summary>
        /// Gets or sets the connection id.
        /// </summary>
        /// <value>
        /// The connection id.
        /// </value>
        public string ConnectionId { get; set; }

        /// <summary>
        /// Gets or sets the impersonated user password.
        /// </summary>
        /// <value>
        /// The impersonated user password.
        /// </value>
        public string ImpersonatedUserPassword { get; set; }

        /// <summary>
        /// Gets or sets the impersonated user domain.
        /// </summary>
        /// <value>
        /// The impersonated user domain.
        /// </value>
        public string ImpersonatedUserDomain { get; set; }

        /// <summary>
        /// Gets or sets the result message.
        /// </summary>
        /// <value>
        /// The result message.
        /// </value>
        public string ResultMessage { get; set; }

        /// <summary>
        /// Gets a value indicating whether [error or abort check].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [error or abort check]; otherwise, <c>false</c>.
        /// </value>
        public bool ErrorOrAbortCheck
        {
            get
            {
                return IndexModel.ErrorOrAbort;
            }
        }

        /// <summary>
        /// Gets the process complete message.
        /// </summary>
        public string ProcessCompleteMessage
        {
            get
            {
                return "Process Complete";
            }
        }

        /// <summary>
        /// Gets or sets the cancel token.
        /// </summary>
        /// <value>
        /// The cancel token.
        /// </value>
        public static CancellationToken CancelToken { get; set; }

        /// <summary>
        /// Gets or sets the cancel.
        /// </summary>
        /// <value>
        /// The cancel.
        /// </value>
        public static CancellationTokenSource Cancel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [error or abort].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [error or abort]; otherwise, <c>false</c>.
        /// </value>
        public static bool ErrorOrAbort { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexModel"/> class.
        /// </summary>
        public IndexModel()
        {
            Results = new List<EchannelLog>();
            Steps = new ConcurrentQueue<string>();
        }

        /// <summary>
        /// Determines whether the specified object is valid.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>
        /// A collection that holds failed-validation information.
        /// </returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(SearchStartText) && string.IsNullOrEmpty(SearchEndText))
            {
                yield return new ValidationResult("These fields should not be empty", new string[] { "SearchStartText", "SearchEndText" });
            }
            
            if (!IsProduction)
            {
                if (string.IsNullOrEmpty(FilePath))
                {
                    yield return new ValidationResult("These fields should not be empty", new string[] { "FilePath" });
                }
            }
        }
    }
}