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

            questions.Add(new Question() { User = new User() { FirstName = "UserOne", Infix = "of", LastName = "LastName" }, Content = "Questio one!!1", CreationTime = DateTime.Now, Id = 1, QuestionState = QuestionState.Open, SourceDefinition = new SourceDefinition { CreationTime = DateTime.UtcNow, Id = 1, Description = "Mail", Name = "Mail" }, Keywords = new List<Keyword> { new Keyword { Affinity = 1, CreationTime = DateTime.UtcNow, Id = 1, Name = "Henk" } } });
            questions.Add(new Question() { User = new User() { FirstName = "UserOne", Infix = "of", LastName = "LastName" }, Content = "Questio one!!1", CreationTime = DateTime.Now, Id = 1, QuestionState = QuestionState.Open, SourceDefinition = new SourceDefinition { CreationTime = DateTime.UtcNow, Id = 1, Description = "Mail", Name = "Mail" }, Keywords = new List<Keyword> { new Keyword { Affinity = 1, CreationTime = DateTime.UtcNow, Id = 1, Name = "Henk" } } });
            questions.Add(new Question() { User = new User() { FirstName = "UserOne", Infix = "of", LastName = "LastName" }, Content = "Questio one!!1", CreationTime = DateTime.Now, Id = 1, QuestionState = QuestionState.Open, SourceDefinition = new SourceDefinition { CreationTime = DateTime.UtcNow, Id = 1, Description = "Mail", Name = "Mail" }, Keywords = new List<Keyword> { new Keyword { Affinity = 1, CreationTime = DateTime.UtcNow, Id = 1, Name = "Henk" } } });
            questions.Add(new Question() { User = new User() { FirstName = "UserOne", Infix = "of", LastName = "LastName" }, Content = "Questio one!!1", CreationTime = DateTime.Now, Id = 1, QuestionState = QuestionState.Open, SourceDefinition = new SourceDefinition { CreationTime = DateTime.UtcNow, Id = 1, Description = "Mail", Name = "Mail" }, Keywords = new List<Keyword> { new Keyword { Affinity = 1, CreationTime = DateTime.UtcNow, Id = 1, Name = "Henk" } } });
            questions.Add(new Question() { User = new User() { FirstName = "UserOne", Infix = "of", LastName = "LastName" }, Content = "Questio one!!1", CreationTime = DateTime.Now, Id = 1, QuestionState = QuestionState.Open, SourceDefinition = new SourceDefinition { CreationTime = DateTime.UtcNow, Id = 1, Description = "Mail", Name = "Mail" }, Keywords = new List<Keyword> { new Keyword { Affinity = 1, CreationTime = DateTime.UtcNow, Id = 1, Name = "Henk" } } });
            questions.Add(new Question() { User = new User() { FirstName = "UserOne", Infix = "of", LastName = "LastName" }, Content = "Questio one!!1", CreationTime = DateTime.Now, Id = 1, QuestionState = QuestionState.Open, SourceDefinition = new SourceDefinition { CreationTime = DateTime.UtcNow, Id = 1, Description = "Mail", Name = "Mail" }, Keywords = new List<Keyword> { new Keyword { Affinity = 1, CreationTime = DateTime.UtcNow, Id = 1, Name = "Henk" } } });
            
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
