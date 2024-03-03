using MapDataProvider.Models.MapElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProvider.Models
{
    public class MapDataCollection
    {
        public string Name { get; set; }
        public List<Polygon> Polygons { get; set; } = new List<Polygon>();
        public List<Line> Lines { get; set; } = new List<Line>();
        public List<Exception> Errors { get; set; } = new List<Exception>();
    }
}
