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
    /// All these test will use mock data.
    /// </summary>
    [TestClass]
    public class IntelliCloudManagerTest
    {
        #region Fields

        private IntelliCloudManager manager;
        private Mock<IValidation> validation;
        private List<AnswerEntity> answers;
        private List<ReviewEntity> reviews;
        private List<QuestionEntity> questions;
        private List<UserEntity> users;

        #endregion Fields

        #region Methods

        [TestInitialize]
        public void Initialize()
        {
            validation = new Mock<IValidation>();
            this.manager = new IntelliCloudManager(null, validation.Object);

            //add test data 
            //TODO: if not necassary, delete this
            users = new List<UserEntity>();
            users.Add(new UserEntity() { CreationTime = DateTime.Now, FirstName = "Geert", Id = 0, Infix = "van", LastName = "Hoesel", Type = UserType.Customer});
            users.Add(new UserEntity() { CreationTime = DateTime.Now, FirstName = "Teun", Id = 1, Infix = "van", LastName = "Gisbergen", Type = UserType.Employee });

            answers = new List<AnswerEntity>();
            answers.Add(new AnswerEntity() { AnswerState = AnswerState.UnderReview, Content  = "This is the answer", CreationTime = DateTime.Now, Id = 0, User = users.First()});
            
            questions = new List<QuestionEntity>();
            questions.Add(new QuestionEntity() { Content = "This is the question?", CreationTime = DateTime.Now, Id = 0, QuestionState = QuestionState.UpForAnswer, Answer = answers.First(), User = users.First()});

            reviews = new List<ReviewEntity>();
            reviews.Add(new ReviewEntity() { Answer = answers.First(), Content = "This is the review", CreationTime = DateTime.Now, Id = 0, ReviewState = ReviewState.Open, User = users.First(u => u.Type.Equals(UserType.Employee))});
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        #region Tests

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
