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
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Version { get; set; }
        public double MSRP { get; set; }
        public double EbayPrice { get; set; }
        public double PricePaid { get; set; }
        public int Watts { get; set; }
        public int VramSize { get; set; }
        public DateTime DateReleased { get; set; }
        public string AmdOrNvidia { get; set; }

        public Gpu()
        {
            Id = -1;
            Name = "unknown";
            Manufacturer = "unknown";
            Version = "unknown";
            MSRP = -1;
            EbayPrice = -1;
            PricePaid = -1;
            Watts = -1;
            VramSize = -1;
            DateReleased = DateTime.Now;
            AmdOrNvidia = "unknown";
        }
    }
}
