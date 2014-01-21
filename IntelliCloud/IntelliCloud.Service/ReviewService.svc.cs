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
        private readonly ReviewManager manager;

        public ReviewService()
        {
            this.manager = new ReviewManager();
        }
        
        public void CreateReview(int employeeId, int answerId, string review)
        {
            this.manager.CreateReview(employeeId, answerId, review);
        }

        public void UpdateReview(string id, ReviewState reviewState)
        {
            this.manager.UpdateReview(id, reviewState);
        }

        public Review GetReview(string id)
        {
            return this.manager.GetReview(id);
        }

        public User GetUser(string id)
        {
            return this.manager.GetUser(id);
        }

        public Answer GetAnswer(string id)
        {
            return this.manager.GetAnswer(id);
        }

        public IList<Review> GetReviews()
        {
            return this.manager.GetReviews();
        }
    }
}
