namespace Live.Log.Extractor.Web.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;

    public class LoginDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        [DisplayName("Mainframe UserId")]
        [Required(ErrorMessage = "Please enter Mainframe UserId.")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [DisplayName("Mainframe Password")]
        [Required(ErrorMessage = "Please enter Mainframe Password.")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the login query.
        /// </summary>
        /// <value>
        /// The login query.
        /// </value>
        public string LoginQuery 
        { 
            get 
            {
                return ConfigurationManager.AppSettings["LoginQuery"];
            } 
        }

        /// <summary>
        /// Gets or sets the login region.
        /// </summary>
        /// <value>
        /// The login region.
        /// </value>
        public string LoginRegion 
        { 
            get 
            {
                return ConfigurationManager.AppSettings["LoginRegion"];
            }
        }
    }
}