using nl.fhict.IntelliCloud.Common.DataTransfer;
using System;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;

namespace nl.fhict.IntelliCloud.Business.Authorization
{
    /// <summary>
    /// Attribute used for all methods that do not require authorization.
    /// </summary>
    public class AuthorizationOptional : Attribute, IOperationBehavior, IParameterInspector
    {
        /// <summary>
        /// The AuthorizationHandler used to parse the AuthorizationToken HTTP header value and retrieve user info from the Access Token issuer.
        /// </summary>
        private AuthorizationHandler authorizationHandler;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AuthorizationOptional()
        {
            this.authorizationHandler = new AuthorizationHandler();
        }

        /// <summary>
        /// Method that provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors, security extensions, and other custom extension objects.
        /// </summary>
        /// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param>
        /// <param name="dispatchOperation">The run-time object that exposes customization properties for the operation described by operationDescription.</param>
        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.ParameterInspectors.Add(this);
        }

        /// <summary>
        /// Method that is called before client calls are sent and after service responses are returned.
        /// </summary>
        /// <param name="operationName">The name of the operation.</param>
        /// <param name="inputs">The objects passed to the method by the client.</param>
        /// <returns>The correlation state that is returned as the correlationState parameter in AfterCall - null if the correlation state is not used.</returns>
        public object BeforeCall(string operationName, object[] inputs)
        {
            // Get the value of the AuthorizationToken HTTP header
            IncomingWebRequestContext requestContext = WebOperationContext.Current.IncomingRequest;
            string authorizationToken = requestContext.Headers["AuthorizationToken"];

            // Object of class OpenIDUserInfo that will contain an instance of class OpenIDUserInfo on success or null if no user info could be retrieved
            OpenIDUserInfo userInfo = null;

            // Object of class User that will contain an instance of class User on success or null if no user could be matched
            User matchedUser = null;

            // Check if an authorization token has been supplied
            if (!String.IsNullOrWhiteSpace(authorizationToken))
            {
                // Check if user info could be retrieved
                if (this.authorizationHandler.TryRetrieveUserInfo(authorizationToken, out userInfo))
                {
                    // Try to match a user and set it in the matchedUser reference
                    if (!this.authorizationHandler.TryMatchUser(userInfo, out matchedUser))
                    {
                        // Try to create a new user based on the retrieved user info
                        if (!this.authorizationHandler.TryCreateNewUser(userInfo, out matchedUser))
                        {
                            // Failed to create a new user - throw a 500 Internal Server Error error
                            throw new WebFaultException(HttpStatusCode.InternalServerError);
                        }
                    }
                }
                else
                {
                    // An invalid authorization token has been supplied - throw a 401 Unauthorized error
                    throw new WebFaultException(HttpStatusCode.Unauthorized);
                }
            }

            // Store the matched User object (which may also be a newly created User object if no existing user could be matched)
            AuthorizationHandler.AuthorizedUser = matchedUser;

            // We do not intend to use a correlation state, so we just return null
            return null;
        }

        #region Interface methods that require no implementation

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        public void Validate(OperationDescription operationDescription)
        {
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
        }

        #endregion
    }
}