using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace nl.fhict.IntelliCloud.DataTransfer
{
    [DataContract]
    public class Question
    {
        [DataMember]
        public int Id = 0;

        [DataMember]
        public string QuestionText = string.Empty;

        [DataMember]
        public int AskerId = 0;

        [DataMember]
        public int AnswererId = 0;

        [DataMember]
        public int QuestionStateId = 0;

        [DataMember]
        public int SourceId = 0;

    }
}