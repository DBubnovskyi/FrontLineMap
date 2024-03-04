using Newtonsoft.Json;
using System.Collections.Generic;

namespace MapDataProvider.Models
{
    /// <summary>
    /// Config file model for data providers
    /// </summary>
    internal class Config
    {
        /// <summary>
        /// List of data sources that contain config file
        /// </summary>
        [JsonProperty("sources")]
        public List<Source> Sources { get; set; }
    }
}
