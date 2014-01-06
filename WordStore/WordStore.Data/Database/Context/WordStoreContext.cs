using IntelliCloud.WordStore.Common;
using IntelliCloud.WordStore.Data.Database.Model;
using System.Data.Entity;
using System.Data.SqlClient;

namespace IntelliCloud.WordStore.Data.Database.Context
{
    /// <summary>
    /// A database context providing database objects and functionality.
    /// </summary>
    internal class WordStoreContext : DbContext, IWordStoreContext
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WordStoreContext"/> class.
        /// </summary>
        public WordStoreContext()
            : base(GetConnectionString())
        {
            this.Keywords = this.Set<KeywordEntity>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The database set containing all instances of <see cref="KeywordEntity"/>.
        /// </summary>
        public IDbSet<KeywordEntity> Keywords { get; private set; }

        /// <summary>
        /// The database set containing all instances of <see cref="WordEntity"/>.
        /// </summary>
        public IDbSet<WordEntity> Words { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Retrieves a connection string constructed by the available values provided in the application settings.
        /// </summary>
        /// <returns>A new connection string.</returns>
        private static string GetConnectionString()
        {
            SqlConnectionStringBuilder sqlConnection = new SqlConnectionStringBuilder()
            {
                InitialCatalog = ApplicationSettings.WordStoreCatalog,
                DataSource = ApplicationSettings.WordStoreDataSource,
                Password = ApplicationSettings.WordStorePassword,
                UserID = ApplicationSettings.WordStoreUserId,
                PersistSecurityInfo = true,
                IntegratedSecurity = false,
            };

            return sqlConnection.ConnectionString;
        }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but before the model has 
        /// been locked down and used to initialize the context.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context is created. The 
        /// model for that context is then cached and is for all further instances of the context in the app domain. 
        /// This caching can be disabled by setting the ModelCaching property on the given ModelBuilder, but note that 
        /// this can seriously degrade performance. More control over caching is provided through use of the 
        /// <see cref="DbModelBuilder"/> and <see cref="DbContextFactory"/> classes directly.
        /// </remarks>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<KeywordEntity>()
                .HasMany(x => x.Words)
                .WithMany(x => x.Keywords)
                .Map(x =>
                {
                    x.MapLeftKey("KeywordId");
                    x.MapRightKey("WordId");
                    x.ToTable("KeywordWords");
                });
        }

        #endregion Methods
    }
}
