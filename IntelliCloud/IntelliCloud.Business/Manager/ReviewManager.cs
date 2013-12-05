using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Data.Model;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    public class ReviewManager : BaseManager
    {
        public ReviewManager(IntelliCloudContext context, IValidation validation)
            : base(context, validation)
        {
            //the intellicloudcontext and validation are given threw here
        }

        public ReviewManager()
        {
            //new static objects are being made in the basemanager
        }

        public void UpdateReview(string reviewId, ReviewState reviewState)
        {
            Validation.IdCheck(reviewId);

            using (var context = new IntelliCloudContext())
            {
                var id = Convert.ToInt32(reviewId);
                ReviewEntity review = context.Reviews.First(r => r.Id.Equals(id));
                review.ReviewState = reviewState;

                context.SaveChanges();
            }
        }

        public void CreateReview(int employeeId, int answerId, string review)
        {
            Validation.IdCheck(answerId);
            Validation.IdCheck(employeeId);
            Validation.StringCheck(review);

            using (var context = new IntelliCloudContext())
            {
                ReviewEntity reviewEntity = new ReviewEntity();
                reviewEntity.Answer = context.Answers.First(q => q.Id.Equals(answerId));
                reviewEntity.Content = review;
                reviewEntity.ReviewState = ReviewState.Open;
                reviewEntity.User = context.Users.First(u => u.Id.Equals(employeeId));

                context.Reviews.Add(reviewEntity);

                context.SaveChanges();
            }
        }

        public List<Review> GetReviews(int answerId)
        {
            Validation.IdCheck(answerId);

            using (var context = new IntelliCloudContext())
            {

                List<ReviewEntity> reviewEntities = (from r in context.Reviews.Include("Answer").Include("User")
                                                     where r.Answer.Id == answerId
                                                     select r).ToList();

                return ConvertEntities.ReviewEntityListToReviewList(reviewEntities);
            }
        }
    }
}
