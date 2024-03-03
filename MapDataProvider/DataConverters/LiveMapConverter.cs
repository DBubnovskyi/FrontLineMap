using MapDataProvider.DataConverters.Contracts;
using MapDataProvider.DataSource;
using MapDataProvider.DataSourceProvoders.Models.DeepState;
using MapDataProvider.Models;
using MapDataProvider.Models.MapElement;
using System.Drawing;

namespace MapDataProvider.DataConverters
{
    internal class LiveMapConverter : IDataConverter
    {
        public MapDataCollection ConvertMapData(string jsonInput)
        {
            var data = LiveMapModel.Deserialize(jsonInput);
            var result = new MapDataCollection();
            result.Name = nameof(DeepStateDataModel);

            foreach (var item in data.Polygons.Features)
            {
                Color color = ColorTranslator.FromHtml("#ff5252");
                var fill = new SolidBrush(Color.FromArgb(90, color));
                var stroke = new Pen(Color.FromArgb(128, color), 1);

                Style style = new Style()
                {
                    Fill = fill,
                    Stroke = stroke
                };
                foreach (var coordSeV2 in item.Geometry.Coordinates)
                {
                    Polygon polygon = new Polygon()
                    {
                        Name = item.Name,
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
