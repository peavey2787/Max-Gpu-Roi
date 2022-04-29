using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace Max_Gpu_Roi
{
    internal class NiceHash
    {
        public static async void SaveAllGpuInfoToFile(string fileName)
        {
            // Get hashrate info from NiceHash
            // only do this once or if the count is low and its defaultgpulist
            try
            {
                var gpus = await MinerStat.GetAllGpus(); // Get all gpu names
                var gpusToAdd = new List<Gpu>();
                foreach (var gpu in gpus)
                {
                    var hashes = new List<Hashrate>();
                    // AMD
                    if (gpu.Manufacturer.ToLower() == "amd")
                    {
                        if (hashes.Count == 0 && (gpu.ModelNumber == "6800" || gpu.ModelNumber == "6900"))
                        {
                            gpu.VramSize = 16;
                            hashes = await NiceHash.GetHashratesForGpu(await NiceHash.GetNiceHashUrl(gpu));
                            gpu.Hashrates.AddRange(hashes);
                            continue;
                        }
                        if (hashes.Count == 0 && (gpu.ModelNumber == "6700"))
                        {
                            gpu.VramSize = 12;
                            hashes = await NiceHash.GetHashratesForGpu(await NiceHash.GetNiceHashUrl(gpu));
                            gpu.Hashrates.AddRange(hashes);
                            continue;
                        }
                        if (hashes.Count == 0 && gpu.ModelNumber.ToLower() == "vega")
                        {
                            hashes = await NiceHash.GetHashratesForGpu(await NiceHash.GetNiceHashUrl(gpu));
                            gpu.Hashrates.AddRange(hashes);
                            continue;
                        }
                        if (hashes.Count == 0 && gpu.VersionPrefix == "rx")
                        {
                            // 8gb
                            gpu.VramSize = 8;
                            hashes = await NiceHash.GetHashratesForGpu(await NiceHash.GetNiceHashUrl(gpu));
                            gpu.Hashrates.AddRange(hashes);

                            // Try getting the lower variant of this gpu and set the vram
                            var lowerVariant = gpu.CloneGpu();
                            lowerVariant.VramSize = 4;
                            hashes = await NiceHash.GetHashratesForGpu(await NiceHash.GetNiceHashUrl(lowerVariant));
                            // Change eth to etc cause 4gb can't mine eth
                            foreach (var hash in hashes)
                            {
                                if (hash.Coin.ToLower() == "eth")
                                    hash.Coin = "etc";
                            }
                            lowerVariant.Hashrates.AddRange(hashes);
                            if (!gpusToAdd.Contains(lowerVariant))
                                gpusToAdd.Add(lowerVariant);
                            continue;
                        }

                    }

                    // NVIDIA
                    // Get lhr variant for 3070ti/3080ti
                    if (gpu.Manufacturer == "nvidia" && (gpu.ModelNumber == "3070" || gpu.ModelNumber == "3080") && gpu.VersionSuffix == "ti") //int.TryParse(gpu.ModelNumber, out var modelNum) && modelNum > 3000)
                    {
                        gpu.Lhr = true;
                        hashes = await NiceHash.GetHashratesForGpu(await NiceHash.GetNiceHashUrl(gpu));
                        continue;
                    }

                    // Get 12gb 3080
                    if (gpu.ModelNumber == "3080" && gpu.VersionSuffix != "ti" && !gpu.Lhr)
                    {
                        // Get 12gb variant
                        var lowerVariant = gpu.CloneGpu();
                        lowerVariant.VramSize = 12;
                        lowerVariant.Lhr = true;
                        hashes = await NiceHash.GetHashratesForGpu("nvidia-rtx-3080-12gb");
                        lowerVariant.Hashrates.AddRange(hashes);
                        if (!gpusToAdd.Contains(lowerVariant))
                            gpusToAdd.Add(lowerVariant);
                    }
                    // Get 12gb 2060
                    if (gpu.ModelNumber == "2060" && gpu.VersionSuffix != "super")
                    {
                        // Get 12gb variant
                        var lowerVariant = gpu.CloneGpu();
                        lowerVariant.VramSize = 12;
                        hashes = await NiceHash.GetHashratesForGpu(await NiceHash.GetNiceHashUrl(lowerVariant) + "-" + lowerVariant.VramSize + "gb");
                        lowerVariant.Hashrates.AddRange(hashes);
                        if (!gpusToAdd.Contains(lowerVariant))
                            gpusToAdd.Add(lowerVariant);
                    }
                    // If this is a 1060 try with 3gb/6gb vrams
                    if (gpu.ModelNumber == "1060")
                    {
                        gpu.VramSize = 6;
                        hashes = await NiceHash.GetHashratesForGpu(await NiceHash.GetNiceHashUrl(gpu) + "-" + gpu.VramSize + "gb");
                        gpu.Hashrates.AddRange(hashes);

                        // Try getting the lower variant of this gpu and set the vram
                        var lowerVariant = gpu.CloneGpu();
                        lowerVariant.VramSize = 3;
                        hashes = await NiceHash.GetHashratesForGpu(await NiceHash.GetNiceHashUrl(lowerVariant) + "-" + lowerVariant.VramSize + "gb");
                        lowerVariant.Hashrates.AddRange(hashes);
                        if (!gpusToAdd.Contains(lowerVariant))
                            gpusToAdd.Add(lowerVariant);
                    }

                    // Get the rest of the nvidia hashrate info
                    hashes = await NiceHash.GetHashratesForGpu(await NiceHash.GetNiceHashUrl(gpu)); // get eth/erg/rvn/cfx hashrates if available
                    gpu.Hashrates.AddRange(hashes);
                }
                gpus.AddRange(gpusToAdd);
                JsonCrud.SaveGpuList(gpus, fileName); // Write hashrate info to json file
            }
            catch (Exception ex) { }
        }
        private static string GetHashrate(string gpuUrlName)
        {
            // Attempt to get hashrates from nicehash
            var url = "https://www.nicehash.com/profitability-calculator/";
            url += gpuUrlName;

            new DriverManager().SetUpDriver(new ChromeConfig());
            var driver = new ChromeDriver();

            driver.Navigate().GoToUrl(url);
            Thread.Sleep(7000);
            var v = By.ClassName("algos");
            //var algoss = driver.FindElements(v); ToDo: find it this way

            var source = driver.PageSource;
            driver.Quit();

            if (source.IndexOf("class=\"algos\"") == -1)
            {
                var come = "get me to manually inject good data for debugging"; // No hashrate info returned from nicehash
            }

            return source;
        }
        public static async Task<List<Hashrate>> GetHashratesForGpu(string gpuUrlName)
        {
            var hashrates = new List<Hashrate>();

            var source = GetHashrate(gpuUrlName);

            var algos = new List<string>();
            algos.Add("DaggerHashimoto");
            algos.Add("Autolykos");
            algos.Add("Octopus");
            algos.Add("KAWPOW");

            var algosStart = source.IndexOf("class=\"algos\"");           

            if (algosStart != -1)
            {
                foreach (var algo in algos)
                {
                    var hashSpeed = "";
                    var power = "";
                    try
                    {
                        var algoStart = source.IndexOf(algo, algosStart) + algo.Length;

                        var watts = "text-muted power\">";
                        var wattsStart = source.IndexOf(watts, algoStart) + watts.Length;
                        var wattsEnd = source.IndexOf("W", wattsStart);
                        var algoEnd = wattsEnd;
                        power = source.Substring(wattsStart, wattsEnd - wattsStart);

                        string algoChunk = source.Substring(algoStart, algoEnd - algoStart);

                        var hash = "text-muted\">";
                        var hashStart = source.IndexOf(hash, algoStart) + hash.Length;
                        var hashEnd = source.IndexOf("<", hashStart);
                        hashSpeed = source.Substring(hashStart, hashEnd - hashStart);
                        hashSpeed = hashSpeed.Replace(" MH/s", "");

                    }
                    catch { }

                    var hashrate = new Hashrate();

                    switch (algo)
                    {
                        case "DaggerHashimoto":
                            hashrate.Algorithm = "Ethash";
                            hashrate.Coin = "Eth";
                            break;
                        case "Autolykos":
                            hashrate.Algorithm = algo;
                            hashrate.Coin = "Erg";
                            break;
                        case "Octopus":
                            hashrate.Algorithm = algo;
                            hashrate.Coin = "Cfx";
                            break;
                        case "KAWPOW":
                            hashrate.Algorithm = algo;
                            hashrate.Coin = "Rvn";
                            break;
                        default:
                            break;
                    }
                    hashrate.HashrateSpeed = double.TryParse(hashSpeed, out var hashrateDbl) ? hashrateDbl : 0; 
                    hashrate.Watts = int.TryParse(power, out var powerInt) ? powerInt : 0;

                    // Don't add duplicates
                    var addHashrate = true;
                    foreach(var hash in hashrates)
                    {
                        if(hash.Algorithm == algo)
                        { 
                            addHashrate = false; 
                            break; 
                        }
                    }
                    if(addHashrate)    
                        hashrates.Add(hashrate);
                }
            }
            return hashrates;
        }

        public static async Task<string> GetNiceHashUrl(Gpu gpu)
        {
            var url = gpu.Manufacturer.ToLower();

            url += gpu.VersionPrefix == null ? "" : "-" + gpu.VersionPrefix.ToLower();
            url += gpu.ModelNumber == null ? "" : "-" + gpu.ModelNumber.ToLower();
            url += gpu.VersionSuffix == null ? "" : "-" + gpu.VersionSuffix.ToLower();

            // Special Nvidia Cases
            if (gpu.ModelNumber == "3080" && gpu.VramSize == 12)
            { }
            else if (gpu.Lhr)
                url += "-lhr";

            // Special Amd Cases
            var version = gpu.VersionPrefix == null ? "" : gpu.VersionPrefix.ToLower();
            var model = gpu.ModelNumber.ToLower();
            if (version == "r9" || model == "radeon" || model == "vega")
                return url;

            // Most amd cards require gb
            if (gpu.Manufacturer.ToLower() == "amd" && gpu.VramSize > 0)
                url += "-" + gpu.VramSize.ToString() + "gb";

            return url;
        }
    }
}
