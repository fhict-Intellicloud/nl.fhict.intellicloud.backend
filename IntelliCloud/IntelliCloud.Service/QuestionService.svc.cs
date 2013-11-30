using nl.fhict.IntelliCloud.Business;
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
        
        public List<Common.DataTransfer.Question> GetQuestions(int questionId, int employeeId)
        {
            // TODO remove TOString()
            if (questionId > 0){
                return manager.GetQuestions(questionId);
            }
            else if (employeeId > 0)
            {
                return manager.GetQuestionsForEmployee(employeeId);
            }
            else
            {
                return manager.GetQuestions();
            }
        }

        public void AskQuestion(Common.DataTransfer.Question question)
        {
            manager.AskQuestion(question);
        }

        public void UpdateQuestion(string id, Common.DataTransfer.Question question)
        {            
            manager.UpdateQuestion(id, question);            
        }
    }
}
