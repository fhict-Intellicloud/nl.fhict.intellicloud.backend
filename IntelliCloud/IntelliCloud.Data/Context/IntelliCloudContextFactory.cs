using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nl.fhict.IntelliCloud.Data.Context
{
    /// <summary>
    /// A factory that creates data contexts providing access to the IntelliCloud database.
    /// </summary>
    public class IntelliCloudContextFactory
    {
        /// <summary>
        /// Creates a new <see cref="IntelliCloudContext"/> instance.
        /// </summary>
        /// <returns>The newly created context.</returns>
        public IntelliCloudContext Create()
        {
            return new IntelliCloudContext();
        }
    }
}
