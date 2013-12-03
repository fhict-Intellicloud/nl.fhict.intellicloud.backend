using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using nl.fhict.IntelliCloud.Common.DataTransfer;
using nl.fhict.IntelliCloud.Data.Context;

namespace nl.fhict.IntelliCloud.Business
{
    public class WordStoreHelper
    {
        /// <summary>
        /// A function in which all punctuation is removed and all text is converted to lowercase.
        /// Words with punctuation within them are ignored (eg. andré, hbo'er).
        /// </summary>
        /// <param name="question"></param>
        /// <returns> The given question is returned with all its punctuation removed and converted to lowercase. </returns>
        internal static IList<string> ConvertQuestion(String question)
        {
            return question
                .Split(' ')
                .Select(x => Regex.Replace(x, @"[^a-zA-Z0-9 \ré è û]", string.Empty))
                .Where(x => x != string.Empty).ToList();
        }

        /// <summary>
        /// Function that evaluates all words in the given question to its wordtype.
        /// All verbs are converted to their full version. (eg worked --> to work, werkte --> werken)
        /// </summary>
        /// <param name="question">A given question for which all words are to be resolved to their meaning.</param>
        /// <returns>A list containing the resolved words that are contained in the question</returns>
        internal static IList<Word> ResolveWords(String question)
        {
            WordService service = new WordService();

            return ConvertQuestion(question)
                .SelectMany(x => service.ResolveWord(x))
                .ToList();
        }

        /// <summary>
        /// Function that finds the most likely keywords from a given question. This is done by returning all Nouns and Verb. 
        /// </summary>
        /// <param name="question">A question from which one needs the keywords.</param>
        /// <param name="language">A language for which one wants to select </param>
        /// <returns>Returns a List containing the most likely keywords from a given question.</returns>
        public static IList<Word> FindMostLikelyKeywords(String question, Language language)
        {
            var words = ResolveWords(question)
                .Where(x =>
                    (x.Type == WordType.Noun || x.Type == WordType.Verb)
                    && x.Language == language)
                .ToList();

          //  language = GetLanguage(words);

            return words;
        }

        internal static Language GetLanguage(IList<Word> words)
        {
           // var a = words.Select((x, y) => new { x = x., y = y });
            return Language.Dutch;
        }
    }

    #region wordservice stub
    public class WordService
    {
        public IEnumerable<Word> ResolveWord(string word)
        {

            Dictionary<string, IEnumerable<Word>> words = new Dictionary<string, IEnumerable<Word>>();
            words.Add("juiste", new List<Word>() { new Word("juiste", WordType.Adjective, Language.Dutch) });
            words.Add("tekst", new List<Word>() { new Word("tekst", WordType.Noun, Language.Dutch) });
            words.Add("over", new List<Word>() { new Word("over", WordType.Adverb, Language.Dutch) });
            words.Add("moet", new List<Word>() { new Word("moet", WordType.Noun, Language.Dutch) });
            words.Add("blijven", new List<Word>() { new Word("blijven", WordType.Verb, Language.Dutch) });

            words.Add("formatteer", new List<Word>() { new Word("formateren", WordType.Verb, Language.Dutch) });
            words.Add("computer", new List<Word>() { 
                                        new Word("computer", WordType.Noun, Language.Dutch), 
                                        new Word("computer", WordType.Noun, Language.English) 
                                  });
            words.Add("virus", new List<Word>(){
                                        new Word("virus", WordType.Noun, Language.Dutch),
                                        new Word("virus", WordType.Noun, Language.English)
                                  });

            words.Add("format", new List<Word>() { new Word("to format", WordType.Verb, Language.English) });
            if (words.ContainsKey(word))
                return words[word];
            else
                return new List<Word>() { new Word("invalid word",WordType.Unknown,Language.Unknown)};
        }
    }

    public class Word
    {
        public Word(string value, WordType type, Language language)
        {
            this.Value = value;
            this.Type = type;
            this.Language = language;
        }

        public WordType Type { get; set; }
        public Language Language { get; set; }
        public string Value { get; set; }
    }

    public enum WordType
    {
        Verb, Noun, Interjection, Adverb, Adjective, Pronoun, Article, Unknown
    }

    public enum Language { English, Dutch, Unknown}

    #endregion
}
