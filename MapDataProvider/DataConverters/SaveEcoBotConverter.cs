using MapDataProvider.DataConverters.Contracts;
using MapDataProvider.DataSource;
using MapDataProvider.Models;
using MapDataProvider.Models.MapElement;
using System.Collections.Generic;
using System.Drawing;

namespace MapDataProvider.DataConverters
{
    internal class SaveEcoBotConverter : IDataConverter
    {
        public MapDataCollection ConvertMapData(string jsonInput)
        {
            var data = SaveEcoBotModel.Deserialize(jsonInput);
            var result = new MapDataCollection();
            result.Name = nameof(SaveEcoBotConverter);

            List<SaveEcoBotModel.Element> elements = new List<SaveEcoBotModel.Element>
            {
                data.Polygons.AssessedRussianAdvance,
                data.Polygons.AssessedRussianControl,
                data.Polygons.ClaimedRussianControl,
                data.Polygons.ClaimedUkrainianCounteroffensives,
                data.Polygons.ReportedUkrainianPartisanWarfare,
            };
            foreach (var itemTop in elements)
            {
                int fillOpacity = (int)(255.0 * itemTop.Style.FillOpacity ?? 1);
                Color fillColor = ColorTranslator.FromHtml(itemTop.Style.FillColor);
                var fill = new SolidBrush(Color.FromArgb(fillOpacity, fillColor));

                int strokeOpacity = (int)(255.0 * itemTop.Style.Opacity ?? 1);
                Color strokeColor = ColorTranslator.FromHtml(itemTop.Style.Color);
                float strokeWidth = (float)(itemTop.Style.Weight ?? 1);
                var stroke = new Pen(Color.FromArgb(strokeOpacity, strokeColor), strokeWidth)
                {
                    DashPattern = itemTop.Style.DashArray
                };

                Style style = new Style()
                {
                    Fill = fill,
                    Stroke = stroke
                };

                foreach (var item in itemTop.Polygon.Features)
                {

                    if (item.Geometry == null || item.Geometry.Coordinates == null)
                    {
                        continue;
                    }
                    foreach (var coordSeV2 in item.Geometry.Coordinates)
                    {
                        Polygon polygon = new Polygon()
                        {
                            Name = data.Polygons.AssessedRussianAdvance.Key,
                            Style = style,
                        };
                        foreach (var coordSeV1 in coordSeV2)
                        {
                            var point = new PointLatLng()
                            {
                                Lng = coordSeV1[0],
                                Lat = coordSeV1[1],
                                Height = 0
                            };
                            polygon.Points.Add(point);
                        }
                        result.Polygons.Add(polygon);
                    }
                }
            }
            return result;
        }
    }
}
