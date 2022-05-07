using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max_Gpu_Roi
{
    internal class Constants
    {
        // Results listview
        public const int Coin = 0;
        public const int Gpu = 1;
        public const int Manufacturer = 2;
        public const int MSRP = 3;
        public const int EbayPrice = 4;
        public const int PricePaid = 5;
        public const int UsdPerMhs = 6;
        public const int UsdCosts = 7;
        public const int CryptoCosts = 8;
        public const int Hashrate = 9;
        public const int Watts = 10;
        public const int Efficiency = 11;
        public const int UsdRewards = 12;
        public const int CryptoRewards = 13;
        public const int UsdProfits = 14;
        public const int CryptoProfits = 15;
        public const int Roi = 16;

        // Edit hashrates datagrid view
        public const int HashratesCoin = 0;
        public const int HashratesAlgo = 1;
        public const int HashratesHashrate = 2;
        public const int HashratesSize = 3;
        public const int HashratesWatts = 4;
        public const int HashratesCoin2 = 5;
        public const int HashratesAlgo2 = 6;
        public const int HashratesHashrate2 = 7;
        public const int HashratesSize2 = 8;
        public const int HashratesWatts2 = 9;

        public const double KiloHash = 1000;
        public const double MegaHash = 1000000;
        public const double GigaHash = 1000000000;
        public const double TeraHash = 1000000000000;
        public const double PetaHash = 1000000000000000;
        public const double ExaHash = 1000000000000000000;
        public const double ZettaHash = 1e+21;


        public const int DigitsToRound = 7;

        // Error Messages
        public const string PerformingCalcs = "Please wait while performing calculations. You can apply filters once this process is done.";
        public const string InvalidHodlCoin = "Please select a coin you plan to hodl.";
        public const string InvalidHodlPrice = "The hodl amount needs to be a number.";
    }
}
