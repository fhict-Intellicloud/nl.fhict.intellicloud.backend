using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliCloud.WordStore.Common
{
    /// <summary>
    /// A class providing the values that are set in the application settings.
    /// </summary>
    public static class ApplicationSettings
    {
        #region Fields

        /// <summary>
        /// The key that is used in the Web.config to store the database catalog.
        /// </summary>
        private const string keyWordStoreCatalog = "Data.WordStore.Catalog";

        /// <summary>
        /// The key that is used in the Web.config to store the database datasource.
        /// </summary>
        private const string keyWordStoreDataSource = "Data.WordStore.DataSource";

        /// <summary>
        /// The key that is used in the Web.config to store the database password.
        /// </summary>
        private const string keyWordStorePassword = "Data.WordStore.Password";

        /// <summary>
        /// The key that is used in the Web.config to store the database user id.
        /// </summary>
        private const string keyWordStoreUserId = "Data.WordStore.UserId";

        #endregion Fields

        #region Properties

        /// <summary>
        /// The catalog for the WordStore database.
        /// </summary>
        public static string WordStoreCatalog
        {
            get
            {
                return ConfigurationManager.AppSettings[keyWordStoreCatalog];
            }
        }

        /// <summary>
        /// The datasource for the WordStore database.
        /// </summary>
        public static string WordStoreDataSource
        {
            get
            {
                return ConfigurationManager.AppSettings[keyWordStoreDataSource];
            }
        }

        /// <summary>
        /// The password for the WordStore database.
        /// </summary>
        public static string WordStorePassword
        {
            get
            {
                return ConfigurationManager.AppSettings[keyWordStorePassword];
            }
        }

        /// <summary>
        /// The user id for the WordStore database.
        /// </summary>
        public static string WordStoreUserId
        {
            get
            {
                return ConfigurationManager.AppSettings[keyWordStoreUserId];
            }
        }

        #endregion Properties
    }
}
