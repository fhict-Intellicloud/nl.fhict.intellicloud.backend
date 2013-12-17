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
    /// An interface for a service providing functionality related to answer reviewing.
    /// </summary>
    [ServiceContract]
    public interface IReviewService
    {
        /// <summary>
        /// Retrieve the review with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the review.</param>
        /// <returns>Returns the review with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve reviews by identifier.
        /// </remarks>
        [OperationContract]
        [WebGet(UriTemplate = "reviews/{id}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        Review GetReview(string id);

        /// <summary>
        /// Retrieve the user that gave the review with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the review.</param>
        /// <returns>Returns the user that gave the review with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve the user that gave the 
        /// review.</remarks>
        [OperationContract]
        [WebGet(UriTemplate = "reviews/{id}/user",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        User GetUser(string id);

        /// <summary>
        /// Retrieve the answer for the review with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the review.</param>
        /// <returns>Returns the answer for the review with the given identifier.</returns>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to retrieve the answer of the review.
        /// </remarks>
        [OperationContract]
        [WebGet(UriTemplate = "reviews/{id}/answer",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [AuthorizationRequired(UserType.Employee)]
        Answer GetAnswer(string id);

        /// <summary>
        /// Creates a review for an answer.
        /// </summary>
        /// <param name="employeeId">The identifier of the employee who wrote the review.</param>
        /// <param name="answerId">The identifier of the answer this review is written for.</param>
        /// <param name="review">The review that is given.</param>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to create a review.</remarks>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "reviews",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        [AuthorizationRequired(UserType.Employee)]
        void CreateReview(int employeeId, int answerId, string review);

        /// <summary>
        /// Updates the review with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the review that is updated.</param>
        /// <param name="reviewState">The new state of the review.</param>
        /// <remarks>Only users of type <see cref="UserType.Employee"/> are able to update a review.</remarks>
        [OperationContract]
        [WebInvoke(Method = "PUT",
            UriTemplate = "reviews/{id}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        [AuthorizationRequired(UserType.Employee)]
        void UpdateReview(string id, ReviewState reviewState);
    }
}
