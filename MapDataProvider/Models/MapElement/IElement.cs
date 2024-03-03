using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProvider.Models.MapElement
{
    public interface IElement
    {
        string Name { get; set; }
        Style Style { get; set; }
        List<PointLatLng> Points { get; set; }
    }
}
