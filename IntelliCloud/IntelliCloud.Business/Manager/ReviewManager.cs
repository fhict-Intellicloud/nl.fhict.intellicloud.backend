using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using nl.fhict.IntelliCloud.Common.CustomException;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Data.Model;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    /// <summary>
    /// A class providing functionality related to answer reviewing.
    /// </summary>
    public class ReviewManager : BaseManager
    {
        /// <summary>
        /// Constructor of the reviewmanager class used when testing.
        /// </summary>
        /// <param name="validation">An instance of the validation object <see cref="Validation.cs"/></param>
        public ReviewManager(IValidation validation)
            : base(validation)
        {
        }

        public ReviewManager()
            : base() { }

        /// <summary>
        /// Updates the review with the given identifier.
        /// </summary>
        /// <param name="reviewId">The identifier of the review that is updated.</param>
        /// <param name="reviewState">The new state of the review.</param>
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

        /// <summary>
        /// Creates a review for an answer.
        /// </summary>
        /// <param name="employeeId">The identifier of the employee who wrote the review.</param>
        /// <param name="answerId">The identifier of the answer this review is written for.</param>
        /// <param name="review">The review that is given.</param>
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

        /// <summary>
        /// Retrieves the reviews for the given answer.
        /// </summary>
        /// <param name="answerId">The identifier of the answer, only reviews for this answer are returned.</param>
        /// <returns>Returns the reviews for the given answer.</returns>
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
