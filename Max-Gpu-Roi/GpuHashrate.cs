using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max_Gpu_Roi
{
    internal class GpuHashrate
    {
        public int Id { get; set; }
        public string Coin { get; set; }
        public string Algorithm { get; set; }
        public double Hashrate { get; set; }
        public int Watts { get; set; }
        public bool Lhr { get; set; }

        public GpuHashrate()
        {
            Id = -1;
            Coin = "unknown";
            Algorithm = "unknown";
            Hashrate = -1;
            Watts = -1;
            Lhr = false;
        }

    }
}
