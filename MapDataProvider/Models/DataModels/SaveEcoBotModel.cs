using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace MapDataProvider.DataSource
{
    internal class SaveEcoBotModel
    {
        public static SaveEcoBotModel Deserialize(string json) => JsonConvert.DeserializeObject<SaveEcoBotModel>(json);

        [JsonProperty("datetime")]
        public DateTime? Datetime { get; set; }

        [JsonProperty("polygons")]
        public DataSet Polygons { get; set; }

        public class Element
        {
            [JsonProperty("key")]
            public string Key { get; set; }

            [JsonProperty("datetime")]
            public DateTime? Datetime { get; set; }

            [JsonProperty("polygon")]
            public ElementPolygon Polygon { get; set; }

            [JsonProperty("style")]
            public Style Style { get; set; }
        }

        public class Feature
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("geometry")]
            public Geometry Geometry { get; set; }

            [JsonProperty("properties")]
            public object Properties { get; set; }
        }

        public class Geometry
        {
            [JsonConstructor]
            public Geometry(JToken type, JToken coordinates)
            {
                if (Coordinates == null)
                {
                    Coordinates = new List<List<List<double>>> ();
                }
                if ((string)type == "Polygon")
                {
                    Coordinates = coordinates.ToObject<List<List<List<double>>>>();
                }
                else if ((string)type == "MultiPolygon")
                {
                    _coordinates3 = new List<List<List<double>>>();
                    _coordinates4 = coordinates.ToObject<List<List<List<List<double>>>>>();
                    foreach(var obj in _coordinates4)
                    {
                        foreach(var obj1 in obj)
                        {
                            _coordinates3.Add(obj1);
                        }
                    }
                    Coordinates.AddRange(_coordinates3);
                }
                Type = (string)type;
            }

            [JsonIgnore]
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonIgnore]
            List<List<List<double>>> _coordinates3 { get; set; }
            [JsonIgnore]
            List<List<List<List<double>>>> _coordinates4 { get; set; }


            [JsonIgnore]
            public List<List<List<double>>> Coordinates { get; set; }
        }

        public class ElementPolygon
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("features")]
            public List<Feature> Features { get; set; }
        }

        public class DataSet
        {
            [JsonProperty("assessed_russian_advance")]
            public Element AssessedRussianAdvance { get; set; }

            [JsonProperty("assessed_russian_control")]
            public Element AssessedRussianControl { get; set; }

            [JsonProperty("claimed_ukrainian_counteroffensives")]
            public Element ClaimedUkrainianCounteroffensives { get; set; }

            [JsonProperty("reported_ukrainian_partisan_warfare")]
            public Element ReportedUkrainianPartisanWarfare { get; set; }

            [JsonProperty("claimed_russian_control")]
            public Element ClaimedRussianControl { get; set; }
        }

        public class Style
        {
            [JsonConstructor]
            public Style(JToken dashArray) 
            {
                List<float> termsList = new List<float>();
                if ((string)dashArray != null)
                {
                    string[] strings = dashArray.ToString().Split(',');
                    foreach (string num in strings)
                    {
                        float buf;
                        float.TryParse(num, out buf);
                        termsList.Add(buf);
                    }
                    if(termsList.Count != 0)
                    {
                        DashArray = termsList.ToArray();
                    }
                }
                else
                {
                    DashArray = new float[] { 10 };
                }
            }

            [JsonProperty("fillColor")]
            public string FillColor { get; set; }

            [JsonProperty("color")]
            public string Color { get; set; }

            [JsonProperty("weight")]
            public double? Weight { get; set; }

            [JsonProperty("fillOpacity")]
            public double? FillOpacity { get; set; }

            [JsonProperty("opacity")]
            public double? Opacity { get; set; }

            [JsonIgnore]
            [JsonProperty("dashArray")]
            public float[] DashArray { get; set; }

            [JsonProperty("dashOffset")]
            public string DashOffset { get; set; }
        }
    }
}
