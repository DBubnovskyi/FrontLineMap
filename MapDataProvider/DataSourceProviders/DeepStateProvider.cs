using MapDataProvider.DataConverters;
using System.Collections.Generic;
using System.Linq;

namespace MapDataProvider.DataSourceProvoders.Providers
{
    internal class DeepStateProvider : DataProviderBase
    {
        public DeepStateProvider(DeepStateConverter converter, string name, string webSite, params string[] apiUrls) 
            : this(converter, name, webSite, apiUrls.ToList()){}
        public DeepStateProvider(DeepStateConverter converter, string name, string webSite, List<string> apiUrls) : base(converter, name, webSite, apiUrls)
        {
        }
    }
}
