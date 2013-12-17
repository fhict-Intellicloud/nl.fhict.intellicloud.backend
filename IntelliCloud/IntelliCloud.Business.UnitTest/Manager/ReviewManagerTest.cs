using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using nl.fhict.IntelliCloud.Business.Manager;
using nl.fhict.IntelliCloud.Common.DataTransfer;

namespace nl.fhict.IntelliCloud.Business.UnitTest.Manager
{
    /// <summary>
    /// In this unit test class all methods of the QuestionManager will be tested.
    /// All these test will use mock classes.
    /// </summary>
    [TestClass]
    public class ReviewManagerTest
    {
        #region Fields

        private ReviewManager manager;
        private Mock<IValidation> validation;

        #endregion Fields

        #region Methods

        [TestInitialize]
        public void Initialize()
        {
            validation = new Mock<IValidation>();
            this.manager = new ReviewManager(validation.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        #region Tests

        /// <summary>
        /// In this test we check if the reviewerId and reviewState is being validated in the UpdateReview method.
        /// </summary>
        [TestMethod]
        public void UpdateReviewTest()
        {
            string reviewerId = "1";
            ReviewState reviewState = ReviewState.Open;

            try
            {
                manager.UpdateReview(reviewerId, reviewState);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(reviewerId), Times.Once());
        }

        /// <summary>
        /// In this test we check if the reviewerId, review and answerId is being validated in the CreateReview method.
        /// </summary>
        [TestMethod]
        public void CreateReviewTest()
        {
            int employeeId = 1;
            string review = "This is my review";
            int answerId = 2;

            try
            {
                manager.CreateReview(employeeId, answerId, review);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(answerId), Times.Once());
            validation.Verify(v => v.IdCheck(employeeId), Times.Once());
            validation.Verify(v => v.StringCheck(review), Times.Once());
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
        /// In this test we check if the id is being validated in the GetReview method.
        /// </summary>
        [TestMethod]
        public void GetReviewTest()
        {
            string id = "2";

            try
            {
                manager.GetReview(id);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(id), Times.Once());
        }
        

        #endregion Tests

        #endregion Methods
    }
}
