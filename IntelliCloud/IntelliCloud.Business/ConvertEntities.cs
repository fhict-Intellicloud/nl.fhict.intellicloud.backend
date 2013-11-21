using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Model;

namespace nl.fhict.IntelliCloud.Business
{
    public class ConvertEntities
    {
        /// <summary>
        /// Converts a UserEntity object to a User object.
        /// </summary>
        /// <param name="entity">The UserEntity that has to be converted.</param>
        /// <returns>The user object.</returns>
        public static User UserEntityToUser(UserEntity entity)
        {
            User user = new User();
            user.Id = entity.Id;
            user.FirstName = entity.FirstName;
            user.Infix = entity.Infix;
            user.LastName = entity.LastName;
            user.Username = entity.Username;
            user.Password = entity.Password;
            user.Type = entity.Type;
            user.Sources = SourceEntityListToSources(entity.Sources);

            return user;
        }

        /// <summary>
        /// Converts a list of SourceEntities to a list of Sources.
        /// </summary>
        /// <param name="sourceEntities">The SourceEntities that have to be converted.</param>
        /// <returns>The list of sources.</returns>
        public static List<Source> SourceEntityListToSources(ICollection<SourceEntity> sourceEntities)
        {
            List<Source> sources = new List<Source>();
            foreach (SourceEntity source in sourceEntities)
            {
                Source temp = new Source();
                temp.Id = source.Id;
                temp.SourceDefinition = SourceDefinitionEntityToSourceDefinition(source.SourceDefinition);
                temp.Value = source.Value;
                sources.Add(temp);
            }
            return sources;
        }

        /// <summary>
        /// Converts a SourceDefinitionEntity to a SourceDefinition.
        /// </summary>
        /// <param name="sourceDefinitionEntity">The SourceDefinitionEntity that have to be converted.</param>
        /// <returns>The sourcedefinition object.</returns>
        public static SourceDefinition SourceDefinitionEntityToSourceDefinition(
            SourceDefinitionEntity sourceDefinitionEntity)
        {
            SourceDefinition sourceDefinition = new SourceDefinition();
            sourceDefinition.Id = sourceDefinitionEntity.Id;
            sourceDefinition.Name = sourceDefinitionEntity.Name;
            sourceDefinition.Description = sourceDefinitionEntity.Description;
            return sourceDefinition;
        }

        /// <summary>
        /// Converts a QuestionEntity to a Question.
        /// </summary>
        /// <param name="entity">The QuestionEntity that has to be converted.</param>
        /// <returns>The question onbject.</returns>
        public static Question QuestionEntityToQuestion(QuestionEntity entity)
        {
            Question question = new Question();
            question.Id = entity.Id;
            question.Answerer = UserEntityToUser(entity.Answerer);
            question.User = UserEntityToUser(entity.User);
            question.Content = entity.Content;
            question.QuestionState = entity.QuestionState;
            question.SourceType = SourceDefinitionEntityToSourceDefinition(entity.SourceType);
        }
    }
}
