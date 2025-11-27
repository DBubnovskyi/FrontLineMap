using System;
using Newtonsoft.Json;

namespace MapDataProvider.DataSourceProviders.Models.DeepState
{
    internal class DeepStateHistoryModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("descriptionEn")]
        public string DescriptionEn { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("datetime")]
        public string Datetime { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}
