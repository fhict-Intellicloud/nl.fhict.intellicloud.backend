using nl.fhict.IntelliCloud.Common.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using nl.fhict.IntelliCloud.Business.Authorization;

namespace nl.fhict.IntelliCloud.Service
{
    /// <summary>
    /// An interface for a service providing functionality related to answers.
    /// </summary>
    [ServiceContract]
    public interface IAnswerService
    {
        /// <summary>
        /// Retrieves all the available answers and filtering them using the answer state.
        /// </summary>
        /// <param name="state">The optional answer state, only answers with this state will be returned.</param>
        /// <param name="search">The optional search string for which possible answers are returned.
        /// </param>
        /// <returns>Returns the answers that match the filter.</returns> 
        /// <remarks>Users of type <see cref="UserType.Customer"/> can only retrieve answers of state 
        /// <see cref="AnswerState.Closed"/>.</remarks>
        [OperationContract]
        [WebGet(UriTemplate = "answers?state={state}&search={search}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationOptional]
        IList<Answer> GetAnswers(AnswerState? state = null, string search = null);

        /// <summary>
        /// Retrieve the answer with the given identifier.
        /// </summary>
        /// <param name="answerId">The identifier of the answer.</param>
        /// <returns>Returns the answer with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve answers by identifier.
        /// </remarks>
        [OperationContract]
        [WebGet(UriTemplate = "answers/{answerId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        Answer GetAnswer(string answerId);

        /// <summary>
        /// Retrieve the answerer of the answer with the given identifier.
        /// </summary>
        /// <param name="answerId">The identifier of the answer.</param>
        /// <returns>Returns the user which created the answer with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve the answerer of an answer.
        /// </remarks>
        [OperationContract]
        [WebGet(UriTemplate = "answers/{answerId}/answerer",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        User GetAnswerer(string answerId);

        /// <summary>
        /// Retrieve the feedbacks for the answer with the given identifier, filtered by state.
        /// </summary>
        /// <param name="answerId">The identifier of the answer.</param>
        /// <param name="state">The optional state of the feedbacks which are returned.</param>
        /// <returns>Returns the feedbacks for the answer with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve the feedbacks of an answer.
        /// </remarks>
        [OperationContract]
        [WebGet(UriTemplate = "answers/{answerId}/feedbacks?state={state}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        IList<Feedback> GetFeedbacks(string answerId, FeedbackState? state = null);

        /// <summary>
        /// Retrieve the reviews for the answer with the given identifier, filtered by state.
        /// </summary>
        /// <param name="answerId">The identifier of the answer.</param>
        /// <param name="state">The optional state of the reviews which are returned.</param>
        /// <returns>Returns the reviews for the answer with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve the reviews of an answer.
        /// </remarks>
        [OperationContract]
        [WebGet(UriTemplate = "answers/{answerId}/reviews?state={state}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        IList<Review> GetReviews(string answerId, ReviewState? state = null);

        /// <summary>
        /// Retrieve the keywords for the answer with the given identifier.
        /// </summary>
        /// <param name="answerId">The identifier of the answer.</param>        
        /// <returns>Returns the keywords for the answer with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve the keywords of an answer.
        /// </remarks>
        [OperationContract]
        [WebGet(UriTemplate = "answers/{answerId}/keywords",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        IList<Keyword> GetKeywords(string answerId);

        /// <summary>
        /// Creates a new answer.
        /// </summary>
        /// <param name="questionId">The identifier of the question which is answered.</param>
        /// <param name="answer">The content of the given answer.</param>
        /// <param name="answererId">The employee who answered the question.</param>
        /// <param name="answerState">The state of the answer.</param>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to create an answer.
        /// </remarks>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "answers",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        [AuthorizationRequired(UserType.Employee)]
        void CreateAnswer(int questionId, string answer, int answererId, AnswerState answerState);

        /// <summary>
        /// Updates the answer with the given identifier.
        /// </summary>
        /// <param name="answerId">The identifier of the answer that is updated.</param>
        /// <param name="answerState">The new state of the answer.</param>
        /// <param name="answer">The new content of the given answer.</param>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to update an answer.
        /// </remarks>
        [OperationContract]
        [WebInvoke(Method = "PUT",
            UriTemplate = "answers/{answerId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        [AuthorizationRequired(UserType.Employee)]
        void UpdateAnswer(string answerId, AnswerState answerState, string answer);
    }
}
