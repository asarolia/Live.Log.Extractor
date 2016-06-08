using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Live.Log.Extractor.IndexerService.Infrastructure;
using Live.Log.Extractor.Domain;

namespace Live.Log.Extractor.IndexerService
{
    /// <summary>
    /// Product class
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        public Product()
        {
            RegularExpressions = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        /// <param name="product">The product.</param>
        public Product(ProductType product) : this()
        {
            this.ProductName = product;
        }
        
        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        /// <value>
        /// The name of the product.
        /// </value>
        public ProductType ProductName { get; set; }

        /// <summary>
        /// Gets or sets the regular expressions.
        /// </summary>
        /// <value>
        /// The regular expressions.
        /// </value>
        public List<string> RegularExpressions { get; set; }

        /// <summary>
        /// Gets or sets the index start date.
        /// </summary>
        /// <value>
        /// The index start date.
        /// </value>
        public DateTime IndexStartDate { get; set; }

        /// <summary>
        /// Gets or sets the last indexed date.
        /// </summary>
        /// <value>
        /// The last indexed date.
        /// </value>
        public DateTime? LastIndexedDate { get; set; }
    }
}
