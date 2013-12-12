using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using nl.fhict.IntelliCloud.Business.Manager;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using System;

namespace nl.fhict.IntelliCloud.Business.UnitTest.Manager
{
    /// <summary>
    /// Test class for the FeedbackManager class.
    /// </summary>
    [TestClass]
    public class FeedbackManagerTest
    {
        #region Fields

        private FeedbackManager manager;
        private Mock<IValidation> validation;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Initialization method for the test class.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.validation = new Mock<IValidation>();
            this.manager = new FeedbackManager(this.validation.Object);            
        }

        #region Tests

        /// <summary>
        /// GetFeedbacks test method that checks if validation is performed.
        /// </summary>
        [TestMethod]
        public void GetFeedbacksTest()
        {
            int answerId = 1;

            try
            {
                manager.GetFeedbacks(answerId);
            }
            catch (Exception)
            {
            }

            // Validation should be performed
            validation.Verify(v => v.IdCheck(answerId), Times.Once);
        }

        /// <summary>
        /// CreateFeedback test method that checks if validation is performed.
        /// </summary>
        [TestMethod]
        public void CreateFeedbackTest()
        {
            string feedback = "Lorem ipsum";
            int answerId = 1;
            int questionId = 2;
            FeedbackType feedbackType = FeedbackType.Accepted;
            string feedbackToken = "ABC123";

            try
            {
                manager.CreateFeedback(feedback, answerId, questionId, feedbackType, feedbackToken);
            }
            catch (Exception)
            {
            }

            // Validation should be performed
            validation.Verify(v => v.StringCheck(feedback), Times.Once);
            validation.Verify(v => v.IdCheck(answerId), Times.Once);
            validation.Verify(v => v.IdCheck(questionId), Times.Once);
            validation.Verify(v => v.StringCheck(feedbackToken), Times.Once);
        }

        /// <summary>
        /// UpdateFeedback test method that checks if validation is performed.
        /// </summary>
        [TestMethod]
        public void UpdateFeedbackTest()
        {
            string feedbackId = "1";
            FeedbackState feedbackState = FeedbackState.Closed;

            try
            {
                manager.UpdateFeedback(feedbackId, feedbackState);
            }
            catch (Exception)
            {
            }

            // Validation should be performed
            validation.Verify(v => v.IdCheck(feedbackId), Times.Once);
        }

        #endregion Tests

        #endregion Methods
    }
}
