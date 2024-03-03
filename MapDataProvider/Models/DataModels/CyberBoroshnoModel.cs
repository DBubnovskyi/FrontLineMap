using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProvider.DataSource.CyberBoroshno
{
    internal class CyberBoroshnoModel
    {
        public static CyberBoroshnoModel Deserialize(string json) => JsonConvert.DeserializeObject<CyberBoroshnoModel>(json);

        public string Type { get; set; }
        public List<Feature> Features { get; set; }
        

        public class Feature
        {
            public string Type { get; set; }
            public Geometry Geometry { get; set; }
        }

        public class Geometry
        {
            public string type { get; set; }
            public List<List<List<double>>> Coordinates { get; set; }
        }
    }
}
