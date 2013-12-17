using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nl.fhict.IntelliCloud.Data.IntelliCloud.Context
{
    /// <summary>
    /// A class following the factory design pattern to create Contexts for the services that are used
    /// within intellicloud.
    /// </summary>
    public class ServiceContextFactory
    {
        /// <summary>
        /// Factory method to create new WordStoreContext.
        /// </summary>
        /// <returns></returns>
        public WordStoreContext GetWordStoreContext() {
            return new WordStoreContext();
        }
    }
}
