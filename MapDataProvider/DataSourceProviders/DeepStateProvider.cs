using MapDataProvider.DataConverters;
using MapDataProvider.DataSourceProviders.Models.DeepState;
using MapDataProvider.Helpers;
using MapDataProvider.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapDataProvider.DataSourceProviders.Providers
{
    internal class DeepStateProvider : DataProviderBase
    {
        public DeepStateProvider(DeepStateConverter converter, string name, string WebSite, params string[] apiUrls) 
            : this(converter, name, WebSite, apiUrls.ToList()){}
        public DeepStateProvider(DeepStateConverter converter, string name, string WebSite, List<string> apiUrls) 
            : base(converter, name, WebSite, apiUrls){ }

        public override Task<MapDataCollection> GetDataAsync(bool reloadData = false, int expireTime = 10)
        {
            var res = new MapDataCollection();
            try
            {
                string jsonHistory = WebLoader.Load(ApiUrls[1], WebSite);
                var history = JsonConvert.DeserializeObject<List<DeepStateHistoryModel>>(jsonHistory);
                ApiUrls[0] = ApiUrls[0].Replace("{timestamp}", history.Last().Id.ToString());
            }
            catch (Exception ex)
            {
                res.Metadata.Errors.Add(ex);
            }
            return base.GetDataAsync(reloadData, expireTime);
        }
    }
}
