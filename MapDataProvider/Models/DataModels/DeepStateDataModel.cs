using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProvider.DataSourceProvoders.Models.DeepState
{
    internal class DeepStateDataModel
    {
        public static DeepStateDataModel Deserialize(string json) => JsonConvert.DeserializeObject<DeepStateDataModel>(json);

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("features")]
        public List<Feature> Features { get; set; }


        public class Feature
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("geometry")]
            public Geometry Geometry { get; set; }

            [JsonProperty("properties")]
            public Properties Properties { get; set; }
        }

        public class Geometry
        {
            [JsonConstructor]
            public Geometry(JToken type, JToken coordinates)
            {
                if ((string)type == "Polygon")
                {
                    Coordinates = coordinates.ToObject<List<List<List<double>>>>();
                }
                Type = (string)type;
            }

            [JsonIgnore]
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonIgnore]
            [JsonProperty("coordinates")]
            public List<List<List<double>>> Coordinates { get; set; }
        }

        public class StyleMapHash
        {
            [JsonProperty("normal")]
            public string Normal { get; set; }

            [JsonProperty("highlight")]
            public string Highlight { get; set; }
        }

        public class Properties
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("styleUrl")]
            public string StyleUrl { get; set; }

            [JsonProperty("styleHash")]
            public string StyleHash { get; set; }

            [JsonProperty("styleMapHash")]
            public StyleMapHash StyleMapHash { get; set; }

            [JsonProperty("stroke-opacity")]
            public double? StrokeOpacity { get; set; }

            [JsonProperty("stroke")]
            public string Stroke { get; set; }

            [JsonProperty("stroke-width")]
            public double? StrokeWidth { get; set; }

            [JsonProperty("fill-opacity")]
            public double? FillOpacity { get; set; }

            [JsonProperty("fill")]
            public string Fill { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }
        }
    }
}
