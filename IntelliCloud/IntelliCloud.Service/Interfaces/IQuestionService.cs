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
    /// An interface for a service providing functionality related to questions.
    /// </summary>
    [ServiceContract]
    public interface IQuestionService
    {
        /// <summary>
        /// Retrieves the available questions and filtering them using the state.
        /// </summary>
        /// <param name="state">The optional state of the question, only questions with the given state are returned.</param>
        /// <returns>Returns the questions that match the filter.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve questions by state.
        /// </remarks>
        [OperationContract]
        [WebGet(UriTemplate = "questions?state={state}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        IList<Question> GetQuestions(QuestionState? state = null);

        /// <summary>
        /// Retrieve the question with the given identifier.
        /// </summary>
        /// <param name="questionId">The identifier of the question.</param>
        /// <returns>Returns the question with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve questions by identifier.
        /// </remarks>
        [OperationContract]
        [WebGet(UriTemplate = "questions/{questionId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        Question GetQuestion(string questionId);

        /// <summary>
        /// Retrieve the question for this feedback token.
        /// </summary>
        /// <param name="feedbackToken">The feedback token of the question.</param>
        /// <returns>Returns the question with the given feedback token.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "questions/token/{feedbackToken}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationOptional]
        Question GetQuestionByFeedbackToken(string feedbackToken);

        /// <summary>
        /// Retrieve the user that asked the question with the given identifier.
        /// </summary>
        /// <param name="questionId">The identifier of the question.</param>
        /// <returns>Returns the user that asked the question with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve the asker of a question.
        /// </remarks>
        [OperationContract]
        [WebGet(UriTemplate = "questions/{questionId}/asker",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        User GetAsker(string questionId);

        /// <summary>
        /// Retrieve the user that has answered the question with the given identifier.
        /// </summary>
        /// <param name="questionId">The identifier of the question.</param>
        /// <returns>Returns the user that answered the question with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve the answerer of a question.
        /// </remarks>
        [OperationContract]
        [WebGet(UriTemplate = "questions/{questionId}/answerer",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        User GetAnswerer(string questionId);

        /// <summary>
        /// Retrieve the answer that answered the question with the given identifier.
        /// </summary>
        /// <param name="questionId">The identifier of the question.</param>
        /// <returns>Returns the answer that answered the question with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve the answer of a question.
        /// </remarks>
        [OperationContract]
        [WebGet(UriTemplate = "questions/{questionId}/answer",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        Answer GetAnswer(string questionId);

        /// <summary>
        /// Retrieve the keywords that are linked to the question with the given identifier.
        /// </summary>
        /// <param name="questionId">The identifier of the question.</param>
        /// <returns>Returns the keywords that are linked to the question with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve the keywords of a question.
        /// </remarks>
        [OperationContract]
        [WebGet(UriTemplate = "questions/{questionId}/keywords",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        IList<Keyword> GetKeywords(string questionId);

        /// <summary>
        /// Creates a new question.
        /// </summary>
        /// <param name="source">The source from which the question was send, e.g. "Mail", "Facebook" or "Twitter".
        /// </param>
        /// <param name="reference">The identifier for the source, e.g. the username or email address.</param>
        /// <param name="question">The question that was answered.</param>
        /// <param name="title">The title of the question. The title contains a short summary of the question.</param>
        /// <param name="postId">The identifier of the post this question originates from, for example the Facebook post
        /// identifier.</param>
        /// <param name="isPrivate">When <c>true</c> the question is private, otherwise the question is public. Private 
        /// questions are only available to employees and will never be exposed to customers.</param>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "questions",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        [AuthorizationOptional]
        void CreateQuestion(
            string source, string reference, string question, string title, string postId = null, bool isPrivate = false);

        /// <summary>
        /// Updates the question with the given identifier.
        /// </summary>
        /// <param name="questionId">The identifier of the question that is updated.</param>
        /// <param name="employeeId">The identifier of the employee that is going to answer the question.</param>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to update questions.</remarks>
        [OperationContract]
        [WebInvoke(Method = "PUT",
            UriTemplate = "questions/{questionId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        [AuthorizationRequired(UserType.Employee)]
        void UpdateQuestion(string questionId, int employeeId);
    }
}