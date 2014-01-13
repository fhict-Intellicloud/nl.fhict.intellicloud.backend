using System.Text.RegularExpressions;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Context;
using nl.fhict.IntelliCloud.Data.IntelliCloud.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using nl.fhict.IntelliCloud.Common.CustomException;
using nl.fhict.IntelliCloud.Data.WordStoreService;

namespace nl.fhict.IntelliCloud.Business.Manager
{
    /// <summary>
    /// A class providing functionality related to answers.
    /// </summary>
    public class AnswerManager : BaseManager
    {
        /// <summary>
        /// Constructor method for the answer manager class.
        /// </summary>
        public AnswerManager()
            : base()
        { }

        /// <summary>
        /// Constructor class for the answer manager.
        /// </summary>
        /// <param name="validation">An instance of <see cref="IValidation"/>.</param>
        public AnswerManager(IValidation validation)
            : base(validation)
        { }

        /// <summary>
        /// Retrieve the answer with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the answer.</param>
        /// <returns>Returns the answer with the given identifier.</returns>
        public Answer GetAnswer(string id)
        {
            Validation.IdCheck(id);

            Answer answer = new Answer();

            using (var ctx = new IntelliCloudContext())
            {
                int iId = Convert.ToInt32(id);

                AnswerEntity answerentity = ctx.Answers
                    .Include(a => a.LanguageDefinition)
                    .Include(a => a.User)
                    .Include(a => a.OriginalAnswer)
                    .Single(a => a.Id == iId);

                answer = answerentity.AsAnswer();
            }

            return answer;

        }

        /// <summary>
        /// Creates a new answer.
        /// </summary>
        /// <param name="questionId">The identifier of the question which is answered.</param>
        /// <param name="answer">The content of the given answer.</param>
        /// <param name="answererId">The employee who answered the question.</param>
        /// <param name="answerState">The state of the answer.</param>
        public void CreateAnswer(int questionId, string answer, int answererId, AnswerState answerState)
        {
            Validation.IdCheck(answererId);
            Validation.IdCheck(questionId);
            Validation.StringCheck(answer);

            using (var ctx = new IntelliCloudContext())
            {

                AnswerEntity answerEntity = new AnswerEntity();

                answerEntity.AnswerState = answerState;
                answerEntity.Content = answer;
                answerEntity.CreationTime = DateTime.UtcNow;
                answerEntity.LastChangedTime = DateTime.UtcNow;

                UserEntity user = ctx.Users
                    .Include(u => u.Sources)
                    .SingleOrDefault(ld => ld.Id == answererId);

                if (user == null)
                    throw new NotFoundException("No user entity exists with the specified ID.");

                answerEntity.User = user;

                var words = this.ResolveWords(answer);
                var language = this.GetLanguage(words);
                var keywords = this.RetrieveKeywords(words, language);

                var languageDefinitionEntity = ctx.LanguageDefinitions.AsEnumerable().Single(x => x.Name == this.ToLanguageString(language));

                if (languageDefinitionEntity == null)
                    throw new NotFoundException("No languageDefinition entity exists with the specified name.");

                answerEntity.LanguageDefinition = languageDefinitionEntity;

                ctx.Answers.Add(answerEntity);

                ctx.SaveChanges();

                QuestionEntity question = ctx.Questions
                    .Include(q => q.User)
                    .Include(q => q.User.Sources)
                    .Include(q => q.Source)
                    .Include(q => q.Source.Source)
                    .Include(q => q.Source.Source.SourceDefinition)
                    .Include(q => q.LanguageDefinition)
                    .Single(q => q.Id == questionId);

                question.Answer = answerEntity;
                question.Answerer = user;

                Guid token = Guid.NewGuid();
                question.FeedbackToken = token.ToString();

                answerEntity.IsPrivate = question.IsPrivate;
                
                var keywordEntities = this.GetOrCreateKeywordEntities(ctx, keywords);
                var answerKeys = keywordEntities.Select(keyword =>
                    new AnswerKeyEntity
                    {
                        Answer = answerEntity,
                        Keyword = keyword.Value,
                        CreationTime = DateTime.UtcNow,
                        Affinity = keyword.Key.Affinity,
                    });

                ctx.AnswerKeys.AddRange(answerKeys);
                ctx.SaveChanges();
                
                this.SendAnswerFactory.LoadPlugin(question.Source.Source.SourceDefinition).SendAnswer(question, answerEntity);
            }

        }

