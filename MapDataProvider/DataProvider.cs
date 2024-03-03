using System.Collections.Generic;
using MapDataProvider.DataSourceProvoders.Contracts;
using MapDataProvider.DataConverters;
using MapDataProvider.DataSourceProvoders.Providers;
using MapDataProvider.Healpers;
using MapDataProvider.DataSourceProviders;

namespace MapDataProvider
{
    public class DataProvider
    {
        public DataProvider() {
            LoadSources();
        }
        private readonly List<IDataProvider> _providers = new List<IDataProvider>();

        private void LoadSources()
        {
            var sources = FileReader.ReadConfig().Sources;
            foreach (var item in sources)
            {
                switch (item.Name)
                {
                    case "DeepState":
                        _providers.Add(new DeepStateProvider(new DeepStateConverter(), item.Name, item.Website, item.ApiUrl));
                        break;
                    case "SaveEcoBot":
                        _providers.Add(new SaveEcoBotProvider(new SaveEcoBotConverter(), item.Name, item.Website, item.ApiUrl));
                        break;
                    case "CyberBoroshno":
                        _providers.Add(new CyberBoroshnoProvider(new CyberBoroshnoConverter(), item.Name, item.Website, item.ApiUrl));
                        break;
                    case "LiveMap":
                        _providers.Add(new LiveMapProvider(new LiveMapConverter(), item.Name, item.Website, item.ApiUrl));
                        break;
                    case "Noelreports":
                        _providers.Add(new NoelreportsProvider(new NoelreportsConverter(), item.Name, item.Website, item.ApiUrl));
                        break;
                    default: break;
                }
            }
        }

        public List<IDataProvider> Providers => _providers;
    }
}
