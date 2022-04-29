using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Dynamic;
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
        private bool BusyGettingAllMinerStatCoinInfos = false;
        private Timer minerstatTimer = new Timer();
        private Timer errorMessageTimer = new Timer();
        private Timer errorCountDownTimer = new Timer();
        private int countDown = 10;
        private Ebay ebay = new Ebay();
        private List<List<EbayItem>> previousSearchResults = new List<List<EbayItem>>();
        private Hashrate CopiedHashrate = new Hashrate();
        private ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();
        private List<ListViewItem> CurrentSearchResults = new List<ListViewItem>();
        private List<Gpu> gpus = new List<Gpu>();
        private bool PerformingCalculations = false;
        private bool lastStateShowAmd = true;
        private bool lastStateShowNvidia = true;
        private bool GettingEbayPrice = false;
        private bool searchingByHodlPrice = false;
        private List<EbayItem> EbayItems = new List<EbayItem>();
        private List<Coin> coins = new List<Coin>();
        private bool hideHashrateCoinsMenu = true;
        private ContextMenuStrip resultsContextMenuStrip = new ContextMenuStrip();

        public MaxGpuRoi()
        {
            InitializeComponent();
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
            BusyGettingAllMinerStatCoinInfos = true;
            AllCoinInfos = await MinerStat.GetAllCoins();
            BusyGettingAllMinerStatCoinInfos = false;
        }


        // On Load / Exit
        private void MaxGpuRoi_Load(object sender, EventArgs e)
        {
            // Reduce flickering
            ResultsList.SetDoubleBuffered(true);
            EditGpuList.SetDoubleBuffered(true);
            EditCoinList.SetDoubleBuffered(true);
            ListOfAllCoins.SetDoubleBuffered(true);
            DoubleBuffered = true;

            // Set window size
            Size = new Size(1873, 1300);
            MaximumSize = new Size(1873, 1300);

            // Hide the edit gpu/coin list panel
            EditGpuPanel.SendToBack();
            EditCoinPanel.SendToBack();

            // Put the Edit coinlist panel in the proper place
            EditCoinPanel.Location = new Point(310, 85);

            // Setup Gpu lists 
            GpuLists.Columns.Add("Name", -2, HorizontalAlignment.Left);
            GpuLists.Columns.Add("# of Gpus", -2, HorizontalAlignment.Center);
            GpuLists.Columns[0].Width = 110;
            GpuLists.Columns[1].Width = 70;

            // Setup List of All Coins
            ListOfAllCoins.Columns.Add("Name", -2, HorizontalAlignment.Left);
            ListOfAllCoins.Columns.Add("Symbol", -2, HorizontalAlignment.Center);
            ListOfAllCoins.Columns.Add("Algorithm", -2, HorizontalAlignment.Left);
            ListOfAllCoins.Columns.Add("Price", -2, HorizontalAlignment.Left);
            ListOfAllCoins.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            // Setup Coin lists 
            CoinLists.Columns.Add("Name", -2, HorizontalAlignment.Left);
            CoinLists.Columns.Add("# of Coins", -2, HorizontalAlignment.Center);
            CoinLists.Columns[0].Width = 110;
            CoinLists.Columns[1].Width = 70;

            // Setup Edit Coin list 342
            EditCoinList.Columns.Add("Name", -2, HorizontalAlignment.Center);
            EditCoinList.Columns.Add("Symbol", -2, HorizontalAlignment.Center);
            EditCoinList.Columns.Add("Algorithm", -2, HorizontalAlignment.Center);
            EditCoinList.Columns[0].Width = 150;
            EditCoinList.Columns[1].Width = 60;
            EditCoinList.Columns[2].Width = 100;

            // Setup Edit Gpu list
            EditGpuList.Columns.Add("Maker", -2, HorizontalAlignment.Center);
            EditGpuList.Columns.Add("Prefix", -2, HorizontalAlignment.Center);
            EditGpuList.Columns.Add("Model", -2, HorizontalAlignment.Center);
            EditGpuList.Columns.Add("Suffix", -2, HorizontalAlignment.Center);
            EditGpuList.Columns.Add("Vram", -2, HorizontalAlignment.Center);
            EditGpuList.Columns[0].Width = 48;
            EditGpuList.Columns[1].Width = 44;
            EditGpuList.Columns[2].Width = 83;
            EditGpuList.Columns[3].Width = 84;
            EditGpuList.Columns[4].Width = 42;

            // Setup Edit Gpu Hashrates
            EditHashrates.Columns.Add("", "Coin");
            EditHashrates.Columns.Add("", "Algorithm");
            EditHashrates.Columns.Add("", "Hashrate");
            EditHashrates.Columns.Add("", "Watts");
            EditHashrates.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

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

            // Setup totals list
            TotalsList.Columns.Add("Coin", -2, HorizontalAlignment.Left);
            TotalsList.Columns.Add("Gpu's", -2, HorizontalAlignment.Center);
            TotalsList.Columns.Add("Gpu Costs", -2, HorizontalAlignment.Center);
            TotalsList.Columns.Add("$/Mhs", -2, HorizontalAlignment.Center);
            TotalsList.Columns.Add("Usd Costs", -2, HorizontalAlignment.Center);
            TotalsList.Columns.Add("Crypto Costs per day/week/month", -2, HorizontalAlignment.Center);
            TotalsList.Columns.Add("Hashrate", -2, HorizontalAlignment.Center);
            TotalsList.Columns.Add("Watts", -2, HorizontalAlignment.Center);
            TotalsList.Columns.Add("Efficiency", -2, HorizontalAlignment.Center);
            TotalsList.Columns.Add("Usd Rewards", -2, HorizontalAlignment.Center);
            TotalsList.Columns.Add("Crypto Rewards", -2, HorizontalAlignment.Center);
            TotalsList.Columns.Add("Usd Profits", -2, HorizontalAlignment.Center);
            TotalsList.Columns.Add("Crypto Profits", -2, HorizontalAlignment.Center);
            TotalsList.Columns.Add("R-O-I", -2, HorizontalAlignment.Center);
            TotalsList.Columns[0].Width = 69;
            TotalsList.Columns[1].Width = 45;
            TotalsList.Columns[2].Width = 80;
            TotalsList.Columns[3].Width = 60;
            TotalsList.Columns[4].Width = 150;
            TotalsList.Columns[5].Width = 280;
            TotalsList.Columns[6].Width = 70;
            TotalsList.Columns[7].Width = 50;
            TotalsList.Columns[8].Width = 90;
            TotalsList.Columns[9].Width = 150;
            TotalsList.Columns[10].Width = 280;
            TotalsList.Columns[11].Width = 150;
            TotalsList.Columns[12].Width = 280;
            TotalsList.Columns[13].Width = 80;
            


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
                        gpus = JsonCrud.LoadGpuList(filePath);
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
                        gpus = JsonCrud.LoadGpuList(filePath);

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
                        coins = JsonCrud.LoadCoinList(filePath);
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
                        coins = JsonCrud.LoadCoinList(filePath);

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


        // Results
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var listViewItemsToAdd = new List<ListViewItem>();
            var bufferSize = 5;

            // Start the progress bar
            progressBar.InvokeIfRequired(c => { c.Maximum = gpus.Count + 10; c.Value = 10; });
            int progress = 10;

            // Go through each gpu
            foreach (var gpu in gpus)
            {
                // Apply filters
                if (FilterGpu(gpu))
                    continue;

                var calculations = GetGpuCalculations(gpu);
                var calculation = new Calculation();

                // Only add gpus that have good hashrate info and calculations were performed on them
                if (calculations.Count > 0)
                {
                    // If hodl price isn't empty get the highest usd profits per hashrate calculation in gpu
                    if (searchingByHodlPrice)
                    {
                        // Get highest Usd Profits and only show gpus with seected hodl coin
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

                    // Create listview item
                    var li = CreateResultsListviewItem(gpu, calculation);

                    // Add this gpu and calculation to the results list
                    listViewItemsToAdd.Add(li);

                    // Save master list to apply filters to and prevent unnecessary internet calls 
                    CurrentSearchResults.Add(li);

                    if (listViewItemsToAdd.Count == bufferSize)
                    {
                        ResultsList.InvokeIfRequired(c => { c.Items.AddRange(listViewItemsToAdd.ToArray()); });
                        listViewItemsToAdd = new List<ListViewItem>();
                    }

                    // Update progress bar
                    progress++;
                    backgroundWorker.ReportProgress(progress);
                }
            }
            // Add remaining items
            ResultsList.InvokeIfRequired(c => { c.Items.AddRange(listViewItemsToAdd.ToArray()); });
        }
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Calculate and show the totals
            var total = CalculateTotals();
            PopulateTotalsGui(total);

            // Reset
            searchingByHodlPrice = false;
            progressBar.Value = 0;
            PerformingCalculations = false;
        }
        private void MaxMyROI_Click(object sender, EventArgs e)
        {
            if (PerformingCalculations)
            {
                ShowErrorMessage(Constants.PerformingCalcs);
                return;
            }

            // Error proofing
            if (GpuLists.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a gpu list.");
                return;
            }
            // Error proofing to allow only whole numbers for miner/pool fee
            if (!double.TryParse(PoolMinerFee.Text, out var fee))
            {
                MessageBox.Show("Please make sure the Miner/Pool Fee is a number.");
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
            TotalsList.Items.Clear();
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

                var lvItemsToAdd = new List<ListViewItem>();
                var buffer = 5;

                progressBar.Value = 10;
                progressBar.Maximum = gpus.Count + 10;

                // Go through each gpu
                foreach (var gpu in gpus)
                {
                    progressBar.Increment(1);

                    // Sort by shortest roi
                    var calculations = GetGpuCalculations(gpu);
                    calculations.Sort((y, x) => x.ROI.CompareTo(y.ROI));

                    // Add this gpu and calculation to the results list
                    //lvItemsToAdd.Add(CreateResultsListviewItem(calculations[0]));

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

            if(!backgroundWorker.IsBusy)
                backgroundWorker.RunWorkerAsync();    
        }
        private ListViewItem CreateResultsListviewItem(Gpu gpu, Calculation calculation)
        {
            // Get the corresponding hashrate for the calculation
            var hashrate = new Hashrate();
            foreach (var hash in gpu.Hashrates)
                if (hash.Coin == calculation.Coin)
                    hashrate = hash;

            var li = new ListViewItem();
            dynamic gpuAndCalculation = new ExpandoObject();
            gpuAndCalculation.Gpu = gpu; 
            gpuAndCalculation.Calculation = calculation;
            li.Tag = gpuAndCalculation;
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
            
            if(double.IsInfinity(calculation.CryptoElectricityCost))
                calculation.CryptoElectricityCost = 0;

            var cryptoCosts = decimal.Round((decimal)(calculation.CryptoPoolMinerFeeCost + calculation.CryptoElectricityCost), Constants.DigitsToRound);
            li.SubItems.Add(cryptoCosts.ToString() + " / " + (cryptoCosts * 7).ToString() + " / " + (cryptoCosts * 30));
            li.SubItems.Add(hashrate.HashrateSpeed.ToString());
            li.SubItems.Add(hashrate.Watts.ToString());
            li.SubItems.Add(calculation.Efficiency.ToString("0.000") + " kw/mhs");
            li.SubItems.Add("$" + calculation.UsdRewards.ToString("0.00") + " / $" + (calculation.UsdRewards * 7).ToString("0.00") + " / $" + (calculation.UsdRewards * 30).ToString("0.00"));

            if (double.IsInfinity(calculation.CryptoRewards))
                calculation.CryptoRewards = 0;

            var cryptoRewards = decimal.Round((decimal)calculation.CryptoRewards, Constants.DigitsToRound);
            li.SubItems.Add(cryptoRewards.ToString("0.00") + " " + hashrate.Coin + " / " + (cryptoRewards * 7).ToString("0.00") + " " + hashrate.Coin + " / " + (cryptoRewards * 30).ToString("0.00") + " " + hashrate.Coin);
            li.SubItems.Add("$" + calculation.UsdProfits.ToString("0.00") + " / $" + (calculation.UsdProfits * 7).ToString("0.00") + " / $" + (calculation.UsdProfits * 30).ToString("0.00"));

            if (double.IsInfinity(calculation.CryptoProfits))
                calculation.CryptoProfits = 0;

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
        private List<Calculation> GetGpuCalculations(Gpu gpu)
        {
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
                                    GetEbayPrice(gpu);
                                    foreach (var item in EbayItems)
                                    {
                                        if (double.TryParse(item.Price.ToString(), out var ebayPrice))
                                        {
                                            ebayCalculation = Calculation.Calculate(ebayPrice, hashrate.HashrateSpeed, hashrate.Watts, hashrate.Coin, electricityRate, fee);

                                            // If returned results are too good to be true try the next result
                                            double.TryParse(EbayItems[EbayItems.Count - 1].Price.ToString(), out var mostExpensiveItem);
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
                        var hodlCoin = "";
                        var hodlePrice = "";
                        if (HodlCoin.InvokeRequired)
                        {
                            HodlCoin.Invoke(new MethodInvoker(delegate {
                                hodlCoin = HodlCoin.SelectedItem != null ? HodlCoin.SelectedItem.ToString() : "";
                                hodlePrice = HodlPrice.Text;
                            }));
                        }
                        else
                        {
                            hodlCoin = HodlCoin.SelectedItem.ToString();
                            hodlePrice = HodlPrice.Text;
                        }
                        if (coin.Symbol.ToLower() == hodlCoin.ToLower() && double.TryParse(hodlePrice, out var hodlPrice) && hodlPrice > 0)
                            calculation = Calculation.Calculate(gpuCost, hashrate.HashrateSpeed, hashrate.Watts, hashrate.Coin, electricityRate, fee, hodlPrice); // Check hodl coin price
                        else
                            calculation = Calculation.Calculate(gpuCost, hashrate.HashrateSpeed, hashrate.Watts, hashrate.Coin, electricityRate, fee); // Check real time coin price
                        calculation.Coin = hashrate.Coin;
                        calculation.GpuId = gpu.Id;
                        calculations.Add(calculation);
                    }
                }
            }
            return calculations;
        }
        private void ResultsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ResultsList.SelectedItems.Count > 0)
            {
                //var gpu = ResultsList.SelectedItems[0].Tag as Gpu;
                dynamic gpuAndCalc = ResultsList.SelectedItems[0].Tag;
                var gpu = gpuAndCalc.Gpu;
                ResultsEbayLink.Tag = (gpu.EbayLink != null && gpu.EbayLink != "unknown") ? gpu.EbayLink : "";

                // Get ebay price
                try
                {
                    // Clear previous data
                    ResultsEbayItemSelection.Items.Clear();
                    EbayItemUrl.Tag = "";

                    GetEbayPrice(gpu);

                    foreach (var item in EbayItems)
                    {
                        if (double.TryParse(item.Price.ToString(), out double ePrice))
                        {                            
                            ResultsEbayItemSelection.Items.Add("id " + item.Id + " $" + item.Price + " " + item.Name);
                            EbayItemUrl.Tag = item.Url;
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
        // Results gpu right click menu
        void cms_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            resultsContextMenuStrip.Items.Clear();

            ToolStripMenuItem gpuToolStripMenuItem1 = new ToolStripMenuItem();
            gpuToolStripMenuItem1.DropDownItemClicked += GpuToolStripMenuItem1_DropDownItemClicked;
            gpuToolStripMenuItem1.Text = "Add to Gpu List";

            // Populate the ContextMenuStrip control
            foreach (ListViewItem item in GpuLists.Items)
                gpuToolStripMenuItem1.DropDownItems.Add(item.Text);
            resultsContextMenuStrip.Items.Add(gpuToolStripMenuItem1);

            //resultsContextMenuStrip.Items.Add("-");
            //resultsContextMenuStrip.Items.Add("Remove Gpu");

            e.Cancel = false;
        }
        private void GpuToolStripMenuItem1_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var selectedItem = e.ClickedItem.Text;
            var gpuAndCalc = ResultsList.SelectedItems[0].Tag as dynamic;
            var gpu = gpuAndCalc.Gpu;

            // Get the selected gpu list and load it
            var file = GpuListsDirectory + selectedItem + ".json";
            var gpuListToAddTo = JsonCrud.LoadGpuList(file);

            // Give the gpu a unique id then add gpu to the list
            gpu.Id = new Random().Next(300, 10000);
            gpuListToAddTo.Add(gpu);

            // Increase gpu count in gui for the list that was just added to
            foreach (ListViewItem gpuListName in GpuLists.Items)
                if (gpuListName.Text == selectedItem)
                    gpuListName.SubItems[1].Text = (int.Parse(gpuListName.SubItems[1].Text) + 1).ToString();

            // Save gpu list to file
            JsonCrud.SaveGpuList(gpuListToAddTo, file);
        }
        private void ResultsList_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var focusedItem = ResultsList.FocusedItem;
                if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
                {
                    // ContextMenuStrip
                    resultsContextMenuStrip.Opening += new CancelEventHandler(cms_Opening);
                    resultsContextMenuStrip.ItemClicked += ResultsContextMenuStrip_ItemClicked;
                    ResultsList.ContextMenuStrip = resultsContextMenuStrip;
                }
            }
        }
        private void ResultsContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var selectedItem = e.ClickedItem.Text;
        }

        // Totals
        private dynamic CalculateTotals()
        {
            dynamic total = new List<ExpandoObject>();
            for (int i = 0; i < 4; i++)
            {
                dynamic t = new ExpandoObject();
                t.Coin = "";
                t.CryptoCosts = 0;
                t.CryptoRewards = 0;
                t.CryptoProfits = 0;
                t.GpuCosts = 0;
                t.CostPerMhs = 0;
                t.UsdCosts = 0;
                t.Hashrate = 0;
                t.Watts = 0;
                t.Efficiency = 0;
                t.UsdRewards = 0;
                t.UsdProfits = 0;
                t.ROI = 0;
                t.GpuCount = 0;

                total.Add(t);
            }

            // If there are results
            if (ResultsList.Items.Count > 0)
            {
                int coinSetter = 0;
                foreach (ListViewItem result in ResultsList.Items)
                {
                    dynamic gpuAndCalc = result.Tag;
                    var gpu = gpuAndCalc.Gpu;
                    var calculation = gpuAndCalc.Calculation;

                    // Get the corresponding hashrate for the calculation
                    var hashrate = new Hashrate();
                    foreach (var hash in gpu.Hashrates)
                        if (hash.Coin == calculation.Coin)
                            hashrate = hash;

                    if (calculation != null)
                    {
                        // Add up to 3 different crypto rewards
                        if (coinSetter == 0)
                        {
                            total[1].Coin = calculation.Coin;
                            coinSetter++;
                        }
                        else if (coinSetter == 1 && total[1].Coin.ToLower() != calculation.Coin.ToLower())
                        {
                            total[2].Coin = calculation.Coin;
                            coinSetter++;
                        }
                        else if (coinSetter == 2 && total[1].Coin != calculation.Coin && total[2].Coin != calculation.Coin)
                        {
                            total[3].Coin = calculation.Coin;
                            coinSetter++;
                        }
                       
                        
                        int index = 0;

                        if (hashrate.Coin == total[1].Coin)
                        {
                            total[1].Coin = hashrate.Coin;
                            index = 1;
                        }
                        else if (hashrate.Coin == total[2].Coin)
                        {
                            total[2].Coin = hashrate.Coin;
                            index = 2;
                        }
                        else if (hashrate.Coin == total[3].Coin)
                        {
                            total[3].Coin = hashrate.Coin;
                            index = 3;
                        }

                        // If this is one of the 3 coins we are going to show
                        if (index > 0)
                        {
                            // Add the results up
                            var cryptoCosts = calculation.CryptoElectricityCost + calculation.CryptoPoolMinerFeeCost;
                            total[index].CryptoCosts += cryptoCosts;
                            total[index].CryptoRewards += calculation.CryptoRewards;
                            total[index].CryptoProfits += calculation.CryptoProfits;
                            total[index].GpuCosts += calculation.GpuCosts;
                            total[index].CostPerMhs += calculation.CostPerMhs;
                            total[index].UsdCosts += calculation.UsdElectricityCost + calculation.UsdPoolMinerFeeCost;
                            total[index].Hashrate += hashrate.HashrateSpeed;
                            total[index].Watts += hashrate.Watts;
                            total[index].Efficiency += calculation.Efficiency;
                            total[index].UsdRewards += calculation.UsdRewards;
                            total[index].UsdProfits += calculation.UsdProfits;
                            total[index].ROI += calculation.ROI;
                            total[index].GpuCount += 1;
                        }

                        // Add each result
                        total[0].Coin = "USD";
                        total[0].CryptoCosts = 0; // Doesn't make sense to add different crypto currencies toegether
                        total[0].CryptoRewards = 0;
                        total[0].CryptoProfits = 0;
                        total[0].GpuCosts += calculation.GpuCosts;
                        total[0].CostPerMhs += calculation.CostPerMhs;
                        total[0].UsdCosts += calculation.UsdElectricityCost + calculation.UsdPoolMinerFeeCost;
                        total[0].Hashrate += hashrate.HashrateSpeed;
                        total[0].Watts += hashrate.Watts;
                        total[0].Efficiency += calculation.Efficiency;
                        total[0].UsdRewards += calculation.UsdRewards;
                        total[0].UsdProfits += calculation.UsdProfits;
                        total[0].ROI += calculation.ROI;
                        total[0].GpuCount += 1;
                    }
                }
            }
            return total;
        }
        private void PopulateTotalsGui(dynamic total)
        {
            TotalsList.Items.Clear();

            // Show up to 3 different crypto results
            for (int i = 0; i < 4; i++)
            {
                var li = new ListViewItem();
                li.Text = total[i].Coin;
                li.ImageIndex = GetImageIndex(total[i].Coin);
                li.SubItems.Add(total[i].GpuCount.ToString());
                li.SubItems.Add("$" + total[i].GpuCosts.ToString("0.00"));
                li.SubItems.Add("$" + total[i].CostPerMhs.ToString("0.00"));

                // Usd Costs
                li.SubItems.Add("$" + total[i].UsdCosts.ToString("0.00") + " / $"
                    + (total[i].UsdCosts * 7).ToString("0.00") + " / $" + (total[i].UsdCosts * 30).ToString("0.00"));

                // Crypto Costs
                if (double.IsInfinity(total[i].CryptoCosts))
                    total[i].CryptoCosts = 0;
                var cryptoCosts = decimal.Round((decimal)total[i].CryptoCosts, Constants.DigitsToRound);
                li.SubItems.Add(cryptoCosts.ToString() + " " + total[i].Coin 
                    + " / " + (cryptoCosts * 7).ToString() + " " + total[i].Coin + " / " 
                    + (cryptoCosts * 30).ToString() + " " + total[i].Coin);

                // Hashrate / Watts / Efficiency
                li.SubItems.Add(total[i].Hashrate.ToString("0.00"));
                li.SubItems.Add(total[i].Watts.ToString("0.00"));
                li.SubItems.Add(total[i].Efficiency.ToString("0.00") + " kw/mhs");

                // Usd Rewards
                li.SubItems.Add("$" + total[i].UsdRewards.ToString("0.00") + " / $" 
                    + (total[i].UsdRewards * 7).ToString("0.00") + " / $" + (total[i].UsdRewards * 30).ToString("0.00"));

                // Crypto Rewards
                if (double.IsInfinity(total[i].CryptoRewards))
                    total[i].CryptoRewards = 0;
                var cryptoRewards = decimal.Round((decimal)total[i].CryptoRewards, Constants.DigitsToRound);
                li.SubItems.Add(cryptoRewards.ToString() + " " + total[i].Coin
                    + " / " + (cryptoRewards * 7).ToString() + " " + total[i].Coin + " / "
                    + (cryptoRewards * 30).ToString() + " " + total[i].Coin);

                // Usd Profits
                li.SubItems.Add("$" + total[i].UsdProfits.ToString("0.00") + " / $"
                    + (total[i].UsdProfits * 7).ToString("0.00") + " / $" + (total[i].UsdProfits * 30).ToString("0.00"));

                // Crypto Profits
                if (double.IsInfinity(total[i].CryptoProfits))
                    total[i].CryptoProfits = 0;
                var cryptoProfits = decimal.Round((decimal)total[i].CryptoProfits, Constants.DigitsToRound);
                li.SubItems.Add(cryptoProfits.ToString() + " " + total[i].Coin
                    + " / " + (cryptoProfits * 7).ToString() + " " + total[i].Coin + " / "
                    + (cryptoProfits * 30).ToString() + " " + total[i].Coin);

                // ROI
                li.SubItems.Add(total[i].ROI.ToString("0.00") + " months");

                TotalsList.Items.Add(li);
            }
        }


        // Hodl
        private void PopulateHodlMenu()
        {
            // Clear previous coin list
            HodlCoin.Items.Clear();

            // Populate user coin list to hodl coin dropdown menu
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
        private void HodlPrice_TextChanged(object sender, EventArgs e)
        {
            // If user inputs an amount while performing cals it will use that number in the middle of the process and corrupt the data
            if (PerformingCalculations)
            {
                ShowErrorMessage(Constants.PerformingCalcs);
                HodlPrice.Text = "";
            }
        }

        // Coin list
        private List<Coin> GetCoinsFromList()
        {
            var coinListFileName = CoinListsDirectory + CoinLists.InvokeIfRequired(c => { return c.SelectedItems[0].Text; }) + ".json";
            var coins = JsonCrud.LoadCoinList(coinListFileName);
            return coins;
        }
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

            // Disable user input on the main GUI
            DisableUserInput();

            // If this is a new list
            if (CoinLists.SelectedItems[0].Tag == null)
            {
                CoinListName.Text = "";
                coins = new List<Coin>();
            }
            else
            {
                // Load the coin list from file
                GetCoinsFromList();

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
            }

            // Show the edit panel
            EditCoinPanel.BringToFront();

            // Only call minerstat api if there is no coin info 
            if (AllCoinInfos.Count == 0 && !BusyGettingAllMinerStatCoinInfos)
            {
                try { AllCoinInfos = await MinerStat.GetAllCoins(); }
                catch { }
            }

            // Show list of all available coins
            foreach (var coinInfo in AllCoinInfos)
            {
                if (coinInfo.type == "coin")
                {
                    // Extract only useful data from minerstats coinInfo
                    var coin = new Coin(coinInfo);
                    var li = new ListViewItem(coin.Name);
                    li.Name = coin.Name;
                    li.Tag = coin;
                    li.ImageIndex = 0; // ToDo: Select correct image based on coin
                    li.SubItems.Add(coin.Symbol);
                    li.SubItems.Add(coin.Algorithm);
                    var price = decimal.Round((decimal)coin.Price, Constants.DigitsToRound);
                    if (price < 0)
                        li.SubItems.Add("N/A");
                    else
                        li.SubItems.Add("$" + price.ToString());
                    ListOfAllCoins.Items.Add(li);
                }
            }

            ListOfAllCoins.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            // If this is the default list disable editing the list name
            if (CoinListName.Text == "Default")
                CoinListName.Enabled = false;
            else
                CoinListName.Enabled = true;
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
        private void ImportCoinLists_Click(object sender, EventArgs e)
        {
            var file = GetFileFromUser();
            var coins = GetCoinsFromList();

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
            // If the user isn't creating a new list
            if (CoinLists.SelectedItems.Count > 0 && CoinLists.SelectedItems[0].Text != "")
            {
                // And we are not currently performing calculations
                if (PerformingCalculations)
                {
                    ShowErrorMessage(Constants.PerformingCalcs); // We are performing calcultions so inform the user
                    return;
                }

                // Load the coins
                coins = GetCoinsFromList();

                // Add coins to hodl drop down menu
                PopulateHodlMenu();
            }
        }
        private void ExportCoinLists_Click(object sender, EventArgs e)
        {
            if (CoinLists.SelectedItems != null && CoinLists.SelectedItems.Count > 0)
            {
                var fileName = CoinLists.SelectedItems[0].Text;

                // Load the coin list from file
                var coins = GetCoinsFromList();

                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Filter = "json files (*.json)|*.json";
                saveFileDialog.FileName = fileName;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    JsonCrud.WriteToCoinListFile(saveFileDialog.FileName, coins);    
                }
            }
        }

        
        // Edit coin list
        private void SaveCoinList_Click(object sender, EventArgs e)
        {
            // Inform user if there is no list name but they added coins to the list
            if (CoinListName.Text.Length == 0 && EditCoinList.Items.Count > 0)
            {
                MessageBox.Show("Please give the coin list a name in the textbox above the list.");
                return;
            }
            else if (EditCoinList.Items.Count == 0)
                MessageBox.Show("Please add at least one coin to the list in order to save it.");

            // Update coinlist name and count
            CoinLists.SelectedItems[0].Text = CoinListName.Text;
            CoinLists.SelectedItems[0].SubItems[1].Text = EditCoinList.Items.Count.ToString();

            // Hide this window and enable user input for main gui
            EditCoinPanel.SendToBack();
            EnableUserInput();

            // Extract each coin from the listview
            var coins = new List<Coin>();
            foreach (ListViewItem item in EditCoinList.Items)
            {
                var coin = item.Tag as Coin;
                coins.Add(coin);
            }
            JsonCrud.SaveCoinList(coins, CoinListsDirectory + CoinListName.Text + ".json");
            
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
            EditCoinPanel.SendToBack();
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
                // Don't add duplicates
                if (coinOnEditList.Name == coinToAdd.Name && coinToAdd.Algorithm == coinOnEditList.Algorithm && coinToAdd.Symbol == coinOnEditList.Symbol)
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
        private void EditCoinList_KeyDown(object sender, KeyEventArgs e)
        {
            DeleteCoin.PerformClick();
        }
        private void ListOfAllCoins_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                AddCoin.PerformClick();
        }
        private void CoinListName_KeyDown(object sender, KeyEventArgs e)
        {
            // If user presses enter after editing coin list name
            if (e.KeyData == Keys.Enter && CoinLists.SelectedItems != null && CoinLists.SelectedItems.Count > 0)
            {
                // Save list to file
                if (CoinListName.Text.Length > 0)
                {
                    // Get the old/new file names
                    var oldFileName = CoinListsDirectory + CoinLists.SelectedItems[0].Text + ".json";
                    var newFileName = CoinListsDirectory + CoinListName.Text + ".json";

                    // Rename the coin list name in list of coin lists
                    CoinLists.SelectedItems[0].Text = CoinListName.Text;

                    // Rename the coin list file
                    File.Move(oldFileName, newFileName);
                }
                else
                    MessageBox.Show("The coin list needs to have a name.");

            }
        }

        
        // Gpu list
        private async Task<List<Gpu>> GetGpusFromList()
        {
            return await Task.Factory.StartNew(() =>
            {
                var gpuListFileName = GpuListsDirectory + GpuLists.InvokeIfRequired(c => { return c.SelectedItems[0].Text; }) + ".json";
                var gpus = JsonCrud.LoadGpuList(gpuListFileName);
                return gpus;            
            });
        }
        private void AddNewGpuToEditList()
        {
            if (GpuLists.SelectedItems[0].Tag == null)
            {
                var gpu = new Gpu();
                gpu.Manufacturer = "nvidia";
                gpu.Id = 0;
                gpu.VersionPrefix = "rtx";
                gpu.ModelNumber = "3090";
                gpu.VersionSuffix = "ti";
                gpu.VramSize = 24;
                gpu.MSRP = 1999.00;
                gpu.PricePaid = 2400.00;
                CreateGpuListViewItem(gpu);
                EditGpuList.Items[0].Selected = true;
                GpuListName.Text = "";
                return;
            }
        }
        private void EditGpuLists_Click(object sender, EventArgs e)
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

            // Load gpu list from file
            if (GpuLists.SelectedItems.Count == 1 && GpuLists.SelectedItems[0].Text != "")
                gpus = JsonCrud.LoadGpuList(GpuListsDirectory + GpuLists.SelectedItems[0].Text + ".json");
            else
                gpus = new List<Gpu>();

            // Disable user input on the main GUI
            DisableUserInput();          
            
            // Show the edit gpu list panel
            EditGpuPanel.BringToFront();

            // If this is a new list add one new entry
            if (GpuLists.SelectedItems[0].Text == "")
            {
                var gpu = new Gpu();
                var random = new Random();
                gpu.Id = random.Next(200, 1000000000);
                EditGpuList.Items.Add(CreateGpuListViewItem(gpu));
                EditGpuList.Items[0].Selected = true;
                GpuListName.Text = "";
                GpuListName.Enabled = true;
                return;
            }

            // Add gpu to list
            foreach (var gpu in gpus)
                EditGpuList.Items.Add(CreateGpuListViewItem(gpu));

            // Set the list name
            GpuListName.Text = GpuLists.SelectedItems[0].Text;

            // Select first gpu in the list
            EditGpuList.Items[0].Selected = true;

            // If this is the default list disable editing the list name
            if(GpuListName.Text == "Default")
                GpuListName.Enabled = false;
            else
                GpuListName.Enabled = true;

            // Give the gpu list focus
            EditGpuList.Focus();
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
            var gpuListFileName = GpuListsDirectory + GpuLists.SelectedItems[0].Text; 
            gpus = await GetGpusFromList();

            // Add listview item
            var li = new ListViewItem();
            li.Tag = gpuListFileName;
            li.Text = Path.GetFileNameWithoutExtension(gpuListFileName);
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
        private async void GpuLists_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If the user isn't creating a new list
            if (GpuLists.SelectedItems.Count > 0 && GpuLists.SelectedItems[0].Text != "")
            {
                // And we are not currently performing calculations
                if (PerformingCalculations)
                {
                    // We are performing calcultions so inform the user
                    ShowErrorMessage(Constants.PerformingCalcs);
                    return;
                }
                // Get the list of gpus from the file
                gpus = await GetGpusFromList();
            }
        }
        private void ExportGpuLists_Click(object sender, EventArgs e)
        {
            if (GpuLists.SelectedItems != null && GpuLists.SelectedItems.Count > 0)
            {
                if (GpuLists.SelectedItems != null && GpuLists.SelectedItems.Count > 0)
                {
                    var fileName = GpuLists.SelectedItems[0].Text; // Get the list name

                    SaveFileDialog saveFileDialog = new SaveFileDialog();

                    saveFileDialog.Filter = "json files (*.json)|*.json";
                    saveFileDialog.FileName = fileName;
                    saveFileDialog.RestoreDirectory = true;
                    saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    saveFileDialog.AddExtension = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        JsonCrud.WriteToGpuListFile(saveFileDialog.FileName, gpus);
                    }
                }
            }
        }

        
        // Editing gpu list
        private void EditGpuList_SelectedIndexChanged(object sender, EventArgs e)
        {            
            // Clear previous data
            EditHashrates.Rows.Clear();
            EbayItemSelection.Items.Clear();
            EbayItemUrl.Tag = "";

            // If nothing selected do nothing
            if (EditGpuList.SelectedItems.Count == 0 || GettingEbayPrice)
                return;

            // User creating new gpu list, Empty the current list of gpus
            if (GpuLists.SelectedItems[0].Text == "")
                gpus = new List<Gpu>();

            var gpu = EditGpuList.SelectedItems[0].Tag as Gpu;
            
            // Update gui
            UpdateGpuPanelInfo(gpu);

            GetEbayListings(gpu);
        }
        private void AddGpu_Click(object sender, EventArgs e)
        {
            var gpu = GetGpuPanelInfo();

            // Generate a unique id for this gpu
            var random = new Random();
            gpu.Id = random.Next(200, 1000000000);

            // Add new gpu to Gui for editing
            if (UserEnteredValidGpuData(gpu))
            {
                var li = CreateGpuListViewItem(gpu);
                li.Selected = true;
                EditGpuList.Items.Add(li);
            }
            // Add new gpu to master list
            gpus.Add(gpu);
        }
        private void EditGpuList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && EditGpuList.SelectedItems != null)
            {
                // Get the next items index
                var nextItemIndex = EditGpuList.SelectedItems[0].Index - 1;

                // Remove selected gpu from the list
                gpus.Remove(EditGpuList.SelectedItems[0].Tag as Gpu);
                EditGpuList.Items.Remove(EditGpuList.SelectedItems[0]);

                // Update the list
                SaveGpuList();

                // Select the next item in the edit gpu list
                if (nextItemIndex >= 0)
                    EditGpuList.Items[nextItemIndex].Selected = true;

            }
        }
        private ListViewItem CreateGpuListViewItem(Gpu gpu)
        {
            var li = new ListViewItem();
            li.Text = gpu.Manufacturer;
            li.Tag = gpu;
            li.Name = gpu.Id.ToString();
            li.SubItems.Add(gpu.VersionPrefix);
            
            // Add Lhr to model name if an lhr card
            var modelNum = gpu.ModelNumber;
            if (GpuIsLhr(gpu) && gpu.Lhr)
                modelNum += " Lhr";
            li.SubItems.Add(modelNum);

            li.SubItems.Add(gpu.VersionSuffix);
            li.SubItems.Add(gpu.VramSize.ToString());
            return li;
        }
        private void UpdateGpuPanelInfo(Gpu gpu)
        {
            // Display all info for the selected gpu
            AmdOrNvidia.Text = gpu.Manufacturer;
            AmdOrNvidia.ForeColor = gpu.Manufacturer.ToLower() == "amd" ? Color.Red : Color.Green;
            DateReleased.Text = gpu.DateReleased.ToString("Y");
            MSRP.Text = gpu.MSRP.ToString("0.00");
            EbayPrice.Text = gpu.EbayPrice.ToString();
            PricePaid.Text = gpu.PricePaid.ToString("0.00");
            GpuVersionPrefix.Text = gpu.VersionPrefix;
            GpuModelNumber.Text = gpu.ModelNumber;
            GpuVersionSuffix.Text = gpu.VersionSuffix;
            GpuVramSize.Text = gpu.VramSize.ToString();

            // If this gpu is a lhr/non-lhr 
            if (GpuIsLhr(gpu))
            {
                if (gpu.Lhr)
                    Lhr.Checked = true;                
                else
                    Lhr.Checked = false;                
            }
        }
        private Gpu GetGpuPanelInfo()
        {
            var gpu = new Gpu();           

            gpu.VersionPrefix = GpuVersionPrefix.Text;
            
            // Remove Lhr from name if an lhr card
            var modelNum = GpuModelNumber.Text;
            if (GpuIsLhr(gpu) && gpu.Lhr)
                modelNum = modelNum.Replace(" Lhr", "");
            gpu.ModelNumber = modelNum;
            
            gpu.VersionSuffix = GpuVersionSuffix.Text;
            gpu.VramSize = int.TryParse(GpuVramSize.Text, out var vram) ? vram : 0;
            gpu.Manufacturer = AmdOrNvidia.Text;
            gpu.DateReleased = DateTime.TryParse(DateReleased.Text, out var released) ? released : DateTime.Parse("April 2020");
            gpu.MSRP = double.TryParse(MSRP.Text, out var msrp) ? msrp : 0.0;
            gpu.EbayPrice = double.TryParse(EbayPrice.Text, out var ebayPrice) ? ebayPrice : 0.0;
            gpu.PricePaid = double.TryParse(PricePaid.Text, out var pricePaid) ? pricePaid : 0.00;
            gpu.Lhr = Lhr.Checked;
            return gpu;
        }
        private void SaveGpuList()
        {
            if (GpuListName.Text.Length > 0)
            {
                // Save master gpu list to file
                JsonCrud.SaveGpuList(gpus, GpuListsDirectory + GpuListName.Text + ".json");
            }
            else
                MessageBox.Show("The gpu list needs to have a name.");
        }
        private void UpdateGpuList()
        {
            if (EditGpuList.SelectedItems != null && EditGpuList.SelectedItems.Count > 0)
            {
                // Get old gpu to remove from lists
                var oldGpu = EditGpuList.SelectedItems[0].Tag as Gpu;

                // Extract new gpu info from gui
                var newGpu = GetGpuPanelInfo();
                newGpu.Hashrates = GetSelectedGpuHashratesFromGui();

                // Remove the old gpu data only if user entered valid gpu data
                if (UserEnteredValidGpuData(newGpu))
                {
                    // Remove old gpu from lists
                    int oldIndex = EditGpuList.SelectedItems[0].Index;
                    EditGpuList.Items.Remove(EditGpuList.SelectedItems[0]);
                    gpus.Remove(oldGpu);

                    // Add updated gpu to lists
                    gpus.Add(newGpu);
                    var li = CreateGpuListViewItem(newGpu);
                    li.Selected = true;
                    EditGpuList.Items.Insert(oldIndex, li); // Reselect this gpu

                    // Update gui
                    UpdateGpuPanelInfo(newGpu);

                    // Save updated list to file
                    SaveGpuList();
                }
            }
        }
        private bool UserEnteredValidGpuData(Gpu gpu)
        {
            // Prevent missing/invalid data from getting saved
            if (String.IsNullOrWhiteSpace(GpuModelNumber.Text) || GpuModelNumber.Text.Length == 0)
            {
                MessageBox.Show("Please give the Gpu a name.");
                return false;
            }
            if (String.IsNullOrWhiteSpace(GpuVersionPrefix.Text) || GpuVersionPrefix.Text.Length == 0)
            {
                MessageBox.Show("Please give the Gpu's Version prefix. (i.e. gtx/rtx/rx )");
                return false;
            }
            if (String.IsNullOrWhiteSpace(GpuVramSize.Text) || GpuVramSize.Text.Length == 0 || !int.TryParse(GpuVramSize.Text, out var vram))
            {
                MessageBox.Show("Please give the Gpu a vram size.");
                return false;
            }
            else
                gpu.VramSize = vram;
            if (String.IsNullOrEmpty(AmdOrNvidia.Text) || String.IsNullOrWhiteSpace(AmdOrNvidia.Text))
            {
                MessageBox.Show("Please give the Gpu a maker. (Amd / Nvidia)");
                return false;
            }

            var pricesGiven = false;
            if (!String.IsNullOrWhiteSpace(MSRP.Text) && MSRP.Text.Length > 0)
                pricesGiven = true;
            if (String.IsNullOrWhiteSpace(EbayPrice.Text) || EbayPrice.Text.Length > 0)
                pricesGiven = true;
            if (String.IsNullOrWhiteSpace(PricePaid.Text) && PricePaid.Text.Length > 0)
                pricesGiven = true;

            // If no prices given inform user it can impact the results
            if (!pricesGiven)
                MessageBox.Show("Warning! No price information given, this may impact the results! Consider putting in how much you paid for the Gpu you are editing.");



            if (DateTime.TryParse(DateReleased.Text, out var date))
                gpu.DateReleased = date;
            else
            {
                MessageBox.Show("Please make sure the date format is [ Month Year ] with only a space between them. (i.e. April 2020");
                return false;
            }

            if (double.TryParse(MSRP.Text, out var msrp))
                gpu.MSRP = msrp;
            else
            {
                MessageBox.Show("Please make sure the MSRP is a number.");
                return false;
            }

            if (double.TryParse(EbayPrice.Text, out var ebayPrice))
                gpu.EbayPrice = ebayPrice;
            else
            {
                MessageBox.Show("Please make sure the Ebay Price is a number.");
                return false;
            }

            if (double.TryParse(PricePaid.Text, out var pricePaid))
                gpu.PricePaid = pricePaid;
            else
            {
                MessageBox.Show("Please make sure the Price Paid is a number!");
                return false;
            }
            return true;
        }
        private void GpuListName_KeyDown(object sender, KeyEventArgs e)
        {
            // User presses enter to rename gpu list
            if (e.KeyData == Keys.Enter && GpuLists.SelectedItems != null && GpuLists.Items.Count > 0)
            {
                var gpuList = new List<Gpu>();
                if (GpuListName.Text.Length > 0)
                {
                    // Get the old/new file names
                    var oldFileName = GpuListsDirectory + GpuLists.SelectedItems[0].Text + ".json";
                    var newFileName = GpuListsDirectory + GpuListName.Text + ".json";

                    // Rename the gpu list name in list of gpu lists
                    GpuLists.SelectedItems[0].Text = GpuListName.Text;

                    // Rename the gpu list file
                    var fileRenamed = false;
                    var errorMessage = "";
                    try { File.Move(oldFileName, newFileName); fileRenamed = true; }
                    catch (Exception ex) { errorMessage = ex.Message; }

                    // Save updated gpu list to file
                    JsonCrud.SaveGpuList(gpus, GpuListsDirectory + GpuListName.Text + ".json");
                    fileRenamed = true;

                    // If the file wasn't renamed successfully inform the user
                    if (!fileRenamed)
                        MessageBox.Show("Unable to rename { " + oldFileName + " } to { " + newFileName + "} because " + errorMessage);
                }
                else
                    MessageBox.Show("The gpu list needs to have a name.");
            }
        }
        private void Lhr_Click(object sender, EventArgs e)
        {
            UpdateGpuList();
            SaveGpuList();
        }
        private void GpuVersionPrefix_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                UpdateGpuList();
                SaveGpuList();
            }
        }
        private void GpuModelNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                UpdateGpuList();
                SaveGpuList();
            }
        }
        private void GpuVersionSuffix_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                UpdateGpuList();
                SaveGpuList();
            }
        }
        private void GpuVramSize_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                UpdateGpuList();
                SaveGpuList();
            }
        }
        private void AmdOrNvidia_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                UpdateGpuList();
                SaveGpuList();
            }
        }
        private void DateReleased_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                UpdateGpuList();
                SaveGpuList();
            }
        }
        private void MSRP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                UpdateGpuList();
                SaveGpuList();
            }
        }
        private void EbayPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                UpdateGpuList();
                SaveGpuList();
            }
        }
        private void PricePaid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                UpdateGpuList();
                SaveGpuList();
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
                UpdateGpuList();
                SaveGpuList();
            }
        }
        private void EditHashrates_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (EditHashrates.RowCount > 0)  
            {
                // Save
                GetSelectedGpuHashratesFromGui();
                SaveGpuList();
            }
        }              
        private void EditHashrates_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                HashrateCoinsMenu.Visible = true;
                HashrateCoinsMenu.DroppedDown = true;

                // Populate list only if it is empty 
                if (HashrateCoinsMenu.Items.Count == 0)
                {
                    var allCoins = JsonCrud.LoadCoinList(CoinListsDirectory + "Default.json");
                    foreach (var coin in allCoins)
                        HashrateCoinsMenu.Items.Add(coin.Symbol);
                }

                // Don't select a coin since there isn't one on the hashrate list yet
                if (EditHashrates[e.ColumnIndex, e.RowIndex].Value == null)
                    return;

                // Select the coin on the hashrate list
                hideHashrateCoinsMenu = false;
                var coinSelected = EditHashrates[e.ColumnIndex, e.RowIndex].Value.ToString();
                foreach (var item in HashrateCoinsMenu.Items)
                    if (item.ToString().ToLower() == coinSelected.ToLower())
                    {
                        HashrateCoinsMenu.SelectedItem = item;
                        hideHashrateCoinsMenu = true;
                        break;
                    }
            }
        }
        private void HashrateCoinsMenu_Click(object sender, EventArgs e)
        {
            HashrateCoinsMenu.Visible = false;
        }
        private void HashrateCoinsMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (hideHashrateCoinsMenu)
            {
                HashrateCoinsMenu.Visible = false;
                // User made a selection, save it to the datagrid
                EditHashrates.SelectedRows[0].Cells[0].Value = HashrateCoinsMenu.Text;

                // Get algorithm name from coin list
                var allCoins = JsonCrud.LoadCoinList(CoinListsDirectory + "Default.json");
                foreach (var coin in allCoins)
                    if (HashrateCoinsMenu.Text.ToLower() == coin.Symbol.ToLower())
                        EditHashrates.SelectedRows[0].Cells[1].Value = coin.Name;

                // Update gpu list
                UpdateGpuList();
            }
        }
        private void HashrateCoinsMenu_Leave(object sender, EventArgs e)
        {
            HashrateCoinsMenu.Visible = false;
        }
        private void SaveGpuHashrates_Click(object sender, EventArgs e)
        {
            // Inform user if there is no list name but they added gpus to the list
            if (GpuListName.Text.Length == 0 && EditGpuList.Items != null && EditGpuList.Items.Count > 0)
            {
                MessageBox.Show("Please give the Gpu list a name in the textbox above the list.");
                return;
            }

            // Hide this window and enable the main gui
            EditGpuPanel.SendToBack();
            EnableUserInput();

            UpdateGpuList();
            SaveGpuList();
        }
        private List<Hashrate> GetSelectedGpuHashratesFromGui()
        {
            var hashrates = new List<Hashrate>();
            // Get data from gui list
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
            return hashrates;
        }
        private void CancelGpuHashrates_Click(object sender, EventArgs e)
        {
            if (GpuListName.Text == "")
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

            // Hide this window and enable user input
            EditGpuPanel.SendToBack();
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
            if (e.KeyData == (Keys.Control | Keys.C) && !EditHashrates.IsCurrentCellInEditMode)
            {
                CopiedHashrate.Coin = EditHashrates.SelectedRows[0].Cells[0].Value.ToString();
                CopiedHashrate.Algorithm = EditHashrates.SelectedRows[0].Cells[1].Value.ToString();
                CopiedHashrate.HashrateSpeed = double.Parse(EditHashrates.SelectedRows[0].Cells[2].Value.ToString());
                CopiedHashrate.Watts = int.Parse(EditHashrates.SelectedRows[0].Cells[3].Value.ToString());
            }
            // User is pasting hashrate row
            else if (e.KeyData == (Keys.Control | Keys.V) && !EditHashrates.IsCurrentCellInEditMode)
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
                {
                    EditHashrates.Rows.Add(CopiedHashrate.Coin, CopiedHashrate.Algorithm, CopiedHashrate.HashrateSpeed, CopiedHashrate.Watts);
                    UpdateGpuList();
                }
            }
            else if (e.KeyCode == Keys.Delete && !EditHashrates.IsCurrentCellInEditMode && EditHashrates.SelectedRows.Count > 0)
            {
                EditHashrates.Rows.Remove(EditHashrates.SelectedRows[0]);
                UpdateGpuList();
            }
            else if (e.KeyCode == Keys.Enter && EditHashrates.SelectedRows.Count > 0)
            {
                UpdateGpuList();
            }
        }

        
        // Get/Change ebay item
        private void GetEbayPrice(Gpu gpu)
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
                    EbayItems = searchResults;
                    return;
                }

            }

            // If it hasn't been searched for yet, or its been over 4hrs since
            // last search then search now
            ebayItems = ebay.GetLowestPrice(gpu);

            // And save the data for future searches
            previousSearchResults.Add(ebayItems);

            EbayItems = ebayItems;
        }
        private void GetEbayPrice_Click(object sender, EventArgs e)
        {
            if (EditGpuList.SelectedItems.Count > 0)
            {
                var gpu = EditGpuList.SelectedItems[0].Tag as Gpu;
                GetEbayPrice(gpu);
                if (EbayItems != null && EbayItems.Count > 0)
                {
                    EbayPrice.Text = EbayItems[0].Price.ToString("0.00");
                    EbayItemUrl.Tag = EbayItems[0].Url;
                }
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
            // Get the selected ebay id 
            var startIndex = EbayItemSelection.SelectedItem.ToString().IndexOf("id ") + 3;
            var endIndex = EbayItemSelection.SelectedItem.ToString().IndexOf(" ", startIndex);
            var ebayId = EbayItemSelection.SelectedItem.ToString().Substring(startIndex, endIndex - startIndex);

            // To find the corresponding ebay listing
            foreach (var item in EbayItems)
                if(ebayId == item.Id)
                {
                    EbayPrice.Text = item.Price.ToString("0.00");
                    EbayItemUrl.Tag = item.Url;
                }
        }
        private void ResultsEbayItemSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ebayItem = (ResultsEbayItemSelection.SelectedItem as dynamic).Value;
            ResultsEbayLink.Tag = ebayItem.Url;

            // Find gpu in results list and update its ebay price to the one selected
            if(ResultsList.SelectedItems.Count > 0)
                ResultsList.SelectedItems[0].SubItems[Constants.EbayPrice].Text = "$" + ebayItem.Price.ToString("0.00");            
        }
        private void GetEbayListings(Gpu gpu)
        {
            // Show progress bar
            GettingEbayPrice = true;
            HashratesProgressBar.Visible = true;
            HashratesProgressBar.Value = 90;

            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (object sender, DoWorkEventArgs e) =>
            {
                // Get ebay price
                try
                {
                    GetEbayPrice(gpu);
                    int counter = 10;
                    if (double.TryParse(EbayItems[0].Price.ToString(), out var ebayPrice))
                    {
                        e.Result = EbayItems;
                        worker.ReportProgress(counter);
                        counter++;
                    }
                }
                catch (Exception ex) { }
            };
            worker.ProgressChanged += (object sender, ProgressChangedEventArgs e) =>
            {
                HashratesProgressBar.Value = e.ProgressPercentage;
            };
            worker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
            {
                // Add ebay listings if any found
                var ebayItems = e.Result as List<EbayItem>;
                if (ebayItems != null)
                {
                    EbayPrice.Text = ebayItems[0].Price.ToString();
                    foreach (var item in ebayItems)
                    {
                        EbayItemSelection.Items.Add("id " + item.Id + " $" + item.Price + " " + item.Name);
                    }                        
                    
                    EbayItemSelection.SelectedIndex = 0;
                }

                // Display the hashrate stats in the list
                foreach (var hashrate in gpu.Hashrates)
                    EditHashrates.Rows.Add(hashrate.Coin, hashrate.Algorithm, hashrate.HashrateSpeed, hashrate.Watts);                

                // Hide progress bar
                GettingEbayPrice = false;
                HashratesProgressBar.Value = 0;
                HashratesProgressBar.Visible = false;
            };
            worker.RunWorkerAsync();
        }


        // Filters
        private bool FilterGpu(Gpu gpu)
        {
            // Apply filters
            if (gpu.Manufacturer.ToLower() == "amd" && !ShowAmd.Checked)                
                return true; // remove this item
            if (gpu.Manufacturer.ToLower() == "nvidia" && !ShowNvidia.Checked)
                return true; // remove this item
            if (int.TryParse(VramFilter.Text, out var vram) && vram > 0 && gpu.VramSize <= int.Parse(VramFilter.Text))
                return true;
            
            return false;
        }
        private void FilterResults()
        {
            // Go through all gpus in results and apply filters
            var itemsToShow = new List<ListViewItem>();
            var progress = 0;
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


        // Misc
        #region
        // Allow user to move window
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
        private bool GpuIsLhr(Gpu gpu)
        {
            // If this gpu is a lhr/non-lhr 
            if (gpu.Manufacturer.ToLower() == "nvidia" &&
                ((int.TryParse(gpu.ModelNumber, out var modelNum) && modelNum > 3050 && modelNum != 3090))
                || gpu.ModelNumber.ToLower() == "3080 12gb" )
                return true;
            return false;
        }
        private void ShowErrorMessage(string message)
        {
            errorCountDownTimer.Enabled = true;
            errorMessageTimer.Enabled = true;
            ErrorMessagePanel.Visible = true;
            ErrorMessage.Text = message;
            return;
        }
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
            HodlCoin.Enabled = false;
            HodlPrice.Enabled = false;
            MaxMyROI.Enabled = false;
            Budget.Enabled = false;
            VramFilter.Enabled = false;
            AddGpuList.Enabled = false;
            AddCoinList.Enabled = false;
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
            HodlCoin.Enabled = true;
            HodlPrice.Enabled = true;
            MaxMyROI.Enabled = true;
            Budget.Enabled = true;
            VramFilter.Enabled = true;
            AddGpuList.Enabled = true;
            AddCoinList.Enabled = true;
        }
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
    }
}
