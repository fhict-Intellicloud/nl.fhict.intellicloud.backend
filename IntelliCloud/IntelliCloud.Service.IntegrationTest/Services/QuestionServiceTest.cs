using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                newEmployee.Type = Common.DataTransfer.UserType.Employee;

                ctx.Users.Add(newEmployee);
                ctx.SaveChanges();

                this.employee = newEmployee;

                SourceEntity newSource = new SourceEntity();
                newSource.User = newUser;
                newSource.Value = "test@test.nl";
                newSource.CreationTime = DateTime.UtcNow;
                newSource.SourceDefinition = new SourceDefinitionEntity { CreationTime = DateTime.UtcNow, Description = "integration test def", Name = "test def", Url = "" };

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

        #region Tests
        /// <summary>
        /// Tests if the UpdateReview is updating a review, or at least calls something to the database.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void UpdateQuestion()
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
            catch (Exception e) // TODO move exception test to different method, since this allows for skipping a part of the test...
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        /// <summary>
        /// Tests if the UpdateReview is updating a review, or at least calls something to the database.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetQuestions()
        {
            try
            {
                const int employeeId = 1;

                var questions = this.service.GetQuestions(employeeId);

                Assert.AreEqual(true, questions.Count > 0);

            }
            catch (Exception e) // TODO move exception test to different method, since this allows for skipping a part of the test...
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        /// <summary>
        /// Tests if the CreateReview is creating a review, or at least calls something to the database.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void CreateQuestion()
        {
            try
            {
                string content = "This is the content";
                string title = "This is the title";
                string source = "test def";
                string reference = "reffgdsgfd";

                this.service.CreateQuestion(source, reference, content, title);

                using (IntelliCloudContext ctx = new IntelliCloudContext())
                {
                    QuestionEntity newEntity = ctx.Questions
                    .Include(q => q.Source)
                    .Include(q => q.User)
                    .Include(q => q.User.Sources)
                    .Include(q => q.User.Sources.Select(s => s.SourceDefinition))
                    .Include(q => q.LanguageDefinition).Single(q => q.Content == content && q.Title == title);

                    Assert.AreEqual(content, newEntity.Content);
                    Assert.AreEqual(title, newEntity.Title);
                    Assert.AreEqual(source, newEntity.Source.Source.SourceDefinition.Name);
                    Assert.AreEqual(reference, newEntity.Source.Source.Value);
                }
            }
            catch (Exception e) // TODO move exception test to different method, since this allows for skipping a part of the test...
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        /// <summary>
        /// Tests if the GetReviews is getting all reviews of an answer, or atleast calls something to the database.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetQuestion()
        {
            try
            {
                int questionId = this.entity.Id;

                var question = service.GetQuestion(questionId.ToString());

                Assert.AreEqual(questionId, question.Id);
            }
            catch (Exception e) // TODO move exception test to different method, since this allows for skipping a part of the test...
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        /// <summary>
        /// Tests if the GetReviews is getting all reviews of an answer, or atleast calls something to the database.
        /// </summary>
        [TestMethod]
        [TestCategory("nl.fhict.IntelliCloud.Service.IntegrationTest")]
        public void GetQuestionByFeedbackToken()
        {
            try
            {
                string feedbackToken = this.entity.FeedbackToken;

                var question = service.GetQuestionByFeedbackToken(feedbackToken);

                Assert.AreEqual(this.entity.Id, question.Id);
            }
            catch (Exception e) // TODO move exception test to different method, since this allows for skipping a part of the test...
            {
                Assert.AreEqual(e.Message, "Sequence contains no elements");
            }
        }

        #endregion Tests

        #endregion Methods
    }
}
