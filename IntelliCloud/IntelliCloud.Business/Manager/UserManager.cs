using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Common.DataTransfer.Input;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Data.Model;
using nl.fhict.IntelliCloud.Data.OpenID.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Web;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    /// <summary>
    /// Class used for handling service requests related to users.
    /// </summary>
    public class UserManager : BaseManager
    {
        /// <summary>
        /// Constructor that sets the IValidation property to the given value.
        /// </summary>
        /// <param name="validation">IValidation to be set.</param>
        public UserManager(IValidation validation)
            : base(validation)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UserManager()
            : base()
        {
        }

        #region Methods that are not available directly to the UserService

        /// <summary>
        /// Method for retrieving user info using the AuthorizationToken HTTP header.
        /// </summary>
        /// <returns>Instance of class OpenIDUserInfo on success or null on error.</returns>
        public OpenIDUserInfo GetOpenIDUserInfo()
        {
            // OpenIDUserInfo object that will contain the OpenIDUserInfo object on success
            OpenIDUserInfo userInfo = null;

            // Get the value of the AuthorizationToken HTTP header
            IncomingWebRequestContext requestContext = WebOperationContext.Current.IncomingRequest;
            string authorizationToken = requestContext.Headers["AuthorizationToken"];

            // Only continue if an authorization token is available
            if (!String.IsNullOrWhiteSpace(authorizationToken))
            {
                try
                {
                    // Decode the Base64 representation of the JSON object
                    byte[] tokenBytes = Convert.FromBase64String(authorizationToken);

                    // Parse the token
                    AuthorizationToken parsedToken = null;
                    using (MemoryStream stream = new MemoryStream(tokenBytes))
                    {
                        // Initialize serializer (used for deserializing the JSON representation of the AuthorizationToken)
                        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(AuthorizationToken));
                        parsedToken = (AuthorizationToken)jsonSerializer.ReadObject(stream);
                    }

                    // Get the endpoint URL of the issuer from the context
                    string endpointUrl = "";
                    using (IntelliCloudContext context = new IntelliCloudContext())
                    {
                        SourceDefinitionEntity sourceDefinition = context.SourceDefinitions.SingleOrDefault(sd => sd.Name.Equals(parsedToken.Issuer));
                        endpointUrl = sourceDefinition.Url;
                    }

                    // Get available user information from the Access Token issuer
                    userInfo = OpenIDContext.RetrieveUserInfo(endpointUrl, parsedToken);
                }
                catch (Exception)
                {
                    // Ignore all exceptions because we want to return null if no user info could be retrieved
                }
            }

            // Return the OpenIDUserInfo object
            return userInfo;
        }

        /// <summary>
        /// Method for matching a user based on an instance of class OpenIDUserInfo.
        /// </summary>
        /// <param name="userInfo">The instance of class OpenIDUserInfo that will be used to create the new user.</param>
        /// <returns>Instance of class User.</returns>
        public User MatchUser(OpenIDUserInfo userInfo)
        {
            // Create a new list of sources that should be used to match a user
            IList<UserSource> sources = new List<UserSource>();

            // If present, add sources for the OpenID user ID returned by the Access Token issuer and the user's email address
            if (!String.IsNullOrWhiteSpace(userInfo.Sub)) sources.Add(new UserSource() { Name = userInfo.Issuer, Value = userInfo.Sub });
            if (!String.IsNullOrWhiteSpace(userInfo.Email)) sources.Add(new UserSource() { Name = "Mail", Value = userInfo.Email });

            // Attempt to match a user and return it
            return this.GetUser(null, sources);
        }

        /// <summary>
        /// Method for creating a new user (of UserType Customer) based on an instance of class OpenIDUserInfo.
        /// </summary>
        /// <param name="userInfo">The instance of class OpenIDUserInfo that will be used to create the new user.</param>
        /// <returns>Instance of class User on success.</returns>
        public void CreateUser(OpenIDUserInfo userInfo)
        {
            // Create a new list of sources that should be used to match a user
            IList<UserSource> sources = new List<UserSource>();

            // If present, add sources for the OpenID user ID returned by the Access Token issuer and the user's email address
            if (!String.IsNullOrWhiteSpace(userInfo.Sub)) sources.Add(new UserSource() { Name = userInfo.Issuer, Value = userInfo.Sub });
            if (!String.IsNullOrWhiteSpace(userInfo.Email)) sources.Add(new UserSource() { Name = "Mail", Value = userInfo.Email });

            // Check if the retrieved user info contains the first name and last name
            string firstName = (!String.IsNullOrWhiteSpace(userInfo.GivenName)) ? userInfo.GivenName : null;
            string lastName = (!String.IsNullOrWhiteSpace(userInfo.FamilyName)) ? userInfo.FamilyName : null;

            // Attempt to create a new user
            this.CreateUser(UserType.Customer, sources, firstName, null, lastName);
        }

        /// <summary>
        /// Method for retrieving the User object for the currently authorized user.
        /// </summary>
        /// <returns>Instance of class User on success or null on error.</returns>
        public User GetAuthorizedUser()
        {
            // Retrieve user info about the currently authorized user
            OpenIDUserInfo userInfo = this.GetOpenIDUserInfo();

            // Only continue if user info could be retrieved
            if (userInfo != null)
                return this.MatchUser(userInfo);
            else
                return null;
        }

        #endregion

        /// <summary>
        /// Method for getting a user based on it's ID or a list of UserSource instances.
        /// All parameters are optional - if no parameters are provided the currently logged in user will be returned.
        /// </summary>
        /// <param name="id">The ID of the user. Optional.</param>
        /// <param name="sources">A list of UserSource instances. Optional.</param>
        /// <returns>The matched user based on the values of the parameters, otherwise the currently logged in user.</returns>
        public User GetUser(string id = null, IList<UserSource> sources = null)
        {
            // Validate supplied input data
            if (id != null) Validation.IdCheck(id);

            // User object that will contain the User object on success
            User user = null;

            // Check if any input data is supplied
            if (id == null && sources == null)
            {
                // No input data is supplied, return the currently logged in user
                user = this.GetAuthorizedUser();
            }
            else
            {
                // Get the user from the context
                using (IntelliCloudContext context = new IntelliCloudContext())
                {
                    UserEntity userEntity = null;

                    // Build the query
                    var query = context.Users.Include(u => u.Sources.Select(s => s.SourceDefinition));

                    // Check if an id has been supplied
                    if (id != null)
                    {
                        int iId = Convert.ToInt32(id);
                        query = query.Where(u => u.Id == iId);
                        userEntity = query.SingleOrDefault();
                    }

                    // Check if sources have been supplied
                    if (sources != null && sources.Count > 0)
                        userEntity = query.ToList().Where(u => u.Sources.Any(s => sources.Any(us => us.Name == s.SourceDefinition.Name && us.Value == s.Value))).SingleOrDefault();
                    else
                        userEntity = query.SingleOrDefault();

                    // Convert the UserEntity (if found) to an instance of class User and set in the reference
                    if (userEntity != null)
                        user = ConvertEntities.UserEntityToUser(userEntity);
                }
            }

            // Return the User object
            return user;
        }

        /// <summary>
        /// Method used for creating a new user.
        /// </summary>
        /// <param name="type">The type of user. Required.</param>
        /// <param name="sources">A list of UserSource instances. Required (must contain at least one item).</param>
        /// <param name="firstName">The user's first name. Optional.</param>
        /// <param name="infix">The user's infix. Optional.</param>
        /// <param name="lastName">The user's last name. Optional.</param>
        public void CreateUser(UserType type, IList<UserSource> sources, string firstName = null, string infix = null, string lastName = null)
        {
            // Validate supplied input data
            if (firstName != null) Validation.StringCheck(firstName);
            if (infix != null) Validation.StringCheck(infix);
            if (lastName != null) Validation.StringCheck(lastName);

            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // Create a new user based on the retrieved user info
                UserEntity userEntity = new UserEntity()
                {
                    FirstName = firstName,
                    Infix = infix,
                    LastName = lastName,
                    Type = type,
                    CreationTime = DateTime.UtcNow
                };
                context.Users.Add(userEntity);

                // Add all supplied sources to the user
                foreach (UserSource source in sources)
                {
                    // Check if the source is defined
                    Validation.SourceDefinitionExistsCheck(source.Name);

                    // Create a new source for the source definition
                    SourceDefinitionEntity sourceDefinition = context.SourceDefinitions.SingleOrDefault(sd => sd.Name.Equals(source.Name));
                    SourceEntity sourceEntity = new SourceEntity()
                    {
                        Value = source.Value,
                        CreationTime = DateTime.UtcNow,
                        SourceDefinition = sourceDefinition,
                        User = userEntity,
                    };
                    context.Sources.Add(sourceEntity);
                }

                // Save the changes to the context
                context.SaveChanges();
            }
        }
    }
}