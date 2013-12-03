using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Model;

namespace nl.fhict.IntelliCloud.Business
{
    /// <summary>
    /// This class will implement methods for converting Entities to domain objects.
    /// Only if it is necessary.
    /// </summary>
    public class ConvertEntities
    {
        /// <summary>
        /// Converts a UserEntity object to a User object.
        /// </summary>
        /// <param name="entity">The UserEntity that has to be converted.</param>
        /// <returns>The user object.</returns>
        public User UserEntityToUser(UserEntity entity)
        {
            User user = new User();
            user.Id = entity.Id;
            user.FirstName = entity.FirstName;
            user.Infix = entity.Infix;
            user.LastName = entity.LastName;
            user.Type = entity.Type;
            user.CreationTime = entity.CreationTime;
            user.Sources = SourceEntityListToSources(entity.Sources);

            return user;
        }

        /// <summary>
        /// Converts a list of SourceEntities to a list of Sources.
        /// </summary>
        /// <param name="sourceEntities">The SourceEntities that have to be converted.</param>
        /// <returns>The list of sources.</returns>
        public List<Source> SourceEntityListToSources(ICollection<SourceEntity> sourceEntities)
        {
            List<Source> sources = new List<Source>();
            foreach (SourceEntity source in sourceEntities)
            {
                Source temp = new Source();
                temp.Id = source.Id;
                temp.SourceDefinition = SourceDefinitionEntityToSourceDefinition(source.SourceDefinition);
                temp.Value = source.Value;
                temp.CreationTime = source.CreationTime;
                sources.Add(temp);
            }
            return sources;
        }

        /// <summary>
        /// Converts a SourceDefinitionEntity to a SourceDefinition.
        /// </summary>
        /// <param name="sourceDefinitionEntity">The SourceDefinitionEntity that have to be converted.</param>
        /// <returns>The sourcedefinition object.</returns>
        public SourceDefinition SourceDefinitionEntityToSourceDefinition(
            SourceDefinitionEntity sourceDefinitionEntity)
        {
            SourceDefinition sourceDefinition = new SourceDefinition();
            sourceDefinition.Id = sourceDefinitionEntity.Id;
            sourceDefinition.Name = sourceDefinitionEntity.Name;
            sourceDefinition.Description = sourceDefinitionEntity.Description;
            sourceDefinition.CreationTime = sourceDefinitionEntity.CreationTime;
            return sourceDefinition;
        }

        /// <summary>
        /// Converts a QuestionEntity to a Question.
        /// </summary>
        /// <param name="entity">The QuestionEntity that has to be converted.</param>
        /// <returns>The question object.</returns>
        public Question QuestionEntityToQuestion(QuestionEntity entity)
        {
            Question question = new Question();
            question.Id = entity.Id;
            if (entity.Answerer != null)
            {
                question.Answerer = UserEntityToUser(entity.Answerer);
            }
            question.User = UserEntityToUser(entity.User);
            question.Content = entity.Content;
            question.CreationTime = entity.CreationTime;
            question.QuestionState = entity.QuestionState;
            question.SourceDefinition = SourceDefinitionEntityToSourceDefinition(entity.SourceDefinition);
            return question;
        }

        /// <summary>
        /// Converts an AnswerEntity to an Answer.
        /// </summary>
        /// <param name="entity">The AnswerEntity that has to be converted.</param>
        /// <returns>The Answer object.</returns>
        public Answer AnswerEntityToAnswer(AnswerEntity entity)
        {
            Answer answer = new Answer();

            answer.Id = entity.Id;
            answer.CreationTime = entity.CreationTime;
            answer.Content = entity.Content;
            answer.AnswerState = entity.AnswerState;
            answer.User = UserEntityToUser(entity.User);

            return answer;
        }

        /// <summary>
        /// Converts a list of AnswerEntities to a list of Answers.
        /// </summary>
        /// <param name="entities">The AnswerEntities that have to be converted.</param>
        /// <returns>The list of answers.</returns>
        public List<Answer> AnswerEntityListToAnswerList(List<AnswerEntity> entities)
        {
            List<Answer> answers = new List<Answer>();
            foreach (AnswerEntity entity in entities)
            {                
                answers.Add(AnswerEntityToAnswer(entity));
            }

            return answers;
        }

        /// <summary>
        /// Converts a list of ReviewEntities to a list of Reviews.
        /// </summary>
        /// <param name="entities">The ReviewEntities that have to be converted.</param>
        /// <returns>The list of reviews.</returns>
        public List<Review> ReviewEntityListToReviewList(List<ReviewEntity> entities)
        {
            List<Review> reviews = new List<Review>();
            foreach (ReviewEntity entity in entities)
            {
                Review temp = new Review();
                temp.Id = entity.Id;
                temp.Content = entity.Content;
                temp.ReviewState = entity.ReviewState;
                temp.AnswerId = entity.Answer.Id;
                temp.CreationTime = entity.CreationTime;
                temp.User = this.UserEntityToUser(entity.User);
                reviews.Add(temp);
            }

            return reviews;
        }

        /// <summary>
        /// Converts a list of QuestionEntities to a list of Questions.
        /// </summary>
        /// <param name="entities">The QuestionEntities that have to be converted.</param>
        /// <returns>The list of questions.</returns>
        public List<Question> QuestionEntityListToQuestionList(List<QuestionEntity> entities)
        {
            List<Question> questions = new List<Question>();
            foreach (QuestionEntity entity in entities)
            {
                questions.Add(QuestionEntityToQuestion(entity));
            }
            return questions;
        }

        /// <summary>
        /// Method for converting a FeedbackEntity instance to a Feedback instance.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Instance of class Feedback.</returns>
        public Feedback FeedbackEntityToFeedback(FeedbackEntity entity)
        {
            // Construct and return a new instance of class Feedback, containing the data from the passed entity class
            return new Feedback()
            {
                Content = entity.Content,
                AnswerId = entity.Answer.Id,
                QuestionId = entity.Question.Id,
                User = UserEntityToUser(entity.User),
                FeedbackType = entity.FeedbackType,
                FeedbackState = entity.FeedbackState,
                CreationTime = entity.CreationTime
            };
        }
    }
}
