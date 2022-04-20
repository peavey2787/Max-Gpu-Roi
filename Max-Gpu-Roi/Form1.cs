using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            Hashrates.Columns.Add("Coin", -2, HorizontalAlignment.Left);
            Hashrates.Columns.Add("Algorithm", -2, HorizontalAlignment.Left);
            Hashrates.Columns.Add("Hashrate", -2, HorizontalAlignment.Left);
            Hashrates.Columns.Add("Watts", -2, HorizontalAlignment.Left);
            Hashrates.Columns.Add("Lhr", -2, HorizontalAlignment.Left);
            Hashrates.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

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

            EditGpuList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
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
            if(AllCoinInfos.Count == 0)
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

        private async void EditGpuList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If nothing selected do nothing
            if (EditGpuList.SelectedItems.Count == 0)
                return;

            // Display all info for the selected gpu
            var gpu = EditGpuList.SelectedItems[0].Tag as Gpu;
            AmdOrNvidia.Text = gpu.AmdOrNvidia;
            AmdOrNvidia.ForeColor = gpu.AmdOrNvidia.ToLower() == "amd" ? Color.Red : Color.Green;
            DateReleased.Text = gpu.DateReleased.ToString("Y");
            MSRP.Text = gpu.MSRP.ToString();
            EbayPrice.Text = gpu.EbayPrice.ToString();
            PricePaid.Text = gpu.PricePaid.ToString();
            
            // Read all saved hashrate stats for the selected gpu from the hashrates.json file
            var hashrates = await JsonCrud.GetHashrates(gpu.Id);

            // Display the hashrate stats in the list
            foreach(var hashrate in hashrates)
            {
                var li = new ListViewItem(hashrate.Coin);
                li.ImageIndex = 0;// ToDo: Get the right image for the given coin
                li.SubItems.Add(hashrate.Algorithm);
                li.SubItems.Add(hashrate.Hashrate.ToString());
                li.SubItems.Add(hashrate.Watts.ToString());
                li.SubItems.Add(hashrate.Lhr.ToString());
                Hashrates.Items.Add(li);
            }
            Hashrates.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void AddCoin_Click(object sender, EventArgs e)
        {
            if(ListOfAllCoins.SelectedItems.Count == 0)
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
    }
}