        /// <summary>
        /// Updates the answer with the given identifier.
        /// </summary>
        /// <param name="id">The identifier of the answer that is updated.</param>
        /// <param name="answerState">The new state of the answer.</param>
        /// <param name="answer">The new content of the given answer.</param>
        public void UpdateAnswer(string id, AnswerState answerState, string answer)
        {
            Validation.IdCheck(id);
            Validation.StringCheck(answer);

            using (var ctx = new IntelliCloudContext())
            {
                int iId = Convert.ToInt32(id);

                AnswerEntity answerEntity = ctx.Answers
                    .Include(a => a.LanguageDefinition)
                    .Include(a => a.User)
                    .SingleOrDefault(a => a.Id == iId);

                if (answerEntity == null)
                    throw new NotFoundException("No answer entity exists with the specified ID.");

                answerEntity.AnswerState = answerState;
                answerEntity.Content = answer;
                answerEntity.LastChangedTime = DateTime.UtcNow;

                ctx.SaveChanges();

            }
        }

        public IList<Answer> GetAnswers(AnswerState state, string search)
        {
            Validation.StringCheck(search);

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                // resolve the words
                List<Word> resolvedWords = new List<Word>();
                resolvedWords.AddRange(this.ResolveWords(search));

                // determine language
                Language lan = this.GetLanguage(resolvedWords);

                // find keywords
                List<Entities.Keyword> keywords = new List<Entities.Keyword>();
                keywords.AddRange(this.RetrieveKeywords(resolvedWords, lan));

                List<string> keywordStringval = new List<string>();
                foreach (Entities.Keyword w in keywords)
                {
                    keywordStringval.Add(w.Word.Value);
                }

                // find answer with these keywords
                List<AnswerEntity> answerEntities = (ctx.AnswerKeys
                                                     .Where(ak => keywordStringval.Contains(ak.Keyword.Name))
                                                     .Select(ak => ak.Answer))
                                                     .Include(a => a.User)
                                                     .Include(a => a.LanguageDefinition)
                                                     .Include(a => a.OriginalAnswer)
                                                     .Take(5)
                                                     .ToList();

                return answerEntities.AsAnswers();
            }

        }

        public User GetAnswerer(string id)
        {
            Validation.IdCheck(id);

            User user = new User();

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                int iId = Convert.ToInt32(id);

                AnswerEntity answerEntity = ctx.Answers
                    .Include(a => a.User)
                    .Include(a => a.User.Sources)
                    .SingleOrDefault(a => a.Id == iId);

                if (answerEntity == null)
                    throw new NotFoundException("No answer found for the given Id");

                user = answerEntity.User.AsUser();
            }

            return user;
        }

        public IList<Feedback> GetFeedbacks(string id, FeedbackState? state)
        {
            Validation.IdCheck(id);

            List<Feedback> feedbacks = new List<Feedback>();

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                int iId = Convert.ToInt32(id);
                
                var query = ctx.Feedbacks.Where(f => f.Answer.Id == iId);

                if (state != null)
                {
                    query = query.Where(f => f.FeedbackState == state);
                }

                List<FeedbackEntity> feedbackEntities = query.ToList();

                feedbacks.AddRange(feedbackEntities.AsFeedbacks());
            }

            return feedbacks;
        }

        public IList<Review> GetReviews(string id, ReviewState? state)
        {
            Validation.IdCheck(id);

            List<Review> reviews = new List<Review>();

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                int iId = Convert.ToInt32(id);

                var query = ctx.Reviews.Where(f => f.Answer.Id == iId);

                if (state != null)
                {
                    query = query.Where(f => f.ReviewState == state);
                }

                List<ReviewEntity> reviewEntities = query.ToList();

                reviews.AddRange(reviewEntities.AsReviews());
            }

            return reviews;
        }

        public IList<Keyword> GetKeywords(string id)
        {
            Validation.IdCheck(id);

            List<Keyword> keywords = new List<Keyword>();

            using (IntelliCloudContext ctx = new IntelliCloudContext())
            {
                int iId = Convert.ToInt32(id);

                List<KeywordEntity> keywordEntities = (from k in ctx.Keywords
                                                      join ak in ctx.AnswerKeys
                                                      on k.Id equals ak.Keyword.Id
                                                      select k).ToList();

                keywords.AddRange(keywordEntities.AsKeywords());
            }

            return keywords;
        }


        #region keyword algorith methods

        /// <summary>
        /// A function in which all punctuation is removed and all text is converted to lowercase.
        /// Words with punctuation within them are ignored (eg. andré, hbo'er).
        /// </summary>
        /// <param name="question">A string representing a question that is to be decomposed.</param>
        /// <returns> A list with each word in the sentence is returned. </returns>
        internal IList<string> ConvertAnswer(String answer)
        {
            //Regex that matches every single special character exluding the -
            Regex regex = new Regex("[\\\\+=`~!@#$%^&*()_\\\\\\\\[\\]{}:\\\";\\?<>/.,|]");

            string cleanQuestion = regex.Replace(answer.ToLowerInvariant(), "");

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
        internal IList<Word> ResolveWords(String answer)
        {
            using (WordStoreContext context = new WordStoreContext())
            {
                return ConvertAnswer(answer)
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

        #endregion
    }
}
