using nl.fhict.IntelliCloud.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace IntelliCloud
{
    public class IntelliCloud : IIntelliCloud
    {

        public bool AskQuestion(string source, string reference, string question)
        {
            return true;
        }

        public bool SendAnswer(string questionId, string answer, string answererId)
        {
            return true;
        }

        public List<Question> GetQuestions(string employeeId)
        {
            List<Question> questions = new List<Question>();

            return questions;
        }

        public bool AcceptAnswer(string feedback, string answerId, string questionId)
        {
            return true;
        }

        public bool DeclineAnswer(string feedback, string answerId, string questionId)
        {
            return true;
        }

        public bool SendAnswerForReview(string answer, string questionId, string answererId)
        {
            return true;
        }

        public bool SendReviewForAnswer(string reviewerId, string answerId, string review)
        {
            return true;
        }

        public List<nl.fhict.IntelliCloud.Classes.Review> GetReviewsForAnswer(string answerId)
        {
            List<Review> reviews = new List<Review>();

            return reviews;
        }

        public List<nl.fhict.IntelliCloud.Classes.Answer> GetAnswersUpForReview(string employeeId)
        {
            List<Answer> answers = new List<Answer>();

            return answers;
        }
    }
}
