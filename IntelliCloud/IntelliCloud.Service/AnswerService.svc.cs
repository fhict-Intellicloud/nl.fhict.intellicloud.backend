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
    /// A service providing functionality related to answers.
    /// </summary>
    public class AnswerService : IAnswerService
    {
        public IList<Answer> GetAnswers(AnswerState answerState, int employeeId)
        {
            throw new NotImplementedException();
        }

        public Answer GetAnswer(string id)
        {
            throw new NotImplementedException();
        }

        public void CreateAnswer(int questionId, string answer, int answererId, AnswerState answerState)
        {
            throw new NotImplementedException();
        }

        public void UpdateAnswer(string id, AnswerState answerState)
        {
            throw new NotImplementedException();
        }
    }
}
