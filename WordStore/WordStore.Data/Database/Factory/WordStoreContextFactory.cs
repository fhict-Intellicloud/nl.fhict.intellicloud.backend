using IntelliCloud.WordStore.Data.Database.Context;

namespace IntelliCloud.WordStore.Data.Database.Factory
{
    public class WordStoreContextFactory : IWordStoreContextFactory
    {
        /// <summary>
        /// A factory providing functionality to create new instances of 
        /// <see cref="IWordStoreContextFactory"/>.
        /// </summary>
        public IWordStoreContext Create()
        {
            /// <summary>
            /// Creates a new instance of <see cref="IWordStoreContext"/>.
            /// </summary>
            /// <returns>The created <see cref="IWordStoreContext"/>.</returns>
            return new WordStoreContext();
        }
    }
}
