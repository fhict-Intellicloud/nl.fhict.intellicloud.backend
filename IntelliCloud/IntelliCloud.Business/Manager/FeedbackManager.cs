using nl.fhict.IntelliCloud.Common.CustomException;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Context;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;
using System;
using System.Data.Entity;
using System.Linq;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    /// <summary>
    /// Class used for handling service requests related to user feedback.
    /// </summary>
    public class FeedbackManager : BaseManager
    {
        /// <summary>
        /// Constructor that sets the IValidation property to the given value.
        /// </summary>
        /// <param name="validation">IValidation to be set.</param>
        public FeedbackManager(IValidation validation)
            : base(validation)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FeedbackManager()
            : base()
        {
        }

        /// <summary>
        /// Method used for saving user feedback for an answer.
        /// </summary>
        /// <param name="feedback">The textual feedback given by the user.</param>
        /// <param name="answerId">The id of the answer for which the feedback has been given.</param>
        /// <param name="questionId">The id of the question to which the answer is assigned.</param>
        /// <param name="feedbackType">The type of feedback which indicates if the user accepted or declined the answer.</param>
        /// <param name="feedbackToken">The feedback token is required to provide feedback to answers on a question. It
        /// is used to make sure the user that asked the question is also the user giving the feedback and that feedback
        /// can only be given once.</param>
        public void CreateFeedback(string feedback, int answerId, int questionId, FeedbackType feedbackType, string feedbackToken)
        {
            // Validate input data
            Validation.StringCheck(feedback);
            Validation.IdCheck(answerId);
            Validation.IdCheck(questionId);
            Validation.StringCheck(feedbackToken);

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
                                          .Include(q => q.User)
                                          .Include(q => q.Source)
                                          .Include(q => q.LanguageDefinition)
                                          .Include(q => q.Answer)
                                          .Include(q => q.Answer.User)
                                          .SingleOrDefault(q => q.Id == questionId);

                if (question == null)
                    throw new NotFoundException("No question entity exists with the specified ID.");

                // Check if the user who asked the question is the one to posted the feedback and make sure feedback is
                // only given once.
                if (question.FeedbackToken != feedbackToken)
                    throw new InvalidOperationException(
                        "Feedback can only be given once by the user who asked the question.");
                
                // Set the state of the question to Open - employee needs to process the feedback given by the user
                question.QuestionState = QuestionState.Open;
                // Reset token so feedback can only be given once.
                question.FeedbackToken = null;

                // Store the user's feedback for the given answer
                FeedbackEntity feedbackEntity = new FeedbackEntity();
                feedbackEntity.Question = question;
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
                FeedbackEntity feedback = context.Feedbacks
                                          .Include(f => f.Question)
                                          .Include(f => f.Answer)
                                          .SingleOrDefault(f => f.Id == iFeedbackId);

                if (feedback == null)
                    throw new NotFoundException("No feedback entity exists with the specified ID.");

                // Update the state of the feedback entry
                feedback.FeedbackState = feedbackState;
                feedback.LastChangedTime = DateTime.UtcNow;

                // Save the changes to the context
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Retrieve the answer for the feedback with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the feedback.</param>
        /// <returns>Returns the answer for the feedback with the given identifier.</returns>
        public Answer GetAnswer(string id)
        {
            Validation.IdCheck(id);

            using (var context = new IntelliCloudContext())
            {
                var feedbackId = Convert.ToInt32(id);
                FeedbackEntity feedback = context.Feedbacks.Include(r => r.Answer).SingleOrDefault(r => r.Id.Equals(feedbackId));

                if (feedback == null)
                    throw new NotFoundException("No feedback entity exists with the specified ID.");

                return feedback.Answer.AsAnswer();
            }
        }

        public Question GetQuestion(string id)
        {
            Validation.IdCheck(id);

            using (var context = new IntelliCloudContext())
            {
                var feedbackId = Convert.ToInt32(id);
                FeedbackEntity feedback = context.Feedbacks.Include(r => r.Question).SingleOrDefault(r => r.Id.Equals(feedbackId));

                if (feedback == null)
                    throw new NotFoundException("No feedback entity exists with the specified ID.");

                return feedback.Question.AsQuestion();
            }
        }

        /// <summary>
        /// Retrieve the user that gave the feedback with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the feedback.</param>
        /// <returns>Returns the user that gave the feedback with the given identifier.</returns>
        public User GetUser(string id)
        {
            Validation.IdCheck(id);

            using (var context = new IntelliCloudContext())
            {
                var feedbackId = Convert.ToInt32(id);
                FeedbackEntity feedback = context.Feedbacks.Include(r => r.User).SingleOrDefault(r => r.Id.Equals(feedbackId));

                if (feedback == null)
                    throw new NotFoundException("No feedback entity exists with the specified ID.");

                return feedback.User.AsUser();
            }
        }

        /// <summary>
        /// Retrieve the feedback with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the feedback.</param>
        /// <returns>Returns the feedback with the given identifier.</returns>
        public Feedback GetFeedback(string id)
        {
            Validation.IdCheck(id);

            using (var context = new IntelliCloudContext())
            {
                var feedbackId = Convert.ToInt32(id);
                FeedbackEntity feedback = context.Feedbacks.SingleOrDefault(r => r.Id.Equals(feedbackId));

                if (feedback == null)
                    throw new NotFoundException("No feedback entity exists with the specified ID.");

                return feedback.AsFeedback();
            }
        }
    }
}
