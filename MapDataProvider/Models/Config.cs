using Newtonsoft.Json;
using System.Collections.Generic;

namespace MapDataProvider.Models
{
    internal class Config
    {
        [JsonProperty("sources")]
        public List<Source> Sources { get; set; }

        public class Source
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("web-site")]
            public string Website { get; set; }

            [JsonProperty("api-url")]
            public List<string> ApiUrl { get; set; }
        }
    }
}
