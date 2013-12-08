using nl.fhict.IntelliCloud.Data.OpenID.Model;

namespace nl.fhict.IntelliCloud.Data.OpenID.Context
{
    /// <summary>
    /// Interface used by the OpenIDContext class (for testing purposes).
    /// </summary>
    public interface IOpenIDContext
    {
        /// <summary>
        /// Method for retrieving available user information from the Access Token issuer.
        /// </summary>
        /// <param name="endpointUrl">OpenID Connect Basic UserInfo Endpoint URL of the Access Token issuer.</param>
        /// <param name="token">Instance of class AuthorizationToken.<param>
        /// <returns>Instance of class OpenIDUserInfo.</returns>
        OpenIDUserInfo RetrieveUserInfo(string endpointUrl, AuthorizationToken token);
    }
}
