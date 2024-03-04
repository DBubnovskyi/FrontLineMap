using MapDataProvider.Models;

namespace MapDataProvider.DataConverters.Contracts
{
    /// <summary>
    /// Interface that preset data converter element
    /// </summary>
    internal interface IDataConverter
    {
        /// <summary>
        /// Deserialize geoJson data to <see cref="MapDataCollection"/> object
        /// </summary>
        /// <param name="jsonInput">geoJson string provided by data source in raw form</param>
        /// <returns><see cref="MapDataCollection"/>object with setup parameters</returns>
        MapDataCollection DeserializeMapData(string jsonInput);
    }
}
