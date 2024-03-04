using MapDataProvider.DataConverters;
using System.Collections.Generic;
using System.Linq;

namespace MapDataProvider.DataSourceProviders.Providers
{
    internal class LiveMapProvider : DataProviderBase
    {
        public LiveMapProvider(LiveMapConverter converter, string name, string WebSite, params string[] apiUrls)
            : this(converter, name, WebSite, apiUrls.ToList()) { }
        public LiveMapProvider(LiveMapConverter converter, string name, string WebSite, List<string> apiUrls)
            : base(converter, name, WebSite, apiUrls) { }
    }
}
