using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliMailClient
{

    /// <summary>
    /// This class contains the fields for a new question and is used for serializing it to json
    /// </summary>
    class QuestionMailObject
    {
        String _source;
        String _reference;
        String _question;
        String _title;

        /// <summary>
        /// Create new QuestionMailObject
        /// </summary>
        /// <param name="source">The source of the question, in this case "Mail"</param>
        /// <param name="reference">The e-mailaddress of the sender</param>
        /// <param name="question">The content of the e-mail</param>
        /// <param name="title">The subject of the e-mail</param>
        public QuestionMailObject(String source, String reference, String question, String title)
        {
            this._source = source;
            this._reference = reference;
            this._question = question;
            this._title = title;
        }

        public String source 
        {
            get
            {
                return _source;
            }
        }

        public String reference
        {
            get
            {
                return _reference;
            }
        }

        public String question
        {
            get
            {
                return _question;
            }
        }

        public String title
        {
            get
            {
                return _title;
            }
        }
    }
}
