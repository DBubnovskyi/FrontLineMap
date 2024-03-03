using MapDataProvider.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapDataProvider.DataSourceProvoders.Contracts
{
    public interface IDataProvider
    {
        string Name { get; }
        string WebSite { get; }
        List<string> ApiUrls { get; }
        Task<MapDataCollection> GetDataAsync();
    }
}
