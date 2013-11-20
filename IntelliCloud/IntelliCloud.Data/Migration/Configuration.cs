using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Migrations;
using nl.fhict.IntelliCloud.Data.Context;

namespace nl.fhict.IntelliCloud.Data.Migration
{
    /// <summary>
    /// Configuration class for the <see cref="IntelliCloudContext"/> model.
    /// </summary>
    internal sealed class Configuration : DbMigrationsConfiguration<IntelliCloudContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
        }
    }
}
