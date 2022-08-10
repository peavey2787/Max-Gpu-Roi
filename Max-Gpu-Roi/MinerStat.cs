using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Max_Gpu_Roi
{
    // The API is limited to 12 requests per minute.
    // Do note that the data refreshes every 5-10 minutes,
    // so there is no need to call the endpoint multiple times per minute.
    internal class MinerStat
    {
        public async static Task<CoinInfo> GetCoinInfo(string coinSymbol)
        {
            var url = "https://api.minerstat.com/v2/coins?list=" + coinSymbol;
            var coinInfo = new CoinInfo();


            try
            {
                // Get coin info from minerstat.com as json data
                HttpClient hc = new HttpClient();
                Task<Stream> result = hc.GetStreamAsync(url);
                Stream stream = await result;
                var coinInfos = await JsonSerializer.DeserializeAsync<List<CoinInfo>>(stream);
                stream.Close();
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

        public async static Task<List<CoinInfo>>GetAllCoins()
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
                {
                    // Only get coins with prices and miners that are using the network, also don't get 2miners coins
                    var coinsWithPrices = new List<CoinInfo>();
                    foreach (var coin in coins)
                    {
                        if (coin.price > 0 && coin.volume > 10000 && !coin.name.ToLower().Contains("2miners") && coin.coin != "unknown" && coin.coin.Length > 0)
                        {
                            if (coin.algorithm == "Ethash" && coin.coin != "ETH")
                                continue; // Skip coins on eth, only get ethereum
                            else
                                coinsWithPrices.Add(coin);
                        }
                    }
                    return coinsWithPrices;
                }
                    

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

        public static async Task<List<Gpu>> GetAllGpus(List<CoinInfo> coins)
        {
            var url = "https://api.minerstat.com/v2/hardware";
            var gpus = new List<Gpu>();
            try
            {
                // Get gpu info from minerstat.com as json data
                HttpClient hc = new HttpClient();
                Task<Stream> result = hc.GetStreamAsync(url);
                Stream stream = await result;
                dynamic devices = await JsonSerializer.DeserializeAsync<List<ExpandoObject>>(stream);
                stream.Close();

                int x = 0;
                foreach(dynamic device in devices as dynamic)
                {
                    try
                    {
                        if (device.type.ToString().Length > 0 && device.type.ToString() == "gpu")
                        {
                            // Get gpu stats
                            dynamic algorithms = JsonSerializer.Deserialize<ExpandoObject>(device.algorithms.ToString());

                            var gpu = new Gpu(device.name.ToString());
                            gpu.Manufacturer = device.brand.ToString();

                            if (gpu.Manufacturer.ToLower().Contains("amd"))
                                gpu.Lhr = false;

                            gpu.Id = x;
                            x++;

                            dynamic specs = JsonSerializer.Deserialize<ExpandoObject>(device.specs.ToString());
                            if (specs != null)
                            {
                                foreach (dynamic specObj in specs as dynamic)
                                {
                                    var specName = specObj.Key;
                                    var spec = specObj.Value.ToString();


                                    if (specName.Contains("Memory Size") && spec != "Unknown")
                                    {
                                        // Get Vram size
                                        var vram = spec.ToLower().Replace(" gb", ""); // remove 'gb'
                                        gpu.VramSize = int.TryParse(vram, out int parsedVram) ? parsedVram : 0;
                                    }
                                    if ((specName.Contains("Rated Power") || specName.Contains("GPU Power") || specName == "power")
                                         || specName.Contains("Typical Board Power") && spec != "Unknown")
                                    {
                                        // Get power
                                        var power = spec.ToLower().Replace(" w", ""); // remove 'w'
                                        gpu.Watts = int.TryParse(power, out int parsedPower) ? parsedPower : 0;
                                    }
                                    if (specName.Contains("Release") && spec != "Unknown")
                                    {
                                        // Get released year
                                        var dateReleased = spec.Substring(spec.Length - 4, 4); // Just get the year
                                        var years = int.TryParse(dateReleased, out int parsedDateReleased) ? parsedDateReleased : -200;
                                        gpu.DateReleased = DateTime.Now.AddYears(-(DateTime.Now.Year - years));
                                    }
                                }
                            }

                            if(algorithms != null)
                            {
                                foreach (dynamic alg in algorithms as dynamic)
                                {
                                    var coin = alg.Key;
                                    dynamic hashrateStats = JsonSerializer.Deserialize<ExpandoObject>(alg.Value.ToString());
                                    var speed = hashrateStats.speed.ToString();
                                    var power = hashrateStats.power.ToString();

                                    var hashrate = new Hashrate();
                                    hashrate.HashrateSpeed = int.TryParse(speed, out int parsedSpeed) ? parsedSpeed : 0.0; // / 1000000 : 0.0;

                                    // If we didn't get power from above, try here
                                    if (gpu.Watts == 0)
                                    {
                                        hashrate.Watts = int.TryParse(power, out int parsedPower) ? parsedPower : 0;
                                        if (hashrate.Watts > 0)
                                            gpu.Watts = hashrate.Watts;
                                        else
                                            hashrate.Watts = gpu.Watts;
                                    }

                                    // Skip this one if there are no stats
                                    if (hashrate.HashrateSpeed == 0)
                                        continue;
                                    else
                                    {
                                        // Get coin symbol
                                        foreach (var coinInfo in coins)
                                            if (coin == coinInfo.algorithm)
                                            {
                                                hashrate.Coin = coinInfo;
                                                break;
                                            }

                                        // Add hashrate if it has good data
                                        if (hashrate.Coin != null && hashrate.Coin.coin.Length > 0)
                                            gpu.Hashrates.Add(hashrate);
                                    }
                                }
                            }

                            gpus.Add(gpu);
                        }
                    }
                    catch (System.Exception ex)
                    {    }
                }
            }
            catch (Exception ex) { }
            return gpus;
        }
    }

    internal class CoinInfo
    {
        public string? id { get; set; }
        public string? coin { get; set; }
        public string? name { get; set; }
        public string? type { get; set; }
        public string? algorithm { get; set; }
        public double? network_hashrate { get; set; }
        public object? difficulty { get; set; }
        public double? reward { get; set; }
        public string? reward_unit { get; set; }
        public double? reward_block { get; set; }
        public double? price { get; set; }
        public double? volume { get; set; }
        public int? updated { get; set; }
    }

}
