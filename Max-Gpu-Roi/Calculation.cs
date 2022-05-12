using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max_Gpu_Roi
{
    internal class Calculation
    {
        public double CryptoRewards { get; set; }
        public double CryptoProfits { get; set; }
        public double UsdRewards { get; set; }
        public double UsdProfits { get; set; }
        public double ROI { get; set; }
        public double CostPerMhs { get; set; }
        public double UsdElectricityCost { get; set; }
        public double CryptoElectricityCost { get; set; }
        public double UsdPoolMinerFeeCost { get; set; }
        public double CryptoPoolMinerFeeCost { get; set; }
        public double GpuCosts { get; set; }
        public double GpuHash { get; set; }
        public double Efficiency { get; set; }
        public Calculation DualMineCalculation { get; set; }


        public static Calculation Calculate(double gpuCost, double hashrate, int watts, CoinInfo coin, double electricityRate, double poolMinerFee, double coinPrice = 0.0)
        {
            // Do nothing if no coin is given to perform calculations with
            if (coin == null || coin == new CoinInfo())
                return new Calculation();

            // Using hodl price
            if (coinPrice > 0)
                return PerformCalculation(gpuCost, hashrate, watts, coin, electricityRate, poolMinerFee, coinPrice);

            // Using real time price
            else
                return PerformCalculation(gpuCost, hashrate, watts, coin, electricityRate, poolMinerFee);
        }

        public static Calculation AddCalculations(Calculation calc1, Calculation calc2)
        {
            var total = new Calculation();
            total.UsdElectricityCost = calc1.UsdElectricityCost + calc2.UsdElectricityCost;
            total.UsdPoolMinerFeeCost = calc1.UsdPoolMinerFeeCost + calc2.UsdPoolMinerFeeCost;           
            total.UsdProfits = calc1.UsdProfits + calc2.UsdProfits;
            total.Efficiency = calc1.Efficiency + calc2.Efficiency;
            total.UsdRewards = calc1.UsdRewards + calc2.UsdRewards;
            total.ROI = calc1.GpuCosts / ((calc1.UsdProfits + calc2.UsdProfits) * 30);
            return total;
        }

        private static Calculation PerformCalculation(double gpuCost, double hashrate, int watts, CoinInfo coin, double electricityRate, double poolMinerFee, double coinPrice = 0.0)
        {
            var reward = new Calculation();

            // Set coin price
            var price = coin.price;
            if (coinPrice > 0.0)
                price = coinPrice;

            // Crypto rewards
            var cryptoRewardsCalculated = (coin.reward * hashrate) * 24;
            reward.CryptoRewards = cryptoRewardsCalculated;

            // Usd rewards
            var usdRewardsCalculated = cryptoRewardsCalculated * price;
            reward.UsdRewards = usdRewardsCalculated;

            // Usd Profits 
            var poolMinerFeeCost = usdRewardsCalculated * poolMinerFee; // Get pool/miner fee
            var electrictyCosts = ((watts * 0.001) * 24) * electricityRate; // Calculate electricity costs
            usdRewardsCalculated -= poolMinerFeeCost; // Subtract pool/miner fee
            usdRewardsCalculated -= electrictyCosts; // Subtract electric costs
            reward.UsdElectricityCost = electrictyCosts;
            reward.UsdPoolMinerFeeCost = poolMinerFeeCost;
            reward.UsdProfits = usdRewardsCalculated;

            // Crypto profits
            var cryptoPoolMinerFeeCost = cryptoRewardsCalculated * poolMinerFee; // Get pool/miner fee
            var cryptoElecCosts = electrictyCosts / price; // Calculate electricity costs
            cryptoRewardsCalculated -= cryptoPoolMinerFeeCost; // Subtract pool/miner fee
            cryptoRewardsCalculated -= -cryptoElecCosts; // Subtract electric costs
            reward.CryptoElectricityCost = cryptoElecCosts;
            reward.CryptoPoolMinerFeeCost = cryptoPoolMinerFeeCost;
            reward.CryptoProfits = cryptoRewardsCalculated;

            // Efficiency mhs/watts
            var hashSize = GetHashrateSize(hashrate);
            reward.Efficiency = (hashrate / hashSize) / watts;

            // Roi
            reward.ROI = gpuCost / (reward.UsdProfits * 30);

            // Usd per mhs
            if (hashrate > 0)
            {
                var costPerUsd = (decimal)(gpuCost / hashrate);
                costPerUsd = Math.Round(costPerUsd, 6);
                reward.CostPerMhs = (double)costPerUsd;
            }

            reward.GpuCosts = gpuCost;
            reward.GpuHash = hashrate;

            return reward;
        }
        public static double GetHashrateSize(double hashrate)
        {
            var zettaHash = hashrate / 1e+21;
            var exaHash = hashrate / 1000000000000000000;
            var petaHash = hashrate / 1000000000000000;
            var teraHash = hashrate / 1000000000000;
            var gigaHash = hashrate / 1000000000;
            var megaHash = hashrate / 1000000;
            var kiloHash = hashrate / 1000;

            if (zettaHash > 1)
                return Constants.ZettaHash;
            if (exaHash > 1)
                return Constants.ExaHash;
            if (petaHash > 1)
                return Constants.PetaHash;
            if (teraHash > 1)
                return Constants.TeraHash;
            if (gigaHash > 1)
                return Constants.GigaHash;
            if (megaHash > 1)
                return Constants.MegaHash;
            if (kiloHash > 1)
                return Constants.KiloHash;

            return hashrate;
        }
    }
}
