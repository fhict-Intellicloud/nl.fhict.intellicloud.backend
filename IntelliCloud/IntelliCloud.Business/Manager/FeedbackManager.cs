using nl.fhict.IntelliCloud.Common.CustomException;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

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

            // Return all feedback entries assigned to the specified answer
            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                return context.Feedbacks
                       .Include("Answer")
                       .Include("Question")
                       .Include("Question.SourceDefinition")
                       .Include("User")
                       .Include("User.Sources")
                       .Where(f => f.Answer.Id == answerId)
                       .ToList()
                       .Select(f => ConvertEntities.FeedbackEntityToFeedback(f))
                       .ToList();
            }
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
                // Get the answer entity from the context
                AnswerEntity answer = context.Answers.SingleOrDefault(a => a.Id == answerId);

                if (answer == null)
                    throw new NotFoundException("No answer entity exists with the specified ID.");

                // Set the state of the answer to UnderReview - employee needs to process the feedback given by the user
                answer.AnswerState = AnswerState.UnderReview;

                // Get the question entity from the context
                QuestionEntity question = context.Questions
                                          .Include("User")
                                          .SingleOrDefault(q => q.Id == questionId);

                if (question == null)
                    throw new NotFoundException("No question entity exists with the specified ID.");

                // Set the state of the question to Open - employee needs to process the feedback given by the user
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

            // Convert the textual representation of the id to an integer
            int iFeedbackId = Convert.ToInt32(id);

            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // Get the feedback entity from the context
                FeedbackEntity feedback = context.Feedbacks.SingleOrDefault(f => f.Id == iFeedbackId);

                if (feedback == null)
                    throw new NotFoundException("No feedback entity exists with the specified ID.");

                // Update the state of the feedback entry
                feedback.FeedbackState = feedbackState;

                // Save the changes to the context
                context.SaveChanges();
            }
        }
    }
}
