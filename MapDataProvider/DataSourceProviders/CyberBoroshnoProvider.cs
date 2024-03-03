using MapDataProvider.DataConverters;
using MapDataProvider.Healpers;
using MapDataProvider.Models;
using System;
using System.Threading.Tasks;
using System.Drawing;
using MapDataProvider.Models.MapElement;
using System.Collections.Generic;
using System.Linq;

namespace MapDataProvider.DataSourceProvoders.Providers
{
    internal class CyberBoroshnoProvider : DataProviderBase
    {
        public CyberBoroshnoProvider(CyberBoroshnoConverter converter, string name, string webSite, params string[] apiUrls)
            : this(converter, name, webSite, apiUrls.ToList()) { }
        public CyberBoroshnoProvider(CyberBoroshnoConverter converter, string name, string webSite, List<string> apiUrls) : base(converter, name, webSite, apiUrls)
        {
        }

        public override Task<MapDataCollection> GetDataAsync()
        {
            var res = new MapDataCollection();

            foreach(var url in ApiUrls)
            {
                try
                {
                    var json = WebLoader.Load(AddTimeToUrl(url));
                    var data = Converter.ConvertMapData(json);

                    Color color = ColorTranslator.FromHtml("#ff5252");
                    if (url.Contains("old"))
                    {
                        color = ColorTranslator.FromHtml("#cc2222");
                    }
                    else if (url.Contains("liberated"))
                    {
                        color = ColorTranslator.FromHtml("#00cc00");
                    }
                    else if (url.Contains("confirmed"))
                    {
                        color = ColorTranslator.FromHtml("#555555");
                    }
                    var fill = new SolidBrush(Color.FromArgb(90, color));
                    var stroke = new Pen(Color.FromArgb(160, color), 1);

                    Style style = new Style()
                    {
                        Fill = fill,
                        Stroke = stroke
                    };

                    data.Polygons.ForEach(x =>
                    {
                        x.Style = style;
                    });

                    res.Polygons.AddRange(data.Polygons);
                }
                catch(Exception ex)
                {
                    res.Errors.Add(ex);
                }
            }

            return Task.FromResult(res);
        }

        private string AddTimeToUrl(string url) => url + DateTime.Now.ToString("dd.MM.yyyy");
    }
}
