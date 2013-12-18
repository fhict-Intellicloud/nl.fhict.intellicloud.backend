using Microsoft.VisualStudio.TestTools.UnitTesting;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Context;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace nl.fhict.IntelliCloud.Service.IntegrationTest
{
    /// <summary>
    /// Test class for the service FeedbackService.
    /// </summary>
    [TestClass]
    public class FeedbackServiceTest
    {
        #region Fields

        /// <summary>
        /// Instance of class FeedbackService.
        /// </summary>
        private IFeedbackService service;

        /// <summary>
        /// Fields used during testing.
        /// </summary>
        private UserEntity customerEntity;
        private UserEntity employeeEntity;
        private QuestionEntity questionEntity;
        private AnswerEntity answerEntity;
        private FeedbackEntity feedbackEntity;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Initialization method for each test method.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            this.service = new FeedbackService();
            this.initializeTestData();
        }

        /// <summary>
        /// Cleanup method for each test method.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            this.removeTestData();
        }

        /// <summary>
        /// Method that adds test data to the context (used during testing).
        /// </summary>
        private void initializeTestData()
        {
            // First, make sure to remove any existing data
            this.removeTestData();

            // Add test data
            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // Add a new mail definition
                SourceDefinitionEntity mailSourceDefinition = new SourceDefinitionEntity() { Name = "Mail", CreationTime = DateTime.UtcNow };
                context.SourceDefinitions.Add(mailSourceDefinition);

                // Add a new language definition
                LanguageDefinitionEntity languageDefinition = new LanguageDefinitionEntity() { Name = "English", ResourceName = "EN" };
                context.LanguageDefinitions.Add(languageDefinition);

                // Create a new user entity for a customer
                this.customerEntity = new UserEntity()
                {
                    FirstName = "Name",
                    Infix = "of",
                    LastName = "customer",
                    Type = UserType.Customer,
                    CreationTime = DateTime.UtcNow
                };
                context.Users.Add(this.customerEntity);

                // Create a new source for the customer (email address)
                SourceEntity customerMailSource = new SourceEntity()
                {
                    Value = "customer@domain.com",
                    CreationTime = DateTime.UtcNow,
                    SourceDefinition = mailSourceDefinition,
                    User = this.customerEntity
                };
                context.Sources.Add(customerMailSource);

                // Add a new question
                QuestionSourceEntity questionSource = new QuestionSourceEntity() { Source = customerMailSource };
                context.QuestionSources.Add(questionSource);
                this.questionEntity = new QuestionEntity()
                {
                    Title = "Title of the question",
                    Content = "Content of the question",
                    Source = questionSource,
                    LanguageDefinition = languageDefinition,
                    FeedbackToken = "ABC123",
                    CreationTime = DateTime.UtcNow,
                    User = this.customerEntity
                };
                context.Questions.Add(this.questionEntity);

                // Create a new user entity for an employee
                this.employeeEntity = new UserEntity()
                {
                    FirstName = "Name",
                    Infix = "of",
                    LastName = "employee",
                    Type = UserType.Employee,
                    CreationTime = DateTime.UtcNow
                };
                context.Users.Add(this.customerEntity);

                // Create a new source for the employee (email address)
                SourceEntity employeeMailSource = new SourceEntity()
                {
                    Value = "employee@domain.com",
                    CreationTime = DateTime.UtcNow,
                    SourceDefinition = mailSourceDefinition,
                    User = this.employeeEntity,
                };
                context.Sources.Add(employeeMailSource);

                // Create a new answer
                this.answerEntity = new AnswerEntity()
                {
                    Content = "Content of the answer",
                    LanguageDefinition = languageDefinition,
                    User = this.employeeEntity,
                    CreationTime = DateTime.UtcNow
                };
                context.Answers.Add(this.answerEntity);

                // Set the answer and answerer in the question
                this.questionEntity.Answer = answerEntity;
                this.questionEntity.Answerer = employeeEntity;

                // Add a new feedback entry
                this.feedbackEntity = new FeedbackEntity()
                {
                    Content = "Content of the feedback",
                    FeedbackType = FeedbackType.Declined,
                    FeedbackState = FeedbackState.Open,
                    Answer = this.answerEntity,
                    Question = this.questionEntity,
                    User = this.customerEntity,
                    CreationTime = DateTime.UtcNow
                };
                context.Feedbacks.Add(this.feedbackEntity);

                // Save the changes to the context
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Method that removes test data that was added to the context.
        /// </summary>
        private void removeTestData()
        {
            // Remove all entities from the context
            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                context.Feedbacks.RemoveRange(context.Feedbacks.ToList());
                context.Answers.RemoveRange(context.Answers.ToList());
                context.Questions.RemoveRange(context.Questions.ToList());
                context.QuestionSources.RemoveRange(context.QuestionSources.ToList());
                context.Sources.RemoveRange(context.Sources.ToList());
                context.SourceDefinitions.RemoveRange(context.SourceDefinitions.ToList());
                context.LanguageDefinitions.RemoveRange(context.LanguageDefinitions.ToList());
                context.Users.RemoveRange(context.Users.ToList());

                // Save the changes to the context
                context.SaveChanges();
            }

            // Unset local variables
            customerEntity = null;
            employeeEntity = null;
            questionEntity = null;
            answerEntity = null;
            feedbackEntity = null;
        }

        #region Tests

        /// <summary>
        /// GetFeedbacks test method.
        /// </summary>
        [TestMethod]
        public void GetFeedbacksTest()
        {
            try
            {
                // Get all feedbacks for the answer that was added in the Initialize method
                int answerId = this.answerEntity.Id;
                IList<Feedback> feedbacks = this.service.GetFeedbacks(answerId);

                // The amount of feedbacks should be greater than zero
                Assert.IsTrue(feedbacks.Count > 0);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        /// <summary>
        /// CreateFeedback test method.
        /// </summary>
        [TestMethod]
        public void CreateFeedbackTest()
        {
            try
            {
                // Add a new feedback entry
                string feedback = "Content of the second feedback.";
                int answerId = this.answerEntity.Id;
                int questionId = this.questionEntity.Id;
                FeedbackType feedbackType = FeedbackType.Declined;
                string feedbackToken = "ABC123";
                this.service.CreateFeedback(feedback, answerId, questionId, feedbackType, feedbackToken);

                // Check if the feedback entry was added and contains the correct data
                using (IntelliCloudContext context = new IntelliCloudContext())
                {
                    FeedbackEntity entity = context.Feedbacks
                                            .Include(f => f.Answer)
                                            .Include(f => f.Question)
                                            .Single(f => f.Content.Equals(feedback));

                    Assert.AreEqual(entity.Answer.Id, answerId);
                    Assert.AreEqual(entity.Question.Id, questionId);
                    Assert.AreEqual(entity.FeedbackState, FeedbackState.Open);
                    Assert.AreEqual(entity.FeedbackType, feedbackType);
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        /// <summary>
        /// UpdateFeedback test method.
        /// </summary>
        [TestMethod]
        public void UpdateFeedbackTest()
        {
            try
            {
                // Update the state of the feedback entry from Open to Closed
                int feedbackId = this.feedbackEntity.Id;
                FeedbackState feedbackState = FeedbackState.Closed;
                this.service.UpdateFeedback(feedbackId.ToString(), feedbackState);

                // Check if the state of the feedback entry is Closed
                using (IntelliCloudContext context = new IntelliCloudContext())
                {
                    FeedbackEntity entity = context.Feedbacks.Single(f => f.Id == feedbackId);
                    Assert.AreEqual(entity.FeedbackState, feedbackState);
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        #endregion Tests

        #endregion Methods
    }
}
