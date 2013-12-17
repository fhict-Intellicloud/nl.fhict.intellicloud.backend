using nl.fhict.IntelliCloud.Business.Manager;
using System.Collections.Generic;
using nl.fhict.IntelliCloud.Common.DataTransfer;

namespace nl.fhict.IntelliCloud.Service
{
    /// <summary>
    /// A service providing functionality related to questions.
    /// </summary>
    public class QuestionService : IQuestionService
    {
        private readonly QuestionManager manager;

        public QuestionService()
        {
            this.manager = new QuestionManager();
        }

        public Question GetQuestion(string id)
        {
            return this.manager.GetQuestion(id);
        }

        public void CreateQuestion(
            string source, string reference, string question, string title, string postId = null, bool isPrivate = false)
        {
            this.manager.CreateQuestion(source, reference, question, title, postId, isPrivate);
        }

        public void UpdateQuestion(string id, int employeeId)
        {
            this.manager.UpdateQuestion(id, employeeId);
        }
        
        public Question GetQuestionByFeedbackToken(string feedbackToken)
        {
            return manager.GetQuestionByFeedbackToken(feedbackToken);
        }

        public IList<Question> GetQuestions(QuestionState? state = null)
        {
            return this.manager.GetQuestions(state);
        }

        public User GetAsker(string id)
        {
            return this.manager.GetAsker(id);
        }

        public User GetAnswerer(string id)
        {
            return this.manager.GetAnswerer(id);
        }

        public User GetAnswer(string id)
        {
            return this.manager.GetAnswer(id);
        }

        public IList<Keyword> GetKeywords(string id)
        {
            return this.manager.GetKeywords(id);
        }
    }
}
