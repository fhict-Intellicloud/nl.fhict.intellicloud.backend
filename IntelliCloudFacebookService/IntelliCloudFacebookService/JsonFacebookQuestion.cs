using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliCloudFacebookService
{
    /// <summary>
    /// Used to create a JSON String for the webrequest
    /// </summary>
    public class JsonFacebookQuestion
    {
        /// <summary>
        /// Create new QuestionMailObject
        /// </summary>
        /// <param name="source">The source of the question, in this case "Mail"</param>
        /// <param name="reference">The e-mailaddress of the sender</param>
        /// <param name="question">The content of the e-mail</param>
        /// <param name="title">The subject of the e-mail</param>
        public JsonFacebookQuestion(String source, String reference, String question, String title, String postid)
        {
            this.source = source;
            this.reference = reference;
            this.question = question;
            this.title = title;
            this.postId = postid;
        }

        public String source { get; set; }

        public String reference { get; set; }

        public String question { get; set; }

        public String title { get; set; }

        public String postId { get; set; }
    }
}
