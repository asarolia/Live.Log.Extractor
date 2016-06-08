namespace Live.Log.Extractor.Domain
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
