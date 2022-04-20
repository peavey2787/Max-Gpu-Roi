using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Max_Gpu_Roi
{
    // The API is limited to 12 requests per minute.
    // Do note that the data refreshes every 5-10 minutes,
    // so there is no need to call the endpoint multiple times per minute.
    internal class MinerStat
    {
        public async Task<CoinInfo> GetCoinInfo(string coinSymbol)
        {
            var url = "https://api.minerstat.com/v2/coins?list=";
            var coinInfo = new CoinInfo();

            
            try
            {
                // Get coin info from minerstat.com as json data
                HttpClient hc = new HttpClient();
                Task<Stream> result = hc.GetStreamAsync(url + coinSymbol);
                Stream stream = await result;
                var coinInfos = await JsonSerializer.DeserializeAsync<List<CoinInfo>>(stream);
                stream.Close();

                // Minerstat returned good data
                if (coinInfos != null && coinInfos[0].price != -1)
                    return coinInfos[0];
                
                else
                {
                    // Minerstat didn't return the coin's price/info
                    // ToDo: Get price from coingecko.com
                }
            }
            catch (System.Net.WebException ex)
            {
                coinInfo.name = "Error getting coininfo! " + ex.Message;
            }

            return coinInfo;
        }

        public async Task<List<CoinInfo>> GetAllCoins()
        {
            var url = "https://api.minerstat.com/v2/coins";

            try
            {
                // Get coin info from minerstat.com as json data
                HttpClient hc = new HttpClient();
                Task<Stream> result = hc.GetStreamAsync(url);
                Stream stream = await result;
                var coins = await JsonSerializer.DeserializeAsync<List<CoinInfo>>(stream);
                stream.Close();

                // Minerstat returned good data
                if (coins != null && coins[0].price != -1)
                    return coins;

                else
                {
                    /* ToDo: 
                     Responses and error handling minerstat API use the HTTPS protocols for requests.
                     They return responses in JSON data format. On success, API will always return code 200.
                    List of error codes:
                        500 - API/System error
                        400 - Invalid parameter
                        402 - Limit reached
                        404 - Non-existent content
                     * */
                }
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.Forms.MessageBox.Show("Error getting list of coins! " + ex.Message);
            }

            return new List<CoinInfo>();
        }
    }

    internal class CoinInfo
    {
        public string id { get; set; }
        public string coin { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string algorithm { get; set; }
        public double network_hashrate { get; set; }
        public object difficulty { get; set; }
        public double reward { get; set; }
        public string reward_unit { get; set; }
        public double reward_block { get; set; }
        public double price { get; set; }
        public double volume { get; set; }
        public int updated { get; set; }
    }
}
