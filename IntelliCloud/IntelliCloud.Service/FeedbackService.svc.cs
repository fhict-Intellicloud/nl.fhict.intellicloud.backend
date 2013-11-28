using nl.fhict.IntelliCloud.Business.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using nl.fhict.IntelliCloud.Common.DataTransfer;

namespace nl.fhict.IntelliCloud.Service
{
    /// <summary>
    /// A service providing functionality related to customer feedback.
    /// </summary>
    public class FeedbackService : IFeedbackService
    {
        public IList<Feedback> GetFeedbacks(int answerId)
        {
            throw new NotImplementedException();
        }

        public void CreateFeedback(string feedback, int answerId, int questionId, FeedbackType feedbackType)
        {
            throw new NotImplementedException();
        }
    }
}
