using System.Runtime.Serialization;

namespace nl.fhict.IntelliCloud.Data.OpenID.Model
{
    /// <summary>
    /// Class that represents the contents of an authorization token sent in the AuthorizationToken HTTP header.
    /// </summary>
    [DataContract]
    public class AuthorizationToken
    {
        /// <summary>
        /// Gets or sets the issuer of the Access Token.
        /// </summary>
        [DataMember(Name = "issuer")]
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the Access Token for retrieving available user information from the issuer.
        /// </summary>
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }
    }
}
