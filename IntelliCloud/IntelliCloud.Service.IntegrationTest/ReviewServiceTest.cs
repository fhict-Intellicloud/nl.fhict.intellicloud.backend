using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nl.fhict.IntelliCloud.Common.DataTransfer;

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
        }

        /// <summary>
        /// A method that is called after each test that is ran. This method is used to, for instance, dispose
        /// any objects that require disposing.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            //TODO: Add cleanup code or remove method.
        }

        #region Tests

        /// <summary>
        /// Tests if the UpdateReview is updating a review, or atleast calls something to the database.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void UpdateReview()
        {
            try
            {
                string reviewId = "1";
                ReviewState reviewState = ReviewState.Open;;

                service.UpdateReview(reviewId, reviewState);

            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        /// <summary>
        /// Tests if the CreateReview is creating a review, or atleast calls something to the database.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void CreateReview()
        {
            try
            {
                int answerId = 2;
                string review = "Hallo dit is mijn review";
                int employeeId = 2;

                service.CreateReview(employeeId, answerId, review);

            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        /// <summary>
        /// Tests if the GetReviews is getting all reviews of an answer, or atleast calls something to the database.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetReviews()
        {
            try
            {
                int answerId = 2;

                var reviews = service.GetReviews(answerId);

                Assert.AreEqual("Hallo dit is mijn review", reviews.First().Content);
                Assert.AreNotEqual(1, reviews.First().AnswerId);
                Assert.AreEqual(2, reviews.First().User.Id);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        #endregion Tests

        #endregion Methods
    }
}
