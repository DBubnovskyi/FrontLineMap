using MapDataProvider.Models;
using System.IO;
using System.Reflection;
using System;
using Newtonsoft.Json;

namespace MapDataProvider.Healpers
{
    internal static class FileReader
    {
        private static string _configFile = "sourceCofig.json";
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
            string json = File.ReadAllText($"{AssemblyDirectory}/{_configFile}");
            return JsonConvert.DeserializeObject<Config>(json);
        }
    }
}
