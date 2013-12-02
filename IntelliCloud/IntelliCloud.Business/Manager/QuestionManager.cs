using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    public class QuestionManager
    {
        public IList<Question> GetQuestions(int employeeId)
        {
            Validation.IdCheck(employeeId);

            List<Question> questions = new List<Question>();

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                List<QuestionEntity> questionEntities = (from q in ctx.Questions
                                                         select q).ToList();

                questions = ConvertEntities.QuestionEntityListToQuestion(questionEntities);
            }

            return questions;
        }

        public Question GetQuestion(int id)
        {
            Validation.IdCheck(id);

            Question question = new Question();

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                QuestionEntity entity = (from q in ctx.Questions
                                         where q.Id == id
                                         select q).Single();

                question = ConvertEntities.QuestionEntityToQuestion(entity);
            }
            return question;
        }

        public void CreateQuestion(string source, string reference, string question, string title)
        {
            Validation.StringCheck(source);
            Validation.StringCheck(reference);
            Validation.StringCheck(question);
            Validation.StringCheck(title);

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                QuestionEntity questionEntity = new QuestionEntity();

                questionEntity.Content = question;
                questionEntity.CreationTime = DateTime.UtcNow;
                questionEntity.IsPrivate = false;
                questionEntity.QuestionState = QuestionState.Open;
                questionEntity.Title = title;
                questionEntity.SourceDefinition = ctx.SourceDefinitions.Single(sd => sd.Name.Equals(source));
                
                // Check if the user already exists
                var sourceEntity = ctx.Sources.SingleOrDefault(s => s.SourceDefinition.Id == questionEntity.SourceDefinition.Id && s.Value == reference); 

                if (sourceEntity != null)
                {
                    // user already has an account, use this
                    var userEntity = (from u in ctx.Users
                            where u.Id == sourceEntity.UserId
                            select u).Single();

                    questionEntity.User = userEntity;
                }
                else
                {
                    // user has no account, create one
                    UserEntity newUserEntity = new UserEntity();

                    newUserEntity.CreationTime = DateTime.UtcNow;
                    newUserEntity.Type = UserType.Customer;

                    ctx.Users.Add(newUserEntity);

                    ctx.SaveChanges();

                    questionEntity.User = newUserEntity;   

                    // Mount the source to the new user
                    SourceEntity newSourceEntity = new SourceEntity();
                    newSourceEntity.Value = reference;
                    newSourceEntity.CreationTime = DateTime.UtcNow;
                    newSourceEntity.SourceDefinition = questionEntity.SourceDefinition;
                    newSourceEntity.UserId = newUserEntity.Id;

                    ctx.Sources.Add(newSourceEntity);

                }

                ctx.Questions.Add(questionEntity);

                ctx.SaveChanges();
            }
        }

        public void UpdateQuestion(string id, int employeeId)
        {
            throw new NotImplementedException();
        }
    }
}
