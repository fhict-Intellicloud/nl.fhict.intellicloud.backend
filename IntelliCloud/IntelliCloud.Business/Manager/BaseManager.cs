using nl.fhict.IntelliCloud.Business.Authorization;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Context;
using System.ServiceModel.Web;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    /// <summary>
    /// Other managers will inherit this BaseManager.
    /// This manager has the Validation and ConvertEntities class. Also the IntelliCloudContext for tests.
    /// </summary>
    public abstract class BaseManager
    {
        protected IValidation Validation {get; set;}
        protected ConvertEntities ConvertEntities {get; set;}
        protected IntelliCloudContext IntelliCloudContext { get; set; }
        protected AuthorizationHandler AuthorizationHandler { get; set; }

        /// <summary>
        /// This constructor will construct the BaseManager and instantiate it's properties.
        /// The IntelliCloudContext and IValidation properties are set to the given values.
        /// </summary>
        /// <param name="context">IntelliCloudContext to be set.</param>
        /// <param name="validation">IValidation to be set.</param>
        protected BaseManager(IntelliCloudContext context, IValidation validation)
        {
            Validation = validation;
            ConvertEntities = new ConvertEntities();
            IntelliCloudContext = context;
            AuthorizationHandler = new AuthorizationHandler();
        }

        /// <summary>
        /// This constructor will construct the BaseManager and instantiate it's properties.
        /// </summary>
        protected BaseManager()
        {
            Validation = new Validation();
            ConvertEntities = new ConvertEntities();
            IntelliCloudContext = new IntelliCloudContext();
            AuthorizationHandler = new AuthorizationHandler();
        }

        /// <summary>
        /// Method used to get the authorized user (determined using the AuthorizationToken HTTP header).
        /// </summary>
        /// <returns>Instance of class User or null if no user could be matched.</returns>
        protected User GetAuthorizedUser()
        {
            // User object that will contain the matched user
            User user = null;

            // Get the value of the AuthorizationToken HTTP header
            IncomingWebRequestContext requestContext = WebOperationContext.Current.IncomingRequest;
            string authorizationToken = requestContext.Headers["AuthorizationToken"];

            if (authorizationToken != null)
            {
                // TODO: process authorization token
            }

            // Return the matched user - null if no user could be matched
            return user;
        }
    }
}
