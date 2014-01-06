using System;
using System.Data.Entity;
using System.Linq;
using nl.fhict.IntelliCloud.Common.CustomException;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Context;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;

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
        /// <param name="validation">An instance that implements IValidation.</param>
        public ReviewManager(IValidation validation)
            : base(validation) { }

        /// <summary>
        /// Constructor of the reviewmanager class used when creating services.
        /// </summary>
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
                    review.LastChangedTime = DateTime.UtcNow;

                    context.SaveChanges();
                }
                else
                {
                    throw new NotFoundException("No review entity exists with the specified ID.");
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
        /// Retrieve the answer for the review with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the review.</param>
        /// <returns>Returns the answer for the review with the given identifier.</returns>
        public Answer GetAnswer(string id)
        {
            Validation.IdCheck(id);

            using (var context = new IntelliCloudContext())
            {
                var reviewId = Convert.ToInt32(id);
                ReviewEntity review = context.Reviews.Include(r => r.Answer).SingleOrDefault(r => r.Id.Equals(reviewId));

                if (review == null)
                    throw new NotFoundException("No review entity exists with the specified ID.");                                         

<<<<<<< HEAD
                return review.Answer.AsAnswer();                
=======
                return reviewEntities.AsReviews().ToList();
>>>>>>> upstream/master
            }
        }

        /// <summary>
        /// Retrieve the user that gave the review with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the review.</param>
        /// <returns>Returns the user that gave the review with the given identifier.</returns>
        public User GetUser(string id)
        {
            Validation.IdCheck(id);

            using (var context = new IntelliCloudContext())
            {
                var reviewId = Convert.ToInt32(id);
                ReviewEntity review = context.Reviews.Include(r => r.User).SingleOrDefault(r => r.Id.Equals(reviewId));

                if (review == null)
                    throw new NotFoundException("No review entity exists with the specified ID.");

                return review.User.AsUser();
            }
        }

        /// <summary>
        /// Retrieve the review with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the review.</param>
        /// <returns>Returns the review with the given identifier.</returns>
        public Review GetReview(string id)
        {
            Validation.IdCheck(id);

            using (var context = new IntelliCloudContext())
            {
                var reviewId = Convert.ToInt32(id);
                ReviewEntity review = context.Reviews.SingleOrDefault(r => r.Id.Equals(reviewId));

                if (review == null)
                    throw new NotFoundException("No review entity exists with the specified ID.");

                return review.AsReview();
            }
        }
    }
}
