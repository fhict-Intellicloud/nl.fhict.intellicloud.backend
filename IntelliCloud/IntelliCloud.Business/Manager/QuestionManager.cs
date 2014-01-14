using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using nl.fhict.IntelliCloud.Common.CustomException;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Context;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;
using nl.fhict.IntelliCloud.Data.WordStoreService;
using System.Collections;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    /// <summary>
    /// A class providing functionality related to questions.
    /// </summary>
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
        /// <param name="validation">An instance of <see cref="IValidation"/>.</param>
        public QuestionManager(IValidation validation)
            : base(validation)
        { }

        /// <summary>
        /// Retrieve the question with the given identifier.
        /// </summary>
        /// <param name="questionId">The identifier of the question.</param>
        /// <returns>Returns the question with the given identifier.</returns>
        public Question GetQuestion(string id)
        {
            Validation.IdCheck(id);

            int convertedId = Convert.ToInt32(id);

            Question question = new Question();

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                QuestionEntity entity = (from q in ctx.Questions
                                                                 .Include(q => q.Source)
                                                                 .Include(q => q.User)
                                                                 .Include(q => q.User.Sources)
                                                                 .Include(q => q.Answerer)
                                                                 .Include(q => q.Answerer.Sources)
                                                                 .Include(q => q.LanguageDefinition)
                                                                 .Include(q => q.User.Sources.Select(s => s.SourceDefinition))
                                         where q.Id == convertedId
                                         select q).SingleOrDefault();

                if (entity == null)
                    throw new NotFoundException("No Question entity exists with the specified ID.");

                question = entity.AsQuestion();
            }
            return question;
        }

        /// <summary>
        /// Retrieves the available questions and filtering them using the state.
        /// </summary>
        /// <param name="state">The optional state of the question, only questions with the given state are returned.</param>
        /// <returns>Returns the questions that match the filter.</returns>
        public IList<Question> GetQuestions(QuestionState? state)
        {
            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                var questionEntities = ctx.Questions
                                        .Include(q => q.Source)
                                        .Include(q => q.User)
                                        .Include(q => q.User.Sources)
                                        .Include(q => q.Answerer)
                                        .Include(q => q.Answerer.Sources)
                                        .Include(q => q.LanguageDefinition)
                                        .Include(q => q.User.Sources.Select(s => s.SourceDefinition));

                if (state == null)
                    return questionEntities
                        .ToList()
                        .AsQuestions();
                else
                {
                    return questionEntities
                        .Where(q => q.QuestionState == state)
                        .ToList()
                        .AsQuestions();

                }
            }
        }

        /// <summary>
        /// Creates a new question.
        /// </summary>
        /// <param name="source">The source from which the question was send, e.g. "Mail", "Facebook" or "Twitter".
        /// </param>
        /// <param name="reference">The identifier for the source, e.g. the username or email address.</param>
        /// <param name="question">The question that was answered.</param>
        /// <param name="title">The title of the question. The title contains a short summary of the question.</param>
        /// <param name="postId">The identifier of the post this question originates from, for example the Facebook post
        /// identifier.</param>
        /// <param name="isPrivate">When <c>true</c> the question is private, otherwise the question is public. Private 
        /// questions are only available to employees and will never be exposed to customers.</param>
        public void CreateQuestion(
            string source, string reference, string question, string title, string postId = null, bool isPrivate = false)
        {
            Validation.StringCheck(source);
            Validation.StringCheck(reference);
            Validation.StringCheck(question);
            Validation.StringCheck(title);
            Validation.SourceDefinitionExistsCheck(source);

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                var words = this.ResolveWords(question);
                var language = this.GetLanguage(words);
                var keywords = this.RetrieveKeywords(words, language);

                var languageDefinitionEntity = ctx.LanguageDefinitions.AsEnumerable().Single(x => x.Name == this.ToLanguageString(language));
                var sourceEntity = this.GetOrCreateSourceEntity(ctx, source, reference);

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
                    LanguageDefinition = languageDefinitionEntity,
                    User = sourceEntity.User,
                };
                ctx.Questions.Add(questionEntity);

                var keywordEntities = this.GetOrCreateKeywordEntities(ctx, keywords);
                var questionKeys = keywordEntities.Select(keyword =>
                    new QuestionKeyEntity
                    {
                        Question = questionEntity,
                        Keyword = keyword.Value,
                        CreationTime = DateTime.UtcNow,
                        Affinity = keyword.Key.Affinity,
                    });

                ctx.QuestionKeys.AddRange(questionKeys);
                ctx.SaveChanges();

                questionEntity.Answer = this.GetMatch(ctx, questionEntity);

                /*this.SendAnswerFactory.LoadPlugin(questionEntity.Source.Source.SourceDefinition)
                    .SendQuestionReceived(questionEntity);

                if (questionEntity.Answer != null)
                    this.SendAnswerFactory.LoadPlugin(questionEntity.Source.Source.SourceDefinition)
                        .SendAnswer(questionEntity, questionEntity.Answer);*/
            }
        }

        private IDictionary<Entities.Keyword, KeywordEntity> GetOrCreateKeywordEntities(IntelliCloudContext context, IList<Entities.Keyword> keywords)
        {
            IDictionary<Entities.Keyword, KeywordEntity> entities = new Dictionary<Entities.Keyword, KeywordEntity>();
            keywords.ToList().ForEach(
                (keyword) =>
                {

                    var entity = context.Keywords
                        .SingleOrDefault(x => x.Name == keyword.Word.Value);

                    entities.Add(keyword, entity != null
                        ? entity
                        : new KeywordEntity
                        {
                            CreationTime = DateTime.UtcNow,
                            Name = keyword.Word.Value,
                        });
                });

            context.SaveChanges();

            return entities;
        }

        private AnswerEntity GetMatch(IntelliCloudContext context, QuestionEntity questionEntity)
        {
            var questionKeywords = context.QuestionKeys.Where(x => x.Question.Id == questionEntity.Id);

            var importantKeywords = questionKeywords.Where(x => x.Affinity > 10).Select(x => x.Keyword.Id);
            var relevantAnswerKeys = context.AnswerKeys.Include(x => x.Answer).Where(x => importantKeywords.Contains(x.Keyword.Id));
            var relevantAnswers = relevantAnswerKeys.Select(x => x.Answer).Distinct().ToList();

            IDictionary<AnswerEntity, int> ratedAnswers = new Dictionary<AnswerEntity, int>();
            foreach (var answer in relevantAnswers)
            {
                var answerKeywords = context.AnswerKeys.Where(x => x.Answer.Id == answer.Id);
                var matchingKeywords = answerKeywords.Where(x => questionKeywords.Any(y => y.Keyword.Id == x.Keyword.Id));

                double maximumScore = answerKeywords
                    .Select(x => x.Affinity)
                    .AsEnumerable()
                    .Aggregate((previous, next) => previous + next);

                double score = matchingKeywords.Any()
                    ? matchingKeywords
                        .AsEnumerable()
                        .Select(x => questionKeywords.Single(y => x.Id == y.Id).Affinity/x.Affinity)
                        .Aggregate((previous, next) => previous + next)
                    : 0;

                ratedAnswers.Add(answer, (int)(score/maximumScore * 100));
            }

            if (ratedAnswers.Any())
            {
                var bestMatch = ratedAnswers.OrderByDescending(x => x.Value).First();
                return bestMatch.Value >= 50 ? bestMatch.Key : null;
            }
            else
            {
                return null;
            }
        }

        private SourceEntity GetOrCreateSourceEntity(IntelliCloudContext context, string source, string reference)
        {
            SourceDefinitionEntity sourceDefinitionEntity = context.SourceDefinitions
                .Single(x => x.Name == source);
            SourceEntity sourceEntity = context.Sources
                .Include(x => x.User)
                .SingleOrDefault(x => x.SourceDefinition.Id == sourceDefinitionEntity.Id && x.Value == reference);

            if (sourceEntity != null)
                return sourceEntity;
            else
            {
                var userEntity = new UserEntity
                {
                    CreationTime = DateTime.UtcNow,
                    Type = UserType.Customer
                };

                sourceEntity = new SourceEntity
                {
                    Value = reference,
                    CreationTime = DateTime.UtcNow,
                    SourceDefinition = sourceDefinitionEntity,
                    User = userEntity,
                };

                context.Sources.Add(sourceEntity);
                context.SaveChanges();

                return sourceEntity;
            }
        }

        /// <summary>
        /// Retrieve the question for this feedback token.
        /// </summary>
        /// <param name="feedbackToken">The feedback token of the question.</param>
        /// <returns>Returns the question with the given feedbacktoken.</returns>
        public Question GetQuestionByFeedbackToken(string feedbackToken)
        {
            Validation.StringCheck(feedbackToken);

            Question question = new Question();
            UserManager userManager = new UserManager(this);

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                QuestionEntity entity = (from q in ctx.Questions
                                                                 .Include(q => q.Source)
                                                                 .Include(q => q.User)
                                                                 .Include(q => q.User.Sources)
                                                                 .Include(q => q.Answerer)
                                                                 .Include(q => q.Answerer.Sources)
                                                                 .Include(q => q.LanguageDefinition)
                                                                 .Include(q => q.Source.Source.SourceDefinition)
                                         where q.FeedbackToken == feedbackToken
                                         select q).SingleOrDefault();

                if (entity == null)
                    throw new NotFoundException("No Question entity exists with the specified feedback token.");

                if (entity.IsPrivate && userManager.GetUser().Type != UserType.Employee)
                    throw new NotFoundException("Only employees can retrieve private questions.");

                question = entity.AsQuestion();
            }
            return question;
        }

        /// <summary>
        /// Updates the question with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the question that is updated.</param>
        /// <param name="employeeId">The identifier of the employee that is going to answer the question.</param>
        public void UpdateQuestion(string id, int employeeId)
        {
            // Validate input data
            Validation.IdCheck(id);
            Validation.IdCheck(employeeId);

            // Convert the textual representation of the id to an integer
            int convertedId = Convert.ToInt32(id);

            using (IntelliCloudContext context = new IntelliCloudContext())
            {
                // Get the feedback entity from the context
                QuestionEntity question = context.Questions
                                          .Include(q => q.Source)
                                          .Include(q => q.User)
                                          .Include(q => q.User.Sources)
                                          .Include(q => q.Answerer)
                                          .Include(q => q.Answerer.Sources)
                                          .Include(q => q.LanguageDefinition)
                                          .Include(q => q.User.Sources.Select(s => s.SourceDefinition))
                                          .SingleOrDefault(q => q.Id == convertedId);

                if (question == null)
                    throw new NotFoundException("No question entity exists with the specified ID.");

                UserEntity user = context.Users
                                         .Include(u => u.Sources)
                                         .SingleOrDefault(u => u.Id == employeeId);

                if (user == null)
                    throw new NotFoundException("No user entity exists with the specified ID.");

                // Update the state of the feedback entry
                question.Answerer = user;
                question.LastChangedTime = DateTime.UtcNow;

                // Save the changes to the context
                context.SaveChanges();
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
        internal IList<Word> ResolveWords(String question)
        {
            using (WordStoreContext context = new WordStoreContext())
            {
                return ConvertQuestion(question)
                    .SelectMany(x => context.ResolveWord(x))
                    .ToList();
            }
        }

        /// <summary>
        /// Function that finds the most likely keywords from a given question. This is done by returning all Nouns, Pronouns and Verbs. 
        /// </summary>
        /// <param name="question">A question from which one needs the keywords.</param>
        /// <param name="language">The language one needs the found keywords of. </param>
        /// <returns>Returns a List containing the most likely keywords from a given question.</returns>
        internal IList<Entities.Keyword> RetrieveKeywords(IList<Word> words, Language language)
        {
            return words
                .Where(x => x.Language == language || x.Language == Language.Unknown && x.Type != WordType.Article)
                .GroupBy(x => x.Value)
                .Select(x => 
                    new Entities.Keyword(
                        word: x.First(),
                        affinity: 
                            ((x.First().Type == WordType.Noun || x.First().Type == WordType.Verb || x.First().Type == WordType.Pronoun)) 
                            ? x.Count() * 10 : 
                            x.Count()
                    )).ToList();
        }

        /// <summary>
        /// Function to determine the langauge of set of words. 
        /// </summary>
        /// <param name="words">Set of words from which one needs to determine the language.</param>
        /// <returns>Returns the language that is the most common within the given set.</returns>
        internal Language GetLanguage(IList<Word> words)
        {
            var distinctLanguages = words
                .GroupBy(x => x.Language)
                .Select(x => new { Language = x.Key, Count = x.Distinct().Count() });

            return distinctLanguages.Single(x => x.Count == distinctLanguages.Max(y => y.Count)).Language;
        }
        #endregion

        /// <summary>
        /// Retrieve the user that asked the question with the given identifier.
        /// </summary>
        /// <param name="questionId">The identifier of the question.</param>
        /// <returns>Returns the user that asked the question with the given identifier.</returns>
        public User GetAsker(string id)
        {
            Validation.IdCheck(id);

            int convertedId = Convert.ToInt32(id);

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                return ctx.Questions
                    .Include(q => q.User)
                    .Include(q => q.User.Sources)
                    .Include(q => q.User.Type)
                    .Where(q => q.Id == convertedId)
                    .Select(q => q.User)
                    .Single()
                    .AsUser();
            }
        }

        /// <summary>
        /// Retrieve the user that has answered the question with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the question.</param>
        /// <returns>Returns the user that answered the question with the given identifier.</returns>
        public User GetAnswerer(string id)
        {
            Validation.IdCheck(id);

            int convertedId = Convert.ToInt32(id);
            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                return ctx.Questions
                    .Include(q => q.Answerer)
                    .Include(q => q.Answerer.Sources)
                    .Include(q => q.Answerer.Type)
                    .Where(q => q.Id == convertedId)
                    .Select(q => q.Answerer)
                    .Single()
                    .AsUser();
            }
        }

        /// <summary>
        /// Retrieve the answer that answered the question with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the question.</param>
        /// <returns>Returns the answer that answered the question with the given identifier.</returns>
        public Answer GetAnswer(string id)
        {
            Validation.IdCheck(id);

            int convertedId = Convert.ToInt32(id);

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                return ctx.Questions
                    .Include(q => q.Answer)
                    .Include(q => q.Answer.User)
                    .Where(q => q.Id == convertedId)
                    .Select(q => q.Answer)
                    .Include(q => q.LanguageDefinition)
                    .Single()
                    .AsAnswer();
            }
        }

        /// <summary>
        /// Retrieve the keywords that are linked to the question with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the question.</param>
        /// <returns>Returns the keywords that are linked to the question with the given identifier.</returns>
        public IList<Keyword> GetKeywords(string id)
        {
            Validation.IdCheck(id);

            int convertedId = Convert.ToInt32(id);

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                return ctx.QuestionKeys
                    .Include(x => x.Keyword)
                    .Where(x => x.Question.Id == convertedId)
                    .Select(x => x.Keyword)
                    .ToList()
                    .AsKeywords("QuestionService.svc");
            }
        }

        private string ToLanguageString(Language language)
        {
            switch (language)
            {
                case Language.Dutch:
                    return "Dutch";
                case Language.English:
                    return "English";
                case Language.Unknown:
                    return "Unknown";
                default:
                    throw new ArgumentException("Unknown language");
            }
        }
    }
}
