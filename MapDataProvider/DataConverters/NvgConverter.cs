using MapDataProvider.DataConverters.Contracts;
using MapDataProvider.Models;
using MapDataProvider.Models.MapElement;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace MapDataProvider.DataConverters
{
    /// <summary>
    /// Converter for NVG (NATO Vector Graphics) XML format
    /// </summary>
    internal class NvgConverter : IDataConverter
    {
        private static readonly XNamespace ns2 = "https://tide.act.nato.int/schemas/2012/10/nvg";
        private const int MaxPointsPerElement = 100000; // Збільшено для підтримки складних геометрій
        private const int SimplifyThreshold = 5000; // Спрощувати якщо більше цієї кількості точок
        private const double SimplificationTolerance = 0.0001; // Толерантність для спрощення геометрії

        public MapDataCollection DeserializeMapData(string xmlInput)
        {
            var result = new MapDataCollection
            {
                Name = "NVG File"
            };

            try
            {
                var doc = XDocument.Parse(xmlInput);

                // Parse polygons
                var polygons = doc.Descendants(ns2 + "polygon");
                foreach (var poly in polygons)
                {
                    var polygon = ParsePolygon(poly);
                    if (polygon != null)
                    {
                        result.Polygons.Add(polygon);
                    }
                }

                // Parse polylines
                var polylines = doc.Descendants(ns2 + "polyline");
                foreach (var polyline in polylines)
                {
                    var line = ParsePolyline(polyline);
                    if (line != null)
                    {
                        result.Lines.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Metadata.Errors.Add(ex);
            }

            return result;
        }

        private Polygon ParsePolygon(XElement element)
        {
            try
            {
                var points = ParsePoints(element.Attribute("points")?.Value);
                if (points == null || !points.Any())
                    return null;

                var label = element.Attribute("label")?.Value ?? "";

                Style style;
                try
                {
                    style = ParseStyle(element.Attribute("style")?.Value);
                }
                catch (OutOfMemoryException)
                {
                    // Використовуємо простий стиль при нестачі пам'яті
                    style = GetDefaultStyle();
                }

                var polygon = new Polygon
                {
                    Name = label,
                    Style = style
                };

                foreach (var point in points)
                {
                    polygon.Points.Add(point);
                }

                return polygon;
            }
            catch (OutOfMemoryException)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }

        private Line ParsePolyline(XElement element)
        {
            try
            {
                var points = ParsePoints(element.Attribute("points")?.Value);
                if (points == null || !points.Any())
                    return null;

                var label = element.Attribute("label")?.Value ?? "";

                Style style;
                try
                {
                    style = ParseStyle(element.Attribute("style")?.Value);
                }
                catch (OutOfMemoryException)
                {
                    // Використовуємо простий стиль при нестачі пам'яті
                    style = GetDefaultStyle();
                }

                var line = new Line
                {
                    Name = label,
                    Style = style
                };

                foreach (var point in points)
                {
                    line.Points.Add(point);
                }

                return line;
            }
            catch (OutOfMemoryException)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Конвертує polyline в polygon для сумісності з FrontLineDataExporter
        /// Лінія перетворюється в полігон шляхом дублювання точок у зворотному порядку
        /// </summary>
        private Polygon ConvertPolylineToPolygon(XElement element)
        {
            try
            {
                var points = ParsePoints(element.Attribute("points")?.Value);
                if (points == null || !points.Any())
                    return null;

                var label = element.Attribute("label")?.Value ?? "";

                Style style;
                try
                {
                    style = ParseStyle(element.Attribute("style")?.Value);
                }
                catch (OutOfMemoryException)
                {
                    // Використовуємо простий стиль при нестачі пам'яті
                    style = GetDefaultStyle();
                }

                var polygon = new Polygon
                {
                    Name = label,
                    Style = style
                };

                // Додаємо точки в прямому порядку
                foreach (var point in points)
                {
                    polygon.Points.Add(point);
                }

                // Додаємо точки в зворотному порядку (крім останньої, щоб не дублювати)
                for (int i = points.Count - 2; i >= 0; i--)
                {
                    polygon.Points.Add(points[i]);
                }

                return polygon;
            }
            catch (OutOfMemoryException)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }

        private List<PointLatLng> ParsePoints(string pointsString)
        {
            if (string.IsNullOrWhiteSpace(pointsString))
                return new List<PointLatLng>();

            try
            {
                var pairs = pointsString.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Якщо точок дуже багато, використовуємо sampling
                bool useSampling = pairs.Length > MaxPointsPerElement;
                int step = useSampling ? Math.Max(1, pairs.Length / MaxPointsPerElement) : 1;

                var result = new List<PointLatLng>(Math.Min(pairs.Length / step, MaxPointsPerElement));

                for (int i = 0; i < pairs.Length && result.Count < MaxPointsPerElement; i += step)
                {
                    var coords = pairs[i].Split(',');
                    if (coords.Length >= 2)
                    {
                        if (double.TryParse(coords[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double lng) &&
                            double.TryParse(coords[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double lat))
                        {
                            result.Add(new PointLatLng
                            {
                                Lng = lng,
                                Lat = lat,
                                Height = 0
                            });
                        }
                    }
                }

                // Завжди додаємо останню точку для замкнутих контурів
                if (pairs.Length > 1 && result.Count > 0)
                {
                    var lastCoords = pairs[pairs.Length - 1].Split(',');
                    if (lastCoords.Length >= 2 &&
                        double.TryParse(lastCoords[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double lastLng) &&
                        double.TryParse(lastCoords[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double lastLat))
                    {
                        var lastPoint = new PointLatLng { Lng = lastLng, Lat = lastLat, Height = 0 };
                        // Додаємо тільки якщо це не дублікат
                        if (result[result.Count - 1].Lng != lastLng || result[result.Count - 1].Lat != lastLat)
                        {
                            result.Add(lastPoint);
                        }
                    }
                }

                // Додаткове спрощення якщо все ще забагато точок
                if (result.Count > SimplifyThreshold)
                {
                    result = SimplifyPoints(result, SimplificationTolerance);
                }

                return result;
            }
            catch (OutOfMemoryException)
            {
                // Повертаємо порожній список при нестачі пам'яті
                return new List<PointLatLng>();
            }
            catch
            {
                return new List<PointLatLng>();
            }
        }

        /// <summary>
        /// Спрощує геометрію використовуючи алгоритм Douglas-Peucker
        /// </summary>
        private List<PointLatLng> SimplifyPoints(List<PointLatLng> points, double tolerance)
        {
            if (points == null || points.Count < 3)
                return points ?? new List<PointLatLng>();

            try
            {
                var simplified = DouglasPeucker(points, tolerance);

                // Якщо спрощення не дало результату, повертаємо оригінальні точки
                if (simplified == null || simplified.Count < 2)
                    return points;

                return simplified;
            }
            catch (Exception)
            {
                // Якщо алгоритм спрощення не спрацював, повертаємо оригінальні точки
                return points;
            }
        }

        /// <summary>
        /// Алгоритм Douglas-Peucker для спрощення ліній (ітеративна версія для уникнення переповнення стеку)
        /// </summary>
        private List<PointLatLng> DouglasPeucker(List<PointLatLng> points, double tolerance)
        {
            if (points == null || points.Count < 3)
                return new List<PointLatLng>(points ?? new List<PointLatLng>());

            // Використовуємо масив для позначення важливих точок
            bool[] keepPoints = new bool[points.Count];
            keepPoints[0] = true;
            keepPoints[points.Count - 1] = true;

            // Ітеративна версія зі стеком для уникнення глибокої рекурсії
            var stack = new Stack<Tuple<int, int>>();
            stack.Push(Tuple.Create(0, points.Count - 1));

            int iterations = 0;
            const int maxIterations = 100000; // Захист від безкінечного циклу

            while (stack.Count > 0 && iterations < maxIterations)
            {
                iterations++;
                var segment = stack.Pop();
                int startIndex = segment.Item1;
                int endIndex = segment.Item2;

                if (endIndex - startIndex <= 1)
                    continue;

                // Знаходимо точку з максимальною відстанню
                double maxDistance = 0;
                int maxIndex = startIndex;

                for (int i = startIndex + 1; i < endIndex; i++)
                {
                    double distance = PerpendicularDistance(points[i], points[startIndex], points[endIndex]);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        maxIndex = i;
                    }
                }

                // Якщо відстань достатньо велика, зберігаємо точку і розбиваємо сегмент
                if (maxDistance > tolerance)
                {
                    keepPoints[maxIndex] = true;
                    stack.Push(Tuple.Create(startIndex, maxIndex));
                    stack.Push(Tuple.Create(maxIndex, endIndex));
                }
            }

            // Збираємо результат
            var result = new List<PointLatLng>();
            for (int i = 0; i < points.Count; i++)
            {
                if (keepPoints[i])
                {
                    result.Add(points[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Обчислює перпендикулярну відстань від точки до лінії
        /// </summary>
        private double PerpendicularDistance(PointLatLng point, PointLatLng lineStart, PointLatLng lineEnd)
        {
            double dx = lineEnd.Lng - lineStart.Lng;
            double dy = lineEnd.Lat - lineStart.Lat;

            // Нормалізуємо
            double mag = Math.Sqrt(dx * dx + dy * dy);
            if (mag > 0.0)
            {
                dx /= mag;
                dy /= mag;
            }

            double pvx = point.Lng - lineStart.Lng;
            double pvy = point.Lat - lineStart.Lat;

            // Отримуємо скалярний добуток
            double pvdot = dx * pvx + dy * pvy;

            // Масштабуємо лінійний відрізок до точки проекції
            double dsx = pvdot * dx;
            double dsy = pvdot * dy;

            // Отримуємо вектор від точки до проекції
            double ax = pvx - dsx;
            double ay = pvy - dsy;

            return Math.Sqrt(ax * ax + ay * ay);
        }

        private Style ParseStyle(string styleString)
        {
            try
            {
                // Default style
                var fillColor = Color.FromArgb(64, Color.Yellow); // Semi-transparent yellow
                var strokeColor = Color.FromArgb(255, Color.Red);
                var strokeWidth = 2f;

                if (!string.IsNullOrWhiteSpace(styleString))
                {
                    var styles = styleString.Split(';')
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s.Split(':'))
                        .Where(parts => parts.Length == 2)
                        .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());

                    // Parse fill color
                    if (styles.ContainsKey("fill"))
                    {
                        try
                        {
                            var fillColorHtml = styles["fill"];
                            var color = ColorTranslator.FromHtml(fillColorHtml);

                            // Parse fill-opacity
                            float opacity = 0.25f;
                            if (styles.ContainsKey("fill-opacity") &&
                                float.TryParse(styles["fill-opacity"], NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedOpacity))
                            {
                                opacity = parsedOpacity;
                            }

                            fillColor = Color.FromArgb((int)(opacity * 255), color);
                        }
                        catch
                        {
                            // Use default fill color
                        }
                    }

                    // Parse stroke color
                    if (styles.ContainsKey("stroke"))
                    {
                        try
                        {
                            strokeColor = ColorTranslator.FromHtml(styles["stroke"]);
                        }
                        catch
                        {
                            // Use default stroke color
                        }
                    }

                    // Parse stroke width
                    if (styles.ContainsKey("stroke-width") &&
                        float.TryParse(styles["stroke-width"], NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedWidth))
                    {
                        strokeWidth = parsedWidth;
                    }
                }

                return new Style
                {
                    Fill = new SolidBrush(fillColor),
                    Stroke = new Pen(strokeColor, strokeWidth) { DashPattern = new float[] { 1 } }
                };
            }
            catch (OutOfMemoryException)
            {
                // Повертаємо простий стиль при OutOfMemory
                return GetDefaultStyle();
            }
            catch
            {
                // Повертаємо простий стиль при будь-яких інших помилках
                return GetDefaultStyle();
            }
        }

        /// <summary>
        /// Повертає стиль за замовчуванням
        /// </summary>
        private Style GetDefaultStyle()
        {
            var fillColor = Color.FromArgb(64, Color.Yellow);
            var strokeColor = Color.FromArgb(255, Color.Red);

            return new Style
            {
                Fill = new SolidBrush(fillColor),
                Stroke = new Pen(strokeColor, 2f) { DashPattern = new float[] { 1 } }
            };
        }
    }
}
