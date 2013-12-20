using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using nl.fhict.IntelliCloud.Business.Manager;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using System;
using System.Collections.Generic;

namespace nl.fhict.IntelliCloud.Business.UnitTest.Manager
{
    /// <summary>
    /// Test class for the UserManager class.
    /// </summary>
    [TestClass]
    public class UserManagerTest
    {
        #region Fields

        private UserManager manager;
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
            this.manager = new UserManager(this.validation.Object);
        }

        #region Tests

        #region GetUserTests

        /// <summary>
        /// GetUser test method that checks if no validation is performed when no parameters have been supplied.
        /// </summary>
        [TestMethod]
        public void GetUserTest_WithoutParameters()
        {
            string id = null;
            try
            {
                manager.GetUser();
            }
            catch (Exception)
            { }

            // No validation should be performed when no parameters have been supplied
            validation.Verify(v => v.IdCheck(id), Times.Never);
        }

        /// <summary>
        /// GetUser test method that checks if validation is performed when parameters have been supplied.
        /// </summary>
        [TestMethod]
        public void GetUserTest_WithParameters()
        {
            string id = "1";

            try
            {
                manager.GetUser(id);
            }
            catch (Exception)
            { }

            // Validation should be performed when parameters have been supplied
            validation.Verify(v => v.IdCheck(id), Times.Once);
        }

        #endregion GetUserTests

        #region CreateUserTests

        /// <summary>
        /// CreateUser test method that checks if no validation is performed when no parameters have been supplied.
        /// </summary>
        [TestMethod]
        public void CreateUserTest_WithoutParameters()
        {
            UserType userType = UserType.Customer;
            IList<UserSource> sources = new List<UserSource>();
            string firstName = null;
            string infix = null;
            string lastName = null;
            string avatar = null;

            try
            {
                manager.CreateUser(userType, sources, firstName, infix, lastName, avatar);
            }
            catch (Exception)
            {
            }

            // No validation should be performed when no parameters have been supplied
            validation.Verify(v => v.StringCheck(firstName), Times.Never);
            validation.Verify(v => v.StringCheck(infix), Times.Never);
            validation.Verify(v => v.StringCheck(lastName), Times.Never);
            validation.Verify(v => v.StringCheck(avatar), Times.Never);
        }

        /// <summary>
        /// CreateUser test method that checks if validation is performed when parameters have been supplied.
        /// </summary>
        [TestMethod]
        public void CreateUserTest_WithParameters()
        {
            UserType userType = UserType.Customer;
            IList<UserSource> sources = new List<UserSource>();
            string firstName = "Lorem";
            string infix = "ipsum";
            string lastName = "Dolor";
            string avatar = "picture";

            try
            {
                manager.CreateUser(userType, sources, firstName, infix, lastName, avatar);
            }
            catch (Exception)
            {
            }

            // Validation should be performed when parameters have been supplied
            validation.Verify(v => v.StringCheck(firstName), Times.Once);
            validation.Verify(v => v.StringCheck(infix), Times.Once);
            validation.Verify(v => v.StringCheck(lastName), Times.Once);
            validation.Verify(v => v.StringCheck(avatar), Times.Once);
        }

        #endregion CreateUserTests

        #region GetKeywordsTests

        /// <summary>
        /// Test to validate parameters.
        /// </summary>
        [TestMethod]
        public void GetKeywordTest()
        {
            string id = "1";

            try
            {
                manager.GetKeywords(id);
            }
            catch (Exception)
            { }

            // Validation should be performed when parameters have been supplied
            validation.Verify(v => v.IdCheck(id), Times.Once);
        }

        #endregion GetKeywordsTests

        #region GetQuestionsTests

        /// <summary>
        /// Test to validate parameters.
        /// </summary>
        [TestMethod]
        public void GetQuestionsTest()
        {
            string id = "1";

            try
            {
                manager.GetQuestions(id, null);
            }
            catch (Exception)
            { }

            // Validation should be performed when parameters have been supplied
            validation.Verify(v => v.IdCheck(id), Times.Once);
        }

        #endregion GetQuestionsTests

        #region GetFeedbacksTest
        /// <summary>
        /// Test to validate parameters.
        /// </summary>
        [TestMethod]
        public void GetFeedbacksTest()
        {
            string id = "1";

            try
            {
                manager.GetFeedbacks(id, null);
            }
            catch (Exception)
            { }

            // Validation should be performed when parameters have been supplied
            validation.Verify(v => v.IdCheck(id), Times.Once);
        }

        #endregion GetFeedbacksTests

        #region GetReviewsTests

        /// <summary>
        /// Test to validate parameters.
        /// </summary>
        [TestMethod]
        public void GetReviewsTest()
        {
            string id = "1";

            try
            {
                manager.GetKeywords(id);
            }
            catch (Exception)
            { }

            // Validation should be performed when parameters have been supplied
            validation.Verify(v => v.IdCheck(id), Times.Once);
        }

        #endregion GetReviewsTest

        #region AssignKeywordTests
        /// <summary>
        /// Test to validate parameters.
        /// </summary>
        [TestMethod]
        public void AssignKeywordTest()
        {
            string id = "1",
             keyword = "keyword";
            int affinity = 10;

            try
            {
                manager.AssignKeyword(id, keyword, affinity);
            }
            catch (Exception)
            { }

            // Validation should be performed when parameters have been supplied
            validation.Verify(v => v.IdCheck(id), Times.Once);
            validation.Verify(v => v.AffinityCheck(affinity), Times.Once);
            validation.Verify(v => v.StringCheck(keyword), Times.Once);
        }
        #endregion AssignKeywordTest

        #endregion Tests

        #endregion Methods
    }
}
