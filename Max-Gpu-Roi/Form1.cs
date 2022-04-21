using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace Max_Gpu_Roi
{
    public partial class MaxGpuRoi : Form
    {
        private string GpuListsDirectory = Directory.GetCurrentDirectory();
        private string CoinListsDirectory = Directory.GetCurrentDirectory();
        private List<CoinInfo> AllCoinInfos = new List<CoinInfo>();
        private MinerStat minerStat = new MinerStat();
        private Timer minerstatTimer = new Timer();
        private Ebay ebay = new Ebay();
        private List<EbayItem> previousSearchResults = new List<EbayItem>();
        private GpuHashrate CopiedHashrate = new GpuHashrate();
        public MaxGpuRoi()
        {
            InitializeComponent();
        }

        // Startup
        private async void MaxGpuRoi_Load(object sender, EventArgs e)
        {
            // Set window size
            this.Size = new Size(1873, 942);
            this.MaximumSize = new Size(1873, 942);

            // Set Edit Gpu list panel to the proper location
            EditGpuPanel.Location = new Point(40, 150);

            // Set Edit Coin list panel to the proper location
            EditCoinPanel.Location = new Point(380, 150);

            // Setup Gpu lists 
            GpuLists.Columns.Add("Name", -2, HorizontalAlignment.Left);
            GpuLists.Columns.Add("# of Gpus", -2, HorizontalAlignment.Left);
            GpuLists.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            // Setup List of All Coins
            ListOfAllCoins.Columns.Add("Name", -2, HorizontalAlignment.Left);
            ListOfAllCoins.Columns.Add("Symbol", -2, HorizontalAlignment.Center);
            ListOfAllCoins.Columns.Add("Algorithm", -2, HorizontalAlignment.Left);
            ListOfAllCoins.Columns.Add("Price", -2, HorizontalAlignment.Left);
            ListOfAllCoins.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            // Setup Coin lists 
            CoinLists.Columns.Add("Name", -2, HorizontalAlignment.Left);
            CoinLists.Columns.Add("# of Coins", -2, HorizontalAlignment.Left);
            CoinLists.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            // Setup Edit Coin list
            EditCoinList.Columns.Add("Name", -2, HorizontalAlignment.Left);
            EditCoinList.Columns.Add("Symbol", -2, HorizontalAlignment.Center);
            EditCoinList.Columns.Add("Algorithm", -2, HorizontalAlignment.Left);
            EditCoinList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            // Setup Edit Gpu list
            EditGpuList.Columns.Add("Name", -2, HorizontalAlignment.Left);
            EditGpuList.Columns.Add("Manufacturer", -2, HorizontalAlignment.Left);
            EditGpuList.Columns.Add("Version", -2, HorizontalAlignment.Left);
            EditGpuList.Columns.Add("Vram Size", -2, HorizontalAlignment.Left);
            EditGpuList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            // Setup Edit Gpu Hashrates
            Hashrates.Columns.Add("", "Coin");// .Add("Coin", -2, HorizontalAlignment.Left);
            Hashrates.Columns.Add("", "Algorithm");//, -2, HorizontalAlignment.Left);
            Hashrates.Columns.Add("", "Hashrate");//, -2, HorizontalAlignment.Left);
            Hashrates.Columns.Add("", "Watts");//, -2, HorizontalAlignment.Left);
            Hashrates.Columns.Add("", "Lhr");//, -2, HorizontalAlignment.Left);
            Hashrates.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);//.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            // Setup Results list
            ResultsList.Columns.Add("Coin", -2, HorizontalAlignment.Left);
            ResultsList.Columns.Add("Gpu", -2, HorizontalAlignment.Left);
            ResultsList.Columns.Add("Manufacturer", -2, HorizontalAlignment.Left);
            ResultsList.Columns.Add("Price", -2, HorizontalAlignment.Left);
            ResultsList.Columns.Add("Costs", -2, HorizontalAlignment.Left);
            ResultsList.Columns.Add("Hashrate / Watts", -2, HorizontalAlignment.Left);
            ResultsList.Columns.Add("Rewards", -2, HorizontalAlignment.Left);
            ResultsList.Columns.Add("R-O-I", -2, HorizontalAlignment.Left);
            ResultsList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            SortBy.SelectedItem = "Price per Mhs";

            // Attempt to load gpu defaults
            var li = new ListViewItem("DefaultGpuList");
            var file = GpuListsDirectory + "\\" + "DefaultGpuList.json";
            var gpus = await JsonCrud.LoadGpuList(file);

            // Add gpu listview item
            li.Tag = file;
            li.SubItems.Add(gpus.Count.ToString());
            GpuLists.Items.Add(li);
            GpuLists.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            // Attempt to load coin defaults
            li = new ListViewItem("DefaultCoinList");
            file = CoinListsDirectory + "\\" + "DefaultCoinList.json";
            var coins = await JsonCrud.LoadCoinList(file);

            // Add coin listview item
            li.Tag = file;
            li.SubItems.Add(gpus.Count.ToString());
            CoinLists.Items.Add(li);
            CoinLists.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            // Timer to prevent unnecessary api calls to minerstat
            minerstatTimer.Elapsed += new ElapsedEventHandler(OnMinerStatTimerTickEvent);
            minerstatTimer.Interval = 300000; // 5mins
            minerstatTimer.Enabled = true;
        }

        private async void OnMinerStatTimerTickEvent(object sender, ElapsedEventArgs e)
        {
            // Refresh coin info every 5 minutes since minerstat doesn't update until every 5-10mins
            AllCoinInfos = await minerStat.GetAllCoins();
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



        // Results GUI
        private void AddResultsListviewItem(Gpu gpu, string cryptoSymbol, bool lhr)
        {
            var li = new ListViewItem();
            li.Text = cryptoSymbol;

            // Lookup image for correct symbol
            li.ImageIndex = 0;

            li.SubItems.Add("Gpu: " + gpu.Name);
            li.SubItems.Add("Maker: " + gpu.Manufacturer + Environment.NewLine + "Version: " + gpu.Version);
            li.SubItems.Add("MSRP: $" + gpu.MSRP + "     Ebay: $" + gpu.EbayPrice + Environment.NewLine + "Paid: $" + gpu.PricePaid);

            // Error proofing to allow only whole numbers for miner/pool fee
            if (!int.TryParse(PoolMinerFee.Text, out var fee))
            {
                MessageBox.Show("Miner/Pool Fee Must be a Whole Number!");
                return;
            }

            li.SubItems.Add("Miner/Pool Fee: " + fee + Environment.NewLine + "Electricity: $15");

            // Get mhs and watts
            //var mhs = JsonCrud.GetMhs(gpu.Id, cryptoSymbol, lhr);
            //var watts = JsonCrud.GetWatts(gpu.Id, cryptoSymbol, lhr);
            //li.SubItems.Add("Mhs: " + mhs + Environment.NewLine + "Watts: " + watts);

            // Calculate Rewards
            li.SubItems.Add("Crypto Rewards: " + Environment.NewLine + "USD Rewards: $");

            // Calculate ROI
            li.SubItems.Add("ROI: 1 year 5 months 3 weeks 2 days" + Environment.NewLine + "17.75 months");

            // Add the item and resize the columns to fit all content
            ResultsList.Items.Add(li);
            ResultsList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
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
            Ascending.Enabled = false;
            Descending.Enabled = false;
            ElectricityRate.Enabled = false;
            PoolMinerFee.Enabled = false;
            ShowAmd.Enabled = false;
            ShowNvidia.Enabled = false;
            SortBy.Enabled = false;
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
            Ascending.Enabled = true;
            Descending.Enabled = true;
            ElectricityRate.Enabled = true;
            PoolMinerFee.Enabled = true;
            ShowAmd.Enabled = true;
            ShowNvidia.Enabled = true;
            SortBy.Enabled = true;
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

        // User picked a different gpu to edit
        private async void EditGpuList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset list
            Hashrates.Rows.Clear();

            // If nothing selected do nothing
            if (EditGpuList.SelectedItems.Count == 0)
                return;

            // Display all info for the selected gpu
            var gpu = EditGpuList.SelectedItems[0].Tag as Gpu;
            AmdOrNvidia.Text = gpu.AmdOrNvidia;
            AmdOrNvidia.ForeColor = gpu.AmdOrNvidia.ToLower() == "amd" ? Color.Red : Color.Green;
            DateReleased.Text = gpu.DateReleased.ToString("Y");
            MSRP.Text = gpu.MSRP.ToString("0.00");
            PricePaid.Text = gpu.PricePaid.ToString("0.00");
            GpuManufacturer.Text = gpu.Manufacturer;
            GpuName.Text = gpu.Name;
            GpuVersion.Text = gpu.Version;
            GpuVramSize.Text = gpu.VramSize.ToString();

            // Get ebay price
            var price = await GetEbayPrice(gpu.Name);
            EbayPrice.Text = price.ToString("0.00");

            // Read all saved hashrate stats for the selected gpu from the hashrates.json file
            var hashrates = await JsonCrud.GetHashrates(gpu.Id);

            // Display the hashrate stats in the list
            foreach (var hashrate in hashrates)
            {
                var addHashrate = true;
                for (int x = 0; x <= Hashrates.RowCount - 1; x++)
                {
                    if (Hashrates[0, x].Value != null)
                    {
                        var hashOnList = new GpuHashrate();
                        hashOnList.Id = gpu.Id;
                        hashOnList.Coin = Hashrates[0, x].Value.ToString();
                        hashOnList.Algorithm = Hashrates[1, x].Value.ToString();
                        hashOnList.Hashrate = double.Parse(Hashrates[2, x].Value.ToString());
                        hashOnList.Watts = int.Parse(Hashrates[3, x].Value.ToString());
                        hashOnList.Lhr = Hashrates[4, x].Value.ToString().ToLower() == "true" ? true : false;

                        // If this exact entry is already on the list don't add it
                        if (hashOnList.Id == hashrate.Id && hashOnList.Coin == hashrate.Coin
                            && hashOnList.Algorithm == hashrate.Algorithm
                            && hashOnList.Hashrate == hashrate.Hashrate
                            && hashOnList.Watts == hashrate.Watts
                            && hashOnList.Lhr == hashrate.Lhr)
                            addHashrate = false;
                    }
                }

                if (addHashrate)
                {
                    Hashrates.Rows.Add(hashrate.Coin, hashrate.Algorithm, hashrate.Hashrate, hashrate.Watts, hashrate.Lhr.ToString());
                }
            }
            Hashrates.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }
        private async Task<double> GetEbayPrice(string gpuName)
        {
            // If the gpu is the same (i.e. 3080 asus tuf then 3080 evga ftw3, etc.)
            // don't search Ebay again, load previous data, also only refresh previous
            // data if it has been more than 4 hours since it was last searched

            foreach (var ebayItem in previousSearchResults)
            {
                if (ebayItem.Name == gpuName && DateTime.Compare(ebayItem.LastUpdated.AddHours(4), DateTime.Now) > 0)
                    return ebayItem.Price;
            }

            // If it hasn't been searched for yet, then search now
            var eItem = await ebay.GetLowestPrice(gpuName);

            // Show user the link if they want to view this item on ebay in browser
            EbayItemUrl.Tag = eItem.Url;

            // And save the data for future searches
            previousSearchResults.Add(eItem);
            return eItem.Price;
        }

        // Button Clicks
        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private async void EditGpuLists_Click(object sender, EventArgs e)
        {
            // If no list is selected show a popup to inform the user
            if (GpuLists.SelectedItems.Count == 0)
            {
                MessageBox.Show("No Gpu List Selected to Edit!");
                return;
            }

            // Show the edit panel and disable user input on the main GUI
            EditGpuPanel.Visible = true;
            DisableUserInput();

            // Load the gpu list from file
            var file = GpuLists.SelectedItems[0].Tag.ToString();
            var gpus = await JsonCrud.LoadGpuList(file);

            GpuListName.Text = GpuLists.SelectedItems[0].Text;
            foreach (var gpu in gpus)
            {
                // Only add it if it isn't already on the list
                if (!EditGpuList.Items.ContainsKey(gpu.Id.ToString()))
                {
                    var li = new ListViewItem(gpu.Name);
                    li.Tag = gpu;
                    li.Name = gpu.Id.ToString();
                    li.SubItems.Add(gpu.Manufacturer);
                    li.SubItems.Add(gpu.Version);
                    li.SubItems.Add(gpu.VramSize.ToString());

                    EditGpuList.Items.Add(li);
                }
            }

            EditGpuList.Columns[0].Width = 100;// AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            EditGpuList.Columns[1].Width = 90;
            EditGpuList.Columns[2].Width = 60;
            EditGpuList.Columns[3].Width = 65;

            // Select first gpu in the list
            EditGpuList.Items[0].Selected = true;
        }
        private void CancelGpuHashrates_Click(object sender, EventArgs e)
        {
            EditGpuPanel.Visible = false;
            EnableUserInput();
        }
        private async void EditCoinLists_Click(object sender, EventArgs e)
        {
            // If no list is selected show a popup to inform the user
            if (CoinLists.SelectedItems.Count == 0)
            {
                MessageBox.Show("No Coin List Selected to Edit!");
                return;
            }

            // Show the edit panel and disable user input on the main GUI
            EditCoinPanel.Visible = true;
            DisableUserInput();

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

            // Show list of all available coins

            // Only call minerstat api if there is no coin info 
            if (AllCoinInfos.Count == 0)
                AllCoinInfos = await minerStat.GetAllCoins();

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
        private void CancelEditCoinList_Click(object sender, EventArgs e)
        {
            EditCoinPanel.Visible = false;
            EnableUserInput();
        }
        private void SaveGpuHashrates_Click(object sender, EventArgs e)
        {
            EditGpuPanel.Visible = false;
            EnableUserInput();
            var gpu = EditGpuList.SelectedItems[0].Tag as Gpu;

            // Save hashrate data
            var hashrates = new List<GpuHashrate>();
            // Get data from list
            for (int x = 0; x <= Hashrates.RowCount - 1; x++)
            {
                if (Hashrates[0, x].Value != null)
                {
                    var hashOnList = new GpuHashrate();
                    hashOnList.Id = gpu.Id;
                    hashOnList.Coin = Hashrates[0, x].Value.ToString();
                    hashOnList.Algorithm = Hashrates[1, x].Value.ToString();
                    hashOnList.Hashrate = double.Parse(Hashrates[2, x].Value.ToString());
                    hashOnList.Watts = int.Parse(Hashrates[3, x].Value.ToString());
                    hashOnList.Lhr = Hashrates[4, x].Value.ToString().ToLower() == "true" ? true : false;
                    hashrates.Add(hashOnList);
                }
            }
            JsonCrud.SaveHashrates(hashrates);

            // Save gpu list
            var gpus = new List<Gpu>();
            foreach (ListViewItem item in EditGpuList.Items)
            {
                var gpuToAdd = item.Tag as Gpu;
                gpus.Add(gpuToAdd);
            }
            JsonCrud.SaveGpuList(gpus, GpuListName.Text);
        }
        private async void SaveCoinList_Click(object sender, EventArgs e)
        {
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
            await JsonCrud.SaveCoinList(coins, CoinListName.Text);
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
        private void EbayItemUrl_Click(object sender, EventArgs e)
        {
            // Open ebay item link in browser
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = EbayItemUrl.Tag.ToString();
            p.Start();
        }
        private async void GetEbayPrice_Click(object sender, EventArgs e)
        {
            var price = await GetEbayPrice(GpuName.Text);
            EbayPrice.Text = price.ToString();
        }

        // Delete lists
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
                    if (CoinLists.SelectedItems[0].Text == "DefaultCoinList")
                        return;

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
                    if (GpuLists.SelectedItems[0].Text == "DefaultGpuList")
                        return;

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
        private void DeleteGpu_Click(object sender, EventArgs e)
        {
            // Delete selected gpu from list
            foreach (ListViewItem item in EditGpuList.SelectedItems)
            {
                EditGpuList.Items.Remove(item);
            }

            // Delete all hashrates
            Hashrates.Rows.Clear();
            Hashrates.Refresh();

            // Reset gpu info
            GpuName.Text = "";
            GpuManufacturer.Text = "";
            GpuVersion.Text = "";
            GpuVramSize.Text = "";
            AmdOrNvidia.Text = "";
            DateReleased.Text = "";
            MSRP.Text = "";
            EbayPrice.Text = "";
            PricePaid.Text = "";
        }
        private void UpdateGpuList_Click(object sender, EventArgs e)
        {
            if (EditGpuList.SelectedItems != null)
            {
                var gpu = EditGpuList.SelectedItems[0].Tag as Gpu;
                gpu.Name = GpuName.Text;
                gpu.Manufacturer = GpuManufacturer.Text;
                gpu.Version = GpuVersion.Text;
                gpu.VramSize = int.Parse(GpuVramSize.Text);
                gpu.AmdOrNvidia = AmdOrNvidia.Text;
                gpu.DateReleased = DateTime.Parse(DateReleased.Text);
                gpu.MSRP = double.Parse(MSRP.Text);
                gpu.EbayPrice = double.Parse(EbayPrice.Text);
                gpu.PricePaid = double.Parse(PricePaid.Text);
                EditGpuList.SelectedItems[0].Tag = gpu;

                EditGpuList.SelectedItems[0].Text = gpu.Name;
                EditGpuList.SelectedItems[0].SubItems[1].Text = gpu.Manufacturer;
                EditGpuList.SelectedItems[0].SubItems[2].Text = gpu.Version;
                EditGpuList.SelectedItems[0].SubItems[3].Text = gpu.VramSize.ToString();
            }
        }
        private void AddGpu_Click(object sender, EventArgs e)
        {
            var gpu = new Gpu();

            // Prevent missing data from getting saved
            if (String.IsNullOrEmpty(GpuName.Text) || String.IsNullOrWhiteSpace(GpuName.Text))
            {
                MessageBox.Show("Gpu Name must not be empty!");
                return;
            }
            if (String.IsNullOrEmpty(GpuManufacturer.Text) || String.IsNullOrWhiteSpace(GpuManufacturer.Text))
            {
                MessageBox.Show("Gpu Manufacturer must not be empty!");
                return;
            }
            if (String.IsNullOrEmpty(GpuVersion.Text) || String.IsNullOrWhiteSpace(GpuVersion.Text))
            {
                MessageBox.Show("Gpu Version must not be empty!");
                return;
            }
            if (String.IsNullOrEmpty(GpuVramSize.Text) || String.IsNullOrWhiteSpace(GpuVramSize.Text))
            {
                MessageBox.Show("Gpu Vram Size must not be empty!");
                return;
            }
            if (String.IsNullOrEmpty(AmdOrNvidia.Text) || String.IsNullOrWhiteSpace(AmdOrNvidia.Text))
            {
                MessageBox.Show("Amd / Nvidia must not be empty!");
                return;
            }
            if (String.IsNullOrEmpty(DateReleased.Text) || String.IsNullOrWhiteSpace(DateReleased.Text))
            {
                MessageBox.Show("Date Released must not be empty!");
                return;
            }
            if (String.IsNullOrEmpty(MSRP.Text) || String.IsNullOrWhiteSpace(MSRP.Text))
            {
                MessageBox.Show("MSRP must not be empty!");
                return;
            }
            if (String.IsNullOrEmpty(EbayPrice.Text) || String.IsNullOrWhiteSpace(EbayPrice.Text))
            {
                MessageBox.Show("Ebay Price must not be empty!");
                return;
            }
            if (String.IsNullOrEmpty(PricePaid.Text) || String.IsNullOrWhiteSpace(PricePaid.Text))
            {
                MessageBox.Show("Price Paid must not be empty!");
                return;
            }

            // Prevent incorrect data from being saved
            if (int.TryParse(GpuVramSize.Text, out var vram))
                gpu.VramSize = vram;
            else
            {
                MessageBox.Show("Vram Size must be a whole number!");
                return;
            }

            if (DateTime.TryParse(DateReleased.Text, out var date))
                gpu.DateReleased = date;
            else
            {
                MessageBox.Show("Date must only be - Month Year - with a space between them!");
                return;
            }

            if (Double.TryParse(MSRP.Text, out var msrp))
                gpu.MSRP = msrp;
            else
            {
                MessageBox.Show("MSRP must be a number!");
                return;
            }

            if (Double.TryParse(EbayPrice.Text, out var ebayPrice))
                gpu.EbayPrice = ebayPrice;
            else
            {
                MessageBox.Show("Ebay Price must be a number!");
                return;
            }

            if (Double.TryParse(PricePaid.Text, out var pricePaid))
                gpu.PricePaid = pricePaid;
            else
            {
                MessageBox.Show("Price Paid must be a number!");
                return;
            }

            // Save the data to gpu
            gpu.Name = GpuName.Text;
            gpu.Manufacturer = GpuManufacturer.Text;
            gpu.Version = GpuVersion.Text;
            gpu.AmdOrNvidia = AmdOrNvidia.Text;
            gpu.Id = EditGpuList.Items.Count + 1;

            // Add gpu to the list
            var li = new ListViewItem(gpu.Name);
            li.Tag = gpu;
            li.Name = gpu.Id.ToString();
            li.SubItems.Add(gpu.Manufacturer);
            li.SubItems.Add(gpu.Version);
            li.SubItems.Add(gpu.VramSize.ToString());

            EditGpuList.Items.Add(li);
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
            if (e.KeyData == (Keys.Control | Keys.C))
            {
                CopiedHashrate.Coin = Hashrates.SelectedRows[0].Cells[0].Value.ToString();
                CopiedHashrate.Algorithm = Hashrates.SelectedRows[0].Cells[1].Value.ToString();
                CopiedHashrate.Hashrate = double.Parse(Hashrates.SelectedRows[0].Cells[2].Value.ToString());
                CopiedHashrate.Watts = int.Parse(Hashrates.SelectedRows[0].Cells[3].Value.ToString());
                CopiedHashrate.Lhr = bool.Parse(Hashrates.SelectedRows[0].Cells[4].Value.ToString());
            }
            else if (e.KeyData == (Keys.Control | Keys.V))
            {
                Hashrates.Rows.Add(CopiedHashrate.Coin, CopiedHashrate.Algorithm, CopiedHashrate.Hashrate, CopiedHashrate.Watts, CopiedHashrate.Lhr.ToString());
            }
        }
    }
}
