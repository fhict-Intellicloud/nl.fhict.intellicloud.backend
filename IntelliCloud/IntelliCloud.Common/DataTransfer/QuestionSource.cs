using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nl.fhict.IntelliCloud.Common.DataTransfer
{
    /// <summary>
    /// A class representing the relation between a source and a question. This class is used for question specific 
    /// information about the source, like the identifier of the Twitter or Facebook post.
    /// </summary>
    [DataContract]
    public class QuestionSource
    {
        /// <summary>
        /// Gets or sets the source that is used to return the answer of the question. The source represents the 
        /// identification of a source definition, like the mail address for the source definition 'Mail'.
        /// </summary>
        [DataMember]
        public Source Source { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the post where the question originated from, like the Facebook post 
        /// identifier. This identifier is used so the answer can be replied to this post.
        /// </summary>
        [DataMember]
        public string PostId { get; set; }
    }
}
