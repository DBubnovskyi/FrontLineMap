using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapDataProvider.DataSource 
{
    internal class NoelreportsModel
    {
        private static NoelreportsModel ParseStyle(NoelreportsModel model)
        {
            foreach(var item in model.OverlaysData)
            {
                if (item.Polygons != null)
                {
                    foreach (var polygon in item.Polygons)
                    {
                        var style = model.Styles.FirstOrDefault(x => x.Id == polygon.StyleId);
                        polygon.Style = style;
                    }
                }
            }
            return model;
        }

        public static NoelreportsModel Deserialize(string json){
            var data = JsonConvert.DeserializeObject<NoelreportsModel>(json);
            return ParseStyle(data);
            
        }

        [JsonProperty("meta")]
        public MetaInfo Meta { get; set; }

        [JsonProperty("view")]
        public object View { get; set; }

        [JsonProperty("shareView")]
        public object ShareView { get; set; }

        [JsonProperty("attributes")]
        public AttrData Attributes { get; set; }

        [JsonProperty("styles")]
        public List<Style> Styles { get; set; }

        [JsonProperty("bookmarks")]
        public object Bookmarks { get; set; }

        [JsonProperty("legend")]
        public List<ItemLegend> Legend { get; set; }

        [JsonProperty("overlays")]
        public List<Overlays> OverlaysData { get; set; }

        [JsonProperty("layers")]
        public List<object> Layers { get; set; }

        public class MetaInfo
        {
            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("createdAt")]
            public Uri CreatedAt { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("listType")]
            public string ListType { get; set; }

            [JsonProperty("lastModified")]
            public string LastModified { get; set; }

            [JsonProperty("lastHash")]
            public string LastHash { get; set; }

            [JsonProperty("version")]
            public string Version { get; set; }
        }

        public class AttrData
        {
            [JsonProperty("columns")]
            public List<string> Columns { get; set; }

            [JsonProperty("rows")]
            public List<List<string>> Rows { get; set; }
        }

        public partial class Style
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("color_1")]
            public Color Color1 { get; set; }

            [JsonProperty("color_2")]
            public Color Color2 { get; set; }

            [JsonProperty("weight")]
            public long Weight { get; set; }

            [JsonProperty("lineType")]
            public long LineType { get; set; }

            [JsonProperty("cap")]
            public long Cap { get; set; }

            [JsonProperty("join")]
            public long Join { get; set; }

            [JsonProperty("segment")]
            public List<float> Segment { get; set; }
        }

        public partial class Color
        {
            [JsonProperty("color")]
            public string ColorHtml { get; set; }

            [JsonProperty("opacity")]
            public double Opacity { get; set; }
        }

        public class ItemLegend
        {
            [JsonProperty("styleId")]
            public string StyleId { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("toggle")]
            public bool Toggle { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("toggled")]
            public bool Toggled { get; set; }
        }

        public class Overlays
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("editable")]
            public bool Editable { get; set; }

            [JsonProperty("type")]
            public long Type { get; set; }

            [JsonProperty("open")]
            public bool Open { get; set; }

            [JsonProperty("visible")]
            public bool Visible { get; set; }

            [JsonProperty("overlays")]
            public List<Polygon> Polygons { get; set; }
        }

        public partial class Polygon
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("visible")]
            public bool Visible { get; set; }

            [JsonProperty("editable")]
            public bool Editable { get; set; }

            [JsonProperty("points")]
            public List<List<double>> Points { get; set; }

            [JsonProperty("styleId")]
            public string StyleId { get; set; }

            public Style Style { get; set; }

            [JsonProperty("type")]
            public long LineType { get; set; }

            [JsonIgnore]
            [JsonProperty("metaData")]
            public string MetaData { get; set; }

            [JsonIgnore]
            [JsonProperty("extrude", NullValueHandling = NullValueHandling.Ignore)]
            public long? Extrude { get; set; }
        }
    }
}
