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
    /// <summary>
    /// An interface for a service providing functionality related to answer reviewing.
    /// </summary>
    [ServiceContract]
    public interface IReviewService
    {
        /// <summary>
        /// Retrieves the reviews for the given answer.
        /// </summary>
        /// <param name="answerId">The identifier of the answer, only reviews for this answer are returned.</param>
        /// <returns>Returns the reviews for the given answer.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "reviews?answerId={answerId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        IList<Review> GetReviews(int answerId);

        /// <summary>
        /// Creates a review for an answer.
        /// </summary>
        /// <param name="employeeId">The identifier of the employee who wrote the review.</param>
        /// <param name="answerId">The identifier of the answer this review is written for.</param>
        /// <param name="review">The review that is given.</param>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "reviews",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void CreateReview(int employeeId, int answerId, string review);

        /// <summary>
        /// Updates the review with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the review that is updated.</param>
        /// <param name="reviewState">The new state of the review.</param>
        [OperationContract]
        [WebInvoke(Method = "PUT",
            UriTemplate = "reviews/{id}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void UpdateReview(string id, ReviewState reviewState);        
    }
}
