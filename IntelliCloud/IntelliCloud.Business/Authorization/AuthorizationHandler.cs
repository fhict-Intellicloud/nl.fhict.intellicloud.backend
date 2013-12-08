using nl.fhict.IntelliCloud.Business.Manager;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.OpenID.Model;
using System.Linq;

namespace nl.fhict.IntelliCloud.Business.Authorization
{
    /// <summary>
    /// Class used for user authorization.
    /// </summary>
    public class AuthorizationHandler
    {
        /// <summary>
        /// Property that indicates if the user is authenticated.
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Property that indicates if the authenticated user has sufficient privileges.
        /// </summary>
        public bool IsAuthorized { get; set; }

        /// <summary>
        /// Field that contains an instance of class UserManager, used for matching, creating and updating users.
        /// </summary>
        private UserManager userManager;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AuthorizationHandler()
        {
            this.userManager = new UserManager();
        }

        /// <summary>
        /// Method that checks if the user is authenticated and authorized to execute the method based on the authorization token.
        /// If authorization is optional and the user is not yet authenticated, a new account is created for the user.
        /// </summary>
        /// <param name="allowedUserTypes">Array of authorized UserTypes.</param>
        public void Authorize(UserType[] allowedUserTypes)
        {
            // Get user info using the AuthorizationToken HTTP header
            OpenIDUserInfo userInfo = this.userManager.GetOpenIDUserInfo();

            // Only continue if user info was successfully retrieved from the Access token issuer
            if (userInfo != null)
            {
                // Try to match a user using the user info retrieved from the Access Token issuer
                User matchedUser = this.userManager.MatchUser(userInfo);

                // Set the property that indicates if the user is authenticated
                this.IsAuthenticated = (matchedUser != null);

                // Check if the user is authenticated
                if (this.IsAuthenticated)
                {
                    // The user is authenticated, set the property that indicates if the user is authorized to execute the method
                    this.IsAuthorized = (allowedUserTypes.Count() == 0 || allowedUserTypes.Contains(matchedUser.Type));
                }
                else
                {
                    // The user is not authenticated - check if authorization is optional or if a customer is authorized to execute the method
                    if (allowedUserTypes.Count() == 0 || allowedUserTypes.Contains(UserType.Customer))
                    {
                        // Authorization is optional or a customer is authorized to execute the method, create a new user using the user info retrieved from the Access Token issuer
                        this.userManager.CreateUser(userInfo);

                        // Set the properties that indicate that the user is authenticated and authorized to execute the method
                        this.IsAuthenticated = true;
                        this.IsAuthorized = true;
                    }
                }
            }
        }
    }
}