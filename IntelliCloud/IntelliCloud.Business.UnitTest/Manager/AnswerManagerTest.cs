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
    /// In this unit test class all methods of AnswerManager will be tested.
    /// All these test wil use mock classes.
    /// </summary>
    [TestClass]
    public class AnswerManagerTest
    {
        #region Fields

        private AnswerManager manager;
        private Mock<IValidation> validation;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Method that gets called before each testcase.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            validation = new Mock<IValidation>();
            this.manager = new AnswerManager(validation.Object);
        }

        /// <summary>
        /// Methods that gets called after each test case.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
        }

        #endregion Methods

        #region Tests

        /// <summary>
        /// Test to validate that the employee idee is being validated.
        /// </summary>
        [TestMethod]
        public void GetAnswersTest()
        {
            int employeeId = 1;
            AnswerState answerState = AnswerState.Ready;

            try
            {
                this.manager.GetAnswers(answerState, employeeId);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(employeeId), Times.Once());
        }

        /// <summary>
        /// Test to validate that the answer id is being validated.
        /// </summary>
        [TestMethod]
        public void GetAnswerTest()
        {
            string answerId = "1";

            try
            {
                this.manager.GetAnswer(answerId);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(answerId), Times.Once());
        }

        /// <summary>
        /// Test to validate that the questionid, answererid and and answer are validated.
        /// </summary>
        [TestMethod]
        public void CreateAnswerTest()
        {
            int questionId = 1;
            int answererId = 2;

            // question "What is the answer to life, the universe and everything?"
            // fun fact the japanese words for 4 and 2 are "shi" and "ni"
            // And shini is the japanese word for death, hence the solution 
            // to life, the uneverse and everything is death.
            string answer = "42";
            AnswerState answerState = AnswerState.Ready;

            try
            {
                this.manager.CreateAnswer(questionId, answer, answererId, answerState);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(questionId), Times.Once());
            validation.Verify(v => v.IdCheck(answererId), Times.Once());
            validation.Verify(v => v.StringCheck(answer), Times.Once());
        }

        /// <summary>
        /// Validates that the answerId answer are validated.
        /// </summary>
        [TestMethod]
        public void UpdateAnswerTest()
        {
            string answerId = "1";

            string answer = "Go to you my documents folder and itt will be there.";
            AnswerState answerState = AnswerState.Ready;

            try
            {
                this.manager.UpdateAnswer(answerId, answerState, answer);
            }
            catch (Exception)
            { }

            validation.Verify(v => v.IdCheck(answerId), Times.Once());
            validation.Verify(v => v.StringCheck(answer), Times.Once());
        }

        #endregion Tests
    }
}
