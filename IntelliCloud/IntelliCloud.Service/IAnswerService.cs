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
    /// An interface for a service providing functionality related to answers.
    /// </summary>
    [ServiceContract]
    public interface IAnswerService
    {
        /// <summary>
        /// Retrieves all the available answers and optionally filtering them using the answer state or employee 
        /// identifier.
        /// </summary>
        /// <param name="answerState">The optional answer state, only answers with this state will be returned.
        /// </param>
        /// <param name="employeeId">The optional employee identifier, only answers about which the employee has 
        /// knowledge are returned (keywords between user and answer match).</param>
        /// <returns>Returns the answers that match the filters.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "answers?state={answerState}&employeeId={employeeId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        IList<Answer> GetAnswers(AnswerState answerState, int employeeId);

        /// <summary>
        /// Retrieve the answer with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the answer.</param>
        /// <returns>Returns the answer with the given identifier.</returns>
        [OperationContract]
        [WebGet(UriTemplate = "answers/{id}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Answer GetAnswer(string id);

        /// <summary>
        /// Creates a new answer.
        /// </summary>
        /// <param name="questionId">The identifier of the question which is answered.</param>
        /// <param name="answer">The content of the given answer.</param>
        /// <param name="answererId">The employee who answered the question.</param>
        /// <param name="answerState">The state of the answer.</param>
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "answers",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void CreateAnswer(int questionId, string answer, int answererId, AnswerState answerState);

        /// <summary>
        /// Updates the answer with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the answer that is updated.</param>
        /// <param name="answerState">The new state of the answer.</param>
        [OperationContract]
        [WebInvoke(Method = "PUT",
            UriTemplate = "answers/{id}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        void UpdateAnswer(string id, AnswerState answerState);
    }
}
