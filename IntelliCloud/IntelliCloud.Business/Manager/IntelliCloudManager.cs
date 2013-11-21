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
                questionEntity.CreationTime = DateTime.Now;
                questionEntity.SourceType = ctx.SourceDefinitions.First(sd => sd.Name.Equals(source));
                questionEntity.QuestionState = QuestionState.Open;
               
                // Check if the user already exists
                var users = from u in ctx.Users
                            where u.Id == ctx.Sources.First(s => s.SourceDefinition.Id == questionEntity.SourceType.Id && s.Value == reference).UserId 
                            select u;

                if (users.Count() > 0)
                {
                    // user already has an account, use this
                    questionEntity.User = users.FirstOrDefault();
                }
                else
                {
                    // user has no account, create one
                    UserEntity userEntity = new UserEntity();

                    userEntity.CreationTime = DateTime.Now;
                    userEntity.Type = UserType.Customer;

                    ctx.Users.Add(userEntity);

                    ctx.SaveChanges();

                    questionEntity.User = userEntity;   

                    // Mount the source to the new user
                    SourceEntity sourceEntity = new SourceEntity();
                    sourceEntity.Value = reference;
                    sourceEntity.CreationTime = DateTime.Now;
                    sourceEntity.SourceDefinition = questionEntity.SourceType;
                    sourceEntity.UserId = userEntity.Id;

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

        }

        public void DeclineAnswer(string feedback, string answerId, string questionId)
        {

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
                return new List<Review>(context.Reviews.Select(x => new Review()
                {
                    AnswerId = x.Answer.Id,
                    Content = x.Content,
                    Id = x.Id,
                    ReviewState = x.ReviewState,
                    User = ConvertEntities.UserEntityToUser(x.User)
                }));
            }
        }

        public List<Answer> GetAnswersUpForReview(string employeeId)
        {
            Validation.IdCheck(employeeId);

            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                return new List<Answer>(context.Answers.Select(x => new Answer()
                {
                    Content = x.Content,
                    Id = x.Id,
                    AnswerState = x.AnswerState,
                    Question = ConvertEntities.QuestionEntityToQuestion(x.Question),
                    User = ConvertEntities.UserEntityToUser(x.User)
                }));
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
