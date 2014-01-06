using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WordObject = IntelliCloud.WordStore.Common.Word.Word;

namespace IntelliCloud.WordStore.Common.Word
{
    /// <summary>
    /// A class representing a keyword.
    /// </summary>
    [DataContract]
    public class Keyword
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Keyword"/> class.
        /// </summary>
        /// <param name="value"></param>
        public Keyword(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// The value of the keyword, for instance 'bicycle'.
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// The instances of <see cref="WordObject"/> that correspond to the <see cref="Value"/>.
        /// </summary>
        [DataMember]
        public IList<WordObject> MatchingWords { get; set; }
    }
}
