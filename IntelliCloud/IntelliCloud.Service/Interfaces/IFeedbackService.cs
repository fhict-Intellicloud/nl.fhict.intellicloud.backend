using nl.fhict.IntelliCloud.Common.DataTransfer;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using nl.fhict.IntelliCloud.Business.Authorization;

namespace nl.fhict.IntelliCloud.Service
{
    /// <summary>
    /// An interface for a service providing functionality related to customer feedback.
    /// </summary>
    [ServiceContract]
    public interface IFeedbackService
    {
        /// <summary>
        /// Retrieve the feedback with the given identifier.
        /// </summary>
        /// <param name="feedbackId">The identifier of the feedback.</param>
        /// <returns>Returns the feedback with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve feedback by identifier.
        /// </remarks>
        [OperationContract]
        [WebGet(UriTemplate = "feedbacks/{feedbackId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        Feedback GetFeedback(string feedbackId);

        /// <summary>
        /// Retrieve the user that gave the feedback with the given identifier.
        /// </summary>
        /// <param name="feedbackId">The identifier of the feedback.</param>
        /// <returns>Returns the user that gave the feedback with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve the user that gave the 
        /// feedback.</remarks>
        [OperationContract]
        [WebGet(UriTemplate = "feedbacks/{feedbackId}/user",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        User GetUser(string feedbackId);

        /// <summary>
        /// Retrieve the question for the feedback with the given identifier.
        /// </summary>
        /// <param name="feedbackId">The identifier of the feedback.</param>
        /// <returns>Returns the question for the feedback with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve the question of the 
        /// feedback.</remarks>
        [OperationContract]
        [WebGet(UriTemplate = "feedbacks/{feedbackId}/question",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        Question GetQuestion(string feedbackId);

        /// <summary>
        /// Retrieve the answer for the feedback with the given identifier.
        /// </summary>
        /// <param name="feedbackId">The identifier of the feedback.</param>
        /// <returns>Returns the answer for the feedback with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve the answer of the feedback.
        /// </remarks>
        [OperationContract]
        [WebGet(UriTemplate = "feedbacks/{feedbackId}/answer",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        Answer GetAnswer(string feedbackId);

        /// <summary>
        /// Creates feedback for a answer given to a question. The <see cref="FeedbackType"/> indicates if the
        /// answer was accepted or declined.
        /// </summary>
        /// <param name="feedback">The feedback that is given.</param>
        /// <param name="answerId">The identifier of the answer for which the feedback is given.</param>
        /// <param name="questionId">The identifier of the question for which the feedback is given.</param>
        /// <param name="feedbackType">The feedback type indicating if the answer was accepted or declined.</param>
        /// <param name="feedbackToken">The feedback token is required to provide feedback to answers on a question. It
        /// is used to make sure the user that asked the question is also the user giving the feedback and that feedback
        /// can only be given once.</param>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "feedbacks",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        [AuthorizationOptional]
        void CreateFeedback(string feedback, int answerId, int questionId, FeedbackType feedbackType, string feedbackToken);

        /// <summary>
        /// Updates the feedback with the given identifier.
        /// </summary>
        /// <param name="feedbackId">The identifier of the feedback that is updated.</param>
        /// <param name="feedbackState">The new state of the feedback.</param>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to update the feedback.</remarks>
        [OperationContract]
        [WebInvoke(Method = "PUT",
            UriTemplate = "feedbacks/{feedbackId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        [AuthorizationRequired(UserType.Employee)]
        void UpdateFeedback(string feedbackId, FeedbackState feedbackState);        
    }
}
