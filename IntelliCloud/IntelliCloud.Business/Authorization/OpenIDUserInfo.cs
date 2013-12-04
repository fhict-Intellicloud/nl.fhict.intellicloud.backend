using nl.fhict.IntelliCloud.Common.CustomException;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Data.Model;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace nl.fhict.IntelliCloud.Business.Authorization
{
    /// <summary>
    /// Class that represents user information in the OpenID Connect schema.
    /// </summary>
    [DataContract]
    public class OpenIDUserInfo
    {
        /// <summary>
        /// Gets or sets the identifier/subject of the user.
        /// </summary>
        [DataMember(Name = "sub")]
        public string Sub { get; set; }

        /// <summary>
        /// Gets or sets the full name of the user.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        [DataMember(Name = "given_name")]
        public string GivenName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        [DataMember(Name = "family_name")]
        public string FamilyName { get; set; }

        /// <summary>
        /// Gets or sets the profile URL of the user.
        /// </summary>
        [DataMember(Name = "profile")]
        public string Profile { get; set; }

        /// <summary>
        /// Gets or sets the picture URL of the user.
        /// </summary>
        [DataMember(Name = "picture")]
        public string Picture { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        [DataMember(Name = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets whether the email address of the user has been verified.
        /// </summary>
        [DataMember(Name = "email_verified")]
        public bool EmailVerified { get; set; }

        /// <summary>
        /// Gets or sets the gender of the user.
        /// </summary>
        [DataMember(Name = "gender")]
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the locale of the user.
        /// </summary>
        [DataMember(Name = "locale")]
        public string Locale { get; set; }

        /// <summary>
        /// Method for retrieving available user information from the Access Token issuer.
        /// </summary>
        /// <param name="token">Instance of class AuthorizationToken.<param>
        /// <returns>Instance of class OpenIDUserInfo.</returns>
        public static OpenIDUserInfo Retrieve(AuthorizationToken token)
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
    }
}
