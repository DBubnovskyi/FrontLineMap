using MapDataProvider.DataConverters.Contracts;
using MapDataProvider.DataSourceProvoders.Contracts;
using MapDataProvider.Healpers;
using MapDataProvider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapDataProvider.DataSourceProvoders.Providers
{
    internal abstract class DataProviderBase : IDataProvider
    {
        public DataProviderBase(IDataConverter converter, string name, string webSite, params string[] apiUrls)
            : this(converter, name, webSite, apiUrls.ToList()) { }
        public DataProviderBase(IDataConverter converter, string name, string webSite, List<string> apiUrls)
        {
            Converter = converter;
            Name = name;
            WebSite = webSite;
            ApiUrls = apiUrls;
        }

        public readonly IDataConverter Converter;
        public List<string> ApiUrls { get; }
        public string Name { get;}
        public string WebSite { get;}

        public virtual Task<MapDataCollection> GetDataAsync()
        {
            var res = new MapDataCollection();
            try
            {
                var json = WebLoader.Load(ApiUrls.First());
                res = Converter.ConvertMapData(json);
            }
            catch(Exception ex)
            {
                res.Errors.Add(ex);
            }
            return Task.FromResult(res);
        }
    }
}
