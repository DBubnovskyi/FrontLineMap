using System;
using System.Collections.Generic;

namespace MapDataProvider.Models
{

    /// <summary>
    /// Information about errors and data source
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Represents wey of data load source
        /// </summary>
        public enum ActualLoadSource {
            None,
            LocalFile,
            Api
        }

        /// <summary>
        /// Time when last time cache file was updated, null if no cache file saved
        /// </summary>
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// Show if data where loaded from Api or from local file
        /// </summary>
        public ActualLoadSource ActualSource { get; set; } = ActualLoadSource.None;

        /// <summary>
        /// List of errors that occurred during data loading
        /// </summary>
        public List<Exception> Errors { get; set; } = new List<Exception>();
    }
}
