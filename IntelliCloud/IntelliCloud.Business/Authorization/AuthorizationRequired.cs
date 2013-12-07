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
    /// Attribute used to check if a method can be executed.
    /// </summary>
    public class AuthorizationRequired : Attribute, IOperationBehavior, IParameterInspector
    {
        /// <summary>
        /// The UserType required to allow execution of the method.
        /// </summary>
        private UserType[] allowedUserTypes;

        /// <summary>
        /// Constructor that sets the allowed UserTypes for execution of the method.
        /// </summary>
        /// <param name="allowedUserTypes">The allowed UserTypes for execution of the method.</param>
        public AuthorizationRequired(params UserType[] allowedUserTypes)
        {
            this.allowedUserTypes = allowedUserTypes;
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

            // Check if an authorization token has been supplied
            if (!String.IsNullOrWhiteSpace(authorizationToken))
            {
                // Start the authorization process
                AuthorizationHandler authorizationHandler = new AuthorizationHandler(authorizationToken, this.allowedUserTypes);

                // Check if the user is authenticated and authorized to execute the method
                if (!authorizationHandler.IsAuthenticated)
                    throw new WebFaultException(HttpStatusCode.Unauthorized);
                else if (!authorizationHandler.IsAuthorized)
                    throw new WebFaultException(HttpStatusCode.Forbidden);
            }
            else
            {
                throw new WebFaultException(HttpStatusCode.Unauthorized);
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