using MapDataProvider.DataConverters;
using MapDataProvider.DataSourceProvoders.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProvider.DataSourceProviders
{
    internal class NoelreportsProvider : DataProviderBase
    {
        public NoelreportsProvider(NoelreportsConverter converter, string name, string webSite, params string[] apiUrls)
            : this(converter, name, webSite, apiUrls.ToList()) { }
        public NoelreportsProvider(NoelreportsConverter converter, string name, string webSite, List<string> apiUrls) : base(converter, name, webSite, apiUrls)
        {
        }
    }
}
