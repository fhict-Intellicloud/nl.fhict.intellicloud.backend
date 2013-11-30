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
        public IList<Question> GetQuestions(int employeeId)
        {
            throw new NotImplementedException();
        }

        public Question GetQuestion(string id)
        {
            throw new NotImplementedException();
        }

        public void CreateQuestion(string source, string reference, string question)
        {
            throw new NotImplementedException();
        }

<<<<<<< HEAD
        public void UpdateQuestion(string id, Common.DataTransfer.Question question)
        {            
            manager.UpdateQuestion(id, question);            
=======
        public void UpdateAnswer(string id, int employeeId)
        {
            throw new NotImplementedException();
>>>>>>> upstream/master
        }
    }
}
