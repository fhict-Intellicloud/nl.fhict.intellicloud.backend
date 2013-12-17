using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nl.fhict.IntelliCloud.Data.WordStoreService;

namespace nl.fhict.IntelliCloud.Data.IntelliCloud.Context
{
    /// <summary>
    /// A data context providing acces to wordstore service.
    /// </summary>
    public class WordStoreContext : IDisposable
    {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="WordStoreContext"/> class.
        /// </summary>
        public WordStoreContext()
        {
           this.Client = new WordServiceClient();
        }


        public void Dispose()
        {
        }

        /// <summary>
        /// Gets a instance proxy class to communicate with the WordStoreService.
        /// </summary>
        public IWordService Client { get; private set; }
    }
}
