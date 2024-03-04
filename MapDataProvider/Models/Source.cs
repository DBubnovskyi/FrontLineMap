using System.Collections.Generic;
using Newtonsoft.Json;

namespace MapDataProvider.Models
{
    public class Source : ISource
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("web-site")]
        public string WebSite { get; set; }

        [JsonProperty("api-url")]
        public List<string> ApiUrls { get; set; }
    }
}
