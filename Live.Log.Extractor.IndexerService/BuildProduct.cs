namespace Live.Log.Extractor.IndexerService
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Live.Log.Extractor.IndexerService.Abstract;
    using Live.Log.Extractor.Domain;

    /// <summary>
    /// Build Product class.
    /// </summary>
    class BuildProduct : IBuildProduct
    {
        /// <summary>
        /// Product Instance.
        /// </summary>
        private Product product { get; set; }

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>
        /// The node.
        /// </value>
        private XElement node { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildProduct"/> class.
        /// </summary>
        /// <param name="productType">Type of the product.</param>
        public BuildProduct(ProductType productType)
        {
            this.product= new Product(productType);
            XDocument doc = XDocument.Load(System.Configuration.ConfigurationManager.AppSettings.Get("ResourcePath"));
            this.node = doc.Root.Elements("Product").FirstOrDefault(x => string.Equals(x.Attribute("value").Value, productType.ToString()));
        }

        /// <summary>
        /// Populates the index start date.
        /// </summary>
        /// <param name="productNode">The product node.</param>
        public void PopulateIndexStartDate()
        {
            product.IndexStartDate = DateTime.Parse(this.node.Element("IndexStartDate").Attribute("value").Value);
        }

        /// <summary>
        /// Populates the last search date.
        /// </summary>
        /// <param name="productNode">The product node.</param>
        public void PopulateLastSearchDate()
        {
            string date = this.node.Element("LastIndexedDate").Attribute("value").Value;
        
            if (!string.IsNullOrEmpty(date))
            {
                product.LastIndexedDate = DateTime.Parse(date);
            }
        }

        /// <summary>
        /// Populates the regular expressions.
        /// </summary>
        /// <param name="productNode">The product node.</param>
        public void PopulateRegularExpressions()
        {
            this.node.Element("RegularExpressions").Elements().Attributes("value").ToList().ForEach( value => product.RegularExpressions.Add(value.Value));
        }

        /// <summary>
        /// Gets the product.
        /// </summary>
        /// <returns>
        /// Instance of final Product.
        /// </returns>
        public Product GetProduct()
        {
            return product;
        }
    }
}
