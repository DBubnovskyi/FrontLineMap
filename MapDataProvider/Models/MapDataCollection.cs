using System;
using System.Collections.Generic;
using MapDataProvider.Models.MapElement;
using Newtonsoft.Json;

namespace MapDataProvider.Models
{
    /// <summary>
    /// Contains processed data ready to be presented on the map
    /// </summary>
    public class MapDataCollection
    {
        /// <summary>
        /// Name of the data collection
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of the Polygons that data collection contain
        /// </summary>
        public List<Polygon> Polygons { get; set; } = new List<Polygon>();

        /// <summary>
        /// List of the Lines that data collection contain
        /// </summary>
        public List<Line> Lines { get; set; } = new List<Line>();

        /// <summary>
        /// Information about errors and data source
        /// </summary>
        [JsonIgnore]
        public Metadata Metadata { get; set; } = new Metadata();

        /// <summary>
        /// Serialize <see cref="MapDataCollection"/> for save cache file
        /// </summary>
        /// <returns><see cref="string"/> with serialize json object</returns>
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
