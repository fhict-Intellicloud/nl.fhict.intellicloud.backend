﻿using nl.fhict.IntelliCloud.Common.DataTransfer;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace nl.fhict.IntelliCloud.Service
{
    /// <summary>
    /// An interface for a service providing functionality related to customer feedback.
    /// </summary>
    [ServiceContract]
    public interface IFeedbackService
    {
        /// <summary>
        /// Retrieves the feedback for the given answer.
        /// </summary>
        /// <param name="answerId">The identifier of the answer, only feedback for this answer is returned.</param>
        /// <returns>Returns the feedback for the given answer.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "feedbacks?answerId={answerId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        IList<Feedback> GetFeedbacks(int answerId);

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
        [WebInvoke(Method = "POST",
            UriTemplate = "feedbacks",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void CreateFeedback(string feedback, int answerId, int questionId, FeedbackType feedbackType, string feedbackToken);


        /// <summary>
        /// Updates the feedback with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the feedback that is updated.</param>
        /// <param name="feedbackState">The new state of the feedback.</param>
        [OperationContract]
        [WebInvoke(Method = "PUT",
            UriTemplate = "feedbacks/{id}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void UpdateFeedback(string id, FeedbackState feedbackState);        
    }
}
