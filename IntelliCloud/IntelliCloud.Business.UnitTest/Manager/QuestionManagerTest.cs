using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using nl.fhict.IntelliCloud.Business.Manager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nl.fhict.IntelliCloud.Business.UnitTest.Manager
{
    /// <summary>
    /// In this unit test class all methods of the QuestionManager will be tested.
    /// All these test will use mock classes.
    /// </summary>
    [TestClass]
    public class QuestionManagerTest
    {
        #region Fields

        private QuestionManager manager;
        private Mock<IValidation> validation;

        #endregion Fields

        #region Methods

        [TestInitialize]
        public void Initialize()
        {
            this.validation = new Mock<IValidation>();
            this.manager = new QuestionManager(this.validation.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        #region Tests

        #region question manager tests

        #region GetQuestion test
        /// <summary>
        /// In this test we check if the questionId is being validated in the GetQuestion method.
        /// </summary>
        [TestMethod]
        public void GetQuestionTest()
        {
            string questionId = "1";

            try
            {
                manager.GetQuestion(questionId);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(questionId), Times.Once());
        }
        #endregion

        #region GetQuestions tests 

        #endregion GetQuestions tests

        #region CreateQuestion test
        /// <summary>
        /// In this test we check if the source, reference, question, title, postId and isPrivate is being validated in the CreateQuestion method.
        /// </summary>
        [TestMethod]
        public void CreateQuestionTest()
        {
            string source = "Mail";
            string reference = "Test@Gmail.coom";
            string question = "This is my question";
            string title = "This is my title";
            string postId = "";
            bool isPrivate = true;

            try
            {
                manager.CreateQuestion(source, reference, question, title, postId, isPrivate);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.StringCheck(source), Times.Once());
            validation.Verify(v => v.StringCheck(reference), Times.Once());
            validation.Verify(v => v.StringCheck(question), Times.Once());
            validation.Verify(v => v.StringCheck(title), Times.Once());
        }

        #endregion CreateQuestion test

        #region GetQuestionByFeedbackToken tests
        /// <summary>
        /// In this test we validate the GetQuestionByFeedbackToken method
        /// </summary>
        [TestMethod]
        public void GetQuestionByFeedbackTokenTest()
        {
            string token = "abcdefghijklmnopqrstuvwxyz1234567890";
            
            try
            {
                manager.GetQuestionByFeedbackToken(token);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.StringCheck(token), Times.Once());
        }

        #endregion GetQuestionByFeedbackToken tests

        #region UpdateQuestion
        /// <summary>
        /// In this test we check if the questionId and emplooyeeId is being validated in the UpdateQuestion method.
        /// </summary>
        [TestMethod]
        public void UpdateQuestionTest()
        {
            string questionId = "1";
            int employeeId = 1;

            try
            {
               manager.UpdateQuestion(questionId, employeeId);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(questionId), Times.Once());
            validation.Verify(v => v.IdCheck(employeeId), Times.Once());
        }
        #endregion UpdateQuestion

        #region GetAsker tests
        /// <summary>
        /// Test to validate the getAsker method.
        /// </summary>
        [TestMethod]
        public void GetAskerTest()
        {
            string questionId = "1";

            try
            {
                manager.GetAsker(questionId);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(questionId), Times.Once());
        }
        #endregion GetAsker tests

        #region GetAnswerer tests
        /// <summary>
        /// Test to validate the getAnswerer method.
        /// </summary>
        [TestMethod]
        public void GetAnswererTest()
        {
            string questionId = "1";

            try
            {
                manager.GetAnswerer(questionId);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(questionId), Times.Once());
        }
        #endregion GetAnswerer tests

        #region GetAnswer tests
        /// <summary>
        /// Test to validate the getAnswer method.
        /// </summary>
        [TestMethod]
        public void GetAnswerTest()
        {
            string questionId = "1";

            try
            {
                manager.GetAnswer(questionId);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(questionId), Times.Once());
        }
        #endregion GetAnswer tests

        #region GetKeywords tests
        /// <summary>
        /// Test to validate the getKeywords method.
        /// </summary>
        [TestMethod]
        public void GetKeywordsTest()
        {
            string questionId = "1";

            try
            {
                manager.GetKeywords(questionId);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(questionId), Times.Once());
        }
        #endregion GetKeywords tests

        #region

        #endregion

        #endregion

        #region Question conversion tests


        /// <summary>
        /// Test if all punctuation is removed correctly.
        /// </summary>
        [TestMethod]
        public void convertQuestionTest_noPunctuation()
        {
            string question = "Jui*ste text ,./<>?;:\"[]{}\\|`~!()_-+= die @#$%^&*' ov(er mo&et blijven.";
            IList<string> result = this.manager.ConvertQuestion(question);

            Assert.IsTrue(result.Except(new List<string>() { "juiste", "text", "die", "over", "moet", "blijven" }).Count() == 0);
        }

        /// <summary>
        /// Test if the punctuation used within words is not removed.
        /// </summary>
        [TestMethod]
        public void convertQuestionTest_acceptedPunctuation()
        {
            string question = "midden-amerika andré hbo'er crèche û";
            IList<string> result = this.manager.ConvertQuestion(question);

            Assert.IsTrue(result.Except(new List<string>() { "midden-amerika", "andré", "hbo'er", "crèche", "û" }).Count() == 0);
        }
        #endregion  Question conversion tests

        #endregion Tests

        #endregion Methods
    }
}
