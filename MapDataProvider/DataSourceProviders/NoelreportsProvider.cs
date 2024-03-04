using MapDataProvider.DataConverters;
using MapDataProvider.DataSourceProviders.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProvider.DataSourceProviders
{
    internal class NoelreportsProvider : DataProviderBase
    {
        public NoelreportsProvider(NoelreportsConverter converter, string name, string WebSite, params string[] apiUrls)
            : this(converter, name, WebSite, apiUrls.ToList()) { }
        public NoelreportsProvider(NoelreportsConverter converter, string name, string WebSite, List<string> apiUrls) 
            : base(converter, name, WebSite, apiUrls) {}
    }
}
