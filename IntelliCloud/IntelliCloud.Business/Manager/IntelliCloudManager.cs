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
                List<ReviewEntity> reviewEntities = context.Reviews.Where(r => r.Answer.Id.Equals(Convert.ToInt32(answerId))).ToList();

                List<Review> reviews = new List<Review>();
                foreach (ReviewEntity entity in reviewEntities)
                {
                    Review temp = new Review();
                    temp.AnswerId = entity.Answer.Id;
                    temp.Content = entity.Content;
                    temp.Id = entity.Id;
                    temp.ReviewState = entity.ReviewState;
                    temp.User = ConvertEntities.UserEntityToUser(entity.User);
                    
                    reviews.Add(temp);
                }

                return reviews;
            }
        }

        public List<Answer> GetAnswersUpForReview(string employeeId)
        {
            Validation.IdCheck(employeeId);

            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                List<AnswerEntity> answerEntities =
                    context.Answers.Where(a => a.AnswerState.Equals(AnswerState.UnderReview)).ToList();

                List<Answer> answers = new List<Answer>();
                foreach (AnswerEntity entity in answerEntities)
                {
                    Answer temp = new Answer();
                    temp.Id = entity.Id;
                    temp.Content = entity.Content;
                    temp.AnswerState = entity.AnswerState;
                    temp.Question = ConvertEntities.QuestionEntityToQuestion(entity.Question);
                    temp.User = ConvertEntities.UserEntityToUser(entity.User);

                    answers.Add(temp);
                }

                return answers;
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
