using nl.fhict.IntelliCloud.Common.CustomException;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Data.Model;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace nl.fhict.IntelliCloud.Business.Authorization
{
    /// <summary>
    /// Class used for user authorization.
    /// </summary>
    public class AuthorizationHandler
    {
        /// <summary>
        /// Property that contains a User object representing the authorized user.
        /// </summary>
        public static User AuthorizedUser { get; set; }

        /// <summary>
        /// Method for parsing the Base64 representation of the JSON object to an instance of AuthorizationToken
        /// </summary>
        /// <param name="authenticationToken">Base64 encoded string of the JSON object (value of the AuthorizationToken HTTP header).<param>
        /// <returns>Instance of class AuthorizationToken.</returns>
        private AuthorizationToken ParseToken(string authorizationToken)
        {
            // Decode the Base64 representation of the JSON object
            byte[] tokenBytes = Convert.FromBase64String(authorizationToken);

            using (MemoryStream stream = new MemoryStream(tokenBytes))
            {
                // Initialize serializer (used for deserializing the JSON representation of the AuthorizationToken)
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(AuthorizationToken));
                AuthorizationToken parsedToken = (AuthorizationToken)jsonSerializer.ReadObject(stream);

                return parsedToken;
            }
        }

        /// <summary>
        /// Method for retrieving available user information from the Access Token issuer.
        /// </summary>
        /// <param name="token">Instance of class AuthorizationToken.<param>
        /// <returns>Instance of class OpenIDUserInfo.</returns>
        private OpenIDUserInfo RetrieveUserInfo(AuthorizationToken token)
        {
            // String that will contain the endpoint URL of the issuer specified in the token
            string userInfoEndpointUrl = "";

            // Get the endpoint URL of the issuer from the context
            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                SourceDefinitionEntity sourceDefinition = context.SourceDefinitions.SingleOrDefault(sd => sd.Name.Equals(token.Issuer));

                if (sourceDefinition == null)
                    throw new NotFoundException("No source definition entity exists for the specified issuer.");

                userInfoEndpointUrl = sourceDefinition.Url;
            }

            // Get available user information from the Access Token issuer.
            string requestUrl = String.Format(userInfoEndpointUrl, token.AccessToken);
            WebRequest request = WebRequest.Create(requestUrl);
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string userInfo = reader.ReadToEnd();
            reader.Close();
            response.Close();

            // Convert the user info string to a byte array for further processing
            byte[] userInfoBytes = Encoding.UTF8.GetBytes(userInfo);

            using (MemoryStream stream = new MemoryStream(userInfoBytes))
            {
                // Initialize serializer (used for deserializing the JSON representation of the AuthorizationToken)
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(OpenIDUserInfo));
                OpenIDUserInfo parsedUserInfo = (OpenIDUserInfo)jsonSerializer.ReadObject(stream);

                return parsedUserInfo;
            }
        }

        /// <summary>
        /// Method for matching users based on the authorization token in the AuthorizationToken HTTP header.
        /// </summary>
        /// <param name="authorizationToken">The authorization token that will be used to match users.</param>
        /// <returns>Instance of class User on success - null if no users could be matched.</returns>
        public User MatchUser(string authorizationToken)
        {
            // User object that will contain the matched User object on success
            User matchedUser = null;

            // Only attempt to match users when an authorization token has been supplied
            if ((authorizationToken != null) && (authorizationToken.Length > 0))
            {
                try
                {
                    // Parse the authorization token and retrieve user info from the Access Token issuer.
                    AuthorizationToken parsedToken = this.ParseToken(authorizationToken);
                    OpenIDUserInfo userInfo = this.RetrieveUserInfo(parsedToken);

                    // Only attempt to match users if the Access Token issuer returned user info
                    if (userInfo != null)
                    {
                        // TODO: add matching logic
                    }
                }
                catch
                {
                    // Ignore all exceptions - the value of the authorizationToken parameter is invalid
                    // Return null since no users could be matched
                }
            }

            // Return the matched User object - null if matching failed
            return matchedUser;
        }
    }
}