using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nl.fhict.IntelliCloud.Common.CustomException;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Context;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;
using System.Data.Entity;

namespace nl.fhict.IntelliCloud.Service.IntegrationTest.Services
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
                ctx.AnswerKeys.RemoveRange(ctx.AnswerKeys.ToList());
                ctx.Keywords.RemoveRange(ctx.Keywords.ToList());
                ctx.Answers.RemoveRange(ctx.Answers.ToList());
                ctx.Feedbacks.RemoveRange(ctx.Feedbacks.ToList());
                ctx.Reviews.RemoveRange(ctx.Reviews.ToList());
                ctx.Questions.RemoveRange(ctx.Questions.ToList());
                ctx.QuestionSources.RemoveRange(ctx.QuestionSources.ToList());
                ctx.Sources.RemoveRange(ctx.Sources.ToList());
                ctx.SourceDefinitions.RemoveRange(ctx.SourceDefinitions.ToList());
                ctx.Users.RemoveRange(ctx.Users.ToList());
                ctx.LanguageDefinitions.RemoveRange(ctx.LanguageDefinitions.ToList());
                
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
                language.Name = "English";
                language.ResourceName = "English";
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
                newSource.SourceDefinition = new SourceDefinitionEntity { CreationTime = DateTime.UtcNow, Description = "integration test def", Name = "Mail", Url = "" };

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
                this.answer.AnswerState = AnswerState.UnderReview;

                ReviewEntity review = new ReviewEntity();
                review.Answer = answer;
                review.Content = "This is a review";
                review.CreationTime = DateTime.UtcNow;
                review.LastChangedTime = DateTime.UtcNow;
                review.ReviewState = ReviewState.Open;
                review.User = this.employee;

                FeedbackEntity feedback = new FeedbackEntity();
                feedback.Answer = answer;
                feedback.Content = "This is feedback";
                feedback.CreationTime = DateTime.UtcNow;
                feedback.FeedbackState = FeedbackState.Open;
                feedback.FeedbackType = FeedbackType.Declined;
                feedback.LastChangedTime = DateTime.UtcNow;
                feedback.Question = question;
                feedback.User = customer;

                KeywordEntity keyword = new KeywordEntity();
                keyword.Name = "This";
                keyword.CreationTime = DateTime.UtcNow;
                
                AnswerKeyEntity answerKeyEntity = new AnswerKeyEntity();
                answerKeyEntity.Answer = answer;
                answerKeyEntity.CreationTime = DateTime.UtcNow;
                answerKeyEntity.Keyword = keyword;
                answerKeyEntity.Affinity = 9;

                ctx.Keywords.Add(keyword);
                ctx.AnswerKeys.Add(answerKeyEntity);

                ctx.Feedbacks.Add(feedback);
                ctx.Reviews.Add(review);

                ctx.Questions.Add(this.question);
                ctx.Answers.Add(this.answer);
                ctx.SaveChanges();
            }
        }

        #region Tests

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

        #region GetAnswerer tests
        /// <summary>
        /// Test to validate if the service returns the answerer
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetAnswerer_Test()
        {
            try
            {
                var answerId = this.answer.Id;

                User answerer = this.service.GetAnswerer(answerId.ToString());

                using (IntelliCloudContext ctx = new IntelliCloudContext())
                {
                    var ctxAnswerer = ctx.Users.Single(a => a.Id == this.employee.Id);

                    Assert.AreEqual(ctxAnswerer.Id.ToString(), answerer.Id.ToString().Split('/').Last());
                }
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }
        #endregion GetAnswerer tests

        #region GetFeedbacks tests
        /// <summary>
        /// Test to validate if the service returns the feedbacks
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetFeedbacks_test()
        {
            try
            {
                var answerId = this.answer.Id;

                IList<Feedback> feedbacks = this.service.GetFeedbacks(answerId.ToString());

                Assert.AreEqual(1, feedbacks.Count);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        #endregion

        #region GetReviews tests
        /// <summary>
        /// Test to validate if the service returns the Reviews
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetReviews_test()
        {
            try
            {
                var answerId = this.answer.Id;

                IList<Review> reviews = this.service.GetReviews(answerId.ToString());

                Assert.AreEqual(1, reviews.Count);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        #endregion

        #region GetKeywords tests
        /// <summary>
        /// Test to validate if the service returns the Reviews
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetKeywords_test()
        {
            try
            {
                var answerId = this.answer.Id;

                IList<Keyword> keywords = this.service.GetKeywords(answerId.ToString());

                Assert.AreEqual(1, keywords.Count);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        #endregion

        #region Error handling tests

        /// <summary>
        /// Test to validate an Exception is thrown when no answer can be found.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        [ExpectedException(typeof(System.InvalidOperationException))]
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
        [ExpectedException(typeof(System.ArgumentNullException))]
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
        #endregion Error handling tests

        #endregion Tests

        #endregion Methods
    }
}
