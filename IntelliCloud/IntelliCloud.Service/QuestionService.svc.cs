using nl.fhict.IntelliCloud.Business.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace nl.fhict.IntelliCloud.Service
{
    public class QuestionService : IQuestionService
    {
        private readonly IntelliCloudManager manager;

        public QuestionService()
        {
            this.manager = new IntelliCloudManager();            
        }

        public void AskQuestion(string source, string reference, string question)
        {
            manager.AskQuestion(source, reference, question);
        }

        public List<Common.DataTransfer.Question> GetQuestions(string employeeId)
        {
            return manager.GetQuestions(employeeId);
        }

        public Common.DataTransfer.Question GetQuestionById(string questionId)
        {
            return manager.GetQuestionById(questionId);
        }
    }
}
