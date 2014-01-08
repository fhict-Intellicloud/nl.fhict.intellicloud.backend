using nl.fhict.IntelliCloud.Business.Manager;
using System.Collections.Generic;
using nl.fhict.IntelliCloud.Common.DataTransfer;

namespace nl.fhict.IntelliCloud.Service
{
    /// <summary>
    /// A service providing functionality related to answers.
    /// </summary>
    public class AnswerService : IAnswerService
    {

        private readonly AnswerManager manager;

        public AnswerService()
        {
            this.manager = new AnswerManager();
        }

        public Answer GetAnswer(string id)
        {
            return this.manager.GetAnswer(id);
        }

        public void CreateAnswer(int questionId, string answer, int answererId, AnswerState answerState)
        {
            this.manager.CreateAnswer(questionId, answer, answererId, answerState);
        }

        public void UpdateAnswer(string id, AnswerState answerState, string answer)
        {
            this.manager.UpdateAnswer(id, answerState, answer);
        }

        public IList<Answer> GetAnswers(AnswerState state, string search = null)
        {
            return this.manager.GetAnswers(state, search);
        }

        public User GetAnswerer(string id)
        {
            return this.manager.GetAnswerer(id);
        }

        public IList<Feedback> GetFeedbacks(string id, FeedbackState state)
        {
            return this.manager.GetFeedbacks(id, state);
        }

        public IList<Review> GetReviews(string id, ReviewState state)
        {
            return this.manager.GetReviews(id, state);
        }

        public IList<Keyword> GetKeywords(string id)
        {
            return this.manager.GetKeywords(id);
        }
    }
}
