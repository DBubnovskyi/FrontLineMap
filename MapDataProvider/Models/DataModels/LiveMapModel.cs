
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MapDataProvider.DataSource
{
    internal class LiveMapModel
    {
        public static LiveMapModel Deserialize(string json) => JsonConvert.DeserializeObject<LiveMapModel>(json);

        [JsonProperty("datetime")]
        public DateTime? Datetime { get; set; }

        [JsonProperty("polygon")]
        public DataPolygons Polygons { get; set; }

        public class DataPolygons
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("features")]
            public List<Feature> Features { get; set; }
        }

        public class Feature
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("source")]
            public string Source { get; set; }

            [JsonProperty("id")]
            public int? Id { get; set; }

            [JsonProperty("time")]
            public int? Time { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("geometry")]
            public Geometry Geometry { get; set; }
        }

        public class Geometry
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("coordinates")]
            public List<List<List<double>>> Coordinates { get; set; }
        }
    }
}