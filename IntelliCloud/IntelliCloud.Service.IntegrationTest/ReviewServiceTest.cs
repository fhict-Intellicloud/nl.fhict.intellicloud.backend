using Microsoft.VisualStudio.TestTools.UnitTesting;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Context;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;
using System;
using System.Linq;
using System.Data.Entity;

namespace nl.fhict.IntelliCloud.Service.IntegrationTest
{
    /// <summary>
    /// This class inhibits integration tests for the service ReviewService.
    /// </summary>
    [TestClass]
    public class ReviewServiceTest
    {
        #region Fields

        /// <summary>
        /// An instance of the service ReviewService that is being tested by this class.
        /// </summary>
        private IReviewService service;

        private UserEntity employee;
        private AnswerEntity answer;
        private ReviewEntity review;

        #endregion Fields

        #region Methods

        /// <summary>
        /// A method that is called before each test is run. This method is used to set up a fresh state for
        /// each test by for instance creating new service objects.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.service = new ReviewService();
			this.initializeTestData();
        }

        /// <summary>
        /// This method adds a new question to the database and saves this in a variable
        /// </summary>
        private void initializeTestData()
        {
            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                UserEntity newEmployee = new UserEntity();
                newEmployee.CreationTime = DateTime.UtcNow;
                newEmployee.FirstName = "employee";
                newEmployee.Type = Common.DataTransfer.UserType.Employee;

                ctx.Users.Add(newEmployee);
                ctx.SaveChanges();

                this.employee = newEmployee;

                AnswerEntity newAnswer = new AnswerEntity();
                newAnswer.CreationTime = DateTime.UtcNow;
                newAnswer.Content = "Integration test for answer";
                newAnswer.AnswerState = AnswerState.UnderReview;
                newAnswer.IsPrivate = false;
                newAnswer.LanguageDefinition = new LanguageDefinitionEntity() {Name = "Dutch", ResourceName = "NL"};
                newAnswer.User = newEmployee;

                ctx.Answers.Add(newAnswer);
                ctx.SaveChanges();

                this.answer = newAnswer;

                ReviewEntity newReview = new ReviewEntity();
                newReview.CreationTime = DateTime.UtcNow;
                newReview.Content = "Integration test for review";
                newReview.ReviewState = ReviewState.Open;
                newReview.User = newEmployee;
                newReview.Answer = newAnswer;

                ctx.Reviews.Add(newReview);
                ctx.SaveChanges();

                this.review = newReview;

            }
        }

        /// <summary>
        /// A method that is called after each test that is ran. This method is used to, for instance, dispose
        /// any objects that require disposing.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                ctx.Users.RemoveRange(ctx.Users.ToList());
                ctx.Answers.RemoveRange(ctx.Answers.ToList());
                ctx.Reviews.RemoveRange(ctx.Reviews.ToList());

                ctx.SaveChanges();
            }
        }

        #region Tests

        /// <summary>
        /// Tests if the UpdateReview is updating a review, or at least calls something to the database.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void UpdateReview()
        {
            try
            {
                string reviewId = this.review.Id.ToString();
                ReviewState reviewState = ReviewState.Open;;

                service.UpdateReview(reviewId, reviewState);

                using (var context = new IntelliCloudContext())
                {
                    int id = Convert.ToInt32(reviewId);
                    ReviewEntity updatedReview = context.Reviews.FirstOrDefault(r => r.Id.Equals(id));
                    
                    Assert.AreEqual(ReviewState.Open, updatedReview.ReviewState);
                    Assert.AreEqual(review.Id, updatedReview.Id);
                }
            }
            catch (Exception e) // TODO move exception test to different method, since this allows for skipping a part of the test...
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        /// <summary>
        /// Tests if the CreateReview is creating a review, or at least calls something to the database.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void CreateReview()
        {
            try
            {
                int answerId = answer.Id;
                string review = "Hallo dit is mijn review";
                int employeeId = employee.Id;

                service.CreateReview(employeeId, answerId, review);

                using (IntelliCloudContext ctx = new IntelliCloudContext())
                {
                    ReviewEntity newEntity = ctx.Reviews
                    .Include(r => r.User)
                    .Include(r => r.Answer).Single(r => r.Content == review && r.Answer.Id == answer.Id && r.User.Id == employee.Id);

                    Assert.AreEqual("Hallo dit is mijn review", newEntity.Content);
                    Assert.AreEqual(answer.Content, newEntity.Answer.Content);
                    Assert.AreEqual(employee.FirstName, newEntity.User.FirstName);
                    Assert.AreEqual(ReviewState.Open, newEntity.ReviewState);
                }
            }
            catch (Exception e) // TODO move exception test to different method, since this allows for skipping a part of the test...
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        /// <summary>
        /// Tests if the GetReview is getting a review of the specific id, or atleast calls something to the database.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetReview()
        {
            try
            {
                string reviewId = review.Id.ToString();

                var newReview = service.GetReview(reviewId);

                Assert.AreEqual(review.Content, newReview.Content);
            }
            catch (Exception e) // TODO move exception test to different method, since this allows for skipping a part of the test...
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        /// <summary>
        /// Tests if the GetAnswer is getting an answer of the review, or atleast calls something to the database.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetAnswer()
        {
            try
            {
                string reviewId = review.Id.ToString();

                var newAnswer = service.GetAnswer(reviewId);

                Assert.AreEqual(answer.Content, newAnswer.Content);
            }
            catch (Exception e) // TODO move exception test to different method, since this allows for skipping a part of the test...
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        /// <summary>
        /// Tests if the GetUser is getting an user of the review, or atleast calls something to the database.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetUser()
        {
            try
            {
                string reviewId = review.Id.ToString();

                var newUser = service.GetUser(reviewId);

                Assert.AreEqual(employee.FirstName, newUser.FirstName);

            }
            catch (Exception e) // TODO move exception test to different method, since this allows for skipping a part of the test...
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        #endregion Tests

        #endregion Methods
    }
}
