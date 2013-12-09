using nl.fhict.IntelliCloud.Common.CustomException;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Context;
using nl.fhict.IntelliCloud.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text.RegularExpressions;
using nl.fhict.IntelliCloud.Business.WordService;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    public class QuestionManager : BaseManager
    {

        /// <summary>
        /// Constructor method for the question manager class.
        /// </summary>
        public QuestionManager()
            : base()
        {
        }


        /// <summary>
        /// Constructor class for the question manager.
        /// </summary>
        /// <param name="validation">An in stance of <see cref="IValidation"/>.</param>
        public QuestionManager(IValidation validation)
            : base(validation)
        {
        }

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
                List<QuestionEntity> questionEntities = (from q in ctx.Questions
                                                                 .Include(q => q.Source)
                                                                 .Include(q => q.User)
                                                                 .Include(q => q.User.Sources)
                                                                 .Include(q => q.Answerer)
                                                                 .Include(q => q.Answerer.Sources)
                                                                 .Include(q => q.User.Sources.Select(s => s.SourceDefinition))
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
                                                                 .Include(q => q.Source)
                                                                 .Include(q => q.User)
                                                                 .Include(q => q.User.Sources)
                                                                 .Include(q => q.Answerer)
                                                                 .Include(q => q.Answerer.Sources)
                                                                 .Include(q => q.User.Sources.Select(s => s.SourceDefinition))
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

        #region keyword algorith methods

        /// <summary>
        /// A function in which all punctuation is removed and all text is converted to lowercase.
        /// Words with punctuation within them are ignored (eg. andré, hbo'er).
        /// </summary>
        /// <param name="question">A string representing a question that is to be decomposed.</param>
        /// <returns> A list with each word in the sentence is returned. </returns>
        internal IList<string> ConvertQuestion(String question)
        {
            Regex regex = new Regex("[\\w-']+");




            return regex
                .Matches(question.ToLowerInvariant())
                .Cast<Match>()
                .Where(x => x.Success)
                .Select(x => x.Value)
                .SelectMany(x => x.Split('_'))
                .ToList();
        }

        /// <summary>
        /// Function that evaluates all words in the given question to its wordtypes.
        /// All verbs are converted to their full version. (eg worked --> to work, werkte --> werken)
        /// </summary>
        /// <param name="question">A given question for which all words are to be resolved to their type.</param>
        /// <returns>A list containing the resolved words that were contained in the question</returns>
       public IList<Word> ResolveWords(String question)
        {
            IWordService service = new WordServiceClient();
            return ConvertQuestion(question)
                .SelectMany(x => service.ResolveWord(x))
                .ToList();
        }

        /// <summary>
        /// Function that finds the most likely keywords from a given question. This is done by returning all Nouns, Pronouns and Verbs. 
        /// </summary>
        /// <param name="question">A question from which one needs the keywords.</param>
        /// <param name="language">The language one needs the found keywords of. </param>
        /// <returns>Returns a List containing the most likely keywords from a given question.</returns>
        public IList<Word> FindMostLikelyKeywords(IList<Word> words, Language language)
        {
            return words
                .Where(x =>
                    (x.Type == WordType.Noun || x.Type == WordType.Verb || x.Type == WordType.Pronoun)
                    && x.Language == language)
                .ToList();
        }

        /// <summary>
        /// Function to determine the langauge of set of words. 
        /// </summary>
        /// <param name="words">Set of words from which one needs to determine the language.</param>
        /// <returns>Returns the language that is the most common within the given set.</returns>
        public Language GetLanguage(IList<Word> words)
        {
            var distinctLanguages = words
                .GroupBy(x => x.Language)
                .Select(x => new { Language = x.Key, Count = x.Distinct().Count() });

            return distinctLanguages.Single(x => x.Count == distinctLanguages.Max(y => y.Count)).Language;
        }


        #endregion
    }
}
