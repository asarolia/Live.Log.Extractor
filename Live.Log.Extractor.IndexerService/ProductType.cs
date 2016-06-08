namespace Live.Log.Extractor.IndexerService
{
    using System.ComponentModel;

    /// <summary>
    /// ProductType enum
    /// </summary>
    public enum ProductType
    {
        [Description("MyPolicy")]
        MyPolicy,

        [Description("Motor")]
        Motor,

        [Description("Home")]
        Home
    }
}
