namespace Live.Log.Extractor.Web.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public class ExceedRegion
    {
        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>
        /// The region.
        /// </value>
        [DisplayName("Region")]
        public string CurrentRegion { get; set; }

        /// <summary>
        /// Gets the region data.
        /// </summary>
        public List<Region> RegionData
        {
            get
            {
                return new List<Region>
                {
                    new Region { RegionId= "CITXA2A", RegionName="Leg2-Dev1"},
                    new Region { RegionId= "CITXA3A", RegionName="Leg3-Dev1"},
                    new Region { RegionId= "CITXA4A", RegionName="Leg4-Dev1"},
                    //Start:Added
                    new Region { RegionId= "CITXA9A", RegionName="Leg9-Dev1"},
                    new Region { RegionId= "CITXA8A", RegionName="Leg8-Dev1"},
                    new Region { RegionId= "CITXAXA", RegionName="Leg10-Dev1"},
                    //End:Added
                    new Region { RegionId= "CIDXA2A", RegionName="Leg2-Dev2"},
                    new Region { RegionId= "CIDXA3A", RegionName="Leg3-Dev2"},
                    new Region { RegionId= "CIDXA4A", RegionName="Leg4-Dev2"},
                    new Region { RegionId= "CIUXA2A", RegionName="Leg2-Systest"},
                    //Start:Added
                    new Region { RegionId= "CIUXA9A", RegionName="Leg9-Systest"},
                    new Region { RegionId= "CIUXA8A", RegionName="Leg8-Systest"},
                    new Region { RegionId= "CIUXAXA", RegionName="Leg10-Systest"},
                    //End:Added
                    new Region { RegionId= "CIUXA3A", RegionName="Leg3-Systest"},
                    new Region { RegionId= "CIUXA4A", RegionName="Leg4-Systest"},
                    new Region { RegionId= "CIUXA1A", RegionName="UAT"}
                };
            }
        }
    }
}