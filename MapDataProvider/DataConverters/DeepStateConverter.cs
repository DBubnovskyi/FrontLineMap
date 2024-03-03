using MapDataProvider.DataConverters.Contracts;
using MapDataProvider.DataSourceProvoders.Models.DeepState;
using MapDataProvider.Models;
using MapDataProvider.Models.MapElement;
using System.Drawing;
using System.Linq;

namespace MapDataProvider.DataConverters
{
    internal class DeepStateConverter : IDataConverter
    {
        public MapDataCollection ConvertMapData(string jsonInput)
        {
            var result = new MapDataCollection();
            result.Name = nameof(DeepStateDataModel);
            var model = DeepStateDataModel.Deserialize(jsonInput);

            foreach (var item in model.Features.Where(x => x.Geometry.Type == "Polygon"))
            {
                int fillOpacity = (int)(255.0 * item.Properties.FillOpacity ?? 1);
                Color fillColor = ColorTranslator.FromHtml(item.Properties.Fill);
                var fill = new SolidBrush(Color.FromArgb(fillOpacity, fillColor));

                int strokeOpacity = (int)(255.0 * item.Properties.StrokeOpacity ?? 1);
                Color strokeColor = ColorTranslator.FromHtml(item.Properties.Stroke);
                float strokeWidth = (float)(item.Properties.StrokeWidth ?? 1);
                var stroke = new Pen(Color.FromArgb(strokeOpacity, strokeColor), strokeWidth);

                Style style = new Style()
                {
                    Fill = fill,
                    Stroke = stroke
                };

                foreach (var coordSeV2 in item.Geometry.Coordinates)
                {
                    Polygon polygon = new Polygon()
                    {
                        Name = item.Properties.Name,
                        Style = style,
                    };
                    foreach (var coordSeV1 in coordSeV2)
                    {
                        var dot = new PointLatLng()
                        {
                            Lng = coordSeV1[0],
                            Lat = coordSeV1[1],
                            Height = coordSeV1[2]
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
