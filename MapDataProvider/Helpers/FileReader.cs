using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using MapDataProvider.Models;

namespace MapDataProvider.Helpers
{
    internal static class FileReader
    {
        private static readonly string _configFile = "source.config.json";
        private static string _assemblyDirectoryBuf;
        internal static string AssemblyDirectory
        {
            get
            {
                if (_assemblyDirectoryBuf == null)
                {
                    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    string path = Uri.UnescapeDataString(uri.Path);
                    _assemblyDirectoryBuf = Path.GetDirectoryName(path);
                }
                return _assemblyDirectoryBuf;
            }
        }

        internal static Config ReadConfig()
        {
            string configFile = $"{AssemblyDirectory}/{_configFile}";
            if (File.Exists(configFile))
            {
                string json = File.ReadAllText(configFile);
                return JsonConvert.DeserializeObject<Config>(json);
            }
            return null;
        }
    }
}
