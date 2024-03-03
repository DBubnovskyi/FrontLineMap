using MapDataProvider.Models.MapElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProvider.Healpers
{
    internal class WebLoader
    {
        private readonly static string _userAgent = 
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like " +
            "Gecko) Chrome/15.0.874.121 Safari/535.2";
        public static string Load(string url)
        {
            return LoadAsync(url).Result;
        }

        public async static Task<string> LoadAsync(string url)
        {
            string responseInfo;
            using  (var client = new HttpClient()) {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(_userAgent);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url),
                    
                };
                var response = await client.SendAsync(request).ConfigureAwait(false);
                responseInfo = await response.Content.ReadAsStringAsync();
            };
            return responseInfo;
        }
    }
}
