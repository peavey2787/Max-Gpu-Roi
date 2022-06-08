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
        private Timer minerstatTimer = new Timer();
        private Timer errorMessageTimer = new Timer();
        private Timer errorCountDownTimer = new Timer();
        private int errorMessageCountDown = 10;
        private Ebay ebay = new Ebay();
        private List<List<EbayItem>> previousSearchResults = new List<List<EbayItem>>();
        private Hashrate CopiedHashrate = new Hashrate();
        private ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();
        private List<ListViewItem> CurrentSearchResults = new List<ListViewItem>();
        private List<ListViewItem> filteredSearchResults = new List<ListViewItem>();
        private List<Gpu> defaultGpus = new List<Gpu>();
        private Task PerformingCalculations;
        private bool lastStateShowAmd = true;
        private bool lastStateShowNvidia = true;
        private bool GettingEbayPrice = false;
        private bool searchingByHodlPrice = false;
        private List<CoinInfo> defaultCoins = new List<CoinInfo>();
        private ContextMenuStrip resultsContextMenuStrip = new ContextMenuStrip();
        private bool userChangedEbaySelection = true;
        private Random random = new Random(500 - 10000000);

        public MaxGpuRoi()
        {
            InitializeComponent();
        }

        // Timers
        private void OnErrorCountDownTimerTickEvent(object sender, ElapsedEventArgs e)
        {
            // only run for 10 secs when called
            if (errorMessageCountDown > 0)
            {
                if (ErrorMessageCountDown.InvokeRequired)
                {
                    ErrorMessageCountDown.Invoke(new MethodInvoker(delegate { ErrorMessageCountDown.Text = errorMessageCountDown.ToString() + " secs"; }));
                }
                else
                    ErrorMessageCountDown.Text = errorMessageCountDown.ToString() + " secs";
                errorMessageCountDown--;
            }
            else if (errorMessageCountDown == 0)
            {
                errorCountDownTimer.Enabled = false;
                errorMessageCountDown = 10;
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
            defaultCoins = await MinerStat.GetAllCoins();
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
            Size = new Size(1873, 1170);
            MaximumSize = new Size(1873, 1170);

            // Hide the edit gpu/coin list panel
            EditGpuPanel.SendToBack();
            EditCoinPanel.SendToBack();
            SelectedGpuListName.Text = "";

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
            EditCoinList.Columns.Add("Price", -2, HorizontalAlignment.Center);
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
            EditHashrates.Columns.Add("", "Size");
            EditHashrates.Columns.Add("", "Watts");
            EditHashrates.Columns.Add("", "Coin2");
            EditHashrates.Columns.Add("", "Algorithm2");
            EditHashrates.Columns.Add("", "Hashrate2");
            EditHashrates.Columns.Add("", "Size2");
            EditHashrates.Columns.Add("", "Watts2");
            EditHashrates.Columns[0].Width = 60;
            EditHashrates.Columns[1].Width = 130;
            EditHashrates.Columns[2].Width = 80;
            EditHashrates.Columns[3].Width = 60;
            EditHashrates.Columns[4].Width = 60;
            EditHashrates.Columns[5].Width = 60;
            EditHashrates.Columns[6].Width = 130;
            EditHashrates.Columns[7].Width = 80;
            EditHashrates.Columns[8].Width = 60;
            EditHashrates.Columns[9].Width = 60;
            EditHashrates.Columns[Constants.HashratesAlgo].ReadOnly = true;
            EditHashrates.Columns[Constants.HashratesAlgo2].ReadOnly = true;

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
            ResultsList.Columns.Add("Break Even", -2, HorizontalAlignment.Center);
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

            LoadDefaults();

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

            LoadBackgroundImage();
        }
        private async void LoadDefaults()
        {
            // Add Default coin list item            
            CoinLists.Items.Add("Default");
            defaultCoins = await MinerStat.GetAllCoins();
            CoinLists.Items[0].SubItems.Add(defaultCoins.Count.ToString());
            CoinLists.Items[0].Tag = defaultCoins;

            // Make sure coinlist directory exists, if not create it
            if (!Directory.Exists(CoinListsDirectory))
                Directory.CreateDirectory(CoinListsDirectory);
            else
            {
                var coinLists = Directory.GetFiles(CoinListsDirectory);

                // Get any user created coin lists
                foreach (var filePath in coinLists)
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var coins = JsonCrud.LoadCoinList(filePath);                   

                    if (coins != null && coins.Count > 0)
                    {
                        var li = new ListViewItem(fileName);
                        li.Name = fileName;
                        li.Tag = coins;
                        li.SubItems.Add(coins.Count.ToString());
                        CoinLists.Items.Add(li);
                    }
                }
            }

            // Select the default coinlist
            if (CoinLists.Items.Count > 0)
                CoinLists.Items[0].Selected = true;


            // Add Default gpu list item            
            GpuLists.Items.Add("Default");

            var defaultLoaded = false;

            // Make sure gpulist directory exists, if not create it
            if (!Directory.Exists(GpuListsDirectory))
                Directory.CreateDirectory(GpuListsDirectory);
            else
            {
                var gpuLists = Directory.GetFiles(GpuListsDirectory);

                // Get any user created gpu lists
                foreach (var filePath in gpuLists)
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var gpus = JsonCrud.LoadGpuList(filePath);

                    if (fileName == "Default")
                    {
                        defaultLoaded = true;
                        defaultGpus = gpus;
                        GpuLists.Items[0].SubItems.Add(gpus.Count.ToString());
                        GpuLists.Items[0].Tag = gpus;
                        continue;
                    }

                    if (gpus != null && gpus.Count > 0)
                    {
                        var li = new ListViewItem(fileName);
                        li.Name = fileName;
                        li.Tag = gpus;
                        li.SubItems.Add(gpus.Count.ToString());
                        GpuLists.Items.Add(li);
                    }
                }
            }

            if (!defaultLoaded)
            {
                // Generate a new list
                defaultGpus = await MinerStat.GetAllGpus(defaultCoins);
                GpuLists.Items[0].SubItems.Add(defaultGpus.Count.ToString());
                GpuLists.Items[0].Tag = defaultGpus;
                
                // Save the list to file
                JsonCrud.SaveGpuList(defaultGpus, GpuListsDirectory + "Default" + ".json");
            }

            // Select the default gpulist
            if (GpuLists.Items.Count > 0)
                GpuLists.Items[0].Selected = true;
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void MaxGpuRoi_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal && this.BackgroundImage == null)
                LoadBackgroundImage();
            else if (WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Minimized;
                this.BackgroundImage = null;
            }
        }
        private void LoadBackgroundImage()
        {
            Task<Bitmap>.Factory.StartNew(() =>
            {
                Object rm = Properties.Resources.ResourceManager.GetObject("space-background");
                Bitmap myImage = (Bitmap)rm;
                return myImage;
            }).ContinueWith(t =>
            {
                this.BackgroundImage = t.Result;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        // Results
        private void MaxMyROI_Click(object sender, EventArgs e)
        {
            if (PerformingCalculations != null && !PerformingCalculations.IsCompleted)
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
                SortResults(Constants.UsdProfits); // Sort by column
            }
            else
            {
                SortResults(Constants.Roi); // Sort by column        
                SortResults(Constants.Roi);
            }

            // Clear old results
            progressBar.Value = 0;
            TotalsList.Items.Clear();
            ResultsList.Items.Clear();
            CurrentSearchResults = new List<ListViewItem>();
            ResultsEbayItemSelection.Items.Clear();
            ResultsEbayLink.Tag = null;
            userChangedEbaySelection = false;
            filteredSearchResults.Clear();

            #region budget

            // If budget is given then show most profitable configuration
            /*if (double.TryParse(Budget.Text, out var budget) && budget > 0)
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
                //progressBar.Maximum = gpus.Count + 10;

                // Go through each gpu
                foreach (var gpu in gpus)
                {
                    progressBar.Increment(1);

                    // Sort by shortest roi
                    //var calculations = await GetGpuCalculations(gpu);
                    //calculations.Sort((y, x) => x.ROI.CompareTo(y.ROI));

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

                // if they can afford the top one find out how many of them they can get
                // if they can't get more than one then see if they can get the next gpu and so on

            }*/

            // If no budget given then show the most profitable coins to mine for each gpu
            #endregion
            // Set title
            lblResults.Text = "Most Profitable Coin per Gpu for";
            SelectedGpuListName.Text = GpuLists.SelectedItems[0].Text;

            if (PerformingCalculations == null || PerformingCalculations.IsCompleted)
                PerformingCalculations = PerformCalculations();
        }
        private Task PerformCalculations()
        {
            return Task<bool>.Run( async () =>
            {
                // Get the list of gpus
                var gpus = GetMasterGpuList();
                int progress = 10;
                var tasks = new List<Task>();

                // Start the progress bar
                progressBar.InvokeIfRequired(c => { c.Maximum = gpus.Count + 10; c.Value = 10; });

                // Go through each gpu
                foreach(Gpu device in gpus)
                {
                    var task = Task<bool>.Run( async () =>
                    {
                        var gpu = await PerformCalculationsOnAllHashrates(device);
                        var mostProfitableHash = await GetMostProfitableHashrate(gpu);

                        // Then display the results
                        if (mostProfitableHash != null && mostProfitableHash.Coin != null && mostProfitableHash.Coin.coin != null && mostProfitableHash.Calculation != null)
                        {
                            ListViewItem li = CreateResultsListviewItem(gpu, mostProfitableHash);
                            // Add result to GUI
                            ResultsList.InvokeIfRequired(c => { c.Items.Add(li); });
                            CurrentSearchResults.Add(li);
                        }
                        // Update the progress bar
                        progressBar.InvokeIfRequired(c => { if (c.Value > 0) c.Value = progress; });
                        progress++;
                        
                        return true;
                    });
                    tasks.Add(task);
                }             

                await Task.WhenAll(tasks);

                EditingGpuList = false;

                // Reset progress bar
                progressBar.InvokeIfRequired(c => { c.Value = 0; });

                // Calculate and show the totals
                var total = CalculateTotals();
                PopulateTotalsGui(total);

                // Reset hodl coin
                searchingByHodlPrice = false;
                HodlCoin.InvokeIfRequired(h => { h.Text = ""; });

                return true;
            });
        }
        private async Task<Gpu> PerformCalculationsOnAllHashrates(Gpu gpu)
        {
            var gpuCost = await GetGpuCost(gpu);
            // Go through each saved hashrate
            for (int i = 0; i < gpu.Hashrates.Count; i++)
            {
                if (gpu.Hashrates[i].Coin == null || gpu.Hashrates[i].Coin.coin == null)
                    continue;

                // If watts for this hashrate is given use that, otherwise use the overall rated power
                if (gpu.Hashrates[i].Watts == 0)
                    gpu.Hashrates[i].Watts = gpu.Watts;

                // Get this coins most up to date info from default coin list
                var coin = defaultCoins.Find(c => c.coin.ToLower() == gpu.Hashrates[i].Coin.coin.ToLower());
                if (coin != null)
                    gpu.Hashrates[i].Coin = coin;
                else
                    continue; // No coin found, possibly low 24 volume, or a coin spiked last time app was used and now it has low volume

                // Get calculation
                gpu.Hashrates[i].Calculation = GetCalculationForHashrate(gpuCost, gpu.Hashrates[i]);

                // Get dual hashrate calculation
                if (gpu.HasDualMiningHashrate(gpu.Hashrates[i]))
                {
                    // Get this coins most up to date info from default coin list
                    gpu.Hashrates[i].DualMineHashrate.Coin = defaultCoins.Find(c => c.coin.ToLower() == gpu.Hashrates[i].DualMineHashrate.Coin.coin.ToLower());

                    gpu.Hashrates[i].DualMineHashrate.Calculation = GetCalculationForHashrate(gpuCost, gpu.Hashrates[i].DualMineHashrate);
                }
            }
            return gpu;
        }
        private async Task<Hashrate> GetMostProfitableHashrate(Gpu gpu)
        {
            if (FilterGpu(gpu))
                return new Hashrate();

            var mostProfitableHashrate = new Hashrate();
            var sortedCalculations = new List<Calculation>();

            // Get user's coin list
            var coins = new List<CoinInfo>();
            CoinLists.InvokeIfRequired(c => { if (c.SelectedItems[0].Tag != null) coins = c.SelectedItems[0].Tag as List<CoinInfo>; });

            // Get hodl coin if there is one
            var hodlCoin = "";
            HodlCoin.InvokeIfRequired(c => {
                if (c.SelectedItem != null)
                    hodlCoin = c.SelectedItem.ToString().ToLower();
            });


            var highestUsdProfits = new Hashrate();
            var quickestRoi = new Hashrate(); // (# closest to 0) 
            var lastCalc = new Hashrate();
            foreach (Hashrate hashrate in gpu.Hashrates)
            {
                if (hashrate == null || hashrate.Coin == null)
                    continue;

                // Only get calculations that are on the user's coin list
                var matchingCoin = coins.Find(c => c.coin == hashrate.Coin.coin);
                if (matchingCoin == null)
                    continue;

                // Don't perform eth calculations if the gpu doesn't have enough vram for the DAG
                if (gpu.VramSize > 0 && gpu.VramSize <= 4 && hashrate.Coin.coin.ToLower() == "eth")
                    continue;

                // Set the first valid hashrate as the quickest/highest
                if (quickestRoi.Calculation == null)
                    quickestRoi = hashrate;
                if (highestUsdProfits.Calculation == null)
                    highestUsdProfits = hashrate;

                // If hodl price isn't empty get the highest usd profits per hashrate calculation in gpu
                if (searchingByHodlPrice && hashrate.Coin.coin.ToLower() == hodlCoin || (gpu.HasDualMiningHashrate(hashrate) && hashrate.DualMineHashrate.Coin.coin == hodlCoin))
                {
                    // Add this calculation with the dual mine calculation if there is one
                    var gpuCost = await GetGpuCost(gpu);
                    var calc = GetCalculationForHashrate(gpuCost, hashrate);
                    if (gpu.HasDualMiningHashrate(hashrate) && hashrate.DualMineHashrate.Calculation != null)
                        calc = Calculation.AddCalculations(hashrate.Calculation, GetCalculationForHashrate(gpuCost, hashrate.DualMineHashrate));

                    // Check if this hashrate's usd profits are the new highest
                    if (calc.UsdProfits > highestUsdProfits.Calculation.UsdProfits)
                        highestUsdProfits = hashrate;
                }
                else if (!searchingByHodlPrice)
                {
                    // Add this calculation with the dual mine calculation if there is one
                    var calc = hashrate.Calculation;
                    if (gpu.HasDualMiningHashrate(hashrate) && hashrate.DualMineHashrate.Calculation != null)
                        calc = Calculation.AddCalculations(hashrate.Calculation, hashrate.DualMineHashrate.Calculation);

                    // Gpu didn't have a cost so return highest usd profits
                    if ( (calc.ROI == 0 && quickestRoi.Calculation.ROI == 0)
                        || (calc.ROI == 0 && calc.UsdProfits > quickestRoi.Calculation.UsdProfits) )
                    {
                        quickestRoi = hashrate;
                    }
                    // closest to 0 when roi is positive an quickest is negative closest to 0 when roi and quickest is positive in middle and closest to 0 when roi and quickest is negative on right 
                    else if ((calc.ROI > 0 && quickestRoi.Calculation.ROI < 0 && calc.ROI > quickestRoi.Calculation.ROI)
                        || (calc.ROI > 0 && quickestRoi.Calculation.ROI > 0 && calc.ROI < quickestRoi.Calculation.ROI)
                        || (calc.ROI < 0 && quickestRoi.Calculation.ROI < 0 && calc.ROI > quickestRoi.Calculation.ROI))
                    {
                        quickestRoi = hashrate;
                    }
                    // Not the quickest, continue to sort by checking if it is closer to 0 then the last calc
                    lastCalc = hashrate;
                }
            }
            if (searchingByHodlPrice)
                mostProfitableHashrate = highestUsdProfits;
            else
                mostProfitableHashrate = quickestRoi;

            return mostProfitableHashrate;
        }
        private Calculation GetCalculationForHashrate(double gpuCost, Hashrate hashrate)
        {
            var calculation = new Calculation();

            // Do nothing if no hashrates or coin are given
            if (hashrate.HashrateSpeed == 0 || hashrate.Coin == null || hashrate.Coin.coin == null)
                return calculation;

            // Get user given fees
            var fee = double.TryParse(PoolMinerFee.Text, out double parsedFee) ? parsedFee / 100 : 0;
            var electricityRate = double.TryParse(ElectricityRate.Text, out double parsedElecRate) ? parsedElecRate : 0;

            // Check if using hodl price
            var hodlCoin = "";
            var hodlPrice = "";
            HodlCoin.InvokeIfRequired(c => { hodlCoin = c.SelectedItem != null ? c.SelectedItem.ToString() : ""; });
            HodlPrice.InvokeIfRequired(h => { hodlPrice = h.Text; });

            // Perform calculation with hodl price if given
            if (hashrate.Coin.coin.ToLower() == hodlCoin.ToLower() && double.TryParse(hodlPrice, out var parsedHodlPrice) && parsedHodlPrice > 0)
                calculation = Calculation.Calculate(gpuCost, hashrate.HashrateSpeed, hashrate.Watts, hashrate.Coin, electricityRate, fee, parsedHodlPrice);
            else
                calculation = Calculation.Calculate(gpuCost, hashrate.HashrateSpeed, hashrate.Watts, hashrate.Coin, electricityRate, fee);
            return calculation;
        }
        private ListViewItem CreateResultsListviewItem(Gpu gpu, Hashrate hashrate)
        {
            if (gpu == null || hashrate == null || hashrate.Calculation == null || hashrate.Coin == null)
                return new ListViewItem();

            // Start is same for single/dual mine hashrate calculations
            dynamic gpuAndHash = new ExpandoObject();
            gpuAndHash.Gpu = gpu;
            gpuAndHash.Hashrate = hashrate;

            var li = new ListViewItem();
            li.Tag = gpuAndHash;
            li.ImageIndex = GetImageIndex(hashrate.Coin.coin);
            li.Text = hashrate.Coin.coin;
            li.SubItems.Add(gpu.Name);
            li.SubItems.Add(gpu.Manufacturer);
            li.SubItems.Add("$" + gpu.MSRP.ToString("0.00"));
            li.SubItems.Add("$" + gpu.EbayPrice.ToString("0.00"));
            li.SubItems.Add("$" + gpu.PricePaid.ToString("0.00"));

            // If a dual mine hashrate calculation
            if (gpu.HasDualMiningHashrate(hashrate))
            {
                li.Text += "/" + hashrate.DualMineHashrate.Coin.coin;

                // Dual mine hashrate calculation
                li.SubItems.Add("$" + hashrate.Calculation.CostPerMhs.ToString("0.00") + " " + hashrate.Coin.coin + " / $" + hashrate.DualMineHashrate.Calculation.CostPerMhs.ToString("0.00") + " " + hashrate.DualMineHashrate.Coin.coin);

                var usdCosts = hashrate.Calculation.UsdPoolMinerFeeCost + hashrate.Calculation.UsdElectricityCost + hashrate.DualMineHashrate.Calculation.UsdPoolMinerFeeCost + hashrate.DualMineHashrate.Calculation.UsdElectricityCost;
                li.SubItems.Add("$" + usdCosts.ToString("0.00") + " / $" + (usdCosts * 7).ToString("0.00") + " / $" + (usdCosts * 30).ToString("0.00"));

                var watts = gpu.Watts;
                var cryptoCosts = 0.0m;
                var cryptoRewards = 0.0;
                var cryptoProfits = 0.0;
                var coinSymbol = hashrate.Coin.coin;
                var efficiency = hashrate.Calculation.Efficiency + hashrate.DualMineHashrate.Calculation.Efficiency;
                var roi = hashrate.Calculation.GpuCosts / ((hashrate.Calculation.UsdProfits + hashrate.DualMineHashrate.Calculation.UsdProfits) * 30);

                // Determine which hashrate calculation costs more from power usage
                if (hashrate.Calculation.UsdElectricityCost >= hashrate.DualMineHashrate.Calculation.UsdElectricityCost && hashrate.Calculation.UsdRewards > hashrate.DualMineHashrate.Calculation.UsdRewards)
                {
                    // Use that one to display stuff that wouldn't make sense like combining 2 diff. crypto rewards

                    if (double.IsInfinity(hashrate.Calculation.CryptoElectricityCost))
                        hashrate.Calculation.CryptoElectricityCost = 0;
                    cryptoCosts = decimal.Round((decimal)(hashrate.Calculation.CryptoPoolMinerFeeCost + hashrate.Calculation.CryptoElectricityCost), Constants.DigitsToRound);

                    if (hashrate.Watts > 0)
                        watts = hashrate.Watts;

                    if (double.IsInfinity(hashrate.Calculation.CryptoRewards))
                        cryptoRewards = 0;
                    else
                        cryptoRewards = hashrate.Calculation.CryptoRewards;

                    if (double.IsInfinity(hashrate.Calculation.CryptoProfits))
                        cryptoProfits = 0;
                    else
                        cryptoProfits = hashrate.Calculation.CryptoProfits;
                }
                else
                {
                    if (double.IsInfinity(hashrate.DualMineHashrate.Calculation.CryptoElectricityCost))
                        hashrate.DualMineHashrate.Calculation.CryptoElectricityCost = 0;
                    cryptoCosts = decimal.Round((decimal)(hashrate.DualMineHashrate.Calculation.CryptoPoolMinerFeeCost + hashrate.DualMineHashrate.Calculation.CryptoElectricityCost), Constants.DigitsToRound);

                    if (hashrate.DualMineHashrate.Watts > 0)
                        watts = hashrate.DualMineHashrate.Watts;

                    if (double.IsInfinity(hashrate.DualMineHashrate.Calculation.CryptoRewards))
                        cryptoRewards = 0;
                    else
                        cryptoRewards = hashrate.DualMineHashrate.Calculation.CryptoRewards;

                    coinSymbol = hashrate.DualMineHashrate.Coin.coin;

                    if (double.IsInfinity(hashrate.DualMineHashrate.Calculation.CryptoProfits))
                        cryptoProfits = 0;
                    else
                        cryptoProfits = hashrate.DualMineHashrate.Calculation.CryptoProfits;
                }

                // Crypto costs
                li.SubItems.Add(cryptoCosts.ToString() + " / " + (cryptoCosts * 7).ToString() + " / " + (cryptoCosts * 30));

                // Hashrate speed                
                li.SubItems.Add(ConvertHashrateToReadable(hashrate.HashrateSpeed) + " " + hashrate.Coin.coin + " / " + ConvertHashrateToReadable(hashrate.DualMineHashrate.HashrateSpeed) + " " + hashrate.DualMineHashrate.Coin.coin);

                // Watts
                li.SubItems.Add(watts.ToString());

                // Efficiency
                if (hashrate.Calculation.Efficiency < 0 && hashrate.DualMineHashrate.Calculation.Efficiency < 0)
                    li.SubItems.Add((hashrate.Calculation.Efficiency + hashrate.DualMineHashrate.Calculation.Efficiency).ToString("0.000") + " kw/mhs");
                else
                    li.SubItems.Add(efficiency.ToString("0.000") + " kw/mhs");

                // Usd Rewards
                li.SubItems.Add("$" + (hashrate.Calculation.UsdRewards + hashrate.DualMineHashrate.Calculation.UsdRewards).ToString("0.00") + " / $" + ((hashrate.Calculation.UsdRewards + hashrate.DualMineHashrate.Calculation.UsdRewards) * 7).ToString("0.00") + " / $" + ((hashrate.Calculation.UsdRewards + hashrate.DualMineHashrate.Calculation.UsdRewards) * 30).ToString("0.00"));


                // Crypto Rewards
                li.SubItems.Add(cryptoRewards.ToString("0.00") + " " + coinSymbol + " / " + (cryptoRewards * 7).ToString("0.00") + " " + coinSymbol + " / " + (cryptoRewards * 30).ToString("0.00") + " " + coinSymbol);

                // Usd Profits
                li.SubItems.Add("$" + (hashrate.Calculation.UsdProfits + hashrate.DualMineHashrate.Calculation.UsdProfits).ToString("0.00") + " / $" + ((hashrate.Calculation.UsdProfits + hashrate.DualMineHashrate.Calculation.UsdProfits) * 7).ToString("0.00") + " / $" + ((hashrate.Calculation.UsdProfits + hashrate.DualMineHashrate.Calculation.UsdProfits) * 30).ToString("0.00"));

                // Crypto Profits
                li.SubItems.Add(cryptoProfits.ToString("0.00") + " " + coinSymbol + " / " + (cryptoProfits * 7).ToString("0.00") + " " + coinSymbol + " / " + (cryptoProfits * 30).ToString("0.00") + " " + coinSymbol);

                // ROI
                li.SubItems.Add(roi.ToString("0.00") + " months");

                return li;
            }
            else
            {
                var calculation = hashrate.Calculation;

                // Cost per $
                li.SubItems.Add("$" + calculation.CostPerMhs.ToString("0.00"));

                // Fee
                var usdCosts = calculation.UsdPoolMinerFeeCost + calculation.UsdElectricityCost;
                li.SubItems.Add("$" + usdCosts.ToString("0.00") + " / $" + (usdCosts * 7).ToString("0.00") + " / $" + (usdCosts * 30).ToString("0.00"));

                // Crypto costs
                if (double.IsInfinity(calculation.CryptoElectricityCost))
                    calculation.CryptoElectricityCost = 0;

                var cryptoCosts = decimal.Round((decimal)(calculation.CryptoPoolMinerFeeCost + calculation.CryptoElectricityCost), Constants.DigitsToRound);
                li.SubItems.Add(cryptoCosts.ToString() + " / " + (cryptoCosts * 7).ToString() + " / " + (cryptoCosts * 30));

                // Hashrate
                li.SubItems.Add(ConvertHashrateToReadable(hashrate.HashrateSpeed));

                // Watts
                var watts = gpu.Watts;
                if (hashrate.Watts > 0)
                    watts = hashrate.Watts;
                li.SubItems.Add(watts.ToString());

                // Efficiency
                li.SubItems.Add(calculation.Efficiency.ToString("0.000") + " kw/mhs");

                // Usd Rewards
                li.SubItems.Add("$" + calculation.UsdRewards.ToString("0.00") + " / $" + (calculation.UsdRewards * 7).ToString("0.00") + " / $" + (calculation.UsdRewards * 30).ToString("0.00"));

                if (double.IsInfinity(calculation.CryptoRewards))
                    calculation.CryptoRewards = 0;

                var cryptoRewards = decimal.Round((decimal)calculation.CryptoRewards, Constants.DigitsToRound);
                li.SubItems.Add(cryptoRewards.ToString("0.00") + " " + hashrate.Coin.coin + " / " + (cryptoRewards * 7).ToString("0.00") + " " + hashrate.Coin.coin + " / " + (cryptoRewards * 30).ToString("0.00") + " " + hashrate.Coin.coin);
                li.SubItems.Add("$" + calculation.UsdProfits.ToString("0.00") + " / $" + (calculation.UsdProfits * 7).ToString("0.00") + " / $" + (calculation.UsdProfits * 30).ToString("0.00"));

                if (double.IsInfinity(calculation.CryptoProfits))
                    calculation.CryptoProfits = 0;

                var cryptoProfits = decimal.Round((decimal)calculation.CryptoProfits, Constants.DigitsToRound);
                li.SubItems.Add(cryptoProfits.ToString("0.00") + " " + hashrate.Coin.coin + " / " + (cryptoProfits * 7).ToString("0.00") + " " + hashrate.Coin.coin + " / " + (cryptoProfits * 30).ToString("0.00") + " " + hashrate.Coin.coin);
                li.SubItems.Add(calculation.ROI.ToString("0.00") + " months");

                return li;
            }
        }
        private void ResultsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ResultsList.SelectedItems.Count > 0)
            {
                dynamic gpuAndHash = ResultsList.SelectedItems[0].Tag;
                var gpu = gpuAndHash.Gpu;
                ResultsEbayLink.Tag = (gpu.EbayLink != null && gpu.EbayLink != "unknown") ? gpu.EbayLink : "";

                // Clear previous data
                ResultsEbayItemSelection.Items.Clear();
                EbayItemUrl.Tag = "";

                // Get ebay price/listings
                userChangedEbaySelection = true;
                UpdateEbayItemSelections(gpu, ResultsEbayItemSelection, ResultsEbayLink);
            }
        }
        private void ResultsList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (PerformingCalculations.IsCompleted)
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
                    lvwColumnSorter.Order = SortOrder.Descending;
                else
                    lvwColumnSorter.Order = SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to descending.
                lvwColumnSorter.SortColumn = column;
                lvwColumnSorter.Order = SortOrder.Descending;
            }

            // Perform the sort with these new sort options.
            ResultsList.Sort();
        }
        private void Budget_KeyDown(object sender, KeyEventArgs e)
        {
            if (!PerformingCalculations.IsCompleted && e.KeyCode == Keys.Enter)
            {
                ShowErrorMessage(Constants.PerformingCalcs);
                return;
            }
            if (PerformingCalculations.IsCompleted && e.KeyCode == Keys.Enter)
            {
                if (double.TryParse(Budget.Text, out var budget) && budget > 0)
                    MaxMyROI.PerformClick();
                else
                    ShowErrorMessage("The budget needs to be a number that is greater than 0 please.");
            }
        }

        
        // Results gpu right click menu
        void cms_Opening(object sender, CancelEventArgs e)
        {
            resultsContextMenuStrip.Items.Clear();

            // Add gpu to a gpu list
            ToolStripMenuItem gpuToolStripMenuItem1 = new ToolStripMenuItem();
            gpuToolStripMenuItem1.DropDownItemClicked += GpuToolStripAddGpuToList_DropDownItemClicked;
            gpuToolStripMenuItem1.Text = "Add to Gpu List";

            // Populate the ContextMenuStrip control
            foreach (ListViewItem item in GpuLists.Items)
                gpuToolStripMenuItem1.DropDownItems.Add(item.Text);
            resultsContextMenuStrip.Items.Add(gpuToolStripMenuItem1);

            // Change this gpus coin in results view
            ToolStripMenuItem gpuToolStripMenuItem2 = new ToolStripMenuItem();
            gpuToolStripMenuItem2.DropDownItemClicked += GpuToolStripChangeCoin_DropDownItemClicked;
            gpuToolStripMenuItem2.Text = "Change Coin";

            var gpuAndHash = ResultsList.SelectedItems[0].Tag as dynamic;
            var gpu = gpuAndHash.Gpu;

            foreach (Hashrate hashrate in gpu.Hashrates)
            {
                if(gpu.HasDualMiningHashrate(hashrate))
                    gpuToolStripMenuItem2.DropDownItems.Add(hashrate.Coin.coin + " / " + hashrate.DualMineHashrate.Coin.coin);
                else if(hashrate.Coin != null)
                    gpuToolStripMenuItem2.DropDownItems.Add(hashrate.Coin.coin);
            }
            resultsContextMenuStrip.Items.Add(gpuToolStripMenuItem2);

            //resultsContextMenuStrip.Items.Add("Remove Gpu");

            e.Cancel = false;
        }
        private void GpuToolStripChangeCoin_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Get the coin the user selected
            var selectedCoin = e.ClickedItem.Text;
            var dualCoin = "";
            if (selectedCoin.Contains('/'))
            {
                var start = selectedCoin.IndexOf("/ ") + 2;
                dualCoin = selectedCoin.Substring(start, selectedCoin.Length - start);
                selectedCoin = selectedCoin.Substring(0, start - 3);
            }

            // Get selected gpu
            var gpuAndHash = ResultsList.SelectedItems[0].Tag as dynamic;
            var gpu = gpuAndHash.Gpu;

            // Go through each hashrate
            for(int i = 0; i < gpu.Hashrates.Count; i++)
            {
                // If this hashrate is for the selected coin
                if (gpu.Hashrates[i] != null && gpu.Hashrates[i].Coin != null 
                    && gpu.Hashrates[i].Coin.coin.ToLower() == selectedCoin.ToLower())
                {
                    // If the user selected just one coin this is the right hashrate
                    if (dualCoin == "" && (gpu.Hashrates[i].DualMineHashrate == null || gpu.Hashrates[i].DualMineHashrate.Coin == null || gpu.Hashrates[i].DualMineHashrate.Coin.coin == null || gpu.Hashrates[i].DualMineHashrate.Coin.coin.Length == 0))
                    {
                    }
                    // If the user selected a coin with a dual coin this is the right hashrate
                    else if (dualCoin != "" && gpu.Hashrates[i].DualMineHashrate != null && gpu.Hashrates[i].DualMineHashrate.Coin != null && gpu.Hashrates[i].DualMineHashrate.Coin.coin != null && gpu.Hashrates[i].DualMineHashrate.Coin.coin.ToLower() == dualCoin.ToLower())
                    {
                    }
                    else // Skip everything else (user selects eth but this is eth/ton) or (user selected eth/ton but this is just eth)
                        continue;

                    // Create listview item
                    var li = CreateResultsListviewItem(gpu, gpu.Hashrates[i]);
                    li.Selected = true;

                    // Get the selected item's index
                    var index = ResultsList.SelectedItems[0].Index;

                    // Update gui list
                    ResultsList.Items.Remove(ResultsList.SelectedItems[0]);
                    ResultsList.Items.Insert(index, li);

                    // Recalculate totals
                    PopulateTotalsGui(CalculateTotals());
                    return;

                }
            }            
        }
        private void GpuToolStripAddGpuToList_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var selectedItem = e.ClickedItem.Text;
            var gpuAndHash = ResultsList.SelectedItems[0].Tag as dynamic;
            var gpuToAdd = gpuAndHash.Gpu as Gpu;
            var gpu = gpuToAdd.CloneGpu(random.Next());

            // Get the selected gpu list
            var item = GpuLists.Items.Find(selectedItem, false);
            var gpus = new List<Gpu>();
            if(item[0].Tag != null)
                gpus = item[0].Tag as List<Gpu>;

            // Add gpu to the list
            gpus.Add(gpu);

            // Add gpu list to list name tag
            item[0].Tag = gpus;
            item[0].SubItems[1].Text = (int.Parse( item[0].SubItems[1].Text.ToString() ) + 1).ToString();

            // Save gpu list to file
            var file = GpuListsDirectory + selectedItem + ".json";
            JsonCrud.SaveGpuList(gpus, file);
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

            
            if (filteredSearchResults.Count > 0 || CurrentSearchResults.Count > 0)
            {
                var searchResults = filteredSearchResults.Count > 0 ? filteredSearchResults : CurrentSearchResults;
                int coinSetter = 0;
                foreach (ListViewItem result in searchResults)
                {
                    dynamic gpuAndHash = result.Tag as ExpandoObject;
                    var gpu = gpuAndHash.Gpu;
                    var hashrate = gpuAndHash.Hashrate;

                    // Add up to 3 different crypto rewards
                    if (coinSetter == 0)
                    {
                        total[1].Coin = hashrate.Coin.coin.ToLower();
                        coinSetter++;
                    }
                    else if (coinSetter == 1 && total[1].Coin.ToLower() != hashrate.Coin.coin.ToLower())
                    {
                        total[2].Coin = hashrate.Coin.coin.ToLower();
                        coinSetter++;
                    }
                    else if (coinSetter == 2 && total[1].Coin.ToLower() != hashrate.Coin.coin.ToLower() && total[2].Coin.ToLower() != hashrate.Coin.coin.ToLower())
                    {
                        total[3].Coin = hashrate.Coin.coin.ToLower();
                        coinSetter++;
                    }


                    int index = 0;

                    if (hashrate.Coin.coin.ToLower() == total[1].Coin.ToLower())
                    {
                        total[1].Coin = hashrate.Coin.coin;
                        index = 1;
                    }
                    else if (hashrate.Coin.coin.ToLower() == total[2].Coin.ToLower())
                    {
                        total[2].Coin = hashrate.Coin.coin;
                        index = 2;
                    }
                    else if (hashrate.Coin.coin.ToLower() == total[3].Coin.ToLower())
                    {
                        total[3].Coin = hashrate.Coin.coin;
                        index = 3;
                    }

                    // If this is one of the 3 coins we are going to show
                    if (index > 0)
                    {
                        // Add the results up
                        var cryptoCosts = hashrate.Calculation.CryptoElectricityCost + hashrate.Calculation.CryptoPoolMinerFeeCost;
                        total[index].CryptoCosts += cryptoCosts;
                        total[index].CryptoRewards += hashrate.Calculation.CryptoRewards;
                        total[index].CryptoProfits += hashrate.Calculation.CryptoProfits;
                        total[index].GpuCosts += hashrate.Calculation.GpuCosts;
                        total[index].CostPerMhs += hashrate.Calculation.CostPerMhs;
                        total[index].UsdCosts += hashrate.Calculation.UsdElectricityCost + hashrate.Calculation.UsdPoolMinerFeeCost;
                        total[index].Hashrate += hashrate.HashrateSpeed;
                        total[index].Watts += hashrate.Watts;
                        total[index].Efficiency += hashrate.Calculation.Efficiency;
                        total[index].UsdRewards += hashrate.Calculation.UsdRewards;
                        total[index].UsdProfits += hashrate.Calculation.UsdProfits;
                        total[index].ROI += hashrate.Calculation.ROI;
                        total[index].GpuCount += 1;
                    }

                    // Add each result
                    total[0].Coin = "USD";
                    total[0].CryptoCosts = 0; // Doesn't make sense to add different crypto currencies toegether
                    total[0].CryptoRewards = 0;
                    total[0].CryptoProfits = 0;
                    total[0].GpuCosts += hashrate.Calculation.GpuCosts;
                    total[0].CostPerMhs += hashrate.Calculation.CostPerMhs;
                    total[0].UsdCosts += hashrate.Calculation.UsdElectricityCost + hashrate.Calculation.UsdPoolMinerFeeCost;
                    total[0].Hashrate += hashrate.HashrateSpeed;
                    total[0].Watts += hashrate.Watts;
                    total[0].Efficiency += hashrate.Calculation.Efficiency;
                    total[0].UsdRewards += hashrate.Calculation.UsdRewards;
                    total[0].UsdProfits += hashrate.Calculation.UsdProfits;
                    total[0].ROI += hashrate.Calculation.ROI;
                    total[0].GpuCount += 1;
                }
                
            }

            // Calculate ROI
            if(total[0].GpuCosts > 0)
                total[0].ROI = total[0].GpuCosts / (total[0].UsdProfits * 30);

            // Calculate Efficiency
            var hashSize = Calculation.GetHashrateSize(total[0].Hashrate);
            total[0].Efficiency = (total[0].Hashrate / hashSize) / total[0].Watts;

            // Calculate Roi and Eff. for the top 3 coins
            for(int i = 1; i < 4; i++)
            {
                if (total[i].Hashrate != null && total[i].Hashrate > 0)
                {
                    // Calculate ROI
                    total[i].ROI = total[i].GpuCosts / (total[i].UsdProfits * 30);

                    // Calculate Efficiency
                    hashSize = Calculation.GetHashrateSize(total[i].Hashrate);
                    total[i].Efficiency = (total[i].Hashrate / hashSize) / total[i].Watts;
                }
            }

            return total;
        }
        private void PopulateTotalsGui(dynamic total)
        {
            TotalsList.InvokeIfRequired(c => { c.Items.Clear(); });

            // Show up to 3 different crypto results
            for (int i = 0; i < 4; i++)
            {
                if (total[i] != null && total[i].Hashrate > 0)
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
                    li.SubItems.Add(ConvertHashrateToReadable(total[i].Hashrate));
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

                    TotalsList.InvokeIfRequired(c => { c.Items.Add(li); });
                }
            }
        }


        // Hodl
        private void PopulateHodlMenu()
        {
            // Load the coins
            var coins = CoinLists.SelectedItems[0].Tag as List<CoinInfo>;

            // Clear previous coin list
            HodlCoin.Items.Clear();

            HodlCoin.Items.Add("");

            // Populate user coin list to hodl coin dropdown menu
            foreach (var coin in coins)
                HodlCoin.Items.Add(coin.coin);            
        }
        private void HodlCoin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (double.TryParse(HodlPrice.Text, out var hodlPrice) && hodlPrice > 0)
            {
                if (!PerformingCalculations.IsCompleted)
                {
                    ShowErrorMessage(Constants.PerformingCalcs);
                    return;
                }

                // If user selected the empty item, clear the hodl price
                if (HodlCoin.SelectedItem.ToString() == "")
                    HodlPrice.Text = "";
                else
                {
                    searchingByHodlPrice = true;
                    MaxMyROI.PerformClick();
                }
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
                    if (!PerformingCalculations.IsCompleted)
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
            if (!PerformingCalculations.IsCompleted)
            {
                ShowErrorMessage(Constants.PerformingCalcs);
                HodlPrice.Text = "";
            }
        }


        // Coin list
        private void EditCoinLists_Click(object sender, EventArgs e)
        {
            // Clear edit coin list
            EditCoinList.Items.Clear();

            // If no list is selected show a popup to inform the user
            if (CoinLists.SelectedItems.Count == 0)
            {
                MessageBox.Show("No Coin List Selected to Edit!");
                return;
            }

            // Disable user input on the main GUI
            DisableUserInput();

            // If this is a new list
            if (CoinLists.SelectedItems[0].Tag == null)
            {
                CoinListName.Text = "";
                CoinLists.SelectedItems[0].Tag = new List<CoinInfo>();
            }
            else
            {
                // Set the coin list name
                CoinListName.Text = CoinLists.SelectedItems[0].Text;

                // Get the coins
                var coins = CoinLists.SelectedItems[0].Tag as List<CoinInfo>;

                // Go through each coin
                foreach (var coin in coins)
                {
                    // Only add it if it isn't already on the list
                    if (!EditCoinList.Items.ContainsKey(coin.algorithm))
                    {
                        var li = new ListViewItem(coin.algorithm);
                        li.ImageIndex = GetImageIndex(coin.coin);
                        li.Tag = coin;
                        li.Name = coin.algorithm;
                        li.SubItems.Add(coin.coin);
                        var price = decimal.Round((decimal)coin.price, Constants.DigitsToRound);
                        if (price < 0)
                            li.SubItems.Add("N/A");
                        else
                            li.SubItems.Add("$" + price.ToString("0.00"));
                        EditCoinList.Items.Add(li);
                    }
                }
            }

            // Show the edit panel
            EditCoinPanel.BringToFront();

            // Show list of all available coins
            if (ListOfAllCoins.Items.Count == 0)
            {
                foreach (var coin in defaultCoins)
                {
                    if (coin.type == "coin")
                    {
                        // Extract only useful data from minerstats coinInfo
                        var li = new ListViewItem(coin.algorithm);
                        li.Name = coin.algorithm;
                        li.Tag = coin;
                        li.ImageIndex = GetImageIndex(coin.coin);
                        li.SubItems.Add(coin.coin);
                        var price = decimal.Round((decimal)coin.price, Constants.DigitsToRound);
                        if (price < 0)
                            li.SubItems.Add("N/A");
                        else
                            li.SubItems.Add("$" + price.ToString("0.00"));
                        ListOfAllCoins.Items.Add(li);
                    }
                }
                ListOfAllCoins.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }


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
            // Load the coins from the list
            var coins = JsonCrud.LoadCoinList(file);

            // Add list name to coin lists gui
            var li = new ListViewItem();
            li.Tag = coins;
            li.Text = Path.GetFileNameWithoutExtension(file);
            li.SubItems.Add(coins.Count.ToString());
            li.Selected = true;
            CoinLists.Items.Add(li);
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
                if (PerformingCalculations != null && !PerformingCalculations.IsCompleted)
                {
                    ShowErrorMessage(Constants.PerformingCalcs); // We are performing calcultions so inform the user
                    return;
                }

                // Add coins to hodl drop down menu
                PopulateHodlMenu();
            }
        }
        private void ExportCoinLists_Click(object sender, EventArgs e)
        {
            if (CoinLists.SelectedItems != null && CoinLists.SelectedItems.Count > 0)
            {
                var fileName = CoinLists.SelectedItems[0].Text;

                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Filter = "json files (*.json)|*.json";
                saveFileDialog.FileName = fileName;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveFileDialog.AddExtension = true;

                // Get the coins
                var coins = CoinLists.SelectedItems[0].Tag as List<CoinInfo>;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    JsonCrud.WriteToCoinListFile(saveFileDialog.FileName, coins);  
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

            // Get all coins from gui list
            var coins = new List<CoinInfo>();
            foreach(ListViewItem item in EditCoinList.Items)
            {
                var coin = item.Tag as CoinInfo;
                coins.Add(coin);
            }

            // Save coin list back to gui tag
            CoinLists.SelectedItems[0].Tag = coins;

            // Save coin list to file if it isn't the default list
            if(CoinListName.Text != "Default")
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

            // Get selected coin
             var coinToAdd = ListOfAllCoins.SelectedItems[0].Tag as CoinInfo;

            // Go through all coins on custom list to prevent duplicate entries
            foreach (ListViewItem item in EditCoinList.Items)
            {
                var coinOnEditList = item.Tag as CoinInfo;
                if (coinOnEditList.coin == coinToAdd.coin && coinToAdd.algorithm == coinOnEditList.algorithm)
                    return;
            }

            // This coin isn't on the list yet so add it
            var li = new ListViewItem(coinToAdd.coin);
            li.Name = coinToAdd.name;
            li.ImageIndex = GetImageIndex(coinToAdd.coin);
            li.Tag = coinToAdd;            
            li.SubItems.Add(coinToAdd.algorithm);
            EditCoinList.Items.Add(li);
        }
        private void DeleteCoin_Click(object sender, EventArgs e)
        {
            if (EditCoinList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a Coin on the custom list to delete.");
                return;
            }

            var coinToRemove = EditCoinList.SelectedItems[0].Tag as CoinInfo;

            EditCoinList.Items.RemoveByKey(coinToRemove.name);
        }
        private void EditCoinList_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
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

                    // Rename/create the coin list file
                    if (oldFileName != newFileName)
                    {
                        if (CoinLists.SelectedItems[0].Text.Length > 0 && CoinLists.SelectedItems[0].Text != newFileName)
                            File.Move(oldFileName, newFileName);
                        else
                            File.Create(newFileName);

                        // Rename the coin list name in list of coin lists
                        CoinLists.SelectedItems[0].Text = CoinListName.Text;
                    }
                }
                else
                    MessageBox.Show("The coin list needs to have a name.");
            }
        }

        
        // Gpu list
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

            // Load gpu list
            var gpus = new List<Gpu>();
            if(GpuLists.SelectedItems[0].Tag != null)
                gpus = GpuLists.SelectedItems[0].Tag as List<Gpu>;

            // Disable user input on the main GUI
            DisableUserInput();          
            
            // Show the edit gpu list panel
            EditGpuPanel.BringToFront();

            // If this is a new list add one new entry
            if (GpuLists.SelectedItems[0].Text == "")
            {
                var gpu = new Gpu();
                gpu.Id = random.Next();
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

            // Select first gpu in the list if there are any gpus
            if(EditGpuList.Items.Count > 0)
                EditGpuList.Items[0].Selected = true;

            // If this is the default list disable editing the list name
            if(GpuListName.Text == "Default")
                GpuListName.Enabled = false;
            else
                GpuListName.Enabled = true;

            // Give the gpu list focus
            EditGpuList.Focus();
        }
        private bool AddingNewGpuList = false;
        private void AddGpuList_Click(object sender, EventArgs e)
        {
            var li = new ListViewItem();
            li.SubItems.Add("0");
            GpuLists.Items.Add(li);
            var index = GpuLists.Items.Count - 1;
            GpuLists.Items[index].Selected = true;
            AddingNewGpuList = true;
            EditGpuLists.PerformClick();
        }
        private void ImportGpuLists_Click(object sender, EventArgs e)
        {
            // Get file to import from the user
            var gpuListFileName = GetFileFromUser();

            // Load gpu list file
            var gpus = JsonCrud.LoadGpuList(gpuListFileName);

            // Add the file name to the gpu lists gui
            var li = new ListViewItem();
            li.Tag = gpus;
            li.Text = Path.GetFileNameWithoutExtension(gpuListFileName);
            li.SubItems.Add(gpus.Count.ToString());
            li.Selected = true;
            GpuLists.Items.Add(li);
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
                        UpdateGpuList(true, false);
                    }
                    else
                        MessageBox.Show("Unable to remove list, the list either no longer exists or it is open and in use! Please try again.");
                }
            }
        }
        private void GpuLists_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If the user isn't creating a new list
            if (GpuLists.SelectedItems.Count > 0 && GpuLists.SelectedItems[0].Text != "")
            {
                // And we are not currently performing calculations
                if (PerformingCalculations != null && !PerformingCalculations.IsCompleted)
                {
                    // We are performing calcultions so inform the user
                    ShowErrorMessage(Constants.PerformingCalcs);
                    return;
                }
                AddingNewGpuList = false;
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
                        var gpus = GpuLists.SelectedItems[0].Tag as List<Gpu>;
                        JsonCrud.WriteToGpuListFile(saveFileDialog.FileName, gpus);
                    }
                }
            }
        }


        // Master Gpu List Crud
        private bool EditingGpuList = false;
        private void SaveGpuList()
        {
            var gpus = GetMasterGpuList();

            if (GpuListName.Text == "Default")
            {
                MessageBox.Show("Please create a new list as any edits made to the default list will not be saved.");
                return;
            }

            if (GpuListName.Text.Length > 0)
            {
                // Save gpus to file
                JsonCrud.SaveGpuList(gpus, GpuListsDirectory + GpuListName.Text + ".json");
            }
            else
                MessageBox.Show("The gpu list needs to have a name.");
        }
        private List<Gpu> GetMasterGpuList()
        {
            while(EditingGpuList)
            {
                System.Threading.Thread.Sleep(500);
            }

            if (!EditingGpuList)
            {
                EditingGpuList = true;

                // Get the master gpu list
                var gpus = new List<Gpu>();
                GpuLists.InvokeIfRequired(g => { gpus = g.SelectedItems[0].Tag as List<Gpu>; });
                if (gpus == null)
                    gpus = new List<Gpu>();
                return gpus;
            }

            return new List<Gpu>();
        }
        private void UpdateGpuInMasterGpuList(Gpu gpu)
        {
            // Get the master gpu list
            var gpus = GetMasterGpuList();

            // Get old gpu index to replace in master list
            var index = gpus.FindIndex(g => g.Id == gpu.Id);
            
            // Show error if gpu not found
            if (index == -1)
            {
                MessageBox.Show("Gpu with id: " + gpu.Id + " was not found in the gpu list and was unable to be updated");
                return;
            }

            gpus.RemoveAt(index);
            gpus.Insert(index, gpu);

            // Add master list back to gui and update gpu count
            GpuLists.InvokeIfRequired(g => { g.SelectedItems[0].Tag = gpus; });

            EditingGpuList = false;
        }
        private void RemoveGpuInMasterGpuList(Gpu gpu)
        {
            // Get the master gpu list
            var gpus = GetMasterGpuList();

            // Remove gpu
            gpus.Remove(gpu);

            // Add master list back to gui and update gpu count
            GpuLists.InvokeIfRequired(g => { g.SelectedItems[0].Tag = gpus; g.SelectedItems[0].SubItems[1].Text = gpus.Count.ToString(); });

            EditingGpuList = false;
        }
        private void AddGpuToGpuList(Gpu gpu)
        {
            // Get master list
            var gpus = GetMasterGpuList();

            // Add new gpu to master list
            gpus.Add(gpu);

            // Add master list back to gui list name tag and increase gpu count
            GpuLists.InvokeIfRequired(g => { g.SelectedItems[0].Tag = gpus; g.SelectedItems[0].SubItems[1].Text = gpus.Count.ToString(); });

            EditingGpuList = false;
        }
        // For updated gui and master list when editing gpus
        private void UpdateGpuList(bool removeSelectedGpu = false, bool getGpuInfoFromUserInput = false)
        {
            if (EditGpuList.SelectedItems != null && EditGpuList.SelectedItems.Count > 0)
            {
                // Extract new gpu info from gui
                var gpu = new Gpu();

                if (getGpuInfoFromUserInput)
                {
                    gpu = GetGpuPanelInfo();

                    // Copy gpu id
                    if (EditGpuList.SelectedItems[0].Tag != null)
                    {
                        var oldGpu = EditGpuList.SelectedItems[0].Tag as Gpu;
                        gpu.Id = oldGpu.Id;
                    }
                    // Unless its a new gpu
                    else
                        gpu.Id = random.Next();

                    // Update the gpu in gui list
                    EditGpuList.SelectedItems[0].Text = gpu.Manufacturer;
                    EditGpuList.SelectedItems[0].SubItems[1].Text = gpu.VersionPrefix;
                    EditGpuList.SelectedItems[0].SubItems[2].Text = gpu.ModelNumber;
                    EditGpuList.SelectedItems[0].SubItems[3].Text = gpu.VersionSuffix;
                    EditGpuList.SelectedItems[0].SubItems[4].Text = gpu.VramSize.ToString();
                }
                else
                    gpu = EditGpuList.SelectedItems[0].Tag as Gpu;

                // Extract hashrate info from gui table
                gpu.Hashrates = GetSelectedGpuHashratesFromGui();

                // Update gui gpu
                EditGpuList.SelectedItems[0].Tag = gpu;

                UpdateGpuInMasterGpuList(gpu);

                // Remove old gpu from gui
                if (removeSelectedGpu)
                {
                    RemoveGpuInMasterGpuList(gpu);

                    // Get the previous items index
                    var index = EditGpuList.SelectedItems[0].Index - 1;
                    if (index < 0)
                        index = 0;

                    // Remove gpu from gui
                    EditGpuList.SelectedItems[0].Remove();

                    // Select the previous item in the list
                    if (EditGpuList.Items.Count > 0)
                        EditGpuList.Items[index].Selected = true;
                }

                // Save updated list to file
                SaveGpuList();

                AddingNewGpuList = false;
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

            // Get the selected gpu
            var gpu = EditGpuList.SelectedItems[0].Tag as Gpu;            

            // Update gui
            UpdateGpuPanelInfo(gpu);

            // Show progress bar and get ebay listings for this gpu
            GettingEbayPrice = true;
            HashratesProgressBar.Visible = true;
            HashratesProgressBar.Value = 90;

            UpdateEbayItemSelections(gpu, EbayItemSelection, EbayItemUrl, EbayPrice);

            // Show the hashrates
            DisplayHashrates(gpu);

            // Hide progress bar
            GettingEbayPrice = false;
            HashratesProgressBar.Value = 0;
            HashratesProgressBar.Visible = false;
        }
        private DataGridViewRow GetEmptyRow()
        {
            var hashCells = ConvertHashrateToDgvCell(new Hashrate());
            // Add defaults to data grid view
            var addNewRow = new DataGridViewRow();
            addNewRow.Cells.Add(hashCells.Coin);
            addNewRow.Cells.Add(hashCells.Algo);
            addNewRow.Cells.Add(hashCells.HashSpeed);
            addNewRow.Cells.Add(hashCells.HashSize);
            addNewRow.Cells.Add(hashCells.Watts);
            hashCells = ConvertHashrateToDgvCell(new Hashrate());
            // Add dual mining
            addNewRow.Cells.Add(hashCells.Coin);
            addNewRow.Cells.Add(hashCells.Algo);
            addNewRow.Cells.Add(hashCells.HashSpeed);
            addNewRow.Cells.Add(hashCells.HashSize);
            addNewRow.Cells.Add(hashCells.Watts);

            return addNewRow;
        }
        private void DisplayHashrates(Gpu gpu)
        {
            // Display the hashrate stats in the list
            foreach (var hashrate in gpu.Hashrates)
            {
                // Skip if no coin/algo
                if (hashrate.Coin == null)
                    continue;

                // Use gpu power if no specific watts given for this hashrate
                if (hashrate.Watts == 0)
                    hashrate.Watts = gpu.Watts;

                var hashrateCells = ConvertHashrateToDgvCell(hashrate);

                // Add values to data grid view
                var row = new DataGridViewRow();
                row.Cells.Add(hashrateCells.Coin);
                row.Cells.Add(hashrateCells.Algo);
                row.Cells.Add(hashrateCells.HashSpeed);
                row.Cells.Add(hashrateCells.HashSize);
                row.Cells.Add(hashrateCells.Watts);

                // Add dual mining hashrate if there is one
                if (hashrate.DualMineHashrate != null && hashrate.DualMineHashrate.Coin != null && hashrate.DualMineHashrate.Coin.algorithm != null && hashrate.DualMineHashrate.HashrateSpeed > 0)
                    hashrateCells = ConvertHashrateToDgvCell(hashrate.DualMineHashrate);
                else
                    hashrateCells = ConvertHashrateToDgvCell(new Hashrate());

                row.Cells.Add(hashrateCells.Coin);
                row.Cells.Add(hashrateCells.Algo);
                row.Cells.Add(hashrateCells.HashSpeed);
                row.Cells.Add(hashrateCells.HashSize);
                row.Cells.Add(hashrateCells.Watts);

                // Add hashrate to the gui list
                EditHashrates.InvokeIfRequired(e => { e.Rows.Add(row); });
            }            

            EditHashrates.InvokeIfRequired(e => { e.Rows.Add(GetEmptyRow()); });
        }
        private List<Hashrate> GetSelectedGpuHashratesFromGui()
        {
            var hashrates = new List<Hashrate>();
            // Get data from gui list
            for (int x = 0; x <= EditHashrates.RowCount - 1; x++)
            {
                if (EditHashrates[Constants.HashratesCoin, x].Value != null)
                {
                    var hashrate = GetSelectedGpuHashrateFromGui(x);
                    hashrates.Add(hashrate);
                }
            }
            return hashrates;
        }
        private Hashrate GetSelectedGpuHashrateFromGui(int rowIndex)
        {
            var hashrate = new Hashrate();
            hashrate.DualMineHashrate = new Hashrate();
            hashrate.DualMineHashrate.Coin = new CoinInfo();
            var newEntry = false;

            // Get the hashrate from gui
            hashrate.Coin = EditHashrates[Constants.HashratesAlgo, rowIndex].Tag as CoinInfo;

            // If this is a new hashrate entry
            if ( (hashrate.Coin == null || hashrate.Coin.coin == null) && EditHashrates[Constants.HashratesAlgo, rowIndex].Value != null)
            {
                newEntry = true;
                // Get the coin info for this algorithm
                foreach (CoinInfo coin in defaultCoins)
                    if (EditHashrates[Constants.HashratesAlgo, rowIndex].Value == coin.algorithm)
                    { 
                        hashrate.Coin = coin;
                        break;
                    }
            }

            // Hashrate speed
            var hashSpeed = ConvertHashrateFromReadable(EditHashrates[Constants.HashratesHashrate, rowIndex].Value.ToString() + " " + EditHashrates[Constants.HashratesSize, rowIndex].Value.ToString());
            hashrate.HashrateSpeed = hashSpeed;
            
            // Watts
            if(EditHashrates[Constants.HashratesWatts, rowIndex].Value != null)
                hashrate.Watts = int.TryParse(EditHashrates[Constants.HashratesWatts, rowIndex].Value.ToString(), out var watts) ? watts : 0;

            // Dual mine coin
            if (hashrate.DualMineHashrate != null && EditHashrates[Constants.HashratesCoin2, rowIndex].Value != null)
            {
                var coin2 = EditHashrates[Constants.HashratesAlgo2, rowIndex].Tag as CoinInfo;
                if (coin2 != null)
                    hashrate.DualMineHashrate.Coin = coin2;
            }

            // Dual mine hashrate speed
            if (hashrate.DualMineHashrate != null && EditHashrates[Constants.HashratesHashrate2, rowIndex].Value != null && EditHashrates[Constants.HashratesHashrate2, rowIndex].Value.ToString().Length > 0)
            {
                var hashSpeed2 = ConvertHashrateFromReadable(EditHashrates[Constants.HashratesHashrate2, rowIndex].Value.ToString() + " " + EditHashrates[Constants.HashratesSize2, rowIndex].Value.ToString());
                hashrate.DualMineHashrate.HashrateSpeed = hashSpeed2;
            }
            
            // Dual mine watts
            if (hashrate.DualMineHashrate != null && EditHashrates[Constants.HashratesWatts2, rowIndex].Value != null && EditHashrates[Constants.HashratesWatts2, rowIndex].Value.ToString().Length > 0)
                hashrate.DualMineHashrate.Watts = int.TryParse(EditHashrates[Constants.HashratesWatts2, rowIndex].Value.ToString(), out var parsedWatts2) ? parsedWatts2 : 0;

            // If this is a new entry add the complete hashrate object with coin info back to gui
            if(newEntry)
            {
                var gpu = EditGpuList.SelectedItems[0].Tag as Gpu;
                gpu.Hashrates.Add(hashrate);
                EditGpuList.SelectedItems[0].Tag = gpu;
            }
            return hashrate;
        }
        private dynamic ConvertHashrateToDgvCell(Hashrate hashrate)
        {
            if (hashrate == null || hashrate.Coin == null)
            {
                // Create coin selection combo box
                DataGridViewComboBoxCell coinComboBox = new DataGridViewComboBoxCell();
                coinComboBox.Items.Add(" "); // Add empty item if user wants to remove dual coin
                foreach (var coin in defaultCoins)
                    coinComboBox.Items.Add(coin.coin);                

                var algo = new DataGridViewTextBoxCell();
                algo.Tag = hashrate.Coin;
                algo.Value = "";

                var hashSpeedCell = new DataGridViewTextBoxCell();
                hashSpeedCell.Value = "";

                // Create the hashrate size combo box
                DataGridViewComboBoxCell hashSizeComboBox = new DataGridViewComboBoxCell();
                hashSizeComboBox.Items.Add("H/s");
                hashSizeComboBox.Items.Add("kH/s");
                hashSizeComboBox.Items.Add("MH/s");
                hashSizeComboBox.Items.Add("GH/s");
                hashSizeComboBox.Items.Add("TH/s");
                hashSizeComboBox.Items.Add("PH/s");
                hashSizeComboBox.Items.Add("EH/s");
                hashSizeComboBox.Items.Add("ZH/s");
                hashSizeComboBox.Value = hashSizeComboBox.Items[0].ToString();

                var wattsCell = new DataGridViewTextBoxCell();
                wattsCell.Value = "";

                dynamic hashrateCells = new ExpandoObject();
                hashrateCells.Coin = coinComboBox;
                hashrateCells.Algo = algo;
                hashrateCells.HashSpeed = hashSpeedCell;
                hashrateCells.HashSize = hashSizeComboBox;
                hashrateCells.Watts = wattsCell;

                return hashrateCells;
            }
            else
            {
                // Split up hashrate speed and hashrate size
                var readableHash = ConvertHashrateToReadable(hashrate.HashrateSpeed);
                var start = readableHash.IndexOf(" ") + 1;
                var hashSize = readableHash.Substring(start, readableHash.Length - start);
                var hashSpeed = readableHash.Substring(0, start - 1);

                // Create coin selection combo box
                DataGridViewComboBoxCell coinComboBox = new DataGridViewComboBoxCell();
                coinComboBox.Items.Add(" "); // Add empty item if user wants to remove dual coin
                foreach (var coin in defaultCoins)
                {
                    coinComboBox.Items.Add(coin.coin);
                    if (hashrate.Coin != null && coin.algorithm.ToLower() == hashrate.Coin.algorithm.ToLower())
                    {
                        coinComboBox.Value = coin.coin;
                        hashrate.Coin = coin;
                    }
                }

                // Create the hashrate size combo box
                DataGridViewComboBoxCell hashSizeComboBox = new DataGridViewComboBoxCell();
                hashSizeComboBox.Items.Add("H/s");
                hashSizeComboBox.Items.Add("kH/s");
                hashSizeComboBox.Items.Add("MH/s");
                hashSizeComboBox.Items.Add("GH/s");
                hashSizeComboBox.Items.Add("TH/s");
                hashSizeComboBox.Items.Add("PH/s");
                hashSizeComboBox.Items.Add("EH/s");
                hashSizeComboBox.Items.Add("ZH/s");
                foreach (var item in hashSizeComboBox.Items)
                    if (item.ToString().ToLower() == hashSize.ToLower())
                        hashSizeComboBox.Value = item.ToString();
                
                var algo = new DataGridViewTextBoxCell();
                algo.Tag = hashrate.Coin;
                algo.Value = hashrate.Coin.algorithm;

                var hashSpeedCell = new DataGridViewTextBoxCell();
                hashSpeedCell.Value = hashSpeed;

                var wattsCell = new DataGridViewTextBoxCell();
                wattsCell.Value = hashrate.Watts;

                dynamic hashrateCells = new ExpandoObject();
                hashrateCells.Coin = coinComboBox;
                hashrateCells.Algo = algo;
                hashrateCells.HashSpeed = hashSpeedCell;
                hashrateCells.HashSize = hashSizeComboBox;
                hashrateCells.Watts = wattsCell;

                return hashrateCells;
            }
            return null;
        }
        private void AddGpu_Click(object sender, EventArgs e)
        {
            var gpu = GetGpuPanelInfo();

            // Generate a unique id for this gpu
            gpu.Id = random.Next();

            // Add new gpu to Gui for editing
            if (UserEnteredValidGpuData(gpu))
            {
                var li = CreateGpuListViewItem(gpu);
                li.Selected = true;
                EditGpuList.Items.Add(li);
            }

            AddGpuToGpuList(gpu);
        }
        private void EditGpuList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && EditGpuList.SelectedItems != null)
            {
                // Get the next items index
                var nextItemIndex = EditGpuList.SelectedItems[0].Index - 1;
                if (nextItemIndex < 0)
                    nextItemIndex = 0;

                // Update the lists
                UpdateGpuList(true);
            }
        }
        private ListViewItem CreateGpuListViewItem(Gpu gpu)
        {
            // If adding new gpu give 3090 ti as an example
            if(gpu.ModelNumber == "unknown" && gpu.VersionPrefix == "unknown" && gpu.VersionSuffix == "unknown" && gpu.Manufacturer == "unknown")
            {
                gpu.ModelNumber = "3090";
                gpu.VersionPrefix = "rtx";
                gpu.VersionSuffix = "ti";
                gpu.VramSize = 24;
                gpu.Manufacturer = "nvidia";
            }
            var li = new ListViewItem();
            li.Text = gpu.Manufacturer;
            li.Tag = gpu;
            li.Name = gpu.Id.ToString();
            li.SubItems.Add(gpu.VersionPrefix);
            
            // Add Lhr to model name if an lhr card
            var modelNum = gpu.ModelNumber;
            if (gpu.IsLhr() && gpu.Lhr)
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
            if (gpu.Manufacturer.ToLower() == "nvidia" && gpu.IsLhr())
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

        #region Keydowns
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

                    // Only rename the file if this isn't a new list
                    if (GpuLists.SelectedItems[0].Text.Length > 0)
                    {
                        // Rename the gpu list file
                        var fileRenamed = false;
                        var errorMessage = "";
                        try { File.Move(oldFileName, newFileName); fileRenamed = true; }
                        catch (Exception ex) { errorMessage = ex.Message; }

                        // If the file wasn't renamed successfully inform the user
                        if (!fileRenamed && !AddingNewGpuList && GpuLists.SelectedItems[0].Text.Length > 0)
                            MessageBox.Show("Unable to rename { " + oldFileName + " } to { " + newFileName + "} because " + errorMessage);
                    }
                    UpdateGpuList(false, true);

                    AddingNewGpuList = false;
                }
                else
                    MessageBox.Show("The gpu list needs to have a name.");
            }
        }
        private void Lhr_Click(object sender, EventArgs e)
        {
            UpdateGpuList(false, true);
        }
        private void GpuVersionPrefix_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                UpdateGpuList(false, true);
        }
        private void GpuModelNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                UpdateGpuList(false, true);            
        }
        private void GpuVersionSuffix_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                UpdateGpuList(false, true);
        }
        private void GpuVramSize_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                UpdateGpuList(false, true);
        }
        private void AmdOrNvidia_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                UpdateGpuList(false, true);
        }
        private void DateReleased_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                UpdateGpuList(false, true);
        }
        private void MSRP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                UpdateGpuList(false, true);
        }
        private void EbayPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                UpdateGpuList(false, true);
        }
        private void PricePaid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                UpdateGpuList(false, true);
        }
        #endregion


        // Editing hashrates
        private int EditHashratesSelectedColIndex = 0;
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

            UpdateGpuList(false, true);
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
            AddingNewGpuList = false;
        }
        private void Cb_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // Get the user made selection
            var cb = (DataGridViewComboBoxEditingControl)sender;

            if (cb.SelectedItem == null || cb.SelectedItem == "")
                return;

            var selectedCoinSymbol = cb.SelectedItem.ToString().ToLower();

            // Get algorithm name from coin list
            var selectedCoin = new CoinInfo();
            foreach (CoinInfo coin in defaultCoins)
                if (selectedCoinSymbol == coin.coin.ToLower())
                    selectedCoin = coin;

            // If the first coin column is selected replace the algo next to it
            if (EditHashratesSelectedColIndex == Constants.HashratesCoin)
            {
                EditHashrates[Constants.HashratesAlgo, EditHashrates.SelectedRows[0].Index].Value = selectedCoin.algorithm;
                EditHashrates[Constants.HashratesAlgo, EditHashrates.SelectedRows[0].Index].Tag = selectedCoin;
                // Update gpu list
                UpdateGpuList();
            }

            // If the dual coin column is selected replace the algo next to it
            else if (EditHashratesSelectedColIndex == Constants.HashratesCoin2)
            {
                EditHashrates[Constants.HashratesAlgo2, EditHashrates.SelectedRows[0].Index].Value = selectedCoin.algorithm;
                EditHashrates[Constants.HashratesAlgo2, EditHashrates.SelectedRows[0].Index].Tag = selectedCoin;
                // Update gpu list
                UpdateGpuList();
            }            
        }
        private void Hashrates_KeyDown(object sender, KeyEventArgs e)
        {
            // User is copying hashrate row
            if (e.KeyData == (Keys.Control | Keys.C) && !EditHashrates.IsCurrentCellInEditMode)
            {
                CopiedHashrate = GetSelectedGpuHashrateFromGui(EditHashrates.SelectedRows[0].Index);
            }
            // User is pasting hashrate row
            else if (e.KeyData == (Keys.Control | Keys.V) && !EditHashrates.IsCurrentCellInEditMode)
            {
                var hashCells = ConvertHashrateToDgvCell(CopiedHashrate);
                
                // Add defaults to data grid view
                var row = new DataGridViewRow();
                row.Cells.Add(hashCells.Coin);
                row.Cells.Add(hashCells.Algo);
                row.Cells.Add(hashCells.HashSpeed);
                row.Cells.Add(hashCells.HashSize);
                row.Cells.Add(hashCells.Watts);
                
                if(CopiedHashrate.DualMineHashrate != null && CopiedHashrate.DualMineHashrate.HashrateSpeed > 0)
                    hashCells = ConvertHashrateToDgvCell(CopiedHashrate.DualMineHashrate);
                else
                    hashCells = ConvertHashrateToDgvCell(new Hashrate());

                // Add dual mining
                row.Cells.Add(hashCells.Coin);
                row.Cells.Add(hashCells.Algo);
                row.Cells.Add(hashCells.HashSpeed);
                row.Cells.Add(hashCells.HashSize);
                row.Cells.Add(hashCells.Watts);

                // Remove selected row
                EditHashrates.Rows.Remove(EditHashrates.SelectedRows[0]);

                // Add copied hashrate row
                EditHashrates.Rows.Add(row);

                UpdateGpuList();

            }
            else if (e.KeyCode == Keys.Delete && !EditHashrates.IsCurrentCellInEditMode && EditHashrates.SelectedRows.Count > 0)
            {
                // Remove hashrate from gui list
                EditHashrates.Rows.Remove(EditHashrates.SelectedRows[0]);
                UpdateGpuList();
            }
            else if (e.KeyCode == Keys.Enter && EditHashrates.SelectedRows.Count > 0)
            {
                UpdateGpuList();
            }
        }
        private void EditHashrates_Click(object sender, EventArgs e)
        {
            EditHashratesSelectedColIndex = EditHashrates.CurrentCell.ColumnIndex;

            // If clicking the last row to add new hashrate info, create a new empty row
            if (EditHashrates.SelectedRows.Count > 0 && EditHashrates.SelectedRows[0].Index == EditHashrates.RowCount - 1)
            {
                var index = EditHashrates.SelectedRows[0].Index;
                if (index > 0)
                    index -= 1;
                // Only if the previous row has data
                if ((EditHashrates[Constants.HashratesCoin, index].Value != null && EditHashrates[Constants.HashratesCoin, index].Value.ToString().Length > 0)
                   || (EditHashrates[Constants.HashratesHashrate, index].Value != null && EditHashrates[Constants.HashratesHashrate, index].Value.ToString().Length > 0)
                   || (EditHashrates[Constants.HashratesWatts, index].Value != null && EditHashrates[Constants.HashratesWatts, index].Value.ToString().Length > 0)
                   || (EditHashrates[Constants.HashratesCoin2, index].Value != null && EditHashrates[Constants.HashratesCoin2, index].Value.ToString().Length > 0)
                   || (EditHashrates[Constants.HashratesHashrate2, index].Value != null && EditHashrates[Constants.HashratesHashrate2, index].Value.ToString().Length > 0)
                   || (EditHashrates[Constants.HashratesWatts2, index].Value != null && EditHashrates[Constants.HashratesWatts2, index].Value.ToString().Length > 0))
                {
                    EditHashrates.Rows.Add(GetEmptyRow());
                }
            }
        }
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
            }
        }
        private void EditHashrates_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (EditHashrates.RowCount > 0)
            {
                // Save
                UpdateGpuList();
            }
        }
        private void EditHashrates_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && (e.ColumnIndex == Constants.HashratesCoin || e.ColumnIndex == Constants.HashratesSize || e.ColumnIndex == Constants.HashratesCoin2 || e.ColumnIndex == Constants.HashratesSize2))
            {
                // Due to a Windows 11 ghosting bug in the 
                //  combobox cell, we repaint it a few times, 
                //  for the ghosting to go away
                e.Paint(e.ClipBounds, e.PaintParts);
                e.Paint(e.ClipBounds, e.PaintParts);
                e.Paint(e.ClipBounds, e.PaintParts);
            }
        }
        private void EditHashrates_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Get the column index of the item the user clicked
            EditHashratesSelectedColIndex = EditHashrates.CurrentCell.ColumnIndex;
        }
        private void Hashrates_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Allow user to copy / paste hashrate rows
            if (e.Control is DataGridViewTextBoxEditingControl tb)
            {
                tb.KeyDown -= Hashrates_KeyDown;
                tb.KeyDown += Hashrates_KeyDown;
            }
            else if (e.Control is DataGridViewComboBoxEditingControl cb)
            {
                // Add logic for when user makes a selection in edit hashrates table
                cb.SelectedIndexChanged += Cb_SelectedIndexChanged;
            }
        }



        // Get/Change ebay item
        private async Task<List<EbayItem>> GetEbayPrice(Gpu gpu)
        {
            // If the gpu is the same (i.e. 3080 asus tuf then 3080 evga ftw3, etc.)
            // don't search Ebay again, load previous data, also only refresh previous
            // data if it has been more than 4 hours since it was last searched

            var ebayItems = new List<EbayItem>();

            foreach (List<EbayItem> searchResults in previousSearchResults)
            {
                if (searchResults.Count == 0)
                    continue;
                if (searchResults[0].Name == gpu.Name && DateTime.Compare(searchResults[0].LastUpdated.AddHours(4), DateTime.Now) > 0)
                    return searchResults;     

            }

            // If it hasn't been searched for yet, or its been over 4hrs since
            // last search then search now
            ebayItems = await ebay.GetLowestPrice(gpu);

            // And save the data for future searches
            previousSearchResults.Add(ebayItems);

            return ebayItems;
        }
        private void GetEbayPrice_Click(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                var gpu = new Gpu();
                EditGpuList.InvokeIfRequired(c => { gpu = c.SelectedItems[0].Tag as Gpu; });

                if (gpu != null)
                {
                    var ebayItems = await GetEbayPrice(gpu);
                    if (ebayItems != null && ebayItems.Count > 0)
                    {
                        EbayPrice.Text = ebayItems[0].Price.ToString("0.00");
                        EbayItemUrl.Tag = ebayItems[0].Url;
                        UpdateEbayItemSelections(gpu, EbayItemSelection, ResultsEbayLink, EbayPrice);
                    }
                    else
                    {
                        EbayPrice.Text = "0";
                        MessageBox.Show("Unfortunately I was unable to find any ebay listings for this gpu from a reputable seller.");
                    }
                }
            });
        }
        private void UpdateEbayItemSelections(Gpu gpu, ComboBox selectionList, LinkLabel link, TextBox price = null)
        {
            if (!userChangedEbaySelection)
                return;

            // Clear previous data
            selectionList.Items.Clear();
            selectionList.Tag = null;
            link.Tag = "";

            // Inform user we are searching
            selectionList.Items.Add("Searching ebay...");
            selectionList.SelectedIndex = 0;

            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (object sender, DoWorkEventArgs e) =>
            {
                // Get ebay price
                try
                {
                    e.Result = GetEbayPrice(gpu).Result;
                }
                catch (Exception ex) { }
            };
            worker.ProgressChanged += (object sender, ProgressChangedEventArgs e) =>
            {
                HashratesProgressBar.Value = e.ProgressPercentage;
            };
            worker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
            {
                // Remove our gui user message that we are searching
                selectionList.Items.Clear();

                var ebayItems = e.Result as List<EbayItem>;
                // Add ebay listings if any found
                if (ebayItems != null && ebayItems.Count > 0)
                {
                    // Show the price when editing gpu lists
                    if (price != null)
                        price.Text = ebayItems[0].Price.ToString();

                    // Update the gui link
                    link.Tag = ebayItems[0].Url.ToString();

                    // Update the gui list
                    foreach (var item in ebayItems)
                        selectionList.Items.Add("$" + item.Price + " " + item.Name + " id " + item.Id);

                    // Save the list in case the user wants to change ebay listings
                    selectionList.Tag = ebayItems;
                }
                else
                    selectionList.Items.Add("No reputable listings found");

                // Select the cheapest result
                selectionList.SelectedIndex = 0;
            };
            worker.RunWorkerAsync();
        }
        private void EbayItemSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected item
            var selectedItem = EbayItemSelection.SelectedItem.ToString();

            // Do nothing if user changes the gpu while loading listings
            if (selectedItem == "No reputable listings found" || selectedItem == "Searching ebay...")
                return;

            // Get the selected ebay id 
            int start = selectedItem.IndexOf("id ") + 3;
            var itemId = selectedItem.Substring(start, selectedItem.Length - start).Replace(" ", "");

            // Get the ebay listings
            var ebayItems = EbayItemSelection.Tag as List<EbayItem>;

            // If there were listings found
            if (ebayItems != null && ebayItems.Count > 0)
            {
                // Get the user selected ebay listing
                var ebayItem = ebayItems.Find(item => item.Id == itemId);

                // Update the gui
                EbayPrice.Text = ebayItem.Price.ToString("0.00");
                EbayItemUrl.Tag = ebayItem.Url;
            }
                
        }
        private void EbayItemUrl_Click(object sender, EventArgs e)
        {
            if (EbayItemUrl.Tag == null || EbayItemUrl.Tag.ToString().Length == 0)
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
        private void ResultsEbayLink_Click(object sender, EventArgs e)
        {
            // Open ebay item link in browser
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = ResultsEbayLink.Tag.ToString();
            p.Start();
        }
        private void ResultsEbayItemSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected ebay id 
            var itemId = ResultsEbayItemSelection.SelectedItem.ToString();
            int start = itemId.IndexOf("id ") + 3;
            itemId = itemId.Substring(start, itemId.Length - start).Replace(" ", "");

            // Get the ebay listings
            var ebayItems = ResultsEbayItemSelection.Tag as List<EbayItem>;

            // If there were listings found
            if (ebayItems != null && ebayItems.Count > 0)
            {
                // Get the user selected ebay listing
                var ebayItem = ebayItems.Find(item => item.Id == itemId);

                // Update the gui
                ResultsEbayLink.Tag = ebayItem.Url;

                // Find gpu in results list 
                if (ResultsList.SelectedItems.Count > 0)
                {
                    // and update its ebay price to the one selected
                    ResultsList.SelectedItems[0].SubItems[Constants.EbayPrice].Text = "$" + ebayItem.Price.ToString("0.00");

                    var gpus = GpuLists.SelectedItems[0].Tag as List<Gpu>;

                    // Update the gpu in the master list
                    var gpuAndHash = ResultsList.SelectedItems[0].Tag as dynamic;
                    var gpu = gpuAndHash.Gpu;
                    
                    var gpuToUpdate = gpus.Find(gpus => gpus.Id == gpu.Id);
                    if (gpuToUpdate != null)
                    {
                        gpuToUpdate.EbayPrice = gpu.EbayPrice;

                        GpuLists.SelectedItems[0].Tag = gpus;
                    }
                    // ToDo Perform calculations on this gpu again and re-sort listview
                    
                }
            }
        }



        // Filters
        private bool FilterGpu(Gpu gpu)
        {
            // Apply filters
            if (gpu.Manufacturer.ToLower() == "amd" && !ShowAmd.Checked)                
                return true; // remove this item
            if (gpu.Manufacturer.ToLower() == "nvidia" && !ShowNvidia.Checked)
                return true; // remove this item
            if (int.TryParse(VramFilter.Text, out var vram) && vram > 0 && gpu.VramSize <= vram)
                return true;
            if (int.TryParse(YearsOld.Text, out var yearsOld) && (DateTime.Now.Year - gpu.DateReleased.Year) > yearsOld)
                return true;
            return false;
        }
        private void FilterResults()
        {
            // Clear previous filtered results
            filteredSearchResults.Clear();

            // Go through all gpus in results and apply filters
            foreach (ListViewItem item in CurrentSearchResults)
            {
                var GpuAndCalc = item.Tag as dynamic;
                var gpu = GpuAndCalc.Gpu;

                if (!FilterGpu(gpu))
                    filteredSearchResults.Add(item); // Show this item
            }
            // Clear results list
            ResultsList.Items.Clear();

            // Add gpus to show
            ResultsList.Items.AddRange(filteredSearchResults.ToArray());
            
            // Recalculate totals
            PopulateTotalsGui(CalculateTotals());
        }
        private void ShowNvidia_CheckedChanged(object sender, EventArgs e)
        {
            if (PerformingCalculations != null && PerformingCalculations.IsCompleted)
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
            if (PerformingCalculations != null && PerformingCalculations.IsCompleted)
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
            if(PerformingCalculations != null && !PerformingCalculations.IsCompleted && e.KeyCode == Keys.Enter)
            {
                ShowErrorMessage(Constants.PerformingCalcs);
                return;
            }
            if(PerformingCalculations != null && PerformingCalculations.IsCompleted && e.KeyCode == Keys.Enter)
            {
                FilterResults();
            }
        }
        private void YearsOld_KeyDown(object sender, KeyEventArgs e)
        {
            if (PerformingCalculations != null && !PerformingCalculations.IsCompleted && e.KeyCode == Keys.Enter)
            {
                ShowErrorMessage(Constants.PerformingCalcs);
                return;
            }
            if (PerformingCalculations != null && PerformingCalculations.IsCompleted && e.KeyCode == Keys.Enter)
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
        private string ConvertHashrateToReadable(double hashrate)
        {
            var zettaHash = hashrate / 1e+21;
            var exaHash = hashrate / 1000000000000000000;
            var petaHash = hashrate / 1000000000000000;
            var teraHash = hashrate / 1000000000000;
            var gigaHash = hashrate / 1000000000;
            var megaHash = hashrate / 1000000;
            var kiloHash = hashrate / 1000;
            var hash = hashrate / 1;

            if (zettaHash > 1)
                return zettaHash.ToString("0.00") + " zh/s";
            if (exaHash > 1)
                return exaHash.ToString("0.00") + " eh/s";
            if (petaHash > 1)
                return petaHash.ToString("0.00") + " ph/s";
            if (teraHash > 1)
                return teraHash.ToString("0.00") + " th/s";
            if (gigaHash > 1)
                return gigaHash.ToString("0.00") + " gh/s";
            if (megaHash > 1)
                return megaHash.ToString("0.00") + " mh/s";
            if (kiloHash > 1)
                return kiloHash.ToString("0.00") + " kh/s";

            return hash.ToString("0.00") + " h/s";
        }
        private double ConvertHashrateFromReadable(string hashrate)
        {
            var start = hashrate.IndexOf(" ") + 1;
            var hashSpeed = double.TryParse(hashrate.Substring(0, start - 1), out var parsedHashSpeed) ? parsedHashSpeed : 0.0;
            var hashSize = hashrate.Substring(start, hashrate.Length - start);

            switch (hashSize.ToLower())
            {
                case "h/s":
                    return hashSpeed;

                case "kh/s":
                    return hashSpeed * Constants.KiloHash;

                case "mh/s":
                    return hashSpeed * Constants.MegaHash;

                case "gh/s":
                    return hashSpeed * Constants.GigaHash;

                case "th/s":
                    return hashSpeed * Constants.TeraHash;

                case "ph/s":
                    return hashSpeed * Constants.PetaHash;

                case "eh/s":
                    return hashSpeed * Constants.ExaHash;

                case "zh/s":
                    return hashSpeed * Constants.ZettaHash;
            }
            return hashSpeed;
        }
        private async Task<double> GetGpuCost(Gpu gpu)
        {
            var gpuCost = 0.0;
            if (gpu.PricePaid > 0)
                gpuCost = gpu.PricePaid;
            else
            {
                // Otherwise use ebay price if it isn't 0
                if (gpu.EbayPrice > 0)
                    gpuCost = gpu.EbayPrice;

                // If ebay price is 0 then try to get ebay price now
                else if (gpu.EbayPrice == 0)
                {
                    try
                    {
                        var ebayItems = await GetEbayPrice(gpu);
                        foreach (var item in ebayItems)
                        {
                            if (double.TryParse(item.Price.ToString(), out var ebayPrice))
                            {
                                double.TryParse(ebayItems[ebayItems.Count - 1].Price.ToString(), out var mostExpensiveItem);
                                gpu.EbayLink = item.Url;
                                gpu.EbayPrice = ebayPrice;
                                gpuCost = ebayPrice;
                                break;
                            }
                        }
                    }
                    catch { }
                }

                // If can't get ebay price then try msrp if it isn't 0
                if (gpu.EbayPrice == 0 && gpu.MSRP > 0)
                    gpuCost = gpu.MSRP;
            }
            return gpuCost;
        }
        private int GetImageIndex(string coin)
        {
            var index = 0;
            try { index = coinImageList.Images.IndexOfKey(coin.ToLower() + ".jpg"); }
            catch { index = 0; }            
            return index;
        }
    }
}
