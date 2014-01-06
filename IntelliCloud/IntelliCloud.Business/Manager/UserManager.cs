using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Context;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;
using nl.fhict.IntelliCloud.Data.OpenID.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Web;
using System.Collections;
using nl.fhict.IntelliCloud.Common.CustomException;

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
            string avatar = (!String.IsNullOrWhiteSpace(userInfo.Picture)) ? userInfo.Picture : null;

            // Attempt to create a new user
            this.CreateUser(UserType.Customer, sources, firstName, null, lastName, avatar);
        }

        /// <summary>
        /// Method used for creating a new user.
        /// </summary>
        /// <param name="type">The type of user. Required.</param>
        /// <param name="sources">A list of UserSource instances. Required (must contain at least one item).</param>
        /// <param name="firstName">The user's first name. Optional.</param>
        /// <param name="infix">The user's infix. Optional.</param>
        /// <param name="lastName">The user's last name. Optional.</param>
        /// <param name="avatar">The user's avatar URL. Optional.</param>
        public void CreateUser(UserType type, IList<UserSource> sources, string firstName = null, string infix = null, string lastName = null, string avatar = null)
        {
            // Validate supplied input data
            if (firstName != null) Validation.StringCheck(firstName);
            if (infix != null) Validation.StringCheck(infix);
            if (lastName != null) Validation.StringCheck(lastName);
            if (avatar != null) Validation.StringCheck(avatar);

            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // Create a new user based on the retrieved user info
                UserEntity userEntity = new UserEntity()
                {
                    FirstName = firstName,
                    Infix = infix,
                    LastName = lastName,
                    Type = type,
                    CreationTime = DateTime.UtcNow,
                    Avatar = avatar
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
                    var query = context.Users
                                .Include(u => u.Sources)
                                .Include(u => u.Sources.Select(s => s.SourceDefinition));

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
                        user = userEntity.AsUser();
                }
            }

            // Return the User object
            return user;
        }
 
        #endregion

        public User GetUser(string userId) {

            // Validate the userId
            base.Validation.IdCheck(userId);

            // Get the user from the context
            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // convert id to int
                int parsedId = Convert.ToInt32(userId);

                // Build the query
                var query = context.Users
                    .Where(u => u.Id == parsedId);

                // execute the query and convert the userEntities to Users.
                return query.Single().AsUser();
            }
        }

        /// <summary>
        /// Retrieves the users which match the filter.
        /// </summary>
        /// <param name="after">Optional: Only users that are created or modified after this date time are retrieved,
        /// in UTC time.</param>
        /// <returns>Returns the users which match the filter.</returns>
        public IList<User> GetUsers(DateTime? after)
        {
            //// Get the user from the context
            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // Build the query
                var query = context.Users;


                // Check if the after filter needs to be applied.
                if (after.Value != null)
                {
                    query.Where(u => after.Value.CompareTo(u.CreationTime) < 0);
                }

                // execute the query and convert the userEntities to Users.
                return query.ToList().AsUsers();
            }
        }

        /// <summary>
        /// Retrieves the keywords for the user with the given identifier.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <returns>Returns the keywords for the user with the given identifier.</returns>
        public IList<Keyword> GetKeywords(string userId)
        {
            // Validate the userId
            base.Validation.IdCheck(userId);

            //// Get the keywords from the context
            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // convert id to int
                int parsedId = Convert.ToInt32(userId);

                // Build the query
                var query = context.UserKeys
                        .Where(uk => uk.User.Id == parsedId)
                        .Select(uk => uk.Keyword);

                // execute the query and convert the keywordsEntities to Keywords.
                return query.ToList().AsKeywords();
            }
        }

        /// <summary>
        /// Retrieves the questions for the user with the given identifier. The retrieved questions have keywords which
        /// match with one or more keywords of the user. Also questions which don't match with any user are retrieved.
        /// Only questions with state <see cref="QuestionState.Open"/> and <see cref="QuestionState.UpForAnswer"/> are 
        /// returned.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="after">Optional: Only questions that are created or modified after this date time are retrieved,
        /// in UTC time.</param>
        /// <returns>Returns the questions for the user with the given identifier.</returns>
        public IList<Question> GetQuestions(string userId, DateTime? after)
        {
            // Validate the userId
            base.Validation.IdCheck(userId);

            //// Get the questions from the context
            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // convert id to int
                int parsedId = Convert.ToInt32(userId);

                // Build the query
                var query = context.Questions
                        // get questions claimed by the given user or unclaimed questions
                        .Where(q => q.Answerer.Id == parsedId || q.Answerer == null)
                        // get only open and up for answer questions
                        .Where(q => q.QuestionState == QuestionState.Open || q.QuestionState == QuestionState.UpForAnswer)
                        // get answers which contain keywords that match one or more of the given users keywords
                        .Where(q => context.QuestionKeys
                                            // get answer keys for the given answer
                                            .Where(qk => qk.Id == q.Id)
                                            // select the name of the keyword which serves as the actual keyword
                                            .Select(qk => qk.Keyword.Name)
                                            // check if any keyword of this answer matches any of the users keywords
                                            .Any(k => k.Equals(context.UserKeys
                                                                       // get only user keywords for the given user
                                                                       .Where(u => u.Id == parsedId)
                                                                       // get any of the users keywords and match them
                                                                       .Any()
                                                               )
                                                )
                                            // or get answers that have keywords which don't match any of all the user keywords
                                            || !context.AnswerKeys
                                                        // get answer keys for the given answer
                                                        .Where(ak => ak.Id == q.Id)
                                                        // select the name of the keyword which serves as the actual keyword
                                                        .Select(ak => ak.Keyword.Name)
                                                        // check if any keyword of this answer matches any of the users keywords
                                                        .Any(k => k.Equals(context.UserKeys
                                                                                  // get any of the users keywords and match them
                                                                                  .Any()
                                                                          )
                                                           )
                                );


                // optionallty get only questions after certain date
                if (after.Value != null)
                    query.Where(q => after.Value.CompareTo(q.CreationTime) < 0);

                // execute the query and convert the questionEntities to Questions.
                return query.ToList().AsQuestions();
            }
        }

        /// <summary>
        /// Retrieves the answers which received feedback for the user with the given identifier. The feedback applies
        /// to an answer which has keywords which match with one or more keywords of the user. Also feedback which don't
        /// match with any user is retrieved. Only answers which have open feedback items are returned.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="after">Optional: Only answers that are created or modified after this date time are retrieved,
        /// in UTC time.</param>
        /// <returns>Returns the feedback for the user with the given identifier.</returns>
        public IList<Answer> GetFeedbacks(string userId, DateTime? after)
        {
            // Validate the userId
            base.Validation.IdCheck(userId);

            //// Get the answers from the context
            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // convert id to int
                int parsedId = Convert.ToInt32(userId);

                // Build the query
                var query  = context.Feedbacks
                                    // get only open feedback items
                                    .Where(f => f.FeedbackState == FeedbackState.Open)
                                    // select the answers for the open feedback items
                                    .Select(f => f.Answer)
                                    // get only the answers for the given user
                                    .Where(a => a.Id == parsedId)
                                    // get answers which contain keywords that match one or more of the given users keywords
                                    .Where(a => context.AnswerKeys
                                                        // get answer keys for the given answer
                                                        .Where(ak => ak.Id == a.Id)
                                                        // select the name of the keyword which serves as the actual keyword
                                                        .Select(ak => ak.Keyword.Name)
                                                        // check if any keyword of this answer matches any of the users keywords
                                                        .Any(k => k.Equals(context.UserKeys
                                                                                   // get only user keywords for the given user
                                                                                   .Where(u => u.Id == parsedId)
                                                                                   // get any of the users keywords and match them
                                                                                   .Any()
                                                                          )
                                                           )   
                                            // or get answers that have keywords which don't match any of all the user keywords
                                            || !context.AnswerKeys
                                                        // get answer keys for the given answer
                                                        .Where(ak => ak.Id == a.Id)
                                                        // select the name of the keyword which serves as the actual keyword
                                                        .Select(ak => ak.Keyword.Name)
                                                        // check if any keyword of this answer matches any of the users keywords
                                                        .Any(k => k.Equals(context.UserKeys
                                                                                  // get any of the users keywords and match them
                                                                                  .Any()
                                                                          )
                                                           )
                                           );


                // optionallty get only answers after certain date
                if (after.Value != null)
                    query.Where(a => after.Value.CompareTo(a.CreationTime) < 0);

                // execute the query and convert the questionEntities to Questions.
                return query.ToList().AsAnswers();
            }
        }

        /// <summary>
        /// Retrieves the answers that are under review which can be reviewed by the user with the given identifier. 
        /// An answer can be reviewed by a user when one or more of the keywords of the answer match with the keywords
        /// of the user or if the keywords of the answer don't match with any user. Only answers which have open review
        /// items are returned.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="after">Optional: Only answers that are created or modified after this date time are retrieved,
        /// in UTC time.</param>
        /// <returns>Returns the reviewable answers for the user with the given identifier.</returns>
        public IList<Answer> GetReviews(string userId, DateTime? after)
        {
            // Validate the userId
            base.Validation.IdCheck(userId);

            //// Get the answers from the context
            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // convert id to int
                int parsedId = Convert.ToInt32(userId);

                // Build the query
                var query = context.Reviews
                    // get only open review items
                    .Where(r => r.ReviewState == ReviewState.Open)
                    // select the answers for the open feedbacks
                    .Select(r => r.Answer)
                    // get only the answers for the given user
                    .Where(a => a.Id == parsedId)
                    // get only answer that are under review.
                    .Where(a => a.AnswerState == AnswerState.UnderReview)
                    // get answers which contain keywords that match one or more of the given users keywords
                    .Where(a => context.AnswerKeys
                                // get answer keys for the given answer
                                .Where(ak => ak.Id == a.Id)
                                // select the name of the keyword which serves as the actual keyword
                                .Select(ak => ak.Keyword.Name)
                                // check if any keyword of this answer matches any of the users keywords
                                .Any(k => k.Equals(context.UserKeys
                                    // get only user keywords for the given user
                                                          .Where(u => u.Id == parsedId)
                                    // get any of the users keywords and match them
                                                          .Any()
                                                   )
                                    )
                            // or get answers that have keywords which don't match any of all the user keywords
                            || !context.AnswerKeys
                                // get answer keys for the given answer
                                .Where(ak => ak.Id == a.Id)
                                // select the name of the keyword which serves as the actual keyword
                                .Select(ak => ak.Keyword.Name)
                                // check if any keyword of this answer matches any of the users keywords
                                .Any(k => k.Equals(context.UserKeys
                                                          // get any of the users keywords and match them
                                                          .Any()
                                                   )
                                    )
                           );

                // optionallty get only answers after certain date
                if (after.Value != null)
                    query.Where(a => after.Value.CompareTo(a.CreationTime) < 0);

                // execute the query and convert the questionEntities to Questions.
                return query.ToList().AsAnswers();
            }
        }

        /// <summary>
        /// Assign a keyword to the user with the given identifier.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="keyword">The keyword which is assigned to the user.</param>
        /// <param name="affinity">The affinity the user has with the assigned keyword, on a scale of 1 to 10.</param>
        public void AssignKeyword(string userId, string keyword, int affinity)
        {
            // validate the given id
            base.Validation.IdCheck(userId);
            // validate given keyword string
            base.Validation.StringCheck(keyword);
            // validate given affinity level
            base.Validation.AffinityCheck(affinity);

            // Assign keyword to a user
            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // convert id to int
                int parsedId = Convert.ToInt32(userId);

                UserEntity user;

                try
                {
                    // get user which gets new key
                    user = context.Users
                        .Where(u => u.Id == parsedId)
                        .Single();
                }
                catch (InvalidOperationException)
                {
                    throw new NotFoundException("No user found with the id: " + parsedId);
                }

                // create the new keyword entity
                KeywordEntity keywordEntity = new KeywordEntity()
                {
                    CreationTime = DateTime.UtcNow,
                    Name = keyword
                };

                // add and save new keyword
                context.Keywords.Add(keywordEntity);
                context.SaveChanges();

                // create new user to keyword match
                UserKeyEntity entity = new UserKeyEntity()
                {
                    CreationTime = DateTime.UtcNow,
                    Affinity = affinity,
                    User = user,
                    Keyword = keywordEntity
                };

                // add and save new user to keyword match
                context.UserKeys.Add(entity);
                context.SaveChanges();
            }
        }
    }
}