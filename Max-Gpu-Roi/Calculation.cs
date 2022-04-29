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
        public string Coin { get; set; }
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
        public int GpuId { get; set; }


        public static Calculation Calculate(double gpuCost, double hashrate, int watts, string coinSymbol, double electricityRate, double poolMinerFee, double coinPrice = 0.0)
        {
            var coinInfo = MinerStat.GetCoinInfo(coinSymbol);
            var reward = new Calculation();
            var price = coinInfo.price;
            if (coinPrice > 0.0)
                price = coinPrice;

            // Crypto rewards
            var cryptoRewardsCalculated = (coinInfo.reward * (hashrate * 1000000)) * 24; // ((difficulty * coinInfo.reward_block) / (gpuHash * 1000000)) * 8640;
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

            // Efficiency
            reward.Efficiency = hashrate / watts;
            
            // Roi
            if(reward.UsdProfits > 0)
                reward.ROI = gpuCost / (reward.UsdProfits * 30);

            // Usd per mhs
            reward.CostPerMhs = hashrate > 0 ? gpuCost / hashrate : 0.0;

            reward.GpuCosts = gpuCost;
            reward.GpuHash = hashrate;

            return reward;
        }
    }
}
