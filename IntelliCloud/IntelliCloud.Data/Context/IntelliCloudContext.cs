using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace nl.fhict.IntelliCloud.Data.Context
{
    /// <summary>
    /// A data context providing access to the IntelliCloud database.
    /// </summary>
    public class IntelliCloudContext : DbContext, IDisposable
    {
        public IntelliCloudContext()
            : base(GetConnectionString())
        {
        }

        /// <summary>
        /// Creates a connection string to the IntelliCloud database by using the data from the settings file.
        /// </summary>
        /// <returns>Returns the connection string.</returns>
        private static string GetConnectionString()
        {
            SqlConnectionStringBuilder sqlConnectionString = new SqlConnectionStringBuilder()
            {
                InitialCatalog = ConfigurationManager.AppSettings["IntelliCloud.Database.Catalog"],
                DataSource = ConfigurationManager.AppSettings["IntelliCloud.Database.DataSource"],
                IntegratedSecurity = false,
                MultipleActiveResultSets = true,
                Password = ConfigurationManager.AppSettings["IntelliCloud.Database.Password"],
                PersistSecurityInfo = true,
                UserID = ConfigurationManager.AppSettings["IntelliCloud.Database.Username"]
            };

            return sqlConnectionString.ConnectionString;
        }
    }
}
