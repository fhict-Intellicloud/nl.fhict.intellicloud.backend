using nl.fhict.IntelliCloud.Common.DataTransfer;
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

        public void AskQuestion(string source, string reference, string question)
        {

        }

        public void SendAnswer(string questionId, string answer, string answererId)
        {

        }

        public List<Question> GetQuestions(string employeeId)
        {
            List<Question> questions = new List<Question>();

            return questions;
        }

        public void AcceptAnswer(string feedback, string answerId, string questionId)
        {

        }

        public void DeclineAnswer(string feedback, string answerId, string questionId)
        {

        }

        public void SendAnswerForReview(string answer, string questionId, string answererId)
        {
            

        }

        public void SendReviewForAnswer(string reviewerId, string answerId, string review)
        {

        }

        public List<Review> GetReviewsForAnswer(string answerId)
        {
            List<Review> reviews = new List<Review>();

            return reviews;
        }

        public List<Answer> GetAnswersUpForReview(string employeeId)
        {
            List<Answer> answers = new List<Answer>();

            return answers;
        }


        public Answer GetAnswerById(string answerId)
        {
            return new Answer();

        }

        public Question GetQuestionById(string questionId)
        {
            return new Question();
        }
    }
}
