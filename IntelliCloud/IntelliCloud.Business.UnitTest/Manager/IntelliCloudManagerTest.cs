using System.Configuration;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Runtime.Remoting.Contexts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using nl.fhict.IntelliCloud.Business.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Data.Model;

namespace nl.fhict.IntelliCloud.Business.UnitTest.Manager
{
    /// <summary>
    /// In this unit test class all methods of the IntelliCloudManager will be tested.
    /// All these test will use mock classes.
    /// </summary>
    [TestClass]
    public class IntelliCloudManagerTest
    {
        #region Fields

        private IntelliCloudManager manager;
        private Mock<IValidation> validation;

        #endregion Fields

        #region Methods

        [TestInitialize]
        public void Initialize()
        {
            validation = new Mock<IValidation>();
            this.manager = new IntelliCloudManager(null, validation.Object);            
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
            string reviewState = "Open";

            try
            {
                manager.UpdateReview(reviewerId, reviewState);
            }
            catch (Exception)
            { }
            
            validation.Verify(v => v.IdCheck(reviewerId), Times.Once());
            validation.Verify(v => v.ReviewStateCheck(reviewState), Times.Once());
        }

        /// <summary>
        /// In this test we check if the reviewerId, review and answerId is being validated in the SendReviewForAnswer method.
        /// </summary>
        [TestMethod]
        public void SendReviewForAnswer()
        {
            string reviewerId = "1";
            string review = "This is my review";
            string answerId = "2";

            try
            {
                manager.SendReviewForAnswer(reviewerId, answerId, review);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(answerId), Times.Once());
            validation.Verify(v => v.IdCheck(reviewerId), Times.Once());
            validation.Verify(v => v.StringCheck(review), Times.Once());
        }

        /// <summary>
        /// In this test we check if the answerId is being validated in the GetReviewsForAnswer method.
        /// </summary>
        [TestMethod]
        public void GetReviewsForAnswer()
        {
            string answerId = "2";

            try
            {
                manager.GetReviewsForAnswer(answerId);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(answerId), Times.Once());
        }

        /// <summary>
        /// In this test we check if the employeeId is being validated in the GetAnswersUpForReview method.
        /// </summary>
        [TestMethod]
        public void GetAnswersUpForReview()
        {
            string employeeId = "1";

            try
            {
                manager.GetAnswersUpForReview(employeeId);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(employeeId), Times.Once());
        }

        #endregion Tests

        #endregion Methods
    }
}
