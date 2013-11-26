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
        /// This method is used to ask a question to a employee.
        /// </summary>
        /// <param name="source">the name of the source as the source is known in the database</param>
        /// <param name="reference">the reference needed by the plugin to send the answers back</param>
        /// <param name="question">the question itself</param>
        /// <returns>Returns whether the question upload was succesfull or failed</returns>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "Post/{source}/{reference}/{question}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void AskQuestion(String source, String reference, String question);

        /// <summary>
        /// This method return the question availible to this employee
        /// </summary>
        /// <param name="employeeId">The employeeId of the employee who requests the questions</param>
        /// <returns>Returns the questions availible to this employee</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "GetByEmployeeId/{employeeId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<Question> GetQuestions(String employeeId);

        /// <summary>
        /// This method is used to get the question by the questionId
        /// </summary>
        /// <param name="questionId">this Id of the quetion you want to recieve</param>
        /// <returns>Returns the question you want to use</returns>
        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "GetByQuestionId/{questionId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Question GetQuestionById(String questionId);

    }
}
