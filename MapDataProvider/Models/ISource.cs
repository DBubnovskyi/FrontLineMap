using System.Collections.Generic;
using Newtonsoft.Json;

namespace MapDataProvider.Models
{
    public interface ISource
    {
        /// <summary>
        /// Name of configuration of the Data Provider
        /// </summary>
        [JsonProperty("name")]
        string Name { get; }

        /// <summary>
        /// Data origin WebSite
        /// </summary>
        [JsonProperty("web-site")]
        string WebSite { get; }

        /// <summary>
        /// Url of the API for data loading
        /// </summary>
        [JsonProperty("api-url")]
        List<string> ApiUrls { get; }

        /// <summary>
        /// Load data from remote server
        /// <example>
        ///   example to call method
        ///   <code>
        ///      var result = source.GetDataAsync().Result;
        ///   </code>
        /// </example>
        /// </summary>
        /// <returns>
        ///  <remarcs>
        ///    <para>
        ///     <see cref="Task"/> which is loading data from API asynchronous
        ///    </para>
        ///    <para>
        ///     in Result returns <see cref="MapDataCollection"/> with processed data
        ///    </para>
        ///   </remarcs>
        /// </returns>
    }
}
