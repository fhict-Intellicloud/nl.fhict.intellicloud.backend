using nl.fhict.IntelliCloud.Common.DataTransfer;
using System;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Linq;

namespace nl.fhict.IntelliCloud.Business.Authorization
{
    /// <summary>
    /// Attribute used to check if a method can be executed.
    /// </summary>
    public class AuthorizationRequired : Attribute, IOperationBehavior, IParameterInspector
    {
        /// <summary>
        /// The UserType required to allow execution of the method.
        /// </summary>
        private UserType[] allowedUserTypes;

        /// <summary>
        /// The AuthorizationHandler used to parse the AuthorizationToken HTTP header value and retrieve user info from the Access Token issuer.
        /// </summary>
        private AuthorizationHandler authorizationHandler;

        /// <summary>
        /// Constructor that sets the allowed UserTypes for execution of the method.
        /// </summary>
        /// <param name="allowedUserTypes">The allowed UserTypes for execution of the method.</param>
        public AuthorizationRequired(params UserType[] allowedUserTypes)
        {
            this.allowedUserTypes = allowedUserTypes;
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

            // Attempt to retrieve user info from the Accept Token issuer
            OpenIDUserInfo userInfo;
            this.authorizationHandler.TryRetrieveUserInfo(authorizationToken, out userInfo);

            // Object of class User that will contain an instance of class User on success or null if no user could be matched
            User matchedUser;

            // Check if a user has been matched - throw a 401 Unauthorized error if no user could be matched
            if (!this.authorizationHandler.TryMatchUser(userInfo, out matchedUser))
            {
                throw new WebFaultException(HttpStatusCode.Unauthorized);
            }
            else
            {
                // Check if the matched user has the correct privileges - throw a 403 Forbidden error if the user has insufficient privileges
                if (this.allowedUserTypes.Contains(matchedUser.Type))
                {
                    // Store the matched User object
                    AuthorizationHandler.AuthorizedUser = matchedUser;
                }
                else
                {
                    throw new WebFaultException(HttpStatusCode.Forbidden);
                }
            }

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