using MapDataProvider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProvider.DataConverters.Contracts
{
    internal interface IDataConverter
    {
        MapDataCollection ConvertMapData(string jsonInput);
    }
}
