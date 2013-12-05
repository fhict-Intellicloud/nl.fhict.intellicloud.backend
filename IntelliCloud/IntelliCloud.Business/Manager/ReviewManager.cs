using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using nl.fhict.IntelliCloud.Common.CustomException;
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
            : base() {}

        public void UpdateReview(string reviewId, ReviewState reviewState)
        {
            Validation.IdCheck(reviewId);

            using (var context = new IntelliCloudContext())
            {
                var id = Convert.ToInt32(reviewId);
                ReviewEntity review = context.Reviews.SingleOrDefault(r => r.Id.Equals(id));
                if (review != null)
                {
                    review.ReviewState = reviewState;

                    context.SaveChanges();
                }
                else
                {
                    throw new NotFoundException("Sequence contains no elements");
                }
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
                var answer = context.Answers.SingleOrDefault(q => q.Id.Equals(answerId));
                if (review != null)
                {
                    reviewEntity.Answer = answer;
                }
                else
                {
                    throw new NotFoundException("Sequence contains no elements");
                }

                var user = context.Users.SingleOrDefault(u => u.Id.Equals(employeeId));
                if (user != null)
                {
                    reviewEntity.User = user;
                }
                else
                {
                    throw new NotFoundException("Sequence contains no elements");
                }

                reviewEntity.Content = review;
                reviewEntity.ReviewState = ReviewState.Open;

                context.Reviews.Add(reviewEntity);

                context.SaveChanges();
            }
        }

        public List<Review> GetReviews(int answerId)
        {
            Validation.IdCheck(answerId);

            using (var context = new IntelliCloudContext())
            {

                List<ReviewEntity> reviewEntities = (from r in context.Reviews.Include(r => r.Answer).Include(r => r.User)
                                                     where r.Answer.Id == answerId
                                                     select r).ToList();

                return ConvertEntities.ReviewEntityListToReviewList(reviewEntities);
            }
        }
    }
}
