using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl; 

namespace Max_Gpu_Roi
{
    // The API is limited to 12 requests per minute.
    // Do note that the data refreshes every 5-10 minutes,
    // so there is no need to call the endpoint multiple times per minute.

    internal class MinerStat
    {
        public static CoinInfo GetCoinInfo(string coinSymbol)
        {
            var url = "https://api.minerstat.com/v2/coins?list=" + coinSymbol;            
            var coinInfo = new CoinInfo();


            try
            {
                // Get coin info from minerstat.com as json data
                HttpClient client = new HttpClient();
                var response = client.GetAsync(url).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;
                var coinInfos = JsonSerializer.Deserialize<List<CoinInfo>>(responseString);

                // Minerstat returned good data
                if (coinInfos != null && coinInfos.Count > 0 && coinInfos[0].price != -1)
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

        public static async Task<List<CoinInfo>> GetAllCoins()
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

        public static async Task<List<Gpu>> GetAllGpus()
        {
            var url = "https://api.minerstat.com/v2/hardware";
            var gpus = new List<Gpu>();


            // Get coin info from minerstat.com as json data
            HttpClient hc = new HttpClient();
            Task<Stream> result = hc.GetStreamAsync(url);
            Stream stream = await result;
            var devices = await JsonSerializer.DeserializeAsync<List<MinerStatHardware.Root>>(stream);
            stream.Close();

            int x = 0;
            foreach (MinerStatHardware.Root device in devices)
            {
                // If this device is a gpu with eth hashrate
                if (device.type == "gpu")
                {
                    // Get a list of just gpu names
                    var gpu = new Gpu();

                    gpu = new Gpu(device.name);

                    gpu.Id = x;
                    x++;

                    // Attempt to get their hashrate info for eth (its not very accuarate or complete)
                    /*
                    if (device.algorithms != null && device.algorithms.Ethash != null)
                    {
                        var hashrate = new Hashrate();
                        hashrate.Algorithm = "Ethash";
                        hashrate.Coin = "Eth";
                        hashrate.HashrateSpeed = double.TryParse(device.algorithms.Ethash.speed.ToString(), out var speed) ? speed / 1000000 : 0;
                        hashrate.Watts = int.TryParse(device.algorithms.Ethash.power.ToString(), out var power) ? power : 0;
                        gpu.Hashrates.Add(hashrate);
                    }*/
                    gpus.Add(gpu);
                }
            }


            return gpus;
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
