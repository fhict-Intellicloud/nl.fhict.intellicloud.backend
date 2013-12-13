using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTwitterClient.Business
{
    /// <summary>
    /// Object representing an outgoing question
    /// </summary>
    class TwitterQuestionObject
    {
        /// <summary>
        /// Create new TwitterQuestionObject
        /// </summary>
        /// <param name="source">The source of the question, in this case "Twitter"</param>
        /// <param name="reference">The e-mailaddress of the sender</param>
        /// <param name="question">The content of the e-mail</param>
        /// <param name="title">The subject of the e-mail</param>
        public TwitterQuestionObject(String reference, String question, String title)
        {
            this.source = "Twitter";
            this.reference = reference;
            this.question = question;
            this.title = title;
        }

        //The source of the question in this case twitter
        public String source { get; set; }

        //The screename of the asker, e.g. @IntelliCloudQ
        public String reference { get; set; }

        //The question asked by the user
        public String question { get; set; }

        //The title of the question asked
        public String title { get; set; }
    }
}
