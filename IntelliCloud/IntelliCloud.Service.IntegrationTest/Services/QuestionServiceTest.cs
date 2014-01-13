using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nl.fhict.IntelliCloud.Common.CustomException;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Context;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;

namespace nl.fhict.IntelliCloud.Service.IntegrationTest
{
    /// <summary>
    /// This class inhibits integration tests for the service QuestionService.
    /// </summary>
    [TestClass]
    public class QuestionServiceTest
    {
        #region Fields

        /// <summary>
        /// An instance of the service QuestionService that is being tested by this class.
        /// </summary>
        private IQuestionService service;
        private QuestionEntity entity;
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
            this.service = new QuestionService();
            this.initializeTestData();

        }

        /// <summary>
        /// This method adds a new question to the database and saves this in a variable
        /// </summary>
        private void initializeTestData()
        {
            cleanDatabase();

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                UserEntity newUser = new UserEntity();
                newUser.CreationTime = DateTime.UtcNow;
                newUser.FirstName = "integration";
                newUser.LastName = "test";
                newUser.Type = Common.DataTransfer.UserType.Customer;

                ctx.Users.Add(newUser);
                ctx.SaveChanges();

                UserEntity newEmployee = new UserEntity();
                newEmployee.CreationTime = DateTime.UtcNow;
                newEmployee.FirstName = "employee";
                newEmployee.LastName = "emp";
                newEmployee.Type = Common.DataTransfer.UserType.Employee;

                ctx.Users.Add(newEmployee);
                ctx.SaveChanges();

                this.employee = newEmployee;

                SourceEntity newSource = new SourceEntity();
                newSource.User = newUser;
                newSource.Value = "test@test.nl";
                newSource.CreationTime = DateTime.UtcNow;
                newSource.SourceDefinition = new SourceDefinitionEntity { CreationTime = DateTime.UtcNow, Description = "integration test def", Name = "Mail", Url = "" };

                ctx.Sources.Add(newSource);
                ctx.SaveChanges();

                QuestionEntity newEntity = new QuestionEntity();
                newEntity.IsPrivate = false;
                newEntity.LanguageDefinition = new LanguageDefinitionEntity { Name = "English", ResourceName = "English" };
                newEntity.QuestionState = Common.DataTransfer.QuestionState.Open;
                newEntity.Source = new QuestionSourceEntity { Source = newSource, PostId = "" };
                newEntity.Title = "this is a test question";
                newEntity.User = newUser;
                newEntity.Content = "this is the question i want to ask, please help me?";
                newEntity.CreationTime = DateTime.UtcNow;
                newEntity.FeedbackToken = "feedbackyeah!!@#$%^&*()";

                ctx.Questions.Add(newEntity);

                var dutch = new LanguageDefinitionEntity { Name = "Dutch", ResourceName = "Dutch" };
                var unknown = new LanguageDefinitionEntity { Name = "Unknown", ResourceName = "Unknown" };
                
                ctx.LanguageDefinitions.Add(dutch);
                ctx.LanguageDefinitions.Add(unknown);

                AnswerEntity answer = new AnswerEntity();
                answer.AnswerState = AnswerState.Ready;
                answer.Content = "Dit is een voorbeeld.";
                answer.CreationTime = DateTime.UtcNow;
                answer.IsPrivate = false;
                answer.LanguageDefinition = dutch;
                answer.User = this.employee;

                var keyword1 = new KeywordEntity { CreationTime = DateTime.UtcNow, Name = "dit" };
                var keyword2 = new KeywordEntity { CreationTime = DateTime.UtcNow, Name = "zijn" };
                var keyword3 = new KeywordEntity { CreationTime = DateTime.UtcNow, Name = "een" };
                var keyword4 = new KeywordEntity { CreationTime = DateTime.UtcNow, Name = "voorbeeld" };

                var answerKeys = new[]
                {
                    new AnswerKeyEntity { Affinity = 1, CreationTime = DateTime.UtcNow, Answer = answer, Keyword = keyword1 },
                    new AnswerKeyEntity { Affinity = 10, CreationTime = DateTime.UtcNow, Answer = answer, Keyword = keyword2 },
                    new AnswerKeyEntity { Affinity = 1, CreationTime = DateTime.UtcNow, Answer = answer, Keyword = keyword3 },
                    new AnswerKeyEntity { Affinity = 10, CreationTime = DateTime.UtcNow, Answer = answer, Keyword = keyword4 },
                };

                ctx.AnswerKeys.AddRange(answerKeys);

                ctx.Answers.Add(answer);

                ctx.SaveChanges();

                newEntity = new QuestionEntity();
                newEntity.IsPrivate = false;
                newEntity.LanguageDefinition = dutch;
                newEntity.QuestionState = Common.DataTransfer.QuestionState.Closed;
                newEntity.Source = new QuestionSourceEntity { Source = newSource, PostId = "" };
                newEntity.Title = "this is a test question version 2";
                newEntity.User = newUser;
                newEntity.Content = "this is the question i want to ask, please help me?";
                newEntity.CreationTime = DateTime.UtcNow;
                newEntity.FeedbackToken = "feedbackyeah!!@#$%^&*d()";
                newEntity.Answerer = this.employee;
                newEntity.Answer = answer;
                ctx.Questions.Add(newEntity);
                ctx.SaveChanges();

                KeywordEntity key = new KeywordEntity();
                key.Name = "Android";
                key.CreationTime = DateTime.UtcNow;

                ctx.Keywords.Add(key);

                QuestionKeyEntity qk = new QuestionKeyEntity();
                qk.Keyword = key;
                qk.Question = newEntity;
                qk.CreationTime = DateTime.UtcNow;
                qk.Affinity = 5;

                ctx.QuestionKeys.Add(qk);

                ctx.SaveChanges();

                this.entity = newEntity;
            }
        }

        /// <summary>
        /// A method that is called after each test that is ran. This method is used to, for instance, dispose
        /// any objects that require disposing.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            cleanDatabase();
        }

        private void cleanDatabase() {
            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                ctx.Questions.RemoveRange(ctx.Questions.ToList());
                ctx.QuestionSources.RemoveRange(ctx.QuestionSources.ToList());
                ctx.Sources.RemoveRange(ctx.Sources.ToList());
                ctx.SourceDefinitions.RemoveRange(ctx.SourceDefinitions.ToList());
                ctx.LanguageDefinitions.RemoveRange(ctx.LanguageDefinitions.ToList());
                ctx.Keywords.RemoveRange(ctx.Keywords.ToList());
                ctx.QuestionKeys.RemoveRange(ctx.QuestionKeys.ToList());
                ctx.AnswerKeys.RemoveRange(ctx.AnswerKeys.ToList());
                ctx.Answers.RemoveRange(ctx.Answers.ToList());

                ctx.Users.RemoveRange(ctx.Users.ToList());

                ctx.SaveChanges();
            }
        }

        #region Tests
        #region GetQuestion test

        /// <summary>
        /// Validates the getQuestion method.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetQuestionTest()
        {
            try
            {
                int questionId = this.entity.Id;
                var question = service.GetQuestion(questionId.ToString());
                Assert.AreEqual(this.entity.Content, question.Content);

            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
        #endregion GetQuestion test

        #region GetQuestions tests
        /// <summary>
        /// validates GetQuestions method with Filter.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetQuestionsTest_withFilter()
        {
            try
            {
                var questions = this.service.GetQuestionsByState(QuestionState.Open);
                Assert.AreEqual(true, questions.Count == 1);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        /// <summary>
        /// validates GetQuestions method without Filter.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetQuestionsTest_withoutFilter()
        {
            try
            {
                var questions = this.service.GetQuestions();
                Assert.AreEqual(true, questions.Count == 2);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
        #endregion GetQuestions tests

        #region UpdateQuestion
        /// <summary>
        /// Test to validate the UpdateQuestion.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void UpdateQuestionTest()
        {
            try
            {
                var employeeId = this.employee.Id;
                var questionId = this.entity.Id;

                this.service.UpdateQuestion(questionId.ToString(), employeeId);

                using (IntelliCloudContext ctx = new IntelliCloudContext())
                {
                    var question = ctx.Questions.Single(q => q.Id == questionId);

                    Assert.AreEqual(questionId, question.Id);
                }

            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
        #endregion UpdateQuestion

        #region GetAsker tests
        /// <summary>
        /// Test to validate the getAsker method.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetAskerTest()
        {
            try
            {
                string questionId = this.entity.Id.ToString();
                User u = this.service.GetAsker(questionId);

                Assert.AreEqual(u.LastName, this.entity.User.LastName);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
        #endregion GetAsker tests

        #region GetAnswerer tests
        /// <summary>
        /// Test to validate the getAnswerer method.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetAnswererTest()
        {
            try
            {
                string questionId = this.entity.Id.ToString();
                User u = this.service.GetAnswerer(questionId);

                Assert.AreEqual(u.LastName, this.entity.Answerer.LastName);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
        #endregion GetAnswerer tests

        #region GetAnswer tests
        /// <summary>
        /// Test to validate the getAnswer method.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetAnswerTest()
        {
            try
            {
                string questionId = this.entity.Id.ToString();
                Answer a = this.service.GetAnswer(questionId);

                Assert.AreEqual(a.Content, this.entity.Answer.Content);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
        #endregion GetAnswer tests

        #region GetKeywords tests
        /// <summary>
        /// Test to validate the getAnswer method.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetKeywordsTest()
        {
            try
            {
                string questionId = this.entity.Id.ToString();
                IList<Keyword> keys = this.service.GetKeywords(questionId);

                Assert.AreEqual(keys.Count(), 1);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        #endregion GetKeywords tests

        #endregion Tests

        #region Error handeling tests
        /// <summary>
        /// Test if a NotFoundException is thrown when attempting to get a question
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(NotFoundException))]
        public void GetQuestion_NoQuestion()
        {
            this.service.GetQuestion("12");
        }

        /// <summary>
        /// Test if a NotFoundException is thrown when attempting to get a question with an unrecognizable language.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateQuestion_NoSourceDefinition()
        {
            this.service.CreateQuestion("unregistered source", "ding", "What is the answer to life, the universe and all?", "The great question", isPrivate: false);
        }

        /// <summary>
        /// Test if a NotFoundException is thrown when attempting to get a question that doesnt exist
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(NotFoundException))]
        public void UpdateQuestion_NoQuestion()
        {
            this.service.UpdateQuestion("1234", this.employee.Id);
        }

        /// <summary>
        /// Test if a NotFoundException is thrown when attempting to get a user that doesnt exist
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(NotFoundException))]
        public void UpdateQuestion_NonExistingEmployee()
        {
            this.service.UpdateQuestion(this.entity.Id.ToString(), 1234);
        }
        #endregion Error handeling tests

        #endregion Methods
    }
}
