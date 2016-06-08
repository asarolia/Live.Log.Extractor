namespace Live.Log.Extractor.IndexerService
{
    using Live.Log.Extractor.IndexerService.Abstract;

    /// <summary>
    /// Product director.
    /// </summary>
    class ProductDirector
    {
        /// <summary>
        /// Constructs the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public void Construct(IBuildProduct builder) 
        {
            builder.PopulateIndexStartDate();
            builder.PopulateLastSearchDate();
            builder.PopulateRegularExpressions();
        }
    }
}
