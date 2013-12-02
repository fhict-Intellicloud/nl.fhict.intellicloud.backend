using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    /// <summary>
    /// Class used for handling service requests related to user feedback.
    /// </summary>
    public class FeedbackManager : BaseManager
    {
        /// <summary>
        /// Method used for retrieving all feedback entries assigned to a specific answer.
        /// </summary>
        /// <param name="answerId">The id of the answer to which all feedback entries to be retrieved are assigned to.</param>
        /// <returns>A list of instances of class Feedback.</returns>
        public IList<Feedback> GetFeedbacks(int answerId)
        {
            // Validate input data
            Validation.IdCheck(answerId);

            // Create a new list that will hold all instances of class Feedback assigned to the specified answer
            List<Feedback> feedbacks = new List<Feedback>();

            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // Get all feedback entities assigned to the specified answer
                List<FeedbackEntity> feedbackEntities = (from f in context.Feedbacks
                                                         .Include("Answer")
                                                         .Include("Question")
                                                         .Include("User")
                                                         where f.Answer.Id == answerId
                                                         select f).ToList();

                // Convert the list of feedback entities to instances of class Feedback
                feedbacks = ConvertEntities.FeedbackEntityListToFeedbackList(feedbackEntities);
            }

            // Return all feedback entries assigned to the specified answer
            return feedbacks;
        }

        /// <summary>
        /// Method used for saving user feedback for an answer.
        /// </summary>
        /// <param name="feedback">The textual feedback given by the user.</param>
        /// <param name="answerId">The id of the answer for which the feedback has been given.</param>
        /// <param name="questionId">The id of the question to which the answer is assigned.</param>
        /// <param name="feedbackType">The type of feedback which indicates if the user accepted or declined the answer.</param>
        public void CreateFeedback(string feedback, int answerId, int questionId, FeedbackType feedbackType)
        {
            // Validate input data
            Validation.StringCheck(feedback);
            Validation.IdCheck(answerId);
            Validation.IdCheck(questionId);

            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // Set the state of the answer to Declined
                AnswerEntity answer = context.Answers.Single(a => a.Id == answerId);
                answer.AnswerState = AnswerState.UnderReview;

                // Set the state of the question to Open - employee needs to process the feedback given by the user
                QuestionEntity question = context.Questions
                                          .Include("User")
                                          .Single(q => q.Id == questionId);
                question.QuestionState = QuestionState.Open;

                // Store the user's feedback for the given answer
                FeedbackEntity feedbackEntity = new FeedbackEntity();
                feedbackEntity.Answer = answer;
                feedbackEntity.Content = feedback;
                feedbackEntity.CreationTime = DateTime.UtcNow;
                feedbackEntity.FeedbackState = FeedbackState.Open;
                feedbackEntity.FeedbackType = feedbackType;
                feedbackEntity.User = question.User;
                context.Feedbacks.Add(feedbackEntity);

                // Save the changes to the context
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Method used for updating the state of an existing feedback entry.
        /// </summary>
        /// <param name="id">The id of the feedback entry to update.</param>
        /// <param name="feedbackState">The new state of the feedback entry.</param>
        public void UpdateFeedback(string id, FeedbackState feedbackState)
        {
            // Validate input data
            Validation.IdCheck(id);

            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // Get the feedback entity from the context
                int iFeedbackId = Convert.ToInt32(id);
                FeedbackEntity feedback = context.Feedbacks.Single(f => f.Id == iFeedbackId);

                // Update the state of the feedback entry
                feedback.FeedbackState = feedbackState;

                // Save the changes to the context
                context.SaveChanges();
            }
        }
    }
}
