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
    public class IntelliCloudManager
    {

        public void AskQuestion(string source, string reference, string question)
        {
            Validation.StringCheck(source);
            Validation.SourceDefinitionExistsCheck(source);
            Validation.StringCheck(reference);
            Validation.StringCheck(question);

            // create new context to connect to the database
            using (IntelliCloudContext ctx = new IntelliCloudContext()) {

                QuestionEntity questionEntity = new QuestionEntity();

                questionEntity.Content = question;
                questionEntity.CreationTime = DateTime.UtcNow;
                questionEntity.SourceType = ctx.SourceDefinitions.Single(sd => sd.Name.Equals(source));
                questionEntity.QuestionState = QuestionState.Open;
               
                // Check if the user already exists
                var userEntity = (from u in ctx.Users
                            where u.Id == ctx.Sources.Single(s => s.SourceDefinition.Id == questionEntity.SourceType.Id && s.Value == reference).UserId 
                            select u).SingleOrDefault();

                if (userEntity != null)
                {
                    // user already has an account, use this
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
                    SourceEntity sourceEntity = new SourceEntity();
                    sourceEntity.Value = reference;
                    sourceEntity.CreationTime = DateTime.UtcNow;
                    sourceEntity.SourceDefinition = questionEntity.SourceType;
                    sourceEntity.UserId = newUserEntity.Id;

                    ctx.Sources.Add(sourceEntity);

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

            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                AnswerEntity answerEntity = new AnswerEntity();
                answerEntity.AnswerState = (AnswerState) Enum.Parse(typeof (AnswerState), answerState);
                answerEntity.Content = answer;
                answerEntity.Question = context.Questions.First(q => q.Id.Equals(Convert.ToInt32(questionId)));
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

            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // Set the state of the answer to Accepted
                AnswerEntity answer = context.Answers.Single(a => a.Id == Convert.ToInt32(answerId));
                answer.AnswerState = AnswerState.Accepted;

                // Set the state of the question to Closed - no further action is required
                QuestionEntity question = answer.Question;
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

            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // Set the state of the answer to Declined
                AnswerEntity answer = context.Answers.Single(a => a.Id == Convert.ToInt32(answerId));
                answer.AnswerState = AnswerState.Declined;

                // Set the state of the question to Open - employee needs to process the feedback given by the user
                QuestionEntity question = answer.Question;
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

            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                ReviewEntity review = context.Reviews.First(r => r.Id.Equals(Convert.ToInt32(reviewId)));
                review.ReviewState = (ReviewState)Enum.Parse(typeof(ReviewState), reviewState);

                context.SaveChanges();
            }
        }

        public void SendReviewForAnswer(string reviewerId, string answerId, string review)
        {
            Validation.IdCheck(answerId);
            Validation.IdCheck(reviewerId);
            Validation.StringCheck(review);

            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                ReviewEntity reviewEntity = new ReviewEntity();
                reviewEntity.Answer = context.Answers.First(q => q.Id.Equals(Convert.ToInt32(answerId)));
                reviewEntity.Content = review;
                reviewEntity.ReviewState = ReviewState.Open;
                reviewEntity.User = context.Users.First(u => u.Id.Equals(Convert.ToInt32(reviewerId)));

                context.Reviews.Add(reviewEntity);

                context.SaveChanges();
            }
        }

        public List<Review> GetReviewsForAnswer(string answerId)
        {
            Validation.IdCheck(answerId);

            using (IntelliCloudContext context = new IntelliCloudContext())
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

            using (IntelliCloudContext context = new IntelliCloudContext())
            {

                List<AnswerEntity> answerEntities = (from a in context.Answers.Include("Question").Include("User").Include("Question.User").Include("Question.SourceType").Include("User.Sources")
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
    }
}
