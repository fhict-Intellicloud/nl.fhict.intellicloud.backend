using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using nl.fhict.IntelliCloud.Business.Authorization;
using nl.fhict.IntelliCloud.Common.DataTransfer;

namespace nl.fhict.IntelliCloud.Service
{
    /// <summary>
    /// An interface for a service providing functionality related to users.
    /// </summary>
    [ServiceContract]
    public interface IUserService
    {
        /// <summary>
        /// Retrieves the users which match the filter.
        /// </summary>
        /// <param name="after">Optional: Only users that are created or modified after this date time are retrieved,
        /// in UTC time.</param>
        /// <returns>Returns the users which match the filter.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve users.</remarks>
        [OperationContract]
        [WebGet(UriTemplate = "users?after={after}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        IList<User> GetUsers(DateTime? after = null);

        /// <summary>
        /// Retrieves the user with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the user.</param>
        /// <returns>Returns the user with the given identifier.</returns>
        /// <remarks>Only users that are logged in can retrieve a user. Only users with type 
        /// <see cref="UserType.Employee"/> are able to retrieve other users than their own.</remarks>
        [OperationContract]
        [WebGet(UriTemplate = "users/{id}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired]
        User GetUser(string id);

        /// <summary>
        /// Retrieves the keywords for the user with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the user.</param>
        /// <returns>Returns the keywords for the user with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve their keywords.</remarks>
        [OperationContract]
        [WebGet(UriTemplate = "users/{id}/keywords",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        IList<Keyword> GetKeywords(string id);

        /// <summary>
        /// Retrieves the questions for the user with the given identifier. The retrieved questions have keywords which
        /// match with one or more keywords of the user. Also questions which don't match with any user are retrieved.
        /// Only questions with state <see cref="QuestionState.Open"/> and <see cref="QuestionState.UpForAnswer"/> are 
        /// returned.
        /// </summary>
        /// <param name="id">The identifier of the user.</param>
        /// <param name="after">Optional: Only questions that are created or modified after this date time are retrieved,
        /// in UTC time.</param>
        /// <returns>Returns the questions for the user with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve their questions.</remarks>
        [OperationContract]
        [WebGet(UriTemplate = "users/{id}/questions?after={after}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        IList<Question> GetQuestions(string id, DateTime? after = null);

        /// <summary>
        /// Retrieves the answers which received feedback for the user with the given identifier. The feedback applies
        /// to an answer which has keywords which match with one or more keywords of the user. Also feedback which don't
        /// match with any user is retrieved. Only answers which have open feedback items are returned.
        /// </summary>
        /// <param name="id">The identifier of the user.</param>
        /// <param name="after">Optional: Only answers that are created or modified after this date time are retrieved,
        /// in UTC time.</param>
        /// <returns>Returns the feedback for the user with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve feedback.</remarks>
        [OperationContract]
        [WebGet(UriTemplate = "users/{id}/feedbacks?after={after}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        IList<Answer> GetFeedbacks(string id, DateTime? after = null);

        /// <summary>
        /// Retrieves the answers that are under review which can be reviewed by the user with the given identifier. 
        /// An answer can be reviewed by a user when one or more of the keywords of the answer match with the keywords
        /// of the user or if the keywords of the answer don't match with any user. Only answers which have open review
        /// items are returned.
        /// </summary>
        /// <param name="id">The identifier of the user.</param>
        /// <param name="after">Optional: Only answers that are created or modified after this date time are retrieved,
        /// in UTC time.</param>
        /// <returns>Returns the reviewable answers for the user with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve reviewable answers.
        /// </remarks>
        [OperationContract]
        [WebGet(UriTemplate = "users/{id}/reviews?after={after}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        IList<Answer> GetReviews(string id, DateTime? after = null);

        /// <summary>
        /// Assign a keyword to the user with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the user.</param>
        /// <param name="keyword">The keyword which is assigned to the user.</param>
        /// <param name="affinity">The affinity the user has with the assigned keyword, on a scale of 1 to 10.</param>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to assign keywords.</remarks>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "users/{id}/keywords",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        [AuthorizationRequired(UserType.Employee)]
        void AssignKeyword(string id, string keyword, int affinity);
    }
}
