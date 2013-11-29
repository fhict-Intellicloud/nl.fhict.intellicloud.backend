using nl.fhict.IntelliCloud.Business.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using nl.fhict.IntelliCloud.Common.DataTransfer;

namespace nl.fhict.IntelliCloud.Service
{
    /// <summary>
    /// A service providing functionality related to answer reviewing.
    /// </summary>
    public class ReviewService : IReviewService
    {
        public IList<Review> GetReviews(int answerId)
        {
            throw new NotImplementedException();
        }

        public void CreateReview(int employeeId, int answerId, string review)
        {
            throw new NotImplementedException();
        }

        public void UpdateReview(string id, ReviewState reviewState)
        {
            throw new NotImplementedException();
        }
    }
}
