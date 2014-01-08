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

        /// <summary>
        /// In this test we check if the id is being validated in the GetAnswer method.
        /// </summary>
        [TestMethod]
        public void GetAnswerTest()
        {
            string id = "2";

            try
            {
                manager.GetAnswer(id);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(id), Times.Once());
        }

        /// <summary>
        /// In this test we check if the id is being validated in the GetUser method.
        /// </summary>
        [TestMethod]
        public void GetUserTest()
        {
            string id = "2";

            try
            {
                manager.GetUser(id);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(id), Times.Once());
        }

        /// <summary>
        /// In this test we check if the id is being validated in the GetQuestion method.
        /// </summary>
        [TestMethod]
        public void GetQuestionTest()
        {
            string id = "2";

            try
            {
                manager.GetQuestion(id);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(id), Times.Once());
        }

        /// <summary>
        /// In this test we check if the id is being validated in the GetFeedback method.
        /// </summary>
        [TestMethod]
        public void GetFeedbackTest()
        {
            string id = "2";

            try
            {
                manager.GetFeedback(id);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(id), Times.Once());
        }

        #endregion Tests

        #endregion Methods
    }
}
