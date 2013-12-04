using nl.fhict.IntelliCloud.Common.CustomException;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    public class QuestionManager : BaseManager
    {
        public IList<Question> GetQuestions(int employeeId)
        {
            Validation.IdCheck(employeeId);

            List<Question> questions = new List<Question>();

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                UserEntity employee = (from u in ctx.Users
                                where u.Id == employeeId
                                select u).SingleOrDefault();

                // TODO: Only retrieve questions for retrieved employee.
                // TODO: Make sure only users of type employee can retrieve private questions.
                // TODO fix includes
                List<QuestionEntity> questionEntities = (from q in ctx.Questions
                                                                 .Include(d => d.Source)
                                                                 .Include(d => d.User)
                                                                 .Include(d => d.User.Sources)
                                                                 .Include(d => d.Answerer)
                                                                 .Include(d => d.Answerer.Sources)
                                                             select q).ToList();

                questions = ConvertEntities.QuestionEntityListToQuestionList(questionEntities);
            }

            return questions;
        }

        public Question GetQuestion(int id)
        {
            Validation.IdCheck(id);

            Question question = new Question();

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                // TODO: make sure only users of type employee can retrieve private questions.
                QuestionEntity entity = (from q in ctx.Questions
                                         where q.Id == id
                                         select q).SingleOrDefault();

                if (entity == null)
                    throw new NotFoundException("No Question entity exists with the specified ID.");

                question = ConvertEntities.QuestionEntityToQuestion(entity);
            }
            return question;
        }

        public void CreateQuestion(
            string source, string reference, string question, string title, string postId = null, bool isPrivate = false)
        {
            Validation.StringCheck(source);
            Validation.StringCheck(reference);
            Validation.StringCheck(question);
            Validation.StringCheck(title);

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {                
                // TODO determine real language 
                LanguageDefinitionEntity languageDefinition = ctx.LanguageDefinitions.SingleOrDefault(ld => ld.Name.Equals("English"));

                // TODO remove exception as you probably want to create the language if it doesn't exist.
                if (languageDefinition == null)
                    throw new NotFoundException("No languageDefinition entity exists with the specified ID.");                

                SourceDefinitionEntity sourceDefinition = ctx.SourceDefinitions.SingleOrDefault(sd => sd.Name.Equals(source));

                if (sourceDefinition == null)
                    throw new NotFoundException("The provided source doesn't exists.");
                
                // Check if the user already exists
                SourceEntity sourceEntity = ctx.Sources.SingleOrDefault(s => s.SourceDefinition.Id == sourceDefinition.Id && s.Value == reference);

                UserEntity userEntity;

                if (sourceEntity != null)
                {
                    // user already has an account, use this
                    userEntity = ctx.Users.Single(u => u.Id == sourceEntity.UserId);                    
                }
                else
                {
                    // user has no account, create one
                    userEntity = new UserEntity()
                    {
                        CreationTime = DateTime.UtcNow,
                        Type = UserType.Customer
                    };

                    ctx.Users.Add(userEntity);  

                    // Mount the source to the new user
                    sourceEntity = new SourceEntity()
                    {
                        Value = reference,
                        CreationTime = DateTime.UtcNow,
                        SourceDefinition = sourceDefinition,
                        User = userEntity,
                    };

                    ctx.Sources.Add(sourceEntity);
                }                

                QuestionEntity questionEntity = new QuestionEntity()
                {
                    Content = question,
                    CreationTime = DateTime.UtcNow,
                    IsPrivate = isPrivate,
                    QuestionState = QuestionState.Open,
                    Title = title,
                    Source = new QuestionSourceEntity()
                    {
                        Source = sourceEntity,
                        PostId = postId
                    },
                    LanguageDefinition = languageDefinition,
                    User = userEntity
                };

                ctx.Questions.Add(questionEntity);

                ctx.SaveChanges();
            }
        }

        public void UpdateQuestion(int id, int employeeId)
        {
            Validation.IdCheck(id);
            Validation.IdCheck(employeeId);

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                QuestionEntity questionEntity = (from q in ctx.Questions
                                                 where q.Id == id
                                                 select q).Single();

                questionEntity.Answerer = (from u in ctx.Users
                                           where u.Id == employeeId
                                           select u).Single();
                ctx.SaveChanges();
            }

        }
    }
}
