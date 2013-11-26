using nl.fhict.IntelliCloud.Common.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace nl.fhict.IntelliCloud.Service
{
    [ServiceContract]
    public interface IQuestionService
    {
        /// <summary>
        /// This method is used to get question with multiple possible parameters
        /// </summary>
        /// <param name="questionId">Optional parameter to get a question by Id</param>
        /// <param name="employeeId">Optional parameter to get all the availible question for this employee</param>
        /// <returns>Returns a list of questions</returns>
        [OperationContract]
        [WebGet(UriTemplate = "Get?employeeId={employeeId}&questionId={questionId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        List<Question> GetQuestions(int questionId, int employeeId);
                
        /// <summary>
        /// This method is used to ask a question
        /// </summary>
        /// <param name="question">The question object that will be added to the database</param>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "Post",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void AskQuestion(Question question);

        /// <summary>
        /// This method is used to update a question
        /// </summary>
        /// <param name="id">The id of the question you want to update</param>
        /// <param name="question">The question object used to update the question</param>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "Update/{Id}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void UpdateQuestion(String id, Question question);

    }
}
