using nl.fhict.IntelliCloud.Business.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Common.CustomException;
using System.ServiceModel.Web;
using System.Net;

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

        public IList<Answer> GetAnswers(AnswerState answerState, int? employeeId)
        {
            return manager.GetAnswers(answerState, employeeId);
        }

        public Answer GetAnswer(string id)
        {
            return manager.GetAnswer(id);
        }

        public void CreateAnswer(int questionId, string answer, int answererId, AnswerState answerState)
        {
            manager.CreateAnswer(questionId, answer, answererId, answerState);
        }

        public void UpdateAnswer(string id, AnswerState answerState, string answer)
        {
            manager.UpdateAnswer(id, answerState, answer);
        }
    }
}
