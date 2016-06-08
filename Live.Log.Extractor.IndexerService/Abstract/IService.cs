namespace Live.Log.Extractor.IndexerService
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using Live.Log.Extractor.Domain;

    /// <summary>
    /// IService Contract
    /// </summary>
    [ServiceContract]
    public interface IService
    {
        /// <summary>
        /// Indexes the files.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <returns></returns>
        [OperationContract]
        bool CreateIndexFiles(ProductType product);

        /// <summary>
        /// Gets the last indexed date.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns></returns>
        [OperationContract]
        List<IndexInformation> SearchIndex(ProductType product, string searchText); 
    }
}
