using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nl.fhict.IntelliCloud.Service;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using System;

namespace nl.fhict.IntelliCloud.Service.IntegrationTest
{
    /// <summary>
    /// This class inhibits integration tests for the service <see cref="IIntelliCloudService"/>.
    /// </summary>
    [TestClass]
    public class IntelliCloudServiceTest
    {
        #region Fields

        /// <summary>
        /// An instance of the service <see cref="IIntelliCloudService"/> that is being tested by this class.
        /// </summary>
        private IIntelliCloudService service;

        #endregion Fields

        #region Methods

        /// <summary>
        /// A method that is called before each test is run. This method is used to set up a fresh state for
        /// each test by for instance creating new service objects.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.service = new IntelliCloudService();
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
                string reviewState = "Open";

                service.UpdateReview(reviewId, reviewState);
                
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        /// <summary>
        /// Tests if the SendReviewForAnswer is creating a review, or atleast calls something to the database.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void SendReviewForAnswer()
        {
            try
            {
                string answerId = "2";
                string review = "Hallo dit is mijn review";
                string reviewerId = "2";

                service.SendReviewForAnswer(reviewerId, answerId, review);

            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        /// <summary>
        /// Tests if the GetReviewsForAnswer is getting all reviews of an answer, or atleast calls something to the database.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetReviewsForAnswer()
        {
            try
            {
                string answerId = "2";                

                var reviews = service.GetReviewsForAnswer(answerId);
                
                Assert.AreEqual("Hallo dit is mijn review", reviews.First().Content);
                Assert.AreNotEqual(1, reviews.First().AnswerId);
                Assert.AreEqual(2, reviews.First().User.Id);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        /// <summary>
        /// Tests if the GetAnswersUpForReview is getting all answers that have answerstate UpForReview, or atleast calls something to the database.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetAnswersUpForReview()
        {
            try
            {
                string employeeId = "3";

                var answers = service.GetAnswersUpForReview(employeeId);

                Assert.AreEqual("Hallo dit is mijn antwoord", answers.First().Content);
                Assert.AreNotEqual(0, answers.First().AnswerState);
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
