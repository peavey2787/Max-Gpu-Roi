using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace Max_Gpu_Roi
{
    public partial class MaxGpuRoi : Form
    {
        private string GpuListsDirectory = Directory.GetCurrentDirectory() + "\\GpuLists\\";
        private string CoinListsDirectory = Directory.GetCurrentDirectory() + "\\CoinLists\\";
        private List<CoinInfo> AllCoinInfos = new List<CoinInfo>();
        private Timer minerstatTimer = new Timer();
        private Timer errorMessageTimer = new Timer();
        private Timer errorCountDownTimer = new Timer();
        private int countDown = 10;
        private Ebay ebay = new Ebay();
        private List<List<EbayItem>> previousSearchResults = new List<List<EbayItem>>();
        private Hashrate CopiedHashrate = new Hashrate();
        private ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();
        private List<ListViewItem> CurrentSearchResults = new List<ListViewItem>();
        private bool PerformingCalculations = false;
        private bool lastStateShowAmd = true;
        private bool lastStateShowNvidia = true;
        private bool GettingEbayPrice = false;
        private bool searchingByHodlPrice = false;

        public MaxGpuRoi()
        {
            InitializeComponent();
        }

        private void ShowErrorMessage(string message)
        {
            errorCountDownTimer.Enabled = true;
            errorMessageTimer.Enabled = true;
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = message;
            return;
        }

        // Timers
        private void OnErrorCountDownTimerTickEvent(object sender, ElapsedEventArgs e)
        {
            // only run for 10 secs when called
            if (countDown > 0)
            {
                if (ErrorMessageCountDown.InvokeRequired)
                {
                    ErrorMessageCountDown.Invoke(new MethodInvoker(delegate { ErrorMessageCountDown.Text = countDown.ToString() + " secs"; }));
                }
                else
                    ErrorMessageCountDown.Text = countDown.ToString() + " secs";
                countDown--;
            }
            else if (countDown == 0)
            {
                errorCountDownTimer.Enabled = false;
                countDown = 10;
            }
        }
        private void OnErrorMessageTimerTickEvent(object sender, ElapsedEventArgs e)
        {
            errorMessageTimer.Enabled = false; // only run once when called
            if (ErrorMessagePanel.InvokeRequired)
            {
                ErrorMessagePanel.Invoke(new MethodInvoker(delegate { ErrorMessagePanel.Visible = false; }));
            }
        }
        private async void OnMinerStatTimerTickEvent(object sender, ElapsedEventArgs e)
        {
            // Refresh coin info every 5 minutes since minerstat doesn't update until every 5-10mins
            AllCoinInfos = await MinerStat.GetAllCoins();
        }


        // Startup / Exit
        private async void MaxGpuRoi_Load(object sender, EventArgs e)
        {
            // Set window size
            Size = new Size(1873, 942);
            MaximumSize = new Size(1873, 942);

            // Set Edit Gpu list panel to the proper location
            EditGpuPanel.Location = new Point(40, 150);

            // Set Edit Coin list panel to the proper location
            EditCoinPanel.Location = new Point(380, 150);

            // Setup Gpu lists 
            GpuLists.Columns.Add("Name", -2, HorizontalAlignment.Left);
            GpuLists.Columns.Add("# of Gpus", -2, HorizontalAlignment.Left);
            GpuLists.Columns[0].Width = 60;
            GpuLists.Columns[1].Width = 70;

            // Setup List of All Coins
            ListOfAllCoins.Columns.Add("Name", -2, HorizontalAlignment.Left);
            ListOfAllCoins.Columns.Add("Symbol", -2, HorizontalAlignment.Center);
            ListOfAllCoins.Columns.Add("Algorithm", -2, HorizontalAlignment.Left);
            ListOfAllCoins.Columns.Add("Price", -2, HorizontalAlignment.Left);
            ListOfAllCoins.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            // Setup Coin lists 
            CoinLists.Columns.Add("Name", -2, HorizontalAlignment.Left);
            CoinLists.Columns.Add("# of Coins", -2, HorizontalAlignment.Left);
            CoinLists.Columns[0].Width = 60;
            CoinLists.Columns[1].Width = 70;

            // Setup Edit Coin list
            EditCoinList.Columns.Add("Name", -2, HorizontalAlignment.Center);
            EditCoinList.Columns.Add("Symbol", -2, HorizontalAlignment.Center);
            EditCoinList.Columns.Add("Algorithm", -2, HorizontalAlignment.Center);
            EditCoinList.Columns[0].Width = 150;
            EditCoinList.Columns[1].Width = 60;
            EditCoinList.Columns[2].Width = 105;

            // Setup Edit Gpu list
            EditGpuList.Columns.Add("Maker", -2, HorizontalAlignment.Center);
            EditGpuList.Columns.Add("Prefix", -2, HorizontalAlignment.Center);
            EditGpuList.Columns.Add("Model", -2, HorizontalAlignment.Center);
            EditGpuList.Columns.Add("Suffix", -2, HorizontalAlignment.Center);
            EditGpuList.Columns.Add("Vram", -2, HorizontalAlignment.Center);
            EditGpuList.Columns[0].Width = 60;
            EditGpuList.Columns[1].Width = 60;
            EditGpuList.Columns[2].Width = 60;
            EditGpuList.Columns[3].Width = 60;
            EditGpuList.Columns[4].Width = 50;

            // Setup Edit Gpu Hashrates
            EditHashrates.Columns.Add("", "Coin");// .Add("Coin", -2, HorizontalAlignment.Left);
            EditHashrates.Columns.Add("", "Algorithm");//, -2, HorizontalAlignment.Left);
            EditHashrates.Columns.Add("", "Hashrate");//, -2, HorizontalAlignment.Left);
            EditHashrates.Columns.Add("", "Watts");//, -2, HorizontalAlignment.Left);
            EditHashrates.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);//.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            // Setup Results list
            ResultsList.Columns.Add("Coin", -2, HorizontalAlignment.Left);
            ResultsList.Columns.Add("Gpu", -2, HorizontalAlignment.Center);
            ResultsList.Columns.Add("Maker", -2, HorizontalAlignment.Center);
            ResultsList.Columns.Add("MSRP", -2, HorizontalAlignment.Center);
            ResultsList.Columns.Add("Ebay", -2, HorizontalAlignment.Center);
            ResultsList.Columns.Add("Price Paid", -2, HorizontalAlignment.Center);
            ResultsList.Columns.Add("$/Mhs", -2, HorizontalAlignment.Center);
            ResultsList.Columns.Add("Usd Costs", -2, HorizontalAlignment.Center);
            ResultsList.Columns.Add("Crypto Costs per day/week/month", -2, HorizontalAlignment.Center);
            ResultsList.Columns.Add("Hashrate", -2, HorizontalAlignment.Center);
            ResultsList.Columns.Add("Watts", -2, HorizontalAlignment.Center);
            ResultsList.Columns.Add("Efficiency", -2, HorizontalAlignment.Center);
            ResultsList.Columns.Add("Usd Rewards", -2, HorizontalAlignment.Center);
            ResultsList.Columns.Add("Crypto Rewards", -2, HorizontalAlignment.Center);
            ResultsList.Columns.Add("Usd Profits", -2, HorizontalAlignment.Center);
            ResultsList.Columns.Add("Crypto Profits", -2, HorizontalAlignment.Center);
            ResultsList.Columns.Add("R-O-I", -2, HorizontalAlignment.Center);
            // 1849 / 18 = 102.72 per col to be equal
            //69,91,48,60,63,65,50,144,235,64,44,92,137,208,140,211,107
            ResultsList.Columns[Constants.Coin].Width = 69;
            ResultsList.Columns[Constants.Gpu].Width = 91;
            ResultsList.Columns[Constants.Manufacturer].Width = 48;
            ResultsList.Columns[Constants.MSRP].Width = 60;
            ResultsList.Columns[Constants.EbayPrice].Width = 63;
            ResultsList.Columns[Constants.PricePaid].Width = 65;
            ResultsList.Columns[Constants.UsdPerMhs].Width = 50;
            ResultsList.Columns[Constants.UsdCosts].Width = 144;
            ResultsList.Columns[Constants.CryptoCosts].Width = 235;
            ResultsList.Columns[Constants.Hashrate].Width = 64;
            ResultsList.Columns[Constants.Watts].Width = 44;
            ResultsList.Columns[Constants.Efficiency].Width = 92;
            ResultsList.Columns[Constants.UsdRewards].Width = 137;
            ResultsList.Columns[Constants.CryptoRewards].Width = 208;
            ResultsList.Columns[Constants.UsdProfits].Width = 140;
            ResultsList.Columns[Constants.CryptoProfits].Width = 211;
            ResultsList.Columns[Constants.Roi].Width = 107;

            // Allow sorting results
            ResultsList.ListViewItemSorter = lvwColumnSorter;

            // Make sure gpulist/coinlist directory exists, if not create them
            if (!Directory.Exists(GpuListsDirectory))
                Directory.CreateDirectory(GpuListsDirectory);

            // If there are no files in the gpulists folder let user know they need to create a list or import one
            var gpus = new List<Gpu>();
            var gpuLists = Directory.GetFiles(GpuListsDirectory);
            if (gpuLists.Length == 0)
                MessageBox.Show("No gpu lists found! Please create a new list or import one.");
            else
            {
                // Check if user has the default list
                foreach (var filePath in gpuLists)
                {
                    // Make the default list the first listview item and select it if it exists
                    if (Path.GetFileNameWithoutExtension(filePath) == "Default")
                    {
                        gpus = await JsonCrud.LoadGpuList(filePath);
                        var li = new ListViewItem("Default");
                        li.Tag = filePath;
                        li.SubItems.Add(gpus.Count.ToString());
                        GpuLists.Items.Add(li);
                        break;
                    }
                }

                // Get all gpulist file names
                foreach (var filePath in gpuLists)
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    if (fileName == "Default" || fileName == "")
                    { } // Skip default we already added it
                    else
                    {
                        gpus = await JsonCrud.LoadGpuList(filePath);

                        if (gpus != null && gpus.Count > 0)
                        {
                            var li = new ListViewItem(fileName);
                            li.Tag = filePath;
                            li.SubItems.Add(gpus.Count.ToString());
                            GpuLists.Items.Add(li);
                        }
                    }
                }
                // Select the first gpulist
                if (GpuLists.Items.Count > 0)
                    GpuLists.Items[0].Selected = true; //GpuLists.Select();
            }

            // Attempt to load coin defaults
            // Make sure gpulist/coinlist directory exists, if not create them
            if (!Directory.Exists(CoinListsDirectory))
                Directory.CreateDirectory(CoinListsDirectory);

            // If there are no files in the coinlists folder let user know they need to create a list or import one
            var coins = new List<Coin>();
            var coinLists = Directory.GetFiles(CoinListsDirectory);
            if (coinLists.Length == 0)
                MessageBox.Show("No coin lists found! Please create a new list or import one.");
            else
            {
                // Check if user has the default list
                foreach (var filePath in coinLists)
                {
                    // Make the default list the first listview item and select it if it exists
                    if (Path.GetFileNameWithoutExtension(filePath) == "Default")
                    {
                        coins = await JsonCrud.LoadCoinList(filePath);
                        var li = new ListViewItem("Default");
                        li.Tag = filePath;
                        li.SubItems.Add(coins.Count.ToString());
                        CoinLists.Items.Add(li);
                        break;
                    }
                }

                // Get all coinlist file names
                foreach (var filePath in coinLists)
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    if (fileName == "Default")
                    { } // Skip default we already added it
                    else
                    {
                        coins = await JsonCrud.LoadCoinList(filePath);

                        if (coins != null && coins.Count > 0)
                        {
                            var li = new ListViewItem(fileName);
                            li.Tag = filePath;
                            li.SubItems.Add(coins.Count.ToString());
                            CoinLists.Items.Add(li);
                        }
                    }
                }

                // Select the first coinlist
                if (CoinLists.Items.Count > 0)
                    CoinLists.Items[0].Selected = true;
                
                PopulateHodlMenu();
            }

            // Timer to prevent unnecessary api calls to minerstat
            minerstatTimer.Elapsed += new ElapsedEventHandler(OnMinerStatTimerTickEvent);
            minerstatTimer.Interval = 300000; // 5mins
            minerstatTimer.Enabled = true;

            // Error message pop up timer
            errorMessageTimer.Elapsed += new ElapsedEventHandler(OnErrorMessageTimerTickEvent);
            errorMessageTimer.Interval = 10000; // 10 secs

            // Error message countdown timer
            errorCountDownTimer.Elapsed += new ElapsedEventHandler(OnErrorCountDownTimerTickEvent);
            errorCountDownTimer.Interval = 900; // 1 secs
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // Allow user to move window
        #region
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void MaxGpuRoi_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion


        // Results
        private async void MaxMyROI_Click(object sender, EventArgs e)
        {
            if (PerformingCalculations)
            {
                ShowErrorMessage(Constants.PerformingCalcs);
                return;
            }

            // Error proofing
            if (GpuLists.SelectedItems.Count == 0)
            {
                MessageBox.Show("No gpu list selected! Please select a gpu list.");
                return;
            }
            // Error proofing to allow only whole numbers for miner/pool fee
            if (!int.TryParse(PoolMinerFee.Text, out var fee))
            {
                MessageBox.Show("Miner/Pool Fee Must be a Whole Number!");
                return;
            }
            if (searchingByHodlPrice)
            {
                // Twice for descending order
                SortResults(Constants.UsdProfits); // Sort by column
                SortResults(Constants.UsdProfits); 
            }
            else
                SortResults(Constants.Roi); // Sort by column        

            progressBar.Value = 0;
            ResultsList.Items.Clear();
            ResultsEbayItemSelection.Items.Clear();
            ResultsEbayLink.Tag = null;
            PerformingCalculations = true;


            // If budget is given then show most profitable configuration
            if (double.TryParse(Budget.Text, out var budget) && budget > 0)
            {
                lblResults.Text = "Most Profitable Configuration";
                // ToDo add a textbox for user to enter how much it costs them to build/get a rig w/o gpus
                // and a textbox for how many gpus each rig supports 
                // and a textbox for how many open gpu slots they have now
                // and a radio button asking if user is going to replace the thermal pads with
                // $35 per gpu for custom ready to go kritical pads
                // Or $70 per gpu for copper pads
                // Or Custom pads costing (textbox for gpu thermal pad cost)
                // Or Not replacing pads
                // If no data default to a rig costing $500 and supporting 6 gpus  (https://www.amazon.com/hz/wishlist/ls/3JT2A2G46NWEQ?ref_=wl_share) and not replacing pads
                // Account for needing more rigs w/o gpu's 

                // get all gpus, sort them by shortest roi and go through each one to see if user can afford it
                // Get all gpus
                var allGpus = await JsonCrud.LoadGpuList(GpuLists.SelectedItems[0].Tag.ToString());

                var lvItemsToAdd = new List<ListViewItem>();
                var buffer = 5;

                progressBar.Value = 10;
                progressBar.Maximum = allGpus.Count + 10;

                // Go through each gpu
                foreach (var gpu in allGpus)
                {
                    progressBar.Increment(1);

                    // Sort by shortest roi
                    var calculations = await GetGpuCalculations(gpu);
                    calculations.Sort((y, x) => x.ROI.CompareTo(y.ROI));

                    // Add this gpu and calculation to the results list
                    lvItemsToAdd.Add(CreateResultsListviewItem(gpu, calculations[0]));

                    if (lvItemsToAdd.Count == buffer)
                    {
                        ResultsList.Items.AddRange(lvItemsToAdd.ToArray());
                        lvItemsToAdd = new List<ListViewItem>();
                    }
                }

                // Add remaining items
                ResultsList.Items.AddRange(lvItemsToAdd.ToArray());

                progressBar.Increment(5); // Update progress

                // Save master list to apply filters to
                CurrentSearchResults = new List<ListViewItem>();
                foreach (ListViewItem item in ResultsList.Items)
                    CurrentSearchResults.Add(item);
                FilterResults();

                progressBar.Value = 0;
                PerformingCalculations = false;

                // if they can afford the top one find out how many of them they can get
                // if they can't get more than one then see if they can get the next gpu and so on

            }

            // If no budget given then show the most profitable coins to mine for each gpu
            lblResults.Text = "Most Profitable Coin per Gpu";

            // Load the gpu list from file
            var file = GpuLists.SelectedItems[0].Tag.ToString();
            var gpus = await JsonCrud.LoadGpuList(file);

            var listViewItemsToAdd = new List<ListViewItem>();
            var bufferSize = 5;
            progressBar.Value = 10;
            progressBar.Maximum = gpus.Count + 10;
            int sortByIndex = 0;

            // Go through each gpu
            foreach (var gpu in gpus)
            {
                if(gpu.ModelNumber == "1060 6gb")
                {
                    var come = "get me for debugging";
                }
                progressBar.Increment(1);

                var calculations = await GetGpuCalculations(gpu);
                var calculation = new Calculation();

                // Only add gpus that have good hashrate info and calculations were performed on them
                if (calculations.Count > 0)
                {
                    // If hodl price isn't empty get the highest usd profits per hashrate calculation in gpu
                    if (searchingByHodlPrice)
                    {
                        // Get highest Usd Profits and only show gpus with selected hodl coin
                        var highest = calculations[0];
                        foreach (var calc in calculations)
                            if (calc.UsdProfits > highest.UsdProfits)
                                highest = calc;                        
                        calculation = highest;
                    }
                    else
                    {
                        // Get quickest Roi
                        var quickest = calculations[0];
                        foreach (var calc in calculations)
                            if (calc.ROI > 0 && calc.ROI < quickest.ROI)
                                quickest = calc;
                        calculation = quickest;
                    }

                    // Add this gpu and calculation to the results list
                    listViewItemsToAdd.Add(CreateResultsListviewItem(gpu, calculation));
                    if (listViewItemsToAdd.Count == bufferSize)
                    {
                        ResultsList.Items.AddRange(listViewItemsToAdd.ToArray());
                        listViewItemsToAdd = new List<ListViewItem>();
                    }
                }
            }

            // Add remaining items
            ResultsList.Items.AddRange(listViewItemsToAdd.ToArray());

            progressBar.Increment(5); // Update progress

            // Save master list to apply filters to
            CurrentSearchResults = new List<ListViewItem>();
            foreach (ListViewItem item in ResultsList.Items)
                CurrentSearchResults.Add(item);

            FilterResults(); // Apply filters

            // Reset
            searchingByHodlPrice = false;
            progressBar.Value = 0;
            PerformingCalculations = false;
        }
        private ListViewItem CreateResultsListviewItem(Gpu gpu, Calculation calculation)
        {
            // Get the most profitable hashrate for this gpu
            var hashrate = new Hashrate();
            foreach (var hash in gpu.Hashrates)
                if (hash.Coin == calculation.Coin)
                    hashrate = hash;

            var li = new ListViewItem();
            li.Tag = gpu;
            li.ImageIndex = GetImageIndex(hashrate.Coin);
            li.Text = hashrate.Coin;
            li.SubItems.Add(gpu.Name);
            li.SubItems.Add(gpu.Manufacturer);
            li.SubItems.Add("$" + gpu.MSRP.ToString("0.00"));
            li.SubItems.Add("$" + gpu.EbayPrice.ToString("0.00"));
            li.SubItems.Add("$" + gpu.PricePaid.ToString("0.00"));
            li.SubItems.Add("$" + calculation.CostPerMhs.ToString("0.00"));       
            var usdCosts = calculation.UsdPoolMinerFeeCost + calculation.UsdElectricityCost;
            li.SubItems.Add("$" + usdCosts.ToString("0.00") + " / $" + (usdCosts * 7).ToString("0.00") + " / $" + (usdCosts * 30).ToString("0.00") );
            var cryptoCosts = decimal.Round((decimal)(calculation.CryptoPoolMinerFeeCost + calculation.CryptoElectricityCost), Constants.DigitsToRound);
            li.SubItems.Add(cryptoCosts.ToString() + " / " + (cryptoCosts * 7).ToString() + " / " + (cryptoCosts * 30));
            li.SubItems.Add(hashrate.HashrateSpeed.ToString());
            li.SubItems.Add(hashrate.Watts.ToString());
            li.SubItems.Add(calculation.Efficiency.ToString("0.000") + " kw/mhs");
            li.SubItems.Add("$" + calculation.UsdRewards.ToString("0.00") + " / $" + (calculation.UsdRewards * 7).ToString("0.00") + " / $" + (calculation.UsdRewards * 30).ToString("0.00"));
            var cryptoRewards = decimal.Round((decimal)calculation.CryptoRewards, Constants.DigitsToRound);
            li.SubItems.Add(cryptoRewards.ToString("0.00") + " " + hashrate.Coin + " / " + (cryptoRewards * 7).ToString("0.00") + " " + hashrate.Coin + " / " + (cryptoRewards * 30).ToString("0.00") + " " + hashrate.Coin);
            li.SubItems.Add("$" + calculation.UsdProfits.ToString("0.00") + " / $" + (calculation.UsdProfits * 7).ToString("0.00") + " / $" + (calculation.UsdProfits * 30).ToString("0.00"));
            var cryptoProfits = decimal.Round((decimal)calculation.CryptoProfits, Constants.DigitsToRound);
            li.SubItems.Add(cryptoProfits.ToString("0.00") + " " + hashrate.Coin + " / " + (cryptoProfits * 7).ToString("0.00") + " " + hashrate.Coin + " / " + (cryptoProfits * 30).ToString("0.00") + " " + hashrate.Coin);
            li.SubItems.Add(calculation.ROI.ToString("0.00") + " months");

            return li;
        }
        private void ResultsList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (!PerformingCalculations)
                SortResults(e.Column);
            else
                ShowErrorMessage(Constants.PerformingCalcs);
        }
        private void SortResults(int column = 0)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            ResultsList.Sort();
        }
        private async Task<List<Calculation>> GetGpuCalculations(Gpu gpu)
        {
            // Get coin list filename
            var file = CoinLists.SelectedItems[0].Tag.ToString();

            // Load saved Coin list
            var coins = new List<Coin>();
            coins = await JsonCrud.LoadCoinList(file);

            var calculations = new List<Calculation>();
            // And go through each saved hashrate
            foreach (var hashrate in gpu.Hashrates)
            {
                // To find coins that are only on
                foreach (Coin coin in coins)
                {
                    // This users coin list
                    if (hashrate.Coin.ToLower() == coin.Symbol.ToLower() && hashrate.HashrateSpeed > 0 && hashrate.Watts > 0)
                    {
                        var fee = double.TryParse(PoolMinerFee.Text, out double parsedFee) ? parsedFee / 100 : 0;
                        var electricityRate = double.TryParse(ElectricityRate.Text, out double parsedElecRate) ? parsedElecRate : 0;

                        // Then Perform calculations for this coin
                        // if price paid isn't 0 use that
                        var gpuCost = 0.0;
                        if (gpu.PricePaid > 0)
                            gpuCost = gpu.PricePaid;
                        else
                        {
                            // Otherwise use ebay price if it isn't 0
                            if (gpu.EbayPrice > 0)
                                gpuCost = gpu.EbayPrice;

                            // If it is 0 then try to get ebay price now
                            else if (gpu.EbayPrice == 0)
                            {
                                // Get ebay Price
                                try
                                {
                                    var ebayCalculation = new Calculation();
                                    var ebayItems = await GetEbayPrice(gpu);
                                    foreach (var item in ebayItems)
                                    {
                                        if (double.TryParse(item.Price.ToString(), out var ebayPrice))
                                        {
                                            ebayCalculation = await Calculation.Calculate(ebayPrice, hashrate.HashrateSpeed, hashrate.Watts, hashrate.Coin, electricityRate, fee);

                                            // If returned results are too good to be true try the next result
                                            double.TryParse(ebayItems[ebayItems.Count - 1].Price.ToString(), out var mostExpensiveItem);
                                            if (ebayCalculation.CostPerMhs < 5 && mostExpensiveItem > 800)
                                                continue;
                                            else
                                            {
                                                gpu.EbayLink = item.Url;
                                                gpu.EbayPrice = ebayPrice;
                                                gpuCost = ebayPrice;
                                                break;
                                            }
                                        }
                                    }
                                }
                                catch { }
                            }
                            // If can't get ebay price then try msrp if it isn't 0
                            if (gpu.EbayPrice == 0 && gpu.MSRP > 0)
                                gpuCost = gpu.MSRP;
                        }

                        // Perform calculation
                        var calculation = new Calculation();
                        if (HodlCoin.SelectedItem != null && coin.Symbol.ToLower() == HodlCoin.SelectedItem.ToString().ToLower() && double.TryParse(HodlPrice.Text, out var hodlPrice) && hodlPrice > 0)
                            calculation = await Calculation.Calculate(gpuCost, hashrate.HashrateSpeed, hashrate.Watts, hashrate.Coin, electricityRate, fee, hodlPrice); // Check hodl coin price
                        else
                            calculation = await Calculation.Calculate(gpuCost, hashrate.HashrateSpeed, hashrate.Watts, hashrate.Coin, electricityRate, fee); // Check real time coin price
                        calculation.Coin = hashrate.Coin;
                        calculation.GpuId = gpu.Id;
                        calculations.Add(calculation);
                    }
                }
            }
            return calculations;
        }
        private async void ResultsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ResultsList.SelectedItems.Count > 0)
            {
                var gpu = ResultsList.SelectedItems[0].Tag as Gpu;
                ResultsEbayLink.Tag = (gpu.EbayLink != null || gpu.EbayLink != "unknown") ? gpu.EbayLink : "";

                // Get ebay price
                try
                {
                    // Clear previous data
                    ResultsEbayItemSelection.Items.Clear();
                    EbayItemUrl.Tag = "";

                    var ebayItems = await GetEbayPrice(gpu);

                    foreach (var item in ebayItems)
                    {
                        if (double.TryParse(item.Price.ToString(), out var ebayPrice))
                        {
                            ResultsEbayItemSelection.Items.Add(new { Text = item.Name + " $" + ebayPrice.ToString("0.00"), Value = item });
                        }
                    }
                    ResultsEbayItemSelection.SelectedIndex = 0;

                }
                catch (Exception ex) { }
            }
        }
        private void Budget_KeyDown(object sender, KeyEventArgs e)
        {
            if (PerformingCalculations && e.KeyCode == Keys.Enter)
            {
                ShowErrorMessage(Constants.PerformingCalcs);
                return;
            }
            if (!PerformingCalculations && e.KeyCode == Keys.Enter)
            {
                if (double.TryParse(Budget.Text, out var budget) && budget > 0)
                    MaxMyROI.PerformClick();
                else
                    ShowErrorMessage("The budget needs to be a number that is greater than 0 please.");
            }
        }
        private int GetImageIndex(string coin)
        {
            var index = 0;
            switch (coin.ToLower())
            {
                case "eth":
                    index = 1;
                    break;
                case "erg":
                    index = 2;
                    break;
                case "rvn":
                    index = 3;
                    break;
                case "cfx":
                    index = 4;
                    break;
                default:
                    break;
            }
            return index;
        }


        // Disable/Enable GUI while editing gpu/coin lists
        private void DisableUserInput()
        {
            GpuLists.Enabled = false;
            ImportGpuLists.Enabled = false;
            ExportGpuLists.Enabled = false;
            EditGpuLists.Enabled = false;
            CoinLists.Enabled = false;
            ImportCoinLists.Enabled = false;
            ExportCoinLists.Enabled = false;
            EditCoinLists.Enabled = false;
            Budget.Enabled = false;
            MaxMyROI.Enabled = false;
            ElectricityRate.Enabled = false;
            PoolMinerFee.Enabled = false;
            ShowAmd.Enabled = false;
            ShowNvidia.Enabled = false;
        }
        private void EnableUserInput()
        {
            GpuLists.Enabled = true;
            ImportGpuLists.Enabled = true;
            ExportGpuLists.Enabled = true;
            EditGpuLists.Enabled = true;
            CoinLists.Enabled = true;
            ImportCoinLists.Enabled = true;
            ExportCoinLists.Enabled = true;
            EditCoinLists.Enabled = true;
            Budget.Enabled = true;
            MaxMyROI.Enabled = true;
            ElectricityRate.Enabled = true;
            PoolMinerFee.Enabled = true;
            ShowAmd.Enabled = true;
            ShowNvidia.Enabled = true;
        }


        // Show file dialog for user to select a gpu/coin list file
        private string GetFileFromUser()
        {
            var filePath = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.Filter = "Json files (*.json)|*.json";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    filePath = openFileDialog.FileName;
            }
            return filePath;
        }

        // Hodl
        private async void PopulateHodlMenu()
        {
            // Populate user coin list to hodl coin dropdown menu
            var file = CoinLists.SelectedItems[0].Tag.ToString();
            var coins = await JsonCrud.LoadCoinList(file);
            foreach (var coin in coins)
                HodlCoin.Items.Add(coin.Symbol);
        }
        private void HodlCoin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (double.TryParse(HodlPrice.Text, out var hodlPrice) && hodlPrice > 0)
            {
                if (PerformingCalculations)
                {
                    ShowErrorMessage(Constants.PerformingCalcs);
                    return;
                }
                searchingByHodlPrice = true;
                MaxMyROI.PerformClick();
            }
        }
        private void HodlPrice_KeyDown(object sender, KeyEventArgs e)
        {       
            if(e.KeyCode == Keys.Enter)
            {
                // remove potential commas
                var price = HodlPrice.Text.Replace(",", "");
                if (double.TryParse(price, out var hodlPrice) && hodlPrice > 0)
                {
                    if (PerformingCalculations)
                    {
                        ShowErrorMessage(Constants.PerformingCalcs);
                        return;
                    }
                    
                    if (HodlCoin.Text.Length > 0)
                    {
                        searchingByHodlPrice = true;
                        MaxMyROI.PerformClick();
                    }
                    else
                        ShowErrorMessage(Constants.InvalidHodlCoin);
                }
                else
                    ShowErrorMessage(Constants.InvalidHodlPrice);                

            }
        }

        // Coin list
        private async void EditCoinLists_Click(object sender, EventArgs e)
        {
            // Clear edit coin list
            EditCoinList.Items.Clear();

            // If no list is selected show a popup to inform the user
            if (CoinLists.SelectedItems.Count == 0)
            {
                MessageBox.Show("No Gpu List Selected to Edit!");
                return;
            }

            // Show the edit panel and disable user input on the main GUI
            EditCoinPanel.Visible = true;
            DisableUserInput();

            // If this is a new list
            if (CoinLists.SelectedItems[0].Tag == null)
            {

            }
            else
            {
                // Load the coin list from file
                var file = CoinLists.SelectedItems[0].Tag.ToString();
                var coins = await JsonCrud.LoadCoinList(file);

                CoinListName.Text = CoinLists.SelectedItems[0].Text;

                // Go through each coin
                foreach (var coin in coins)
                {
                    // Only add it if it isn't already on the list
                    if (!EditCoinList.Items.ContainsKey(coin.Name))
                    {
                        var li = new ListViewItem(coin.Name);
                        li.ImageIndex = 0; // ToDo: Select correct image based on coin
                        li.Tag = coin;
                        li.Name = coin.Name;
                        li.SubItems.Add(coin.Symbol);
                        li.SubItems.Add(coin.Algorithm);
                        EditCoinList.Items.Add(li);
                    }
                }
                EditCoinList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }

            // Show list of all available coins

            // Only call minerstat api if there is no coin info 
            if (AllCoinInfos.Count == 0)
                AllCoinInfos = await MinerStat.GetAllCoins();

            foreach (var coinInfo in AllCoinInfos)
            {
                if (coinInfo.type == "coin")
                {
                    // Extract only useful data from minerstats coinInfo
                    var coin = new Coin();
                    coin.Name = coinInfo.name;
                    coin.Symbol = coinInfo.coin;
                    coin.Algorithm = coinInfo.algorithm;

                    var li = new ListViewItem(coin.Name);
                    li.Name = coin.Name;
                    li.Tag = coin;
                    li.ImageIndex = 0; // ToDo: Select correct image based on coin
                    li.SubItems.Add(coin.Symbol);
                    li.SubItems.Add(coin.Algorithm);
                    ListOfAllCoins.Items.Add(li);
                }
            }

            ListOfAllCoins.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        private void AddCoinList_Click(object sender, EventArgs e)
        {
            var li = new ListViewItem();
            li.SubItems.Add("0");
            CoinLists.Items.Add(li);
            var index = CoinLists.Items.Count - 1;
            CoinLists.Items[index].Selected = true;
            EditCoinLists.PerformClick();
        }
        private async void ImportCoinLists_Click(object sender, EventArgs e)
        {
            var file = GetFileFromUser();
            var coins = await JsonCrud.LoadCoinList(file);

            // Add listview item
            var li = new ListViewItem();
            li.Tag = file;
            li.Text = Path.GetFileNameWithoutExtension(file);
            li.SubItems.Add(coins.Count.ToString());
            CoinLists.Items.Add(li);
            CoinLists.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        private void CoinLists_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DialogResult confirmRemoval = MessageBox.Show("Are you sure you want to remove this list?", "Confirm list removal", MessageBoxButtons.YesNo);
                if (confirmRemoval == DialogResult.Yes)
                {
                    var file = CoinListsDirectory + "\\" + CoinLists.SelectedItems[0].Text + ".json";
                    var fi = new FileInfo(file);

                    // Don't delete default list
                    if (CoinLists.SelectedItems[0].Text == "Default")
                        return;
                    else if (CoinLists.SelectedItems[0].Text == "")
                    {
                        // New list created but user didn't rename it (assuming user created new list then clicked cancel)
                        CoinLists.Items.Remove(CoinLists.SelectedItems[0]);
                        return;
                    }

                    if (File.Exists(file) && !JsonCrud.IsFileLocked(fi))
                    {
                        File.Delete(file);
                        CoinLists.Items.Remove(CoinLists.SelectedItems[0]);
                    }
                    else
                        MessageBox.Show("Unable to remove list, the list either no longer exists or it is open and in use! Please try again.");
                }
            }
        }
        private void CoinLists_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CoinLists.SelectedItems.Count > 0)
            {
                if (PerformingCalculations)
                {
                    ShowErrorMessage(Constants.PerformingCalcs);
                    return;
                }

                PopulateHodlMenu();
            }
        }
        private async void ExportCoinLists_Click(object sender, EventArgs e)
        {
            if (CoinLists.SelectedItems != null && CoinLists.SelectedItems.Count > 0)
            {
                var fileName = CoinLists.SelectedItems[0].Text;
                
                // Load the coin list from file
                var file = CoinLists.SelectedItems[0].Tag.ToString();
                var coins = await JsonCrud.LoadCoinList(file);


                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Filter = "json files (*.json)|*.json";
                saveFileDialog.FileName = fileName;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    await JsonCrud.WriteToCoinListFile(saveFileDialog.FileName, coins);    
                }
            }
        }

        // Edit coin list
        private async void SaveCoinList_Click(object sender, EventArgs e)
        {
            // Inform user if there is no list name but they added coins to the list
            if (CoinListName.Text.Length == 0 && CoinLists.Items.Count > 0)
            {
                MessageBox.Show("Coin List Name must not be empty! Please give the coin list a name in the textbox above the list.");
                return;
            }

            // Remove window and enable user input for main gui
            EditCoinPanel.Visible = false;
            EnableUserInput();

            // Don't save the list if there are no coins in the list 
            if (EditCoinList.Items.Count == 0)
                return;

            // Extract each coin from the listview
            var coins = new List<Coin>();
            foreach (ListViewItem item in EditCoinList.Items)
            {
                var coin = item.Tag as Coin;
                coins.Add(coin);
            }
            await JsonCrud.SaveCoinList(coins, CoinListsDirectory + CoinListName.Text + ".json");
        }
        private void CancelEditCoinList_Click(object sender, EventArgs e)
        {
            // If user has coins on the list, but the list doesn't have a name and user clicks cancel make sure they want to discard this list
            if (CoinLists.Items.Count > 0 && CoinListName.Text.Length == 0)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to discard this list?", "Discard List Confirmation", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    // continue to discard this list
                }
                else if (dialogResult == DialogResult.No)
                {
                    return; // User doesn't want to discard this list
                }
            }

            // Remove this window and enable user input
            EditCoinPanel.Visible = false;
            EnableUserInput();

            // Remove list if user clicked to add new list but then clicked cancel
            if (CoinLists.SelectedItems[0].Text == "")
            {
                // New list created but user didn't rename it (assuming user created new list then clicked cancel)
                CoinLists.Items.Remove(CoinLists.SelectedItems[0]);
                return;
            }
        }
        private void AddCoin_Click(object sender, EventArgs e)
        {
            if (ListOfAllCoins.SelectedItems.Count == 0)
            {
                MessageBox.Show("No Coin Selected From List of All Coins!");
                return;
            }

            var coinToAdd = ListOfAllCoins.SelectedItems[0].Tag as Coin;

            // Go through all coins on custom list to prevent duplicate entries
            foreach (ListViewItem item in EditCoinList.Items)
            {
                var coinOnEditList = item.Tag as Coin;
                if (coinOnEditList.Name == coinToAdd.Name)
                    return;
            }

            // This coin isn't on the list yet so add it
            var li = new ListViewItem(coinToAdd.Name);
            li.Name = coinToAdd.Name;
            li.ImageIndex = 0; // ToDo: Get correct pic for coin
            li.Tag = coinToAdd;
            li.SubItems.Add(coinToAdd.Symbol);
            li.SubItems.Add(coinToAdd.Algorithm);
            EditCoinList.Items.Add(li);
        }
        private void DeleteCoin_Click(object sender, EventArgs e)
        {
            if (EditCoinList.SelectedItems.Count == 0)
            {
                MessageBox.Show("No Coin Selected From Custom List to Delete!");
                return;
            }

            var coinToRemove = EditCoinList.SelectedItems[0].Tag as Coin;

            EditCoinList.Items.RemoveByKey(coinToRemove.Name);
        }



        // Gpu list
        private async void EditGpuLists_Click(object sender, EventArgs e)
        {
            // Clear edit gpulist/hashrates
            EditGpuList.Items.Clear();
            EditHashrates.Rows.Clear();

            // If no list is selected show a popup to inform the user
            if (GpuLists.SelectedItems.Count == 0)
            {
                MessageBox.Show("No Gpu List Selected to Edit!");
                return;
            }

            // Show the edit panel and disable user input on the main GUI
            EditGpuPanel.Visible = true;
            DisableUserInput();

            // If this is a new list add one new entry
            if (GpuLists.SelectedItems[0].Tag == null)
            {
                var gpu = new Gpu();
                gpu.Manufacturer = "nvidia";
                gpu.Id = 0;
                gpu.VersionPrefix = "rtx";
                gpu.ModelNumber = "3090";
                gpu.VersionSuffix = "ti";
                gpu.VramSize = 24;
                gpu.AmdOrNvidia = "nvidia";
                gpu.MSRP = 1999.00;
                gpu.PricePaid = 2400.00;
                AddGpuToEditListView(gpu);
                EditGpuList.Items[0].Selected = true;
                GpuListName.Text = "";
                return;
            }

            // Load the gpu list from file
            var fileName = GpuLists.SelectedItems[0].Tag.ToString();
            var gpus = await JsonCrud.LoadGpuList(fileName);

            GpuListName.Text = GpuLists.SelectedItems[0].Text; // Set the list name
            foreach (var gpu in gpus)
                AddGpuToEditListView(gpu);            

            // Select first gpu in the list
            EditGpuList.Items[0].Selected = true;

            // If this is the default list disable editing the list name
            if(GpuListName.Text == "Default")
                GpuListName.Enabled = false;
            else
                GpuListName.Enabled = true;
        }
        private void AddGpuList_Click(object sender, EventArgs e)
        {
            var li = new ListViewItem();
            li.SubItems.Add("0");
            GpuLists.Items.Add(li);
            var index = GpuLists.Items.Count - 1;
            GpuLists.Items[index].Selected = true;
            EditGpuLists.PerformClick();
        }
        private async void ImportGpuLists_Click(object sender, EventArgs e)
        {
            var file = GetFileFromUser();
            var gpus = await JsonCrud.LoadGpuList(file);

            // Add listview item
            var li = new ListViewItem();
            li.Tag = file;
            li.Text = Path.GetFileNameWithoutExtension(file);
            li.SubItems.Add(gpus.Count.ToString());
            GpuLists.Items.Add(li);
            GpuLists.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        private void GpuLists_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DialogResult confirmRemoval = MessageBox.Show("Are you sure you want to remove this list?", "Confirm list removal", MessageBoxButtons.YesNo);
                if (confirmRemoval == DialogResult.Yes)
                {
                    var file = GpuListsDirectory + "\\" + GpuLists.SelectedItems[0].Text + ".json";
                    var fi = new FileInfo(file);

                    // Don't delete default list
                    if (GpuLists.SelectedItems[0].Text == "Default")
                        return;
                    else if (GpuLists.SelectedItems[0].Text == "")
                    {
                        // New list created but user didn't rename it (assuming user created new list then clicked cancel)
                        GpuLists.Items.Remove(GpuLists.SelectedItems[0]);
                        return;
                    }

                    if (File.Exists(file) && !JsonCrud.IsFileLocked(fi))
                    {
                        File.Delete(file);
                        GpuLists.Items.Remove(GpuLists.SelectedItems[0]);
                    }
                    else
                        MessageBox.Show("Unable to remove list, the list either no longer exists or it is open and in use! Please try again.");
                }
            }
        }
        private void GpuLists_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CoinLists.SelectedItems.Count > 0)
            {
                if (PerformingCalculations)
                {
                    ShowErrorMessage(Constants.PerformingCalcs);
                    return;
                }

                PopulateHodlMenu();
            }
        }
        private async void ExportGpuLists_Click(object sender, EventArgs e)
        {
            if (GpuLists.SelectedItems != null && GpuLists.SelectedItems.Count > 0)
            {
                if (GpuLists.SelectedItems != null && GpuLists.SelectedItems.Count > 0)
                {
                    var fileName = GpuLists.SelectedItems[0].Text;

                    // Load the coin list from file
                    var file = GpuLists.SelectedItems[0].Tag.ToString();
                    var gpus = await JsonCrud.LoadGpuList(file);


                    SaveFileDialog saveFileDialog = new SaveFileDialog();

                    saveFileDialog.Filter = "json files (*.json)|*.json";
                    saveFileDialog.FileName = fileName;
                    saveFileDialog.RestoreDirectory = true;
                    saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    saveFileDialog.AddExtension = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        await JsonCrud.WriteToGpuListFile(saveFileDialog.FileName, gpus);
                    }
                }
            }
        }

        // Editing gpu list
        private async void EditGpuList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If nothing selected do nothing
            if (EditGpuList.SelectedItems.Count == 0 || GettingEbayPrice)
                return;

            var gpu = EditGpuList.SelectedItems[0].Tag as Gpu;

            GettingEbayPrice = true;
            HashratesProgressBar.Visible = true;
            HashratesProgressBar.Value = 90;

            // Reset list
            EditHashrates.Rows.Clear();

            // Display all info for the selected gpu
            AmdOrNvidia.Text = gpu.AmdOrNvidia;
            AmdOrNvidia.ForeColor = gpu.AmdOrNvidia.ToLower() == "amd" ? Color.Red : Color.Green;
            DateReleased.Text = gpu.DateReleased.ToString("Y");
            MSRP.Text = gpu.MSRP.ToString("0.00");
            EbayPrice.Text = gpu.EbayPrice.ToString();
            PricePaid.Text = gpu.PricePaid.ToString("0.00");
            GpuVersionPrefix.Text = gpu.VersionPrefix;
            GpuModelNumber.Text = gpu.ModelNumber;
            GpuVersionSuffix.Text = gpu.VersionSuffix;
            GpuVramSize.Text = gpu.VramSize.ToString();
            if (gpu.Lhr)
                LhrLabel.Text = "Lhr";
            else
                LhrLabel.Text = "";

            // Get ebay price
            try
            {
                // Clear previous data
                EbayItemSelection.Items.Clear();
                EbayItemUrl.Tag = "";

                var ebayItems = await GetEbayPrice(gpu);
                if (double.TryParse(ebayItems[0].Price.ToString(), out var ebayPrice))
                {
                    foreach (var item in ebayItems)
                    {
                        EbayItemSelection.Items.Add(new { Text = item.Name + " $" + ebayPrice.ToString("0.00"), Value = item });
                    }
                    EbayItemSelection.SelectedIndex = 0;
                }
            }
            catch (Exception ex) { }

            // Display the hashrate stats in the list
            foreach (var hashrate in gpu.Hashrates)
            {
                EditHashrates.Rows.Add(hashrate.Coin, hashrate.Algorithm, hashrate.HashrateSpeed, hashrate.Watts);
            }
            EditHashrates.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            GettingEbayPrice = false;
            HashratesProgressBar.Value = 0;
            HashratesProgressBar.Visible = false;
        }
        private void AddGpu_Click(object sender, EventArgs e)
        {
            var gpu = GetGpuInfoFromGui();
            var random = new Random();
            gpu.Id = random.Next(200-1000000000);

            if(UserEnteredValidGpuData(gpu))
                AddGpuToEditListView(gpu);
        }
        private void DeleteGpu_Click(object sender, EventArgs e)
        {
            // Delete selected gpu from list
            foreach (ListViewItem item in EditGpuList.SelectedItems)
            {
                EditGpuList.Items.Remove(item);
            }

            // Delete all hashrates
            EditHashrates.Rows.Clear();
            EditHashrates.Refresh();

            // Reset gpu info
            GpuModelNumber.Text = "";
            GpuVersionPrefix.Text = "";
            GpuVersionSuffix.Text = "";
            GpuVramSize.Text = "";
            AmdOrNvidia.Text = "";
            DateReleased.Text = "";
            MSRP.Text = "";
            EbayPrice.Text = "";
            PricePaid.Text = "";
        }
        private void AddGpuToEditListView(Gpu gpu)
        {
            var li = new ListViewItem();
            li.Text = gpu.Manufacturer;
            li.Tag = gpu;
            li.Name = gpu.Id.ToString();
            li.SubItems.Add(gpu.VersionPrefix);
            li.SubItems.Add(gpu.ModelNumber);
            li.SubItems.Add(gpu.VersionSuffix);
            li.SubItems.Add(gpu.VramSize.ToString());
            EditGpuList.Items.Add(li);
        }
        private Gpu GetGpuInfoFromGui()
        {
            var gpu = new Gpu();
            if (EditGpuList.SelectedItems == null)
                return gpu;   
            
            gpu = EditGpuList.SelectedItems[0].Tag as Gpu;
            gpu.VersionPrefix = GpuVersionPrefix.Text;
            gpu.ModelNumber = GpuModelNumber.Text;
            gpu.VersionSuffix = GpuVersionSuffix.Text;
            gpu.VramSize = int.TryParse(GpuVramSize.Text, out var vram) ? vram : 0;
            gpu.AmdOrNvidia = AmdOrNvidia.Text;
            gpu.DateReleased = DateTime.TryParse(DateReleased.Text, out var released) ? released : DateTime.Parse("April 2020");
            gpu.MSRP = double.TryParse(MSRP.Text, out var msrp) ? msrp : 0.0;
            gpu.EbayPrice = double.TryParse(EbayPrice.Text, out var ebayPrice) ? ebayPrice : 0.0;
            gpu.PricePaid = double.TryParse(PricePaid.Text, out var pricePaid) ? pricePaid : 0.00;
            return gpu;
        }
        private void UpdateGpuGUI()
        {
            if (EditGpuList.SelectedItems != null)
            {
                // Save the updated gpu info to the listview item
                EditGpuList.SelectedItems[0].Tag = GetGpuInfoFromGui(); 
                // Save list to file
                SaveGpuList();
            }
        }
        private async void SaveGpuList()
        {
            var gpus = new List<Gpu>();
            if (GpuListName.Text.Length > 0)
            {
                // Get list of all gpus
                for (int i = 0; i < EditGpuList.Items.Count; i++)
                    gpus.Add(EditGpuList.Items[i].Tag as Gpu);
            }
            await JsonCrud.SaveGpuList(gpus, GpuListsDirectory + GpuListName.Text + ".json");
        }
        private bool UserEnteredValidGpuData(Gpu gpu)
        {
            // Prevent missing data from getting saved
            if (String.IsNullOrEmpty(GpuModelNumber.Text) || String.IsNullOrWhiteSpace(GpuModelNumber.Text))
            {
                MessageBox.Show("Gpu Name must not be empty!");
                return false;
            }
            if (String.IsNullOrEmpty(GpuVersionPrefix.Text) || String.IsNullOrWhiteSpace(GpuVersionPrefix.Text))
            {
                MessageBox.Show("Gpu Version must not be empty! (i.e. gtx/rtx/rx )");
                return false;
            }
            if (String.IsNullOrEmpty(GpuVersionSuffix.Text) || String.IsNullOrWhiteSpace(GpuVersionSuffix.Text))
            {
                MessageBox.Show("Gpu Version must not be empty! (i.e. super/ti/xt");
                return false;
            }
            if (String.IsNullOrEmpty(GpuVramSize.Text) || String.IsNullOrWhiteSpace(GpuVramSize.Text))
            {
                MessageBox.Show("Gpu Vram Size must not be empty!");
                return false;
            }
            if (String.IsNullOrEmpty(AmdOrNvidia.Text) || String.IsNullOrWhiteSpace(AmdOrNvidia.Text))
            {
                MessageBox.Show("Amd / Nvidia must not be empty!");
                return false;
            }

            var pricesGiven = false;
            if (String.IsNullOrEmpty(MSRP.Text) || String.IsNullOrWhiteSpace(MSRP.Text))
                { }
            else
                pricesGiven = true;
            if (String.IsNullOrEmpty(EbayPrice.Text) || String.IsNullOrWhiteSpace(EbayPrice.Text))
                { }
            else
                pricesGiven = true;
            if (String.IsNullOrEmpty(PricePaid.Text) || String.IsNullOrWhiteSpace(PricePaid.Text))
                { }
            else
                pricesGiven = true;

            // If no prices given inform user it can impact the results
            if (!pricesGiven)
                MessageBox.Show("Warning! No price information given, this may impact the results! Consider putting in how much you paid for the gpu you are editing.");

            // Prevent incorrect data from being saved
            if (int.TryParse(GpuVramSize.Text, out var vram))
                gpu.VramSize = vram;
            else
            {
                MessageBox.Show("Vram Size must be a whole number!");
                return false;
            }

            if (DateTime.TryParse(DateReleased.Text, out var date))
                gpu.DateReleased = date;
            else
            {
                MessageBox.Show("Date must only be - Month Year - with a space between them! (i.e. April 2020");
                return false;
            }

            if (double.TryParse(MSRP.Text, out var msrp))
                gpu.MSRP = msrp;
            else
            {
                MessageBox.Show("MSRP must be a number!");
                return false;
            }

            if (double.TryParse(EbayPrice.Text, out var ebayPrice))
                gpu.EbayPrice = ebayPrice;
            else
            {
                MessageBox.Show("Ebay Price must be a number!");
                return false;
            }

            if (double.TryParse(PricePaid.Text, out var pricePaid))
                gpu.PricePaid = pricePaid;
            else
            {
                MessageBox.Show("Price Paid must be a number!");
                return false;
            }
            return true;
        }
        private void GpuVersionPrefix_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && EditGpuList.SelectedItems != null)
            {
                UpdateGpuGUI();
            }
        }
        private void GpuListName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && EditGpuList.SelectedItems != null)
            {
                UpdateGpuGUI();
            }
        }
        private void GpuModelNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && EditGpuList.SelectedItems != null)
            {
                UpdateGpuGUI();
            }
        }
        private void GpuVersionSuffix_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && EditGpuList.SelectedItems != null)
            {
                UpdateGpuGUI();
            }
        }
        private void GpuVramSize_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && EditGpuList.SelectedItems != null)
            {
                UpdateGpuGUI();
            }
        }
        private void AmdOrNvidia_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && EditGpuList.SelectedItems != null)
            {
                UpdateGpuGUI();
            }
        }
        private void DateReleased_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && EditGpuList.SelectedItems != null)
            {
                UpdateGpuGUI();
            }
        }
        private void MSRP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && EditGpuList.SelectedItems != null)
            {
                UpdateGpuGUI();
            }
        }
        private void EbayPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && EditGpuList.SelectedItems != null)
            {
                UpdateGpuGUI();
            }
        }
        private void PricePaid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && EditGpuList.SelectedItems != null)
            {
                UpdateGpuGUI();
            }
        }
        // Editing hashrates
        private void EditHashrates_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Make sure all columns in the row have data before saving
            var coin = EditHashrates[Constants.HashratesCoin, e.RowIndex].Value != null ? EditHashrates[Constants.HashratesCoin, e.RowIndex].Value.ToString() : "";
            var algo = EditHashrates[Constants.HashratesAlgo, e.RowIndex].Value != null ? EditHashrates[Constants.HashratesAlgo, e.RowIndex].Value.ToString() : "";
            var hashrate = EditHashrates[Constants.HashratesHashrate, e.RowIndex].Value != null ? EditHashrates[Constants.HashratesHashrate, e.RowIndex].Value.ToString() : "";
            var watts = EditHashrates[Constants.HashratesWatts, e.RowIndex].Value != null ? EditHashrates[Constants.HashratesWatts, e.RowIndex].Value.ToString() : "";

            // Validate users data
            var validData = true;
            if (coin.Length > 2)
            {

            }
            else
                validData = false;

            if (algo.Length > 2)
            {

            }
            else
                validData = false;

            if (double.TryParse(hashrate, out var parsedHash) && parsedHash > 0)
            {

            }
            else
                validData = false;

            if (int.TryParse(watts, out var parsedWatts) && parsedWatts > 0)
            {

            }
            else
                validData = false;

            if (validData)
            {
                // Save
                UpdateGpuHashrate();
                SaveGpuList();
            }

        }
        private void EditHashrates_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (EditHashrates.RowCount > 0)  
            {
                // Save
                UpdateGpuHashrate();
                SaveGpuList();
            }
        }
        private void SaveGpuHashrates_Click(object sender, EventArgs e)
        {
            // Inform user if there is no list name but they added gpus to the list
            if (GpuListName.Text.Length == 0 && EditGpuList.Items.Count > 0)
            {
                MessageBox.Show("Gpu List Name must not be empty! Please give the gpu list a name in the textbox above the list.");
                return;
            }

            EditGpuPanel.Visible = false;
            EnableUserInput();
            
            UpdateGpuHashrate();

            SaveGpuList(); // Save list to file
        }
        private void UpdateGpuHashrate()
        {
            var gpu = EditGpuList.SelectedItems[0].Tag as Gpu;

            var hashrates = new List<Hashrate>();
            // Get data from list
            for (int x = 0; x <= EditHashrates.RowCount - 1; x++)
            {
                if (EditHashrates[0, x].Value != null)
                {
                    var hashOnList = new Hashrate();
                    hashOnList.Coin = EditHashrates[0, x].Value.ToString();
                    hashOnList.Algorithm = EditHashrates[1, x].Value.ToString();
                    hashOnList.HashrateSpeed = double.TryParse(EditHashrates[2, x].Value.ToString(), out var hashrate) ? hashrate : 0.0;
                    hashOnList.Watts = int.TryParse(EditHashrates[3, x].Value.ToString(), out var watts) ? watts : 0;
                    hashrates.Add(hashOnList);
                }
            }
            gpu.Hashrates = new List<Hashrate>();// Remove old hashrates
            gpu.Hashrates.AddRange(hashrates); // Add updated hashrates to this gpu
            EditGpuList.SelectedItems[0].Tag = gpu; // Update gpu in edit gpulist
        }
        private void CancelGpuHashrates_Click(object sender, EventArgs e)
        {
            // If user has gpus on the list, but the list doesn't have a name and user clicks cancel make sure they really want to discard this list
            if (EditGpuList.Items.Count > 0 && GpuListName.Text.Length == 0)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to discard this list?", "Discard List Confirmation", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    // continue to discard this list
                }
                else if (dialogResult == DialogResult.No)
                {
                    return; // User doesn't want to discard this list
                }
            }

            // Remove this window and enable user input
            EditGpuPanel.Visible = false;
            EnableUserInput();

            // Remove list if user clicked to add new list but then clicked cancel
            if (GpuLists.SelectedItems[0].Text == "")
            {
                // New list created but user didn't rename it (assuming user created new list then clicked cancel)
                GpuLists.Items.Remove(GpuLists.SelectedItems[0]);
                return;
            }
        }
        // Allow user to copy / paste hashrate rows
        private void Hashrates_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewTextBoxEditingControl tb)
            {
                tb.KeyDown -= Hashrates_KeyDown;
                tb.KeyDown += Hashrates_KeyDown;
            }
        }
        private void Hashrates_KeyDown(object sender, KeyEventArgs e)
        {
            //ToDo make a dropdown option for coin and algorithm

            // User is copying hashrate row
            if (e.KeyData == (Keys.Control | Keys.C))
            {
                CopiedHashrate.Coin = EditHashrates.SelectedRows[0].Cells[0].Value.ToString();
                CopiedHashrate.Algorithm = EditHashrates.SelectedRows[0].Cells[1].Value.ToString();
                CopiedHashrate.HashrateSpeed = double.Parse(EditHashrates.SelectedRows[0].Cells[2].Value.ToString());
                CopiedHashrate.Watts = int.Parse(EditHashrates.SelectedRows[0].Cells[3].Value.ToString());
            }
            // User is pasting hashrate row
            else if (e.KeyData == (Keys.Control | Keys.V))
            {
                // Make sure the user isn't trying to add an entry for the same coin
                var addHashrate = true;
                for (int x = 0; x <= EditHashrates.RowCount - 1; x++)
                {
                    if (EditHashrates[0, x].Value != null)
                    {
                        var hashOnList = new Hashrate();
                        hashOnList.Coin = EditHashrates[0, x].Value.ToString();
                        hashOnList.Algorithm = EditHashrates[1, x].Value.ToString();
                        hashOnList.HashrateSpeed = double.Parse(EditHashrates[2, x].Value.ToString());
                        hashOnList.Watts = int.Parse(EditHashrates[3, x].Value.ToString());

                        // If there is already an entry for this coin on the list don't add it
                        if (hashOnList.Coin == CopiedHashrate.Coin)
                            addHashrate = false;
                    }
                }

                if (addHashrate)
                    EditHashrates.Rows.Add(CopiedHashrate.Coin, CopiedHashrate.Algorithm, CopiedHashrate.HashrateSpeed, CopiedHashrate.Watts);
            }
            else if (e.KeyCode == Keys.Delete && EditHashrates.SelectedRows.Count > 0)
            {
                EditHashrates.Rows.Remove(EditHashrates.SelectedRows[0]);
            }
        }


        // Get/Change ebay item
        private async Task<List<EbayItem>> GetEbayPrice(Gpu gpu)
        {
            var ebayItems = new List<EbayItem>();
            // If the gpu is the same (i.e. 3080 asus tuf then 3080 evga ftw3, etc.)
            // don't search Ebay again, load previous data, also only refresh previous
            // data if it has been more than 4 hours since it was last searched            


            foreach (List<EbayItem> searchResults in previousSearchResults)
            {
                if (searchResults.Count == 0)
                    continue;
                if (searchResults[0].Name == gpu.Name && DateTime.Compare(searchResults[0].LastUpdated.AddHours(4), DateTime.Now) > 0)
                {
                    return searchResults;
                }

            }

            // If it hasn't been searched for yet, or its been over 4hrs since
            // last search then search now
            ebayItems = await ebay.GetLowestPrice(gpu);

            // And save the data for future searches
            previousSearchResults.Add(ebayItems);

            return ebayItems;
        }
        private async void GetEbayPrice_Click(object sender, EventArgs e)
        {
            if (EditGpuList.SelectedItems.Count > 0)
            {
                var gpu = EditGpuList.SelectedItems[0].Tag as Gpu;
                var price = await GetEbayPrice(gpu);
                if (price != null && price.Count > 0)
                    EbayPrice.Text = price[0].Price.ToString("0.00");
                else
                {
                    EbayPrice.Text = "0";
                    MessageBox.Show("Unfortunately I was unable to find any ebay listings for this gpu from a reputable seller.");
                }
            }

        }
        private void ResultsEbayLink_Click(object sender, EventArgs e)
        {
            // Open ebay item link in browser
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = ResultsEbayLink.Tag.ToString();
            p.Start();
        }
        private void EbayItemUrl_Click(object sender, EventArgs e)
        {
            if (EbayItemUrl.Tag == null)
            {
                MessageBox.Show("No ebay url found! If you see an ebay price it is from the last known listing.");
                return;
            }
            // Open ebay item link in browser
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = EbayItemUrl.Tag.ToString();
            p.Start();
        }
        private void EbayItemSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            EbayItemSelection.SelectedItem.ToString();
            var ebayItem = (EbayItemSelection.SelectedItem as dynamic).Value;

            EbayPrice.Text = ebayItem.Price.ToString("0.00");
            EbayItemUrl.Tag = ebayItem.Url;
        }
        private void ResultsEbayItemSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ebayItem = (ResultsEbayItemSelection.SelectedItem as dynamic).Value;
            ResultsEbayLink.Tag = ebayItem.Url;
            // ToDo find gpu in results list and update its ebay price to the one selected
        }

        
        // Filters
        private bool FilterGpu(Gpu gpu)
        {
            // Apply filters
            if (gpu.AmdOrNvidia.ToLower() == "amd" && !ShowAmd.Checked)                
                return true; // remove this item
            if (gpu.AmdOrNvidia.ToLower() == "nvidia" && !ShowNvidia.Checked)
                return true; // remove this item
            if (int.TryParse(VramFilter.Text, out var vram) && vram > 0 && gpu.VramSize <= int.Parse(VramFilter.Text))
                return true;
            
            return false;
        }
        private void FilterResults()
        {
            // Go through all gpus in results and apply filters
            var itemsToShow = new List<ListViewItem>();
            foreach (ListViewItem item in CurrentSearchResults)
            {
                var gpu = item.Tag as Gpu;
                if (!FilterGpu(gpu))
                    itemsToShow.Add(item); // Show this item
            }
            // Clear results list
            ResultsList.Items.Clear();

            // Add gpus to show
            foreach (ListViewItem item in itemsToShow)
                ResultsList.Items.Add(item);
        }
        private void ShowNvidia_CheckedChanged(object sender, EventArgs e)
        {
            if (!PerformingCalculations)
            {
                FilterResults();
                if (lastStateShowNvidia)
                    ShowNvidia.Checked = false;
                else
                    ShowNvidia.Checked = true;
                lastStateShowNvidia = ShowNvidia.Checked;
            }
            else
            {
                ShowNvidia.Checked = lastStateShowNvidia;
                ShowErrorMessage(Constants.PerformingCalcs);
            }
        }
        private void ShowAmd_CheckedChanged(object sender, EventArgs e)
        {
            if (!PerformingCalculations)
            {
                FilterResults();
                if (lastStateShowAmd)
                    ShowAmd.Checked = false;
                else
                    ShowAmd.Checked = true;
                lastStateShowAmd = ShowAmd.Checked;
            }
            else
            {
                ShowAmd.Checked = lastStateShowAmd;
                ShowErrorMessage(Constants.PerformingCalcs);
            }
        }
        private void VramFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if(PerformingCalculations && e.KeyCode == Keys.Enter)
            {
                ShowErrorMessage(Constants.PerformingCalcs);
                return;
            }
            if(!PerformingCalculations && e.KeyCode == Keys.Enter)
            {
                FilterResults();
            }
        }

        private void HodlPrice_TextChanged(object sender, EventArgs e)
        {
            // If user inputs an amount while performing cals it will use that number in the middle of the process and corrupt the data
            if (PerformingCalculations)
            {
                ShowErrorMessage(Constants.PerformingCalcs);
                HodlPrice.Text = "";
            }
        }


    }
}
