using MapDataProvider.DataConverters.Contracts;
using MapDataProvider.DataSourceProviders.Contracts;
using MapDataProvider.Helpers;
using MapDataProvider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapDataProvider.DataSourceProviders.Providers
{
    internal abstract class DataProviderBase : IDataProvider
    {
        public DataProviderBase(IDataConverter converter, string name, string WebSite, params string[] apiUrls)
            : this(converter, name, WebSite, apiUrls.ToList()) { }
        public DataProviderBase(IDataConverter converter, string name, string Website, List<string> apiUrls)
        {
            Converter = converter;
            Name = name;
            WebSite = Website;
            ApiUrls = apiUrls;
        }

        public readonly IDataConverter Converter;
        public List<string> ApiUrls { get; }
        public string Name { get;}
        public string WebSite { get;}


        public virtual Task<MapDataCollection> GetDataAsync(bool reloadData = false, int expireTime = 10)
        {
            var res = new MapDataCollection();
            if (reloadData || CacheHandler.IsExpired(Name, expireTime))
            {
                try
                {
                    var json = WebLoader.Load(ApiUrls.First());
                    res = Converter.DeserializeMapData(json);
                }
                catch (Exception ex)
                {
                    res.Metadata.Errors.Add(ex);
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
        public virtual Task<MapDataCollection> GetDataAsync() => GetDataAsync(false, 10);

        public virtual Task<MapDataCollection> GetDataAsync(bool reloadData) => GetDataAsync(reloadData, 10);

        public virtual Task<MapDataCollection> GetDataAsync(int expireTime) => GetDataAsync(false, expireTime);
    }
}
