using MapDataProvider.DataConverters.Contracts;
using MapDataProvider.DataSource.CyberBoroshno;
using MapDataProvider.Models;
using MapDataProvider.Models.MapElement;
namespace MapDataProvider.DataConverters
{
    internal class CyberBoroshnoConverter : IDataConverter
    {
        public MapDataCollection ConvertMapData(string jsonInput)
        {
            var data = CyberBoroshnoModel.Deserialize(jsonInput);
            var result = new MapDataCollection();
            result.Name = nameof(CyberBoroshnoModel);

            foreach (var item in data.Features)
            {
                Style style = new Style();
                foreach (var coordSeV2 in item.Geometry.Coordinates)
                {
                    Polygon polygon = new Polygon()
                    {
                        Name = item.Type,
                        Style = style,
                    };
                    foreach (var coordSeV1 in coordSeV2)
                    {
                        var dot = new PointLatLng()
                        {
                            Lng = coordSeV1[0],
                            Lat = coordSeV1[1],
                            Height = 0
                        };
                        polygon.Points.Add(dot);
                    }
                    result.Polygons.Add(polygon);
                }
            }
            return result;
        }
    }
}
