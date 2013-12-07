using nl.fhict.IntelliCloud.Common.CustomException;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Data.Model;
using nl.fhict.IntelliCloud.Data.OpenID.Context;
using nl.fhict.IntelliCloud.Data.OpenID.Model;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;

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
        /// Field that contains an instance of class ConvertEntities, used for converting entities.
        /// </summary>
        private ConvertEntities convertEntities;

        /// <summary>
        /// Field that contains an instance of class OpenIDContext, used for retrieving user info from Access Token issuers.
        /// </summary>
        private IOpenIDContext openIDContext;

        /// <summary>
        /// Default constructor.
        /// Immediately starts the authorization process.
        /// </summary>
        /// <param name="authorizationToken">The token that should be used in the authorization process.</param>
        /// <param name="allowedUserTypes">Array of authorized UserTypes.</param>
        public AuthorizationHandler(string authorizationToken, UserType[] allowedUserTypes)
        {
            this.convertEntities = new ConvertEntities();
            this.openIDContext = new OpenIDContext();

            // Start the authorization process
            this.Authorize(authorizationToken, allowedUserTypes);
        }

        /// <summary>
        /// Constructor that sets the instance IOpenIDContext.
        /// Immediately starts the authorization process.
        /// </summary>
        /// <param name="authorizationToken">The token that should be used in the authorization process.</param>
        /// <param name="allowedUserTypes">Array of authorized UserTypes.</param>
        public AuthorizationHandler(string authorizationToken, UserType[] allowedUserTypes, IOpenIDContext openIDContext)
        {
            this.convertEntities = new ConvertEntities();
            this.openIDContext = openIDContext;

            // Start the authorization process
            this.Authorize(authorizationToken, allowedUserTypes);
        }

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
            string endpointUrl = "";

            // Get the endpoint URL of the issuer from the context
            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                SourceDefinitionEntity sourceDefinition = context.SourceDefinitions.SingleOrDefault(sd => sd.Name.Equals(token.Issuer));

                if (sourceDefinition == null)
                    throw new NotFoundException("No source definition entity exists for the specified issuer.");

                endpointUrl = sourceDefinition.Url;
            }

            // Get available user information from the Access Token issuer.
            return this.openIDContext.RetrieveUserInfo(endpointUrl, token);
        }

        /// <summary>
        /// Method for matching a user based on an instance of class OpenIDUserInfo.
        /// </summary>
        /// <param name="userInfo">The instance of class OpenIDUserInfo that will be used to match a user.</param>
        /// <returns>Instance of class User on success.</returns>
        private User MatchUser(OpenIDUserInfo userInfo)
        {
            // User object that will contain the matched User object on success
            User matchedUser = null;

            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // Get the user entity from the context
                UserEntity userEntity = context.Users
                                        .Include(u => u.Sources.Select(s => s.SourceDefinition))
                                        .SingleOrDefault(s => s.Sources.Any(a => (a.SourceDefinition.Name == "Mail" && a.Value == userInfo.Email) || (a.SourceDefinition.Name == userInfo.Issuer && a.Value == userInfo.Sub)));

                // Only continue if the user entity was found
                if (userEntity != null)
                {
                    // Update the user's first name and last name
                    userEntity.FirstName = userInfo.GivenName;
                    userEntity.LastName = userInfo.FamilyName;

                    // Update the user's id from the issuer
                    userEntity.Sources.Where(s => s.SourceDefinition.Name == userInfo.Issuer)
                                      .Select(s => { s.Value = userInfo.Sub; return s; });

                    // Save the changes to the context
                    context.SaveChanges();

                    // Convert the UserEntity to an instance of class User and set in the reference
                    matchedUser = this.convertEntities.UserEntityToUser(userEntity);
                }
            }

            // Return the matched User object
            return matchedUser;
        }

        /// <summary>
        /// Method for creating a new user based on an instance of class OpenIDUserInfo.
        /// </summary>
        /// <param name="userInfo">The instance of class OpenIDUserInfo that will be used to create the new user.</param>
        /// <returns>Instance of class User on success.</returns>
        private User CreateUser(OpenIDUserInfo userInfo)
        {
            // Create a new user based on the retrieved user info
            UserEntity userEntity = null;
            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // Create a new user based on the retrieved user info
                userEntity = new UserEntity()
                {
                    FirstName = userInfo.GivenName,
                    LastName = userInfo.FamilyName,
                    Type = UserType.Customer,
                    CreationTime = DateTime.UtcNow
                };
                context.Users.Add(userEntity);

                // Create a new source for the user's email address
                SourceDefinitionEntity mailSourceDefinition = context.SourceDefinitions.SingleOrDefault(sd => sd.Name.Equals("Mail"));
                SourceEntity mailSourceEntity = new SourceEntity()
                {
                    Value = userInfo.Email,
                    CreationTime = DateTime.UtcNow,
                    SourceDefinition = mailSourceDefinition,
                    User = userEntity,
                };
                context.Sources.Add(mailSourceEntity);

                // Create a new source for the user's id from the issuer
                SourceDefinitionEntity issuerSourceDefinition = context.SourceDefinitions.SingleOrDefault(sd => sd.Name.Equals(userInfo.Issuer));
                SourceEntity issuerSourceEntity = new SourceEntity()
                {
                    Value = userInfo.Sub,
                    CreationTime = DateTime.UtcNow,
                    SourceDefinition = issuerSourceDefinition,
                    User = userEntity,
                };
                context.Sources.Add(issuerSourceEntity);

                // Save the changes to the context
                context.SaveChanges();
            }

            // Convert the UserEntity instance to an instance of class User and return it
            return this.convertEntities.UserEntityToUser(userEntity);
        }

        /// <summary>
        /// Method that checks if the user is authenticated and authorized to execute the method based on the authorization token.
        /// If authorization is optional and the user is not yet authenticated, a new account is created for the user.
        /// </summary>
        /// <param name="authorizationToken">The token that should be used in the authorization process.</param>
        /// <param name="allowedUserTypes">Array of authorized UserTypes.</param>
        public void Authorize(string authorizationToken, UserType[] allowedUserTypes)
        {
            try
            {
                // Parse the authorization token and retrieve user info from the Access Token issuer.
                AuthorizationToken parsedToken = this.ParseToken(authorizationToken);
                OpenIDUserInfo userInfo = this.RetrieveUserInfo(parsedToken);
                User matchedUser = this.MatchUser(userInfo);

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
                        this.CreateUser(userInfo);

                        this.IsAuthenticated = true;
                        this.IsAuthorized = true;
                    }
                }
            }
            catch
            {
                // Ignore all exceptions, failed to verify if the user is authorized to execute the method
            }
        }
    }
}