using MapDataProvider.DataConverters;
using MapDataProvider.DataSourceProviders.Contracts;
using MapDataProvider.Helpers;
using MapDataProvider.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapDataProvider.DataSourceProviders.Providers
{
    /// <summary>
    /// Data provider for loading NVG files from local file system
    /// </summary>
    internal class NvgFileProvider : IDataProvider
    {
        private readonly NvgConverter _converter;
        private string _lastLoadedFilePath;

        public NvgFileProvider(NvgConverter converter, string name, string webSite)
        {
            _converter = converter;
            Name = name;
            WebSite = webSite;
        }

        public string Name { get; }
        public string WebSite { get; }
        public List<string> ApiUrls { get; } = new List<string>();

        public Task<MapDataCollection> GetDataAsync(bool reloadData = false, int expireTime = 10)
        {
            var res = new MapDataCollection();

            // Спочатку перевіряємо чи є закешовані дані
            bool hasCachedData = !CacheHandler.IsExpired(Name, expireTime);

            // Якщо є кеш і не запитано перезавантаження - повертаємо з кешу
            if (!reloadData && hasCachedData)
            {
                res = CacheHandler.LoadFromCache(Name);
                return Task.FromResult(res);
            }

            // Відкриваємо діалог для вибору файлу
            try
            {
                // Open file dialog to select NVG file
                string filePath = SelectFile();

                if (string.IsNullOrEmpty(filePath))
                {
                    // User cancelled file selection, try to load from cache
                    if (hasCachedData)
                    {
                        res = CacheHandler.LoadFromCache(Name);
                        return Task.FromResult(res);
                    }

                    res.Metadata.Errors.Add(new Exception("File selection cancelled and no cached data available"));
                    return Task.FromResult(res);
                }

                _lastLoadedFilePath = filePath;

                // Read file content
                string xmlContent = File.ReadAllText(filePath);

                // Convert to MapDataCollection
                res = _converter.DeserializeMapData(xmlContent);
                res.Metadata.ActualSource = Metadata.ActualLoadSource.LocalFile;
                res.Metadata.LastUpdated = DateTime.Now;
                res.Name = $"NVG File: {Path.GetFileName(filePath)}";

                // Update cache
                CacheHandler.UpdateCache(Name, res);
            }
            catch (Exception ex)
            {
                res.Metadata.Errors.Add(ex);
            }

            return Task.FromResult(res);
        }

        public Task<MapDataCollection> GetDataAsync() => GetDataAsync(false, 10);

        public Task<MapDataCollection> GetDataAsync(bool reloadData) => GetDataAsync(reloadData, 10);

        public Task<MapDataCollection> GetDataAsync(int expireTime) => GetDataAsync(false, expireTime);

        /// <summary>
        /// Opens file dialog for user to select NVG file
        /// </summary>
        /// <returns>Selected file path or null if cancelled</returns>
        private string SelectFile()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select NVG File";
                openFileDialog.Filter = "NVG Files (*.nvg)|*.nvg|XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = false;

                // Set initial directory to last loaded file location if available
                if (!string.IsNullOrEmpty(_lastLoadedFilePath) && File.Exists(_lastLoadedFilePath))
                {
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(_lastLoadedFilePath);
                    openFileDialog.FileName = Path.GetFileName(_lastLoadedFilePath);
                }

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileName;
                }
            }

            return null;
        }
    }
}
