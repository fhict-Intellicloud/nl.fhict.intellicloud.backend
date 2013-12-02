using nl.fhict.IntelliCloud.Business;
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
    /// A service providing functionality related to questions.
    /// </summary>
    public class QuestionService : IQuestionService
    {
        private readonly QuestionManager manager;

        public QuestionService()
        {
            this.manager = new QuestionManager();
        }

        public IList<Question> GetQuestions(int employeeId)
        {
            return manager.GetQuestions(employeeId);
        }

        public Question GetQuestion(string id)
        {
            return manager.GetQuestion(Convert.ToInt32(id));
        }

        public void CreateQuestion(string source, string reference, string question, string title)
        {
            manager.CreateQuestion(source, reference, question, title);
        }

        public void UpdateQuestion(string id, int employeeId)
        {
            manager.UpdateQuestion(Convert.ToInt32(id), employeeId);
        }
    }
}
