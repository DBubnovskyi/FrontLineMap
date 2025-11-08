using System;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
using MapDataProvider.Models.MapElement;
using MapDataProvider.DataConverters;
using MapDataProvider.Helpers;
using MapDataProvider.Models;

namespace MapDataProvider.DataSourceProviders.Providers
{
    internal class CyberBoroshnoProvider : DataProviderBase
    {
        public CyberBoroshnoProvider(CyberBoroshnoConverter converter, string name, string WebSite, params string[] apiUrls)
            : this(converter, name, WebSite, apiUrls.ToList()) { }
        public CyberBoroshnoProvider(CyberBoroshnoConverter converter, string name, string WebSite, List<string> apiUrls) 
            : base(converter, name, WebSite, apiUrls){}

        public override Task<MapDataCollection> GetDataAsync(bool reloadData = false, int expireTime = 10)
        {
            var res = new MapDataCollection();
            if (reloadData || CacheHandler.IsExpired(Name, expireTime))
            {
                foreach (var url in ApiUrls)
                {
                    try
                    {
                        var json = WebLoader.Load(AddTimeToUrl(url), WebSite);
                        var data = Converter.DeserializeMapData(json);

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
                        var stroke = new Pen(Color.FromArgb(160, color), 1) { DashPattern = new float[] { 10 } };

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
                    catch (Exception ex)
                    {
                        res.Metadata.Errors.Add(ex);
                    }
                }
                res.Metadata.ActualSource = Metadata.ActualLoadSource.Api;
                res.Metadata.LastUpdated = DateTime.Now;
                CacheHandler.UpdateCache(Name, res);
            }
            else
            {
                res = CacheHandler.LoadFromCache(Name);
            }

            return Task.FromResult(res);
        }
        public override Task<MapDataCollection> GetDataAsync() => GetDataAsync(false, 10);

        public override Task<MapDataCollection> GetDataAsync(bool reloadData) => GetDataAsync(reloadData, 10);

        public override Task<MapDataCollection> GetDataAsync(int expireTime) => GetDataAsync(false, expireTime);

        private string AddTimeToUrl(string url) => url + DateTime.Now.ToString("dd.MM.yyyy");
    }
}
