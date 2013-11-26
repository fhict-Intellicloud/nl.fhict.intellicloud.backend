using nl.fhict.IntelliCloud.Common.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace nl.fhict.IntelliCloud.Service
{
    [ServiceContract]
    public interface IReviewService
    {
        /// <summary>
        /// This method is used to send a review for a specific answer
        /// </summary>
        /// <param name="reviewerId">The Id of the employee who wrote the review</param>
        /// <param name="answerId">The Id of the answer this review is written for</param>
        /// <param name="review">The review text itself</param>
        /// <returns>Return whether the review was recieved by the server</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "PostForAnswer",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void SendReviewForAnswer(String reviewerId, String answerId, String review);

        /// <summary>
        /// This method is used to update the reviewstate of an review.
        /// </summary>
        /// <param name="reviewId">The reviewId of the review that has to be updated</param>
        /// <param name="reviewState">The reviewstate it should be updated to</param>
        /// <returns>Returns whether the update was succesfull or failed</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "Update",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void UpdateReview(String reviewId, String reviewState);

        /// <summary>
        /// This method is used to get all the Reviews written for a specific answer
        /// </summary>
        /// <param name="answerId">The Id of the answer you want to get the reviews for</param>
        /// <returns>Return a list containing all the reviews for this specified answer</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "GetForAnswer/{answerId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<Review> GetReviewsForAnswer(String answerId);

    }
}
