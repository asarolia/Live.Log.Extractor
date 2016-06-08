namespace Live.Log.Extractor.IndexerService.Abstract
{
    interface IBuildProduct
    {
        /// <summary>
        /// Populates the index start date.
        /// </summary>
        void PopulateIndexStartDate();

        /// <summary>
        /// Populates the last search date.
        /// </summary>
        void PopulateLastSearchDate();

        /// <summary>
        /// Populates the regular expressions.
        /// </summary>
        void PopulateRegularExpressions();

        /// <summary>
        /// Gets the product.
        /// </summary>
        /// <returns>Instance of final Product.</returns>
        Product GetProduct();
    }
}
