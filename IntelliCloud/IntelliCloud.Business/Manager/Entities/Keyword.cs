using nl.fhict.IntelliCloud.Data.WordStoreService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nl.fhict.IntelliCloud.Business.Manager.Entities
{
    public class Keyword
    {
        public Keyword(Word word, int affinity)
        {
            this.Word = word;
            this.Affinity = affinity;
        }

        public Word Word { get; private set; }

        public int Affinity { get; private set; }
    }
}
