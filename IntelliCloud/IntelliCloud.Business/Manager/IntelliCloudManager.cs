using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Data.Model;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    public class IntelliCloudManager : BaseManager
    {
        public IntelliCloudManager(IntelliCloudContext context, IValidation validation)
            : base(context, validation)
        {
            //the intellicloudcontext and validation are given threw here
        }

        public IntelliCloudManager()
        {
            //new static objects are being made in the basemanager
        }

        public void AskQuestion(string source, string reference, string question)
        {
            Validation.StringCheck(source);
            Validation.SourceDefinitionExistsCheck(source);
            Validation.StringCheck(reference);
            Validation.StringCheck(question);

            // create new context to connect to the database
            using (var ctx = new IntelliCloudContext())
            {

                QuestionEntity questionEntity = new QuestionEntity();

                questionEntity.Content = question;
                questionEntity.CreationTime = DateTime.UtcNow;
                questionEntity.SourceDefinition = ctx.SourceDefinitions.Single(sd => sd.Name.Equals(source));
                questionEntity.QuestionState = QuestionState.Open;
               
                // Check if the user already exists
                var sourceEntity = ctx.Sources.SingleOrDefault(s => s.SourceDefinition.Id == questionEntity.SourceDefinition.Id && s.Value == reference); 

                if (sourceEntity != null)
                {
                    // user already has an account, use this
                    var userEntity = (from u in ctx.Users
                            where u.Id == sourceEntity.UserId
                            select u).Single();

                    questionEntity.User = userEntity;
                }
                else
                {
                    // user has no account, create one
                    UserEntity newUserEntity = new UserEntity();

                    newUserEntity.CreationTime = DateTime.UtcNow;
                    newUserEntity.Type = UserType.Customer;

                    ctx.Users.Add(newUserEntity);

                    ctx.SaveChanges();

                    questionEntity.User = newUserEntity;   

                    // Mount the source to the new user
                    SourceEntity newSourceEntity = new SourceEntity();
                    newSourceEntity.Value = reference;
                    newSourceEntity.CreationTime = DateTime.UtcNow;
                    newSourceEntity.SourceDefinition = questionEntity.SourceDefinition;
                    newSourceEntity.UserId = newUserEntity.Id;

                    ctx.Sources.Add(newSourceEntity);

                }

                ctx.Questions.Add(questionEntity);

                ctx.SaveChanges();
            }
        }

        public void SendAnswer(string questionId, string answerId)
        {

        }

        public void CreateAnswer(string questionId, string answer, string answererId, string answerState)
        {
            Validation.StringCheck(answer);
            Validation.IdCheck(questionId);
            Validation.IdCheck(answererId);
            Validation.AnswerStateCheck(answerState);

            using (var context = new IntelliCloudContext())
            {
                AnswerEntity answerEntity = new AnswerEntity();
                answerEntity.AnswerState = (AnswerState) Enum.Parse(typeof (AnswerState), answerState);
                answerEntity.Content = answer;
                answerEntity.User = context.Users.First(u => u.Id.Equals(Convert.ToInt32(answererId)));

                context.Answers.Add(answerEntity);

                context.SaveChanges();
            }
        }

        public void UpdateAnswer(string answerId, string answerState)
        {

        }

        public List<Question> GetQuestions(string employeeId)
        {
            List<Question> questions = new List<Question>();

            return questions;
        }

        public void AcceptAnswer(string feedback, string answerId, string questionId)
        {
            // Validate input data
            Validation.StringCheck(feedback);
            Validation.IdCheck(answerId);
            Validation.IdCheck(questionId);

            using (var context = new IntelliCloudContext())
            {
                // Set the state of the answer to Accepted
                int iAnswerId = Convert.ToInt32(answerId);
                AnswerEntity answer = context.Answers
                    .Include("Question")
                    .Include("Question.User")
                    .Include("Question.User.Sources")
                    .Single(a => a.Id == iAnswerId);

                answer.AnswerState = AnswerState.UnderReview;

                // Set the state of the question to Closed - no further action is required
                QuestionEntity question = context.Questions.Single(a => a.Id == Convert.ToInt32(questionId));
                question.QuestionState = QuestionState.Closed;

                // Store the user's feedback for the given answer
                FeedbackEntity feedbackEntity = new FeedbackEntity();
                feedbackEntity.Answer = answer;
                feedbackEntity.Content = feedback;
                feedbackEntity.CreationTime = DateTime.UtcNow;
                feedbackEntity.FeedbackState = FeedbackState.Open;
                feedbackEntity.FeedbackType = FeedbackType.Accepted;
                feedbackEntity.User = question.User;
                
                context.Feedbacks.Add(feedbackEntity);

                context.SaveChanges();
            }
        }

        public void DeclineAnswer(string feedback, string answerId, string questionId)
        {
            // Validate input data
            Validation.StringCheck(feedback);
            Validation.IdCheck(answerId);
            Validation.IdCheck(questionId);

            using (var context = new IntelliCloudContext())
            {
                // Set the state of the answer to Declined
                AnswerEntity answer = context.Answers.Single(a => a.Id == Convert.ToInt32(answerId));
                answer.AnswerState = AnswerState.UnderReview;

                // Set the state of the question to Open - employee needs to process the feedback given by the user
                QuestionEntity question = context.Questions.Single(a => a.Id == Convert.ToInt32(questionId));
                question.QuestionState = QuestionState.Open;

                // Store the user's feedback for the given answer
                FeedbackEntity feedbackEntity = new FeedbackEntity();
                feedbackEntity.Answer = answer;
                feedbackEntity.Content = feedback;
                feedbackEntity.CreationTime = DateTime.UtcNow;
                feedbackEntity.FeedbackState = FeedbackState.Open;
                feedbackEntity.FeedbackType = FeedbackType.Declined;                
                feedbackEntity.User = question.User;

                context.Feedbacks.Add(feedbackEntity);

                context.SaveChanges();
            }
        }

        public void UpdateReview(string reviewId, string reviewState)
        {
            Validation.IdCheck(reviewId);
            Validation.ReviewStateCheck(reviewState);

            using (var context = new IntelliCloudContext())
            {
                var id = Convert.ToInt32(reviewId);
                ReviewEntity review = context.Reviews.First(r => r.Id.Equals(id));
                review.ReviewState = (ReviewState)Enum.Parse(typeof(ReviewState), reviewState);

                context.SaveChanges();
            }
        }

        public void SendReviewForAnswer(string reviewerId, string answerId, string review)
        {
            Validation.IdCheck(answerId);
            Validation.IdCheck(reviewerId);
            Validation.StringCheck(review);

            using (var context = new IntelliCloudContext())
            {
                var aId = Convert.ToInt32(answerId);
                var rId = Convert.ToInt32(reviewerId);

                ReviewEntity reviewEntity = new ReviewEntity();
                reviewEntity.Answer = context.Answers.First(q => q.Id.Equals(aId));
                reviewEntity.Content = review;
                reviewEntity.ReviewState = ReviewState.Open;
                reviewEntity.User = context.Users.First(u => u.Id.Equals(rId));

                context.Reviews.Add(reviewEntity);

                context.SaveChanges();
            }
        }

        public List<Review> GetReviewsForAnswer(string answerId)
        {
            Validation.IdCheck(answerId);

            using (var context = new IntelliCloudContext())
            {

                int iAnswerId = int.Parse(answerId);

                List<ReviewEntity> reviewEntities = (from r in context.Reviews.Include("Answer").Include("User")
                                                     where r.Answer.Id == iAnswerId
                                                     select r).ToList();

                return ConvertEntities.ReviewEntityListToReviewList(reviewEntities);
            }
        }

        public List<Answer> GetAnswersUpForReview(string employeeId)
        {
            Validation.IdCheck(employeeId);

            using (var context = new IntelliCloudContext())
            {

                List<AnswerEntity> answerEntities = (from a in context.Answers.Include("User")
                                                    where a.AnswerState == (AnswerState.UnderReview)
                                                    select a).ToList();

                return ConvertEntities.AnswerEntityListToAnswerList(answerEntities);
            }          
        }
        
        public Answer GetAnswerById(string answerId)
        {
            return new Answer();

        }

        public Question GetQuestionById(string questionId)
        {
            return new Question();
        }

        public List<Question> GetQuestions(int questionId)
        {
            using (var ctx = new IntelliCloudContext())
            {
                List<QuestionEntity> questions = (from q in ctx.Questions
                                                      .Include("User")
                                                      .Include("SourceType")
                                                      .Include("User.Sources")
                                                      .Include("Answerer")
                                                      .Include("Answerer.Sources")
                                                  where q.Id == questionId
                                                  select q).ToList();
                return ConvertEntities.QuestionEntityListToQuestionList(questions);
            }
        }

        public List<Question> GetQuestionsForEmployee(int employeeId)
        {
            //TODO implement algorithem to match employee to questions
            using (var ctx = new IntelliCloudContext())
            {
                List<QuestionEntity> questions = (from q in ctx.Questions
                                                      .Include("User")
                                                      .Include("SourceType")
                                                      .Include("User.Sources")
                                                      .Include("Answerer")
                                                      .Include("Answerer.Sources") 
                                                  where q.QuestionState == QuestionState.Open
                                                  select q).ToList();
                return ConvertEntities.QuestionEntityListToQuestionList(questions);
            }
        }

        public List<Question> GetQuestions()
        {
            using (var ctx = new IntelliCloudContext())
            {
                List<QuestionEntity> questions = (from q in ctx.Questions
                                                      .Include("User")
                                                      .Include("SourceType")
                                                      .Include("User.Sources")
                                                      .Include("Answerer")
                                                      .Include("Answerer.Sources") 
                                                  select q).ToList();
                return ConvertEntities.QuestionEntityListToQuestionList(questions);
            }
        }

        public void AskQuestion(Question question)
        {
        }

        public void UpdateQuestion(string id, Question question)
        {
            
        }
    }
}
