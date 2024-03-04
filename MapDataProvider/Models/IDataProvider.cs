using MapDataProvider.Models;
using System;
using System.Threading.Tasks;

namespace MapDataProvider.DataSourceProviders.Contracts
{
    /// <summary>
    /// Interface of data provider type
    /// </summary>
    public interface IDataProvider : ISource
    {
        /// <summary>
        /// Load data from Api or from local cache file
        /// </summary>
        /// <param name="reloadData">reload data from Api and updating cache file ignoring data expireTime</param>
        /// <param name="expireTime">
        /// reload data from Api and updating cache file if <see cref="MapDataCollection.Metadata"/> param 
        /// <see cref="Metadata.LastUpdated"/> is more days than value
        /// </param>
        /// <returns></returns>
        Task<MapDataCollection> GetDataAsync(bool reloadData = false, int expireTime = 10);
        Task<MapDataCollection> GetDataAsync(bool reloadData);
        Task<MapDataCollection> GetDataAsync(int expireTime);
        Task<MapDataCollection> GetDataAsync();
    }
}
