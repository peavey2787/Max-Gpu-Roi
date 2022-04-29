namespace Max_Gpu_Roi
{
    internal class Coin
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Algorithm { get; set; }
        public double Price { get; set; }

        public Coin()
        {
            Name = "";
            Symbol = "";
            Algorithm = "";
            Price = 0.0;
        }
        public Coin(CoinInfo coinInfo)
        {
            Name = coinInfo.name;
            Symbol = coinInfo.coin;
            Algorithm = coinInfo.algorithm;
            Price = coinInfo.price;
        }
    }
}
