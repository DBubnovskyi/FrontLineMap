using System;
using System.IO;
using MapDataProvider.Models;
using Newtonsoft.Json;

namespace MapDataProvider.Helpers
{
    internal static class CacheHandler
    {
        internal static bool IsExpired(string providerName, int expireDays)
        {
            string cachePath = $"{FileReader.AssemblyDirectory}\\Cache\\{providerName}.json";
            if (File.Exists(cachePath))
            {
                DateTime modification = File.GetLastWriteTime(cachePath);
                if ((DateTime.Now - modification).TotalDays < expireDays)
                {
                    return false;
                }
            }
            return true;
        }

        internal static void UpdateCache(string providerName, MapDataCollection mapData)
        {
            string cacheFolder = $"{FileReader.AssemblyDirectory}\\Cache\\"; 
            string serializedStr = mapData.Serialize();
            if (!Directory.Exists(cacheFolder))
            {
                Directory.CreateDirectory(cacheFolder);
            }
            string cachePath = Path.Combine(cacheFolder, $"{providerName}.json");
            File.WriteAllText(cachePath, serializedStr);
        }
        internal static MapDataCollection LoadFromCache(string providerName)
        {
            string cachePath = $"{FileReader.AssemblyDirectory}\\Cache\\{providerName}.json";
            if (File.Exists(cachePath))
            {
                string json = File.ReadAllText(cachePath);
                var cache = JsonConvert.DeserializeObject<MapDataCollection>(json);
                cache.Metadata.ActualSource = Metadata.ActualLoadSource.LocalFile;
                cache.Metadata.LastUpdated = File.GetLastWriteTime(cachePath);
                return cache;
            }
            return null;
        }
    }
}
