using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;

namespace nl.fhict.IntelliCloud.Business.Plugins.Loader
{
    /// <summary>
    /// An interface for a plugin providing functionality to send answers using a specified service, like mail, 
    /// Facebook or Twitter.
    /// </summary>
    public interface ISendAnswerPlugin
    {
        /// <summary>
        /// Send the answer using the plugin. The answer is send to the source specified in 
        /// <see cref="Question.Source"/>. The <see cref="QuestionSourceEntity"/> contains the <see cref="SourceEntity"/> with the 
        /// username or email address. When available also the post identifier to which needs to be replied is contained
        /// in the <see cref="QuestionSourceEntity"/>.
        /// </summary>
        /// <param name="question">The question which is answered.</param>
        /// <param name="answer">The answer which needs to be send.</param>
        void SendAnswer(QuestionEntity question, AnswerEntity answer);

        /// <summary>
        /// Send a received confirmation to the source.
        /// </summary>
        /// <param name="question"></param>
        void SendQuestionReceived(QuestionEntity question);
    }
}
