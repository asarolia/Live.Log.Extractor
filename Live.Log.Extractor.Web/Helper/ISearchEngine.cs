namespace Live.Log.Extractor.Web.Helper
{
    using System.Collections.Generic;
    using Live.Log.Extractor.Domain;
    using Live.Log.Extractor.Web.Models;

    /// <summary>
    /// Search Engine Interface
    /// </summary>
    interface ISearchEngine
    {
        /// <summary>
        /// Searches the in production servers parallely.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="productionServers">The production servers.</param>
        void SearchInProductionServersParallely(IndexModel model, List<string> productionServers);

        /// <summary>
        /// Searches the in production servers sequentially.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="productionServers">The production servers.</param>
        void SearchInProductionServersSequentially(IndexModel model, List<string> productionServers);

        /// <summary>
        /// Writes to file.
        /// </summary>
        /// <param name="concurrentQueue">The concurrent queue.</param>
        /// <param name="model">The model.</param>
        void WriteToFile(List<EchannelLog> concurrentQueue, IndexModel model);

        /// <summary>
        /// Brodcast the messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void BrodCast(string connectionId, string message);
    }
}
