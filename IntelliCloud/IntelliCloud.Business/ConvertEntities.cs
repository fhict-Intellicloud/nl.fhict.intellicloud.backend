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
<<<<<<< HEAD
        /// <returns>The question onbject.</returns>
        public Question QuestionEntityToQuestion(QuestionEntity entity)
=======
        /// <returns>The question object.</returns>
        public static Question QuestionEntityToQuestion(QuestionEntity entity)
>>>>>>> upstream/master
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

<<<<<<< HEAD
        public List<Answer> AnswerEntityListToAnswerList(List<AnswerEntity> entities)
        {
            List<Answer> answers = new List<Answer>();
            foreach (AnswerEntity entity in entities)
            {
                Answer temp = new Answer();
                temp.Id = entity.Id;
                temp.CreationTime = entity.CreationTime;
                temp.Content = entity.Content;
                temp.AnswerState = entity.AnswerState;
                temp.Question = this.QuestionEntityToQuestion(entity.Question);
                temp.User = this.UserEntityToUser(entity.User);
                answers.Add(temp);
=======
        public static Answer AnswerEntityToAnswer(AnswerEntity entity)
        {
            Answer answer = new Answer();

            answer.Id = entity.Id;
            answer.CreationTime = entity.CreationTime;
            answer.Content = entity.Content;
            answer.AnswerState = entity.AnswerState;
            answer.User = ConvertEntities.UserEntityToUser(entity.User);

            return answer;
        }

        public static List<Answer> AnswerEntityListToAnswerList(List<AnswerEntity> entities)
        {
            List<Answer> answers = new List<Answer>();
            foreach (AnswerEntity entity in entities)
            {                
                answers.Add(AnswerEntityToAnswer(entity));
>>>>>>> upstream/master
            }

            return answers;
        }

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

        public List<Question> QuestionEntityListToQuestion(List<QuestionEntity> entities)
        {
            List<Question> questions = new List<Question>();
            foreach (QuestionEntity entity in entities)
            {
                questions.Add(QuestionEntityToQuestion(entity));
            }
            return questions;
        }

    }
}
