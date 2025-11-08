using System.Collections.Generic;
using MapDataProvider.DataSourceProviders.Contracts;
using MapDataProvider.DataSourceProviders.Providers;
using MapDataProvider.DataSourceProviders;
using MapDataProvider.DataConverters;
using MapDataProvider.Helpers;

namespace MapDataProvider
{
    /// <summary>
    /// Main data provider class
    /// <remarks>
    ///   <para>
    ///      Initializes the data providers from the configuration file.
    ///   </para>
    /// </remarks>
    /// </summary>
    public class DataProvider
    {
        /// <summary>
        /// Create instance of the <see cref="DataProvider"/> type and load data providers from config file
        /// <remarks>
        ///   <para>
        ///      Populate <see cref="Providers"/> List depending on config file settings
        ///   </para>
        /// </remarks>
        /// </summary>
        public DataProvider()
        {
            LoadSources();
        }
        private List<IDataProvider> _providers = new List<IDataProvider>();

        /// <summary>
        /// Load Data Providers from config file 
        /// <remarks>
        ///   <para>
        ///      Populate <see cref="Providers"/> List depending on config file settings
        ///   </para>
        /// </remarks>
        /// </summary>
        private void LoadSources()
        {
            var config = FileReader.ReadConfig();
            if (config != null)
            {
                foreach (var item in config.Sources)
                {
                    switch (item.Name)
                    {
                        case "DeepState":
                            _providers.Add(new DeepStateProvider(new DeepStateConverter(), item.Name, item.WebSite, item.ApiUrls));
                            break;
                        case "SaveEcoBot":
                            _providers.Add(new SaveEcoBotProvider(new SaveEcoBotConverter(), item.Name, item.WebSite, item.ApiUrls));
                            break;

                        /*
                        // Сайт більше не існує
                        case "CyberBoroshno":
                            _providers.Add(new CyberBoroshnoProvider(new CyberBoroshnoConverter(), item.Name, item.WebSite, item.ApiUrls));
                            break;
                        */
                        /*
                        // Дані не оновлюються
                        case "LiveMap":
                            _providers.Add(new LiveMapProvider(new LiveMapConverter(), item.Name, item.WebSite, item.ApiUrls));
                            break;
                        */
                        case "Noelreports":
                            _providers.Add(new NoelreportsProvider(new NoelreportsConverter(), item.Name, item.WebSite, item.ApiUrls));
                            break;
                        default: break;
                    }
                }
            }
        }

        /// <summary>
        /// List list of <see cref="IDataProvider"/> with setup parameters for providing data for map
        /// <remarks>
        ///   <para>
        ///      Contain all setting needed for for loading data from source
        ///   </para>
        /// </remarks>
        /// </summary>
        public List<IDataProvider> Providers => _providers;

        /// <summary>
        /// Clears <see cref="Providers"/> List and load providers from config file
        /// </summary>
        public void ReloadProviders()
        {
            _providers = new List<IDataProvider>();
            LoadSources();
        }
    }
}
