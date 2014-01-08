using nl.fhict.IntelliCloud.Business.Manager;
using System.Collections.Generic;
using nl.fhict.IntelliCloud.Common.DataTransfer;

namespace nl.fhict.IntelliCloud.Service
{
    /// <summary>
    /// A service providing functionality related to customer feedback.
    /// </summary>
    public class FeedbackService : IFeedbackService
    {
        private readonly FeedbackManager manager;

        public FeedbackService()
        {
            this.manager = new FeedbackManager();
        }
        
        public void CreateFeedback(string feedback, int answerId, int questionId, FeedbackType feedbackType, string feedbackToken)
        {
            this.manager.CreateFeedback(feedback, answerId, questionId, feedbackType, feedbackToken);
        }

        public void UpdateFeedback(string id, FeedbackState feedbackState)
        {
            this.manager.UpdateFeedback(id, feedbackState);
        }

        public Feedback GetFeedback(string id)
        {
            return this.manager.GetFeedback(id);
        }

        public User GetUser(string id)
        {
            return this.manager.GetUser(id);
        }

        public Question GetQuestion(string id)
        {
            return this.manager.GetQuestion(id);
        }

        public Answer GetAnswer(string id)
        {
            return this.manager.GetAnswer(id);
        }
    }
}
