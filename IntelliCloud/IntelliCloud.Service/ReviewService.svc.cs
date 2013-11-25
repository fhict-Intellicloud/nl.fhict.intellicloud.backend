using nl.fhict.IntelliCloud.Business.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace nl.fhict.IntelliCloud.Service
{
    public class ReviewService : IReviewService
    {
        private readonly IntelliCloudManager manager;

        public ReviewService()
        {
            this.manager = new IntelliCloudManager();            
        }

        public void SendReviewForAnswer(string reviewerId, string answerId, string review)
        {
            manager.SendReviewForAnswer(reviewerId, answerId, review);
        }

        public void UpdateReview(string reviewId, string reviewState)
        {
            manager.UpdateReview(reviewId, reviewState);
        }

        public List<Common.DataTransfer.Review> GetReviewsForAnswer(string answerId)
        {
            return manager.GetReviewsForAnswer(answerId);
        }
    }
}
