using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max_Gpu_Roi
{
    internal class Gpu
    {
        public int Id { get; set; }
        public string Name { get { return GetUserFriendlyName();  } set { SetName(value); } }
        public string ModelNumber { get; set; } // 3080/6800/580
        public string VersionPrefix { get; set; } // Cmp/gtx/rtx/rx
        public string VersionSuffix { get; set; } // xt/ti/super
        public double MSRP { get; set; }
        public double EbayPrice { get; set; }
        public double PricePaid { get; set; }
        public int Watts { get; set; }
        public int VramSize { get; set; }
        public DateTime DateReleased { get; set; }
        public string Manufacturer { get; set; }
        public bool Lhr { get; set; }
        public string EbayLink { get; set; }
        public List<Hashrate> Hashrates { get; set; }

        public Gpu()
        {
            Id = -1;
            ModelNumber = "unknown";
            VersionPrefix = "unknown";
            VersionSuffix = "unknown";
            MSRP = 0;
            EbayPrice = 0;
            PricePaid = 0;
            Watts = 0;
            VramSize = 0;
            DateReleased = DateTime.Now;
            Manufacturer = "unknown";
            Lhr = false;
            EbayLink = "unknown";
            Hashrates = new List<Hashrate>();
        }
        public Gpu(string unfilteredGpuName)
        {
            Id = -1;
            Lhr = false;
            SetName(unfilteredGpuName);
            MSRP = 0;
            EbayPrice = 0;
            PricePaid = 0;
            Watts = 0;
            DateReleased = DateTime.Now;
            Hashrates = new List<Hashrate>();
            EbayLink = "unknown";
        }

        protected void SetName(string unfilteredGpuName)
        {
            List<string> versionPrefixs = new List<string>() {
                 "quadro rtx",
                 "quadro",
                 "cmp",
                 "gtx",
                 "rtx",
                 "rx",
                 "r9",
             };
            List<string> modelNumber = new List<string>() {
                 "a6000",
                 "a30",
                 "30hx",
                 "40hx",
                 "50hx",
                 "90hx",
                 "100hx",
                 "170hx",
                 "220hx",
                 "a40",
                 "a2000",
                 "a4000",
                 "a5000",
                 "gp100",
                 "p2200",
                 "4000",
                 "8000",
                 "p102",
                 "p104",
                 "p106",
                 "t4",
                 "p40",
                 "a100",
                 "radeon",
                 "vega",
                 "5300",
                 "5500",
                 "5600",
                 "5700",
                 "6400",
                 "6500",
                 "6600",
                 "6700",
                 "6800",
                 "6900",
                 "1050",
                 "1060",
                 "1070",
                 "1080",
                 "1650",
                 "1660",
                 "2050",
                 "2060",
                 "2070",
                 "2080",
                 "3050",
                 "3060",
                 "3070",
                 "3080",
                 "3090",
                 "4050",
                 "4060",
                 "4070",
                 "4080",
                 "4090",
                 "7350",
                 "7450",
                 "7470",
                 "7510",
                 "7570",
                 "7670",
                 "7730",
                 "7750",
                 "7770",
                 "7790",
                 "7850",
                 "7870",
                 "7950",
                 "7970",
                 "7990",
                 "960",
                 "970",
                 "980",
                 "390",
                 "390x",
                 "380",
                 "380x",
                 "370",
                 "460",
                 "470",
                 "480",
                 "490",
                 "550",
                 "560",
                 "570",
                 "580",
                 "590",
                 "titan rtx",
                 "titan v",
                 "titan xp",
                 "titan x",
             };

            List<string> versionSuffixs = new List<string>() {
                 "pro vii",
                 "vii",
                 "xt",
                 "xp",
                 "ti",
                 "super",
                 "fury",
                 "56",
                 "64",
                 "100",
                 "frontier edition",
                 "pro"
             };

            List<string> nonLhr = new List<string>() {
                 "nonlhr",
                 "non-lhr",
                 "no lhr",
                 "nolhr",
                 "fhr",
                 "founders edition",
                 "fe",
                 "founders"
             };

            List<string> lhr = new List<string>() {
                 "lhr",
                 "lhrv1",
                 "lhrv2",
                 "lhr v1",
                 "lhr-v1",
                 "lhr v2",
                 "lhr-v2"
             };
            List<string> manufacterer = new List<string>() {
                 "nvidia",
                 "amd"
             };

            var gpuName = unfilteredGpuName.ToLower();

            // Get Nvidia/Amd
            int index = manufacterer.FindIndex(a => gpuName.Contains(a));
            if (index > -1)
            {
                Manufacturer = manufacterer[index];
                gpuName = gpuName.Replace(Manufacturer, ""); // remove manufacturer
            }

            // Get version prefix
            index = versionPrefixs.FindIndex(a => gpuName.Contains(a));
            if (index > -1)
            {
                VersionPrefix = index > 0 ? versionPrefixs[index] : "";
                gpuName = gpuName.Replace(VersionPrefix, ""); // remove version prefix
            }

            // Get version suffix
            index = versionSuffixs.FindIndex(a => gpuName.Contains(a));
            if (index > -1 && !unfilteredGpuName.Contains("560") && !unfilteredGpuName.Contains("6400") && gpuName != " titan v")
            {
                VersionSuffix = index > 0 ? versionSuffixs[index] : "";
                gpuName = gpuName.Replace(VersionSuffix, ""); // remove version suffix
            }

            // Get Lhr
            var specificallyNonLhr = false;
            index = nonLhr.FindIndex(a => gpuName.Contains(a));
            if (index > -1)
            {
                gpuName = gpuName.Replace(nonLhr[index], ""); // If this gpu is specifically non-lhr, remove it from the name
                specificallyNonLhr = true;
            }

            index = lhr.FindIndex(a => gpuName.Contains(a));
            if (index > -1)
            {
                gpuName = gpuName.Replace(lhr[index], ""); // If this gpu is specifically lhr, remove it from the name
                Lhr = true;
            }

            // Get Vram size if given in name
            index = gpuName.Contains("gb") ? gpuName.IndexOf("gb") : -1;
            if (index > -1)
            {
                var vram = gpuName.Substring(index - 4, 2); // get the 2 digits right before 'gb'
                gpuName = gpuName.Replace(vram + "gb", "");
                if (int.TryParse(gpuName, out var vramSize))
                    VramSize = vramSize;
            }

            gpuName = gpuName.Replace("-", ""); // remove dashes and spaces
            while (gpuName.Contains(" ")) { gpuName = gpuName.Replace(" ", ""); } // remove all spaces   

            // Get model number
            index = modelNumber.FindIndex(a => gpuName.Contains(a));
            if (index > -1)
                ModelNumber = modelNumber[index]; // Found model from list of known gpus
            else
            {
                // Whats left should be the model number         
                ModelNumber = gpuName;
            }
        }
        public bool IsLhr()
        {
            // If this gpu is a lhr/non-lhr 
            if (Manufacturer.ToLower() == "nvidia" &&
                ((int.TryParse(ModelNumber, out var modelNum) && modelNum > 3050 && modelNum != 3090))
                || ModelNumber.ToLower() == "3080 12gb")
                return true;
            return false;
        }
        public bool HasDualMiningHashrate(Hashrate hashrate)
        {
            if (hashrate.DualMineHashrate != null && hashrate.DualMineHashrate.Coin != null 
                && hashrate.DualMineHashrate.Coin.coin != null && hashrate.DualMineHashrate.HashrateSpeed > 0)
                return true;
            return false;
        }
        public Gpu CloneGpu(int newRandomId)
        {
            var newGpu = new Gpu();
            newGpu.Id = newRandomId;
            newGpu.ModelNumber = ModelNumber;
            newGpu.Manufacturer = Manufacturer;
            newGpu.Lhr = Lhr;
            newGpu.VersionPrefix = VersionPrefix;
            newGpu.VersionSuffix = VersionSuffix;
            newGpu.VramSize = VramSize;
            newGpu.Hashrates = Hashrates;
            newGpu.DateReleased = DateReleased;
            newGpu.EbayLink = EbayLink;
            newGpu.EbayPrice = EbayPrice;
            newGpu.PricePaid = PricePaid;
            newGpu.MSRP = MSRP;
            newGpu.Watts = Watts;
            return newGpu;
        }

        public string GetUserFriendlyName()
        {
            var name = VersionPrefix;
            name += ModelNumber != "" ? " " + ModelNumber : "";
            name += VersionSuffix != "" ? " " + VersionSuffix : "";
            if (Lhr)
                name += " lhr";
            return name;
        }

    }
    internal class Hashrate
    {
        public CoinInfo? Coin { get; set; }
        public double HashrateSpeed { get; set; }
        public int Watts { get; set; }
        public Hashrate? DualMineHashrate { get; set; }
        public Calculation? Calculation { get; set; }
        public Hashrate()
        {
            HashrateSpeed = 0;
            Watts = 0;
        }
    }
}
