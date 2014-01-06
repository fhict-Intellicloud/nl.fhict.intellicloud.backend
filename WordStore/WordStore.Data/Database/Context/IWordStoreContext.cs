using IntelliCloud.WordStore.Data.Database.Model;
using System;
using System.Data.Entity;

namespace IntelliCloud.WordStore.Data.Database.Context
{
    /// <summary>
    /// An interface for a database context providing database objects and functionality.
    /// </summary>
    public interface IWordStoreContext : IDisposable
    {
        #region Properties

        /// <summary>
        /// The database set containing all instances of <see cref="KeywordEntity"/>.
        /// </summary>
        IDbSet<KeywordEntity> Keywords { get; }

        /// <summary>
        /// The database set containing all instances of <see cref="WordEntity"/>.
        /// </summary>
        IDbSet<WordEntity> Words { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>The number of objects written to the underlying database.</returns>
        int SaveChanges();

        #endregion Methods
    }
}
