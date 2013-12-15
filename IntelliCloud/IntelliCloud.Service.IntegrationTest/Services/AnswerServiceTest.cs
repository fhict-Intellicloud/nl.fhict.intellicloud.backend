using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nl.fhict.IntelliCloud.Common.CustomException;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Data.Model;

namespace nl.fhict.IntelliCloud.Service.IntegrationTest.Manager
{
    /// <summary>
    /// This class inhibits the integration tests for the AnswerService.
    /// </summary>
    [TestClass]
    public class AnswerServiceTest
    {
        #region Fields

        /// <summary>
        /// An instance of the service AnswerService that is being tested by this class.
        /// </summary>
        private IAnswerService service;

        private QuestionEntity question;
        private AnswerEntity answer;
        private UserEntity employee;

        #endregion Fields

        #region Methods

        /// <summary>
        /// A method that is called before each test is run. This method is used to set up a fresh state for
        /// each test by for instance creating new service objects.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.service = new AnswerService();
            this.initializeTestData();
        }

        /// <summary>
        /// A method that is called after each test that is ran. This method is used to, for instance, dispose
        /// any objects that require disposing.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                ctx.Questions.RemoveRange(ctx.Questions.ToList());
                ctx.QuestionSources.RemoveRange(ctx.QuestionSources.ToList());
                ctx.Sources.RemoveRange(ctx.Sources.ToList());
                ctx.SourceDefinitions.RemoveRange(ctx.SourceDefinitions.ToList());
                ctx.LanguageDefinitions.RemoveRange(ctx.LanguageDefinitions.ToList());
                ctx.Users.RemoveRange(ctx.Users.ToList());

                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// This method adds a new answer to the database and saves this in a variable
        /// </summary>
        private void initializeTestData()
        {
            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                // creating a language for testing
                LanguageDefinitionEntity language = new LanguageDefinitionEntity();
                language.Name = "Dutch";
                language.ResourceName = "Dutch";
                ctx.LanguageDefinitions.Add(language);

                // create test customer and employee;
                this.employee = new UserEntity();
                this.employee.CreationTime = DateTime.UtcNow;
                this.employee.FirstName = "employee";
                this.employee.Infix = "";
                this.employee.LastName = "test";
                this.employee.Type = UserType.Employee;

                UserEntity customer = new UserEntity();
                customer.CreationTime = DateTime.UtcNow;
                customer.FirstName = "Jan";
                customer.Infix = "de";
                customer.LastName = "tested";
                customer.Type = UserType.Customer;

                ctx.Users.Add(customer);
                ctx.Users.Add(this.employee);
                ctx.SaveChanges();

                // create test source for the question
                SourceEntity newSource = new SourceEntity();
                newSource.User = customer;
                newSource.Value = "test@test.nl";
                newSource.CreationTime = DateTime.UtcNow;
                newSource.SourceDefinition = new SourceDefinitionEntity { CreationTime = DateTime.UtcNow, Description = "integration test def", Name = "test def", Url = "" };

                ctx.Sources.Add(newSource);
                ctx.SaveChanges();

                // create test question
                this.question = new QuestionEntity();
                this.question.IsPrivate = false;
                this.question.LanguageDefinition = language;
                this.question.QuestionState = QuestionState.Open;
                this.question.Source = new QuestionSourceEntity { Source = newSource, PostId = "" };
                this.question.Title = "The omni-question.";
                this.question.User = customer;
                this.question.Content = "What is the answer to life, the universe and everything?";
                this.question.CreationTime = DateTime.UtcNow;
                this.question.FeedbackToken = "supertoken#$%^&";

                this.answer = new AnswerEntity();
                this.answer.IsPrivate = false;
                this.answer.LanguageDefinition = language;
                this.answer.CreationTime = DateTime.Now;
                this.answer.Content = "The answer to your question is 42.";
                this.answer.User = this.employee;
                this.answer.AnswerState = AnswerState.Ready;

                ctx.Questions.Add(this.question);
                ctx.Answers.Add(this.answer);
                ctx.SaveChanges();
            }
        }

        #region Tests

        #region GetAnswers tests
        /// <summary>
        /// Test to vlaidate all answers are found without the use of filters.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetAnswersTest_NoFilter()
        {
            try
            {
                // TODO: getAnswer summary states that paramters are optional this is not the case.
                var questions = this.service.GetAnswers(AnswerState.NotNeeded, null);
                Assert.IsTrue(questions.Count == 1);

            }
            catch (Exception e) 
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        /// <summary>
        /// Test to vlaidate all answers are found with the use of a state filter.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetAnswersTest_StateFilter()
        {
            try
            {
                // TODO: getAnswer summary states that paramters are optional this is not the case.
                var questions = this.service.GetAnswers(AnswerState.Ready, null);
                Assert.IsTrue(questions.Count == 1);

            }
            catch (Exception e) 
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        /// <summary>
        /// Test to vlaidate all answers are found with the use of a employee filter.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetAnswersTest_EmployeeFilter()
        {
            try
            {
                // TODO: getAnswer summary states that paramters are optional this is not the case.
                var questions = this.service.GetAnswers(AnswerState.NotNeeded, this.employee.Id);
                Assert.IsTrue(questions.Count == 1);

            }
            catch (Exception e) 
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        #endregion GetAnswers tests

        #region GetAnswer tests
        /// <summary>
        /// Test to validate that a specific question can succesfully be retrieved.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetAnswer()
        {
            int answerId = this.answer.Id;

            try
            {
                var question = service.GetAnswer(answerId.ToString());
                Assert.AreEqual(answerId, this.answer.Id);
            }
            catch (Exception e) 
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }
        #endregion GetAnswer test

        #region CreateAnswer tests
        /// <summary>
        /// Test to validate that a answer is properly created and is set to the right question.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void CreateAnswer()
        {
            int questionId = this.question.Id, answererId = this.employee.Id;
            string answer = "The answer to anything is 42!!";
            AnswerState state = AnswerState.Ready;

            try
            {
                this.service.CreateAnswer(questionId, answer, answererId, state);

                using (IntelliCloudContext ctx = new IntelliCloudContext())
                {
                    AnswerEntity newAnswer = ctx.Answers
                        .Where(a => a.Content == answer &&
                            a.User.Id == answererId &&
                            a.AnswerState == state)
                        .Single();

                    Assert.AreEqual(answer, newAnswer.Content);
                    Assert.AreEqual(answererId, newAnswer.User);
                    Assert.AreEqual(state, newAnswer.AnswerState);

                    QuestionEntity newQuestion = ctx.Questions
                        .Where(q => q.Id == questionId)
                        .Single();

                    Assert.IsTrue(newQuestion.Answer.Id == newAnswer.Id);
                }
            }
            catch (Exception e) 
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }

        }
        #endregion CreateAnswer tests

        #region UpdateAnswer tests
        /// <summary>
        /// Test to validate the answer state of an answer can be succesfully updated.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void UpdateAnswerState_Test()
        {
            try
            {
                var answerId = this.answer.Id;

                this.service.UpdateAnswer(answerId.ToString(), AnswerState.UnderReview, this.answer.Content);

                using (IntelliCloudContext ctx = new IntelliCloudContext())
                {
                    var question = ctx.Answers.Single(a => a.Id == answerId);

                    Assert.AreEqual(answerId, this.answer.Id);
                }
            }
            catch (Exception e) 
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        /// <summary>
        /// Test to validate the content of an answer can succesfully be updated. 
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void UpdateAnswer_Test()
        {
            try
            {
                var answerId = this.answer.Id;

                this.service.UpdateAnswer(answerId.ToString(), AnswerState.Ready, "The answer to you question is 42. For reference watch wall-E.");

                using (IntelliCloudContext ctx = new IntelliCloudContext())
                {
                    var question = ctx.Answers.Single(a => a.Id == answerId);

                    Assert.AreEqual(answerId, this.answer.Id);
                }
            }
            catch (Exception e) 
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }
        #endregion UpdateAnswer tests

        #region Error handling tests

        /// <summary>
        /// Test to validate an Exception is thrown when no answer can be found.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(Exception))]
        public void NoAnswerFound()
        {
            this.service.GetAnswer("1000");
        }

        /// <summary>
        /// Test if an ArgumentException is thrown when a negative id is given.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(ArgumentException))]
        public void invalidId_Negative()
        {
            this.service.GetAnswer("-1");
        }

        /// <summary>
        /// Test if an ArgumentException is thrown when a string that is not a number is given.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(ArgumentException))]
        public void invalidId_NotANumber()
        {
            this.service.GetAnswer("one");
        }

        /// <summary>
        /// Test if an ArgumentException is thrown when a empty string is given.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(ArgumentException))]
        public void invalidString_EmptyString()
        {
            this.service.CreateAnswer(0, "", 0, AnswerState.Ready);
        }

        /// <summary>
        /// Test if an ArgumentException is thrown when a string with only spaces is given.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(ArgumentException))]
        public void invalidString_spacesString()
        {
            this.service.CreateAnswer(0, " ", 0, AnswerState.Ready);
        }

        /// <summary>
        /// Test if an ArgumentException is thrown when a null value is given as a string.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(ArgumentException))]
        public void invalidString_Null()
        {
            this.service.CreateAnswer(0, null, 0, AnswerState.Ready);
        }

        /// <summary>
        /// Test if a NotFoundException is thrown when attempting to update inexisting answer.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(NotFoundException))]
        public void UpdateAnswer_NoAnswerFound()
        {
            this.service.UpdateAnswer("10", AnswerState.Ready, "Not important");
        }

        /// <summary>
        /// Test if a NotFoundException is thrown when attempting to create answer
        /// with inexisting answerer.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(NotFoundException))]
        public void CreateAnswer_NoAnswererFound()
        {
            this.service.CreateAnswer(this.question.Id, "Not important", 10, AnswerState.Ready);
        }

        /// <summary>
        /// Test if a NotFoundException is thrown when attempting to create answer
        /// with undetectable language.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(NotFoundException))]
        public void CreateAnswer_InvalidLanguage()
        {
            this.service.CreateAnswer(this.question.Id, "&Undetecable ^&language te67xt.", this.employee.Id, AnswerState.Ready);
        }

        #endregion Error handling tests

        #endregion Tests

        #endregion Methods
    }
}
