using System.Collections.Generic;

namespace MapDataProvider.Models.MapElement
{
    public class BaseElement : IElement
    {
        public string Name { get; set; }
        public Style Style { get; set; }
        public List<PointLatLng> Points { get; set; } = new List<PointLatLng>();
    }
}
