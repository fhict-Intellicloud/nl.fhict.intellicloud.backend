using IntelliCloud.WordStore.Data.Database.Context;

namespace IntelliCloud.WordStore.Data.Database.Factory
{
    /// <summary>
    /// An interface for a factory providing functionality to create new instances of 
    /// <see cref="IWordStoreContextFactory"/>.
    /// </summary>
    public interface IWordStoreContextFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IWordStoreContext"/>.
        /// </summary>
        /// <returns>The created <see cref="IWordStoreContext"/>.</returns>
        IWordStoreContext Create();
    }
}
