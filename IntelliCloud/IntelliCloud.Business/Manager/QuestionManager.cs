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
using DialogueMaster.Babel;
using System.Globalization;

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


                List<QuestionEntity> questionEntities = (from q in ctx.Questions
                                                                 .Include(q => q.Source)
                                                                 .Include(q => q.User)
                                                                 .Include(q => q.User.Sources)
                                                                 .Include(q => q.Answerer)
                                                                 .Include(q => q.Answerer.Sources)
                                                                 .Include(q => q.User.Sources.Select(s => s.SourceDefinition))
                                                         where q.Answer == null || q.Answer.AnswerState != AnswerState.Ready

                                                         select q).ToList();
                if (employeeId != 0)
                {
                    if (employee != null)
                    {
                        List<QuestionEntity> employeeQuestions = new List<QuestionEntity>();
                        List<QuestionEntity> employeeNotFoundQuestions = new List<QuestionEntity>();
                        List<UserKeyEntity> employeeKeys = (from x in ctx.UserKeys where x.User.Id == employee.Id select x)
                            .Include(q => q.User)
                            .Include(q => q.Keyword)
                            .Include(q => q.Affinity).ToList();
                        List<QuestionKeyEntity> questionKeyEntities = (from q in ctx.QuestionKeys select q)
                            .Include(q => q.Question)
                            .Include(q => q.Keyword)
                            .Include(q => q.Affinity).ToList();
                        //Check if employee keys contains questions keys.
                        foreach (QuestionEntity question in questionEntities)
                        {
                            bool foundUserQuestion = false;
                            List<QuestionKeyEntity> questionKeys = (from q in questionKeyEntities where q.Question.Id == question.Id select q).ToList();
                            foreach (QuestionKeyEntity qke in questionKeys)
                            {
                                if (!foundUserQuestion)
                                {
                                    foreach (UserKeyEntity uke in employeeKeys)
                                    {
                                        if (qke.Keyword.Id == uke.Keyword.Id && !foundUserQuestion)
                                        {
                                            employeeQuestions.Add(question);
                                            foundUserQuestion = true;
                                        }
                                    }
                                }
                            }
                            if (!foundUserQuestion)
                            {
                                employeeNotFoundQuestions.Add(question);
                            }
                        }

                        //If there are remaining questions that are open and not found for this employee
                        //check if other employees are experts in this question. If not add them also to this employee
                        if (employeeNotFoundQuestions.Count != 0)
                        {
                            List<UserKeyEntity> otherUserKeys = (from u in ctx.UserKeys where u.User.Id != employeeId select u).ToList();
                            foreach (QuestionEntity question in employeeNotFoundQuestions)
                            {
                                bool foundUserQuestion = false;
                                List<QuestionKeyEntity> questionKeys = (from q in questionKeyEntities where q.Question.Id == question.Id select q).ToList();
                                foreach (QuestionKeyEntity qke in questionKeys)
                                {
                                    if (!foundUserQuestion)
                                    {
                                        foreach (UserKeyEntity uke in otherUserKeys)
                                        {
                                            if (qke.Keyword.Id == uke.Keyword.Id && !foundUserQuestion)
                                            {
                                                foundUserQuestion = true;
                                            }
                                        }
                                    }
                                }
                                if (!foundUserQuestion)
                                {
                                    employeeQuestions.Add(question);
                                }
                            }
                        }
                        questions = ConvertEntities.QuestionEntityListToQuestionList(employeeQuestions);
                    }
                    else
                    {
                        throw new NotFoundException("No User entity exists with the specified ID.");
                    }
                }
                else
                {
                    questions = ConvertEntities.QuestionEntityListToQuestionList(questionEntities);
                }
                
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
                String languageName = "English";
                IBabelModel m_Model = BabelModel._AllModel;
                DialogueMaster.Classification.ICategoryList result = m_Model.ClassifyText(question);
                if (result.Count > 0)
                {
                    CultureInfo info = CultureInfo.GetCultureInfoByIetfLanguageTag(result[0].Name);
                    languageName = info.EnglishName;
                }
                LanguageDefinitionEntity languageDefinition = ctx.LanguageDefinitions.SingleOrDefault(ld => ld.Name.Equals(languageName));

                // create the language if it doesn't exist.
                if (languageDefinition == null)
                {
                    languageDefinition = new LanguageDefinitionEntity();
                    languageDefinition.Name = languageName;
                    ctx.LanguageDefinitions.Add(languageDefinition);
                    ctx.SaveChanges();
                }

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
            //Regex that matches every single special character exluding the -
            Regex regex = new Regex("[\\\\+=`~!@#$%^&*()_\\\\\\\\[\\]{}:\\\";\\?<>/.,|]");

            string cleanQuestion = regex.Replace(question.ToLowerInvariant(), "");

            return cleanQuestion.Split(' ')
                .Where(x => x != "-" && x != "'")
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
