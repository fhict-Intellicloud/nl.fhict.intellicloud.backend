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
    /// <summary>
    /// An interface for a service providing functionality related to questions.
    /// </summary>
    [ServiceContract]
    public interface IQuestionService
    {
        /// <summary>
        /// Retrieves the available questions and optionally filtering them using the employee identifier.
        /// </summary>
        /// <param name="employeeId">The optional employee identifier, only questions about which the employee has 
        /// knowledge are returned (keywords between user and question match).</param>
        /// <returns>Returns the questions that match the filters.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "questions?employeeId={employeeId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        IList<Question> GetQuestions(int employeeId);

        /// <summary>
        /// Retrieve the question with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the question.</param>
        /// <returns>Returns the question with the given identifier.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "questions/{id}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Question GetQuestion(string id);

        /// <summary>
        /// Creates a new question.
        /// </summary>
        /// <param name="source">The source from which the question was send, e.g. "Mail", "Facebook" or "Twitter".</param>
        /// <param name="reference">The identifier for the source, e.g. the username or email address.</param>
        /// <param name="question">The question that was answered.</param>
        /// <param name="title">The title of the question. The title contains a short summary of the question.</param>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "questions",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void CreateQuestion(string source, string reference, string question, string title);

        /// <summary>
        /// Updates the question with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the question that is updated.</param>
        /// <param name="employeeId">The identifier of the employee that is going to answer the question.</param>
        [OperationContract]
        [WebInvoke(Method = "PUT",
            UriTemplate = "questions/{id}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void UpdateQuestion(string id, int employeeId);
    }
}