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

        public IList<Feedback> GetFeedbacks(int answerId)
        {
            return this.manager.GetFeedbacks(answerId);
        }

        public void CreateFeedback(string feedback, int answerId, int questionId, FeedbackType feedbackType)
        {
            this.manager.CreateFeedback(feedback, answerId, questionId, feedbackType);
        }

        public void UpdateFeedback(string id, FeedbackState feedbackState)
        {
            this.manager.UpdateFeedback(id, feedbackState);
        }
    }
}
