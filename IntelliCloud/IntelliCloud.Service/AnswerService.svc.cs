using nl.fhict.IntelliCloud.Business.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace nl.fhict.IntelliCloud.Service
{
    public class AnswerService : IAnswerService
    {
        private readonly IntelliCloudManager manager;

        public AnswerService()
        {
            this.manager = new IntelliCloudManager();            
        }

        public void AcceptAnswer(string feedback, string answerId, string questionId)
        {
            manager.AcceptAnswer(feedback, answerId, questionId);
        }

        public void DeclineAnswer(string feedback, string answerId, string questionId)
        {
            manager.DeclineAnswer(feedback, answerId, questionId);
        }

        public List<Common.DataTransfer.Answer> GetAnswersUpForReview(string employeeId)
        {
            return manager.GetAnswersUpForReview(employeeId);
        }

        public Common.DataTransfer.Answer GetAnswerById(string answerId)
        {
            return manager.GetAnswerById(answerId)
;
        }
    }
}
