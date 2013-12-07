using nl.fhict.IntelliCloud.Data.OpenID.Model;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace nl.fhict.IntelliCloud.Data.OpenID.Context
{
    /// <summary>
    /// Class used for retrieving user info from an OpenID provider.
    /// </summary>
    public class OpenIDContext : IOpenIDContext
    {
        /// <summary>
        /// Method for retrieving available user information from the Access Token issuer.
        /// </summary>
        /// <param name="endpointUrl">OpenID Connect Basic UserInfo Endpoint URL of the Access Token issuer.</param>
        /// <param name="token">Instance of class AuthorizationToken.<param>
        /// <returns>Instance of class OpenIDUserInfo.</returns>
        public OpenIDUserInfo RetrieveUserInfo(string endpointUrl, AuthorizationToken token)
        {
            // Get available user information from the Access Token issuer.
            string requestUrl = String.Format(endpointUrl, token.AccessToken);
            WebRequest request = WebRequest.Create(requestUrl);
            request.Timeout = 30000;
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
                parsedUserInfo.Issuer = token.Issuer;

                return parsedUserInfo;
            }
        }
    }
}
