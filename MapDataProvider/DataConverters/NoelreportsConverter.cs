using MapDataProvider.DataConverters.Contracts;
using MapDataProvider.DataSource;
using MapDataProvider.Models;
using MapDataProvider.Models.MapElement;
using System.Drawing;

using Color = System.Drawing.Color;
using Polygon = MapDataProvider.Models.MapElement.Polygon;
using Style = MapDataProvider.Models.MapElement.Style;

namespace MapDataProvider.DataConverters
{
    internal class NoelreportsConverter : IDataConverter
    {
        public MapDataCollection DeserializeMapData(string jsonInput)
        {
            var data = NoelreportsModel.Deserialize(jsonInput);
            var result = new MapDataCollection
            {
                Name = nameof(NoelreportsConverter)
            };

            foreach (var item in data.OverlaysData)
            {
                if (item.Polygons != null)
                {
                    foreach (var poly in item.Polygons)
                    {
                        if (poly.Points != null)
                        {

                            int strokeOpacity = (int)(255.0 * poly.Style?.Color1?.Opacity ?? 1);
                            Color strokeColor = ColorTranslator.FromHtml(poly.Style?.Color1?.ColorHtml ?? "#EEEEEE");
                            float strokeWidth = (float)(poly.Style?.Weight ?? 1);
                            var stroke = new Pen(Color.FromArgb(strokeOpacity, strokeColor), strokeWidth)
                            {
                                DashPattern = poly.Style.Segment != null ? poly.Style.Segment.ToArray() : new float[] { 10 }
                            };

                            if (poly.Style?.Color2 == null)
                            {
                                Style style = new Style()
                                {
                                    Stroke = stroke
                                };
                                Line line = new Line()
                                {
                                    Name = item.Name,
                                    Style = style,
                                };
                                foreach (var coord in poly.Points)
                                {
                                    var dot = new PointLatLng()
                                    {
                                        Lat = coord[0],
                                        Lng = coord[1],
                                        Height = 0
                                    };
                                    line.Points.Add(dot);
                                }
                                result.Lines.Add(line);
                            }
                            else
                            {
                                int fillOpacity = (int)(255.0 * poly.Style?.Color2?.Opacity ?? 1);
                                Color fillColor = ColorTranslator.FromHtml(poly.Style?.Color2?.ColorHtml ?? "#EEEEEE");
                                var fill = new SolidBrush(Color.FromArgb(fillOpacity, fillColor));
                                Style style = new Style()
                                {
                                    Stroke = stroke,
                                    Fill = fill,
                                };
                                Polygon polygon = new Polygon()
                                {
                                    Name = item.Name,
                                    Style = style,
                                };
                                foreach (var coord in poly.Points)
                                {
                                    var dot = new PointLatLng()
                                    {
                                        Lat = coord[0],
                                        Lng = coord[1],
                                        Height = 0
                                    };
                                    polygon.Points.Add(dot);
                                }
                                result.Polygons.Add(polygon);
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
