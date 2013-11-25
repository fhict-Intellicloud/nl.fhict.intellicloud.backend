using nl.fhict.IntelliCloud.Business.Manager;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using System;
using System.Collections.Generic;

namespace nl.fhict.IntelliCloud.Service
{
    public class IntelliCloudService : IIntelliCloudService
    {

        private readonly IntelliCloudManager manager;

        public IntelliCloudService()
        {
            this.manager = new IntelliCloudManager();            
        }

        public void AskQuestion(string source, string reference, string question)
        {
            manager.AskQuestion(source, reference, question);
        }

        public void SendAnswer(string questionId, string answerId)
        {

        }

        public void CreateAnswer(string questionId, string answer, string answererId, string answerState)
        {
            manager.CreateAnswer(questionId, answer, answererId, answerState);   
        }

        public void UpdateAnswer(string answerId, string answerState)
        {
            
        }

        public List<Question> GetQuestions(string employeeId)
        {
            List<Question> questions = new List<Question>();

            questions.Add(new Question() { User = new User() { FirstName = "UserOne", Infix = "of", LastName = "LastName" } });
            questions.Add(new Question() { User = new User() { FirstName = "UserTwo", Infix = "of", LastName = "LastName" } });
            questions.Add(new Question() { User = new User() { FirstName = "UserThree", Infix = "of", LastName = "LastName" } });
            questions.Add(new Question() { User = new User() { FirstName = "UserFour", Infix = "of", LastName = "LastName" } });
            questions.Add(new Question() { User = new User() { FirstName = "UserFive", Infix = "of", LastName = "LastName" } });
            questions.Add(new Question() { User = new User() { FirstName = "UserSix", Infix = "of", LastName = "LastName" } });

            return questions;
        }

        public void AcceptAnswer(string feedback, string answerId, string questionId)
        {
            manager.AcceptAnswer(feedback, answerId, questionId);
        }

        public void DeclineAnswer(string feedback, string answerId, string questionId)
        {
            manager.DeclineAnswer(feedback, answerId, questionId);
        }

        public void SendReviewForAnswer(string reviewerId, string answerId, string review)
        {
            manager.SendReviewForAnswer(reviewerId, answerId, review);
        }

        public void UpdateReview(string reviewId, string reviewState)
        {
            
        }

        public List<Review> GetReviewsForAnswer(string answerId)
        {
            return manager.GetReviewsForAnswer(answerId);
        }

        public List<Answer> GetAnswersUpForReview(string employeeId)
        {
            return manager.GetAnswersUpForReview(employeeId);
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
