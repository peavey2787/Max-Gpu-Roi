namespace Max_Gpu_Roi
{
    partial class MaxGpuRoi
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MaxGpuRoi));
            this.lblGpuLists = new System.Windows.Forms.Label();
            this.lblCoinLists = new System.Windows.Forms.Label();
            this.lblCosts = new System.Windows.Forms.Label();
            this.lblFilters = new System.Windows.Forms.Label();
            this.pnlGpuLists = new System.Windows.Forms.Panel();
            this.GpuLists = new System.Windows.Forms.ListView();
            this.ExportGpuLists = new System.Windows.Forms.Button();
            this.ImportGpuLists = new System.Windows.Forms.Button();
            this.EditGpuLists = new System.Windows.Forms.Button();
            this.pnlCoinLists = new System.Windows.Forms.Panel();
            this.CoinLists = new System.Windows.Forms.ListView();
            this.ExportCoinLists = new System.Windows.Forms.Button();
            this.ImportCoinLists = new System.Windows.Forms.Button();
            this.EditCoinLists = new System.Windows.Forms.Button();
            this.pnlCosts = new System.Windows.Forms.Panel();
            this.lblPoolMinerFeePercentSign = new System.Windows.Forms.Label();
            this.PoolMinerFee = new System.Windows.Forms.TextBox();
            this.ElectricityRate = new System.Windows.Forms.TextBox();
            this.lblPoolMinerFee = new System.Windows.Forms.Label();
            this.lblElectricityRate = new System.Windows.Forms.Label();
            this.pnlFilters = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.SortBy = new System.Windows.Forms.ComboBox();
            this.ShowNvidia = new System.Windows.Forms.CheckBox();
            this.ShowAmd = new System.Windows.Forms.CheckBox();
            this.pnlResultsList = new System.Windows.Forms.Panel();
            this.ResultsList = new System.Windows.Forms.ListView();
            this.coinImageList = new System.Windows.Forms.ImageList(this.components);
            this.lblResults = new System.Windows.Forms.Label();
            this.Ascending = new System.Windows.Forms.RadioButton();
            this.Descending = new System.Windows.Forms.RadioButton();
            this.Exit = new System.Windows.Forms.Button();
            this.Budget = new System.Windows.Forms.TextBox();
            this.lblBudget = new System.Windows.Forms.Label();
            this.MaxMyROI = new System.Windows.Forms.Button();
            this.lblBudgetMoneySign = new System.Windows.Forms.Label();
            this.EditGpuPanel = new System.Windows.Forms.Panel();
            this.CancelGpuHashrates = new System.Windows.Forms.Button();
            this.SaveGpuHashrates = new System.Windows.Forms.Button();
            this.Hashrates = new System.Windows.Forms.ListView();
            this.lblHashrates = new System.Windows.Forms.Label();
            this.PricePaid = new System.Windows.Forms.TextBox();
            this.EbayPrice = new System.Windows.Forms.TextBox();
            this.MSRP = new System.Windows.Forms.TextBox();
            this.lblPricePaid = new System.Windows.Forms.Label();
            this.lblEbayPrice = new System.Windows.Forms.Label();
            this.lblMSRP = new System.Windows.Forms.Label();
            this.lblDateReleased = new System.Windows.Forms.Label();
            this.DateReleased = new System.Windows.Forms.Label();
            this.AmdOrNvidia = new System.Windows.Forms.Label();
            this.EditGpuList = new System.Windows.Forms.ListView();
            this.GpuListName = new System.Windows.Forms.Label();
            this.EditCoinPanel = new System.Windows.Forms.Panel();
            this.lblAllCoinsList = new System.Windows.Forms.Label();
            this.DeleteCoin = new System.Windows.Forms.Button();
            this.AddCoin = new System.Windows.Forms.Button();
            this.ListOfAllCoins = new System.Windows.Forms.ListView();
            this.CancelEditCoinList = new System.Windows.Forms.Button();
            this.SaveCoinList = new System.Windows.Forms.Button();
            this.EditCoinList = new System.Windows.Forms.ListView();
            this.CoinListName = new System.Windows.Forms.Label();
            this.pnlGpuLists.SuspendLayout();
            this.pnlCoinLists.SuspendLayout();
            this.pnlCosts.SuspendLayout();
            this.pnlFilters.SuspendLayout();
            this.pnlResultsList.SuspendLayout();
            this.EditGpuPanel.SuspendLayout();
            this.EditCoinPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblGpuLists
            // 
            this.lblGpuLists.AutoSize = true;
            this.lblGpuLists.BackColor = System.Drawing.Color.Transparent;
            this.lblGpuLists.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblGpuLists.Font = new System.Drawing.Font("Ink Free", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblGpuLists.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblGpuLists.Location = new System.Drawing.Point(-1, 6);
            this.lblGpuLists.Name = "lblGpuLists";
            this.lblGpuLists.Size = new System.Drawing.Size(183, 46);
            this.lblGpuLists.TabIndex = 0;
            this.lblGpuLists.Text = "Gpu Lists";
            // 
            // lblCoinLists
            // 
            this.lblCoinLists.AutoSize = true;
            this.lblCoinLists.BackColor = System.Drawing.Color.Transparent;
            this.lblCoinLists.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblCoinLists.Font = new System.Drawing.Font("Ink Free", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCoinLists.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblCoinLists.Location = new System.Drawing.Point(-3, 8);
            this.lblCoinLists.Name = "lblCoinLists";
            this.lblCoinLists.Size = new System.Drawing.Size(181, 46);
            this.lblCoinLists.TabIndex = 1;
            this.lblCoinLists.Text = "Coin Lists";
            // 
            // lblCosts
            // 
            this.lblCosts.AutoSize = true;
            this.lblCosts.BackColor = System.Drawing.Color.Transparent;
            this.lblCosts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblCosts.Font = new System.Drawing.Font("Ink Free", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCosts.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblCosts.Location = new System.Drawing.Point(-3, -1);
            this.lblCosts.Name = "lblCosts";
            this.lblCosts.Size = new System.Drawing.Size(111, 46);
            this.lblCosts.TabIndex = 2;
            this.lblCosts.Text = "Costs";
            // 
            // lblFilters
            // 
            this.lblFilters.AutoSize = true;
            this.lblFilters.BackColor = System.Drawing.Color.Transparent;
            this.lblFilters.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblFilters.Font = new System.Drawing.Font("Ink Free", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblFilters.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblFilters.Location = new System.Drawing.Point(-3, 0);
            this.lblFilters.Name = "lblFilters";
            this.lblFilters.Size = new System.Drawing.Size(130, 46);
            this.lblFilters.TabIndex = 3;
            this.lblFilters.Text = "Filters";
            // 
            // pnlGpuLists
            // 
            this.pnlGpuLists.BackColor = System.Drawing.Color.Transparent;
            this.pnlGpuLists.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlGpuLists.Controls.Add(this.GpuLists);
            this.pnlGpuLists.Controls.Add(this.ExportGpuLists);
            this.pnlGpuLists.Controls.Add(this.ImportGpuLists);
            this.pnlGpuLists.Controls.Add(this.EditGpuLists);
            this.pnlGpuLists.Controls.Add(this.lblGpuLists);
            this.pnlGpuLists.Location = new System.Drawing.Point(12, 12);
            this.pnlGpuLists.Name = "pnlGpuLists";
            this.pnlGpuLists.Size = new System.Drawing.Size(333, 202);
            this.pnlGpuLists.TabIndex = 5;
            // 
            // GpuLists
            // 
            this.GpuLists.BackColor = System.Drawing.SystemColors.Window;
            this.GpuLists.FullRowSelect = true;
            this.GpuLists.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.GpuLists.HideSelection = false;
            this.GpuLists.Location = new System.Drawing.Point(3, 62);
            this.GpuLists.MultiSelect = false;
            this.GpuLists.Name = "GpuLists";
            this.GpuLists.Size = new System.Drawing.Size(188, 121);
            this.GpuLists.TabIndex = 5;
            this.GpuLists.UseCompatibleStateImageBehavior = false;
            this.GpuLists.View = System.Windows.Forms.View.Details;
            this.GpuLists.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GpuLists_KeyDown);
            // 
            // ExportGpuLists
            // 
            this.ExportGpuLists.BackColor = System.Drawing.Color.RoyalBlue;
            this.ExportGpuLists.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExportGpuLists.Font = new System.Drawing.Font("Ink Free", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ExportGpuLists.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.ExportGpuLists.Location = new System.Drawing.Point(245, 32);
            this.ExportGpuLists.Name = "ExportGpuLists";
            this.ExportGpuLists.Size = new System.Drawing.Size(83, 23);
            this.ExportGpuLists.TabIndex = 4;
            this.ExportGpuLists.Text = "Export";
            this.ExportGpuLists.UseVisualStyleBackColor = false;
            // 
            // ImportGpuLists
            // 
            this.ImportGpuLists.BackColor = System.Drawing.Color.RoyalBlue;
            this.ImportGpuLists.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ImportGpuLists.Font = new System.Drawing.Font("Ink Free", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ImportGpuLists.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.ImportGpuLists.Location = new System.Drawing.Point(245, 3);
            this.ImportGpuLists.Name = "ImportGpuLists";
            this.ImportGpuLists.Size = new System.Drawing.Size(83, 23);
            this.ImportGpuLists.TabIndex = 3;
            this.ImportGpuLists.Text = "Import";
            this.ImportGpuLists.UseVisualStyleBackColor = false;
            this.ImportGpuLists.Click += new System.EventHandler(this.ImportGpuLists_Click);
            // 
            // EditGpuLists
            // 
            this.EditGpuLists.BackColor = System.Drawing.Color.RoyalBlue;
            this.EditGpuLists.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EditGpuLists.Font = new System.Drawing.Font("Ink Free", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.EditGpuLists.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.EditGpuLists.Location = new System.Drawing.Point(245, 103);
            this.EditGpuLists.Name = "EditGpuLists";
            this.EditGpuLists.Size = new System.Drawing.Size(83, 30);
            this.EditGpuLists.TabIndex = 1;
            this.EditGpuLists.Text = "Edit";
            this.EditGpuLists.UseVisualStyleBackColor = false;
            this.EditGpuLists.Click += new System.EventHandler(this.EditGpuLists_Click);
            // 
            // pnlCoinLists
            // 
            this.pnlCoinLists.BackColor = System.Drawing.Color.Transparent;
            this.pnlCoinLists.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCoinLists.Controls.Add(this.CoinLists);
            this.pnlCoinLists.Controls.Add(this.ExportCoinLists);
            this.pnlCoinLists.Controls.Add(this.ImportCoinLists);
            this.pnlCoinLists.Controls.Add(this.EditCoinLists);
            this.pnlCoinLists.Controls.Add(this.lblCoinLists);
            this.pnlCoinLists.Location = new System.Drawing.Point(351, 13);
            this.pnlCoinLists.Name = "pnlCoinLists";
            this.pnlCoinLists.Size = new System.Drawing.Size(333, 201);
            this.pnlCoinLists.TabIndex = 6;
            // 
            // CoinLists
            // 
            this.CoinLists.BackColor = System.Drawing.SystemColors.Window;
            this.CoinLists.FullRowSelect = true;
            this.CoinLists.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.CoinLists.HideSelection = false;
            this.CoinLists.Location = new System.Drawing.Point(3, 61);
            this.CoinLists.MultiSelect = false;
            this.CoinLists.Name = "CoinLists";
            this.CoinLists.Size = new System.Drawing.Size(186, 121);
            this.CoinLists.TabIndex = 6;
            this.CoinLists.UseCompatibleStateImageBehavior = false;
            this.CoinLists.View = System.Windows.Forms.View.Details;
            this.CoinLists.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CoinLists_KeyDown);
            // 
            // ExportCoinLists
            // 
            this.ExportCoinLists.BackColor = System.Drawing.Color.RoyalBlue;
            this.ExportCoinLists.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExportCoinLists.Font = new System.Drawing.Font("Ink Free", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ExportCoinLists.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.ExportCoinLists.Location = new System.Drawing.Point(245, 32);
            this.ExportCoinLists.Name = "ExportCoinLists";
            this.ExportCoinLists.Size = new System.Drawing.Size(83, 23);
            this.ExportCoinLists.TabIndex = 5;
            this.ExportCoinLists.Text = "Export";
            this.ExportCoinLists.UseVisualStyleBackColor = false;
            // 
            // ImportCoinLists
            // 
            this.ImportCoinLists.BackColor = System.Drawing.Color.RoyalBlue;
            this.ImportCoinLists.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ImportCoinLists.Font = new System.Drawing.Font("Ink Free", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ImportCoinLists.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.ImportCoinLists.Location = new System.Drawing.Point(245, 3);
            this.ImportCoinLists.Name = "ImportCoinLists";
            this.ImportCoinLists.Size = new System.Drawing.Size(83, 23);
            this.ImportCoinLists.TabIndex = 4;
            this.ImportCoinLists.Text = "Import";
            this.ImportCoinLists.UseVisualStyleBackColor = false;
            this.ImportCoinLists.Click += new System.EventHandler(this.ImportCoinLists_Click);
            // 
            // EditCoinLists
            // 
            this.EditCoinLists.BackColor = System.Drawing.Color.RoyalBlue;
            this.EditCoinLists.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EditCoinLists.Font = new System.Drawing.Font("Ink Free", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.EditCoinLists.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.EditCoinLists.Location = new System.Drawing.Point(245, 102);
            this.EditCoinLists.Name = "EditCoinLists";
            this.EditCoinLists.Size = new System.Drawing.Size(83, 30);
            this.EditCoinLists.TabIndex = 2;
            this.EditCoinLists.Text = "Edit";
            this.EditCoinLists.UseVisualStyleBackColor = false;
            this.EditCoinLists.Click += new System.EventHandler(this.EditCoinLists_Click);
            // 
            // pnlCosts
            // 
            this.pnlCosts.BackColor = System.Drawing.Color.Transparent;
            this.pnlCosts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCosts.Controls.Add(this.lblPoolMinerFeePercentSign);
            this.pnlCosts.Controls.Add(this.PoolMinerFee);
            this.pnlCosts.Controls.Add(this.ElectricityRate);
            this.pnlCosts.Controls.Add(this.lblPoolMinerFee);
            this.pnlCosts.Controls.Add(this.lblElectricityRate);
            this.pnlCosts.Controls.Add(this.lblCosts);
            this.pnlCosts.Location = new System.Drawing.Point(690, 14);
            this.pnlCosts.Name = "pnlCosts";
            this.pnlCosts.Size = new System.Drawing.Size(340, 147);
            this.pnlCosts.TabIndex = 7;
            // 
            // lblPoolMinerFeePercentSign
            // 
            this.lblPoolMinerFeePercentSign.AutoSize = true;
            this.lblPoolMinerFeePercentSign.BackColor = System.Drawing.Color.Transparent;
            this.lblPoolMinerFeePercentSign.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblPoolMinerFeePercentSign.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblPoolMinerFeePercentSign.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblPoolMinerFeePercentSign.Location = new System.Drawing.Point(278, 104);
            this.lblPoolMinerFeePercentSign.Name = "lblPoolMinerFeePercentSign";
            this.lblPoolMinerFeePercentSign.Size = new System.Drawing.Size(35, 32);
            this.lblPoolMinerFeePercentSign.TabIndex = 7;
            this.lblPoolMinerFeePercentSign.Text = "%";
            // 
            // PoolMinerFee
            // 
            this.PoolMinerFee.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.PoolMinerFee.Location = new System.Drawing.Point(230, 101);
            this.PoolMinerFee.MaxLength = 2;
            this.PoolMinerFee.Name = "PoolMinerFee";
            this.PoolMinerFee.PlaceholderText = "02";
            this.PoolMinerFee.Size = new System.Drawing.Size(45, 37);
            this.PoolMinerFee.TabIndex = 6;
            // 
            // ElectricityRate
            // 
            this.ElectricityRate.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ElectricityRate.Location = new System.Drawing.Point(230, 53);
            this.ElectricityRate.MaxLength = 5;
            this.ElectricityRate.Name = "ElectricityRate";
            this.ElectricityRate.PlaceholderText = "0.10";
            this.ElectricityRate.Size = new System.Drawing.Size(88, 37);
            this.ElectricityRate.TabIndex = 5;
            // 
            // lblPoolMinerFee
            // 
            this.lblPoolMinerFee.AutoSize = true;
            this.lblPoolMinerFee.BackColor = System.Drawing.Color.Transparent;
            this.lblPoolMinerFee.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblPoolMinerFee.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblPoolMinerFee.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblPoolMinerFee.Location = new System.Drawing.Point(35, 104);
            this.lblPoolMinerFee.Name = "lblPoolMinerFee";
            this.lblPoolMinerFee.Size = new System.Drawing.Size(173, 30);
            this.lblPoolMinerFee.TabIndex = 4;
            this.lblPoolMinerFee.Text = "Pool/Miner Fee";
            // 
            // lblElectricityRate
            // 
            this.lblElectricityRate.AutoSize = true;
            this.lblElectricityRate.BackColor = System.Drawing.Color.Transparent;
            this.lblElectricityRate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblElectricityRate.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblElectricityRate.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblElectricityRate.Location = new System.Drawing.Point(35, 60);
            this.lblElectricityRate.Name = "lblElectricityRate";
            this.lblElectricityRate.Size = new System.Drawing.Size(189, 30);
            this.lblElectricityRate.TabIndex = 3;
            this.lblElectricityRate.Text = "Electricity Rate";
            // 
            // pnlFilters
            // 
            this.pnlFilters.BackColor = System.Drawing.Color.Transparent;
            this.pnlFilters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFilters.Controls.Add(this.label1);
            this.pnlFilters.Controls.Add(this.SortBy);
            this.pnlFilters.Controls.Add(this.ShowNvidia);
            this.pnlFilters.Controls.Add(this.ShowAmd);
            this.pnlFilters.Controls.Add(this.lblFilters);
            this.pnlFilters.Location = new System.Drawing.Point(1036, 14);
            this.pnlFilters.Name = "pnlFilters";
            this.pnlFilters.Size = new System.Drawing.Size(500, 147);
            this.pnlFilters.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(207, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 30);
            this.label1.TabIndex = 7;
            this.label1.Text = "Sort By";
            // 
            // SortBy
            // 
            this.SortBy.BackColor = System.Drawing.Color.RoyalBlue;
            this.SortBy.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SortBy.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.SortBy.FormattingEnabled = true;
            this.SortBy.Items.AddRange(new object[] {
            "Price per Mhs",
            "Efficiency",
            "Ebay Price",
            "MSRP",
            "Price Paid",
            "Mhs",
            "Watts",
            "Vram Size",
            "Date Released"});
            this.SortBy.Location = new System.Drawing.Point(307, 99);
            this.SortBy.Name = "SortBy";
            this.SortBy.Size = new System.Drawing.Size(182, 38);
            this.SortBy.TabIndex = 6;
            // 
            // ShowNvidia
            // 
            this.ShowNvidia.AutoSize = true;
            this.ShowNvidia.Checked = true;
            this.ShowNvidia.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowNvidia.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ShowNvidia.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.ShowNvidia.Location = new System.Drawing.Point(12, 99);
            this.ShowNvidia.Name = "ShowNvidia";
            this.ShowNvidia.Size = new System.Drawing.Size(167, 34);
            this.ShowNvidia.TabIndex = 5;
            this.ShowNvidia.Text = "Show Nvidia";
            this.ShowNvidia.UseVisualStyleBackColor = true;
            // 
            // ShowAmd
            // 
            this.ShowAmd.AutoSize = true;
            this.ShowAmd.Checked = true;
            this.ShowAmd.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowAmd.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ShowAmd.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.ShowAmd.Location = new System.Drawing.Point(12, 53);
            this.ShowAmd.Name = "ShowAmd";
            this.ShowAmd.Size = new System.Drawing.Size(144, 34);
            this.ShowAmd.TabIndex = 4;
            this.ShowAmd.Text = "Show Amd";
            this.ShowAmd.UseVisualStyleBackColor = true;
            // 
            // pnlResultsList
            // 
            this.pnlResultsList.BackColor = System.Drawing.Color.Transparent;
            this.pnlResultsList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlResultsList.Controls.Add(this.ResultsList);
            this.pnlResultsList.Controls.Add(this.lblResults);
            this.pnlResultsList.Location = new System.Drawing.Point(14, 255);
            this.pnlResultsList.Name = "pnlResultsList";
            this.pnlResultsList.Size = new System.Drawing.Size(1848, 674);
            this.pnlResultsList.TabIndex = 9;
            // 
            // ResultsList
            // 
            this.ResultsList.HideSelection = false;
            this.ResultsList.Location = new System.Drawing.Point(-1, 58);
            this.ResultsList.MultiSelect = false;
            this.ResultsList.Name = "ResultsList";
            this.ResultsList.Size = new System.Drawing.Size(1849, 616);
            this.ResultsList.SmallImageList = this.coinImageList;
            this.ResultsList.TabIndex = 2;
            this.ResultsList.UseCompatibleStateImageBehavior = false;
            this.ResultsList.View = System.Windows.Forms.View.Details;
            // 
            // coinImageList
            // 
            this.coinImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.coinImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("coinImageList.ImageStream")));
            this.coinImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.coinImageList.Images.SetKeyName(0, "looking-glass.jpg");
            // 
            // lblResults
            // 
            this.lblResults.AutoSize = true;
            this.lblResults.BackColor = System.Drawing.Color.Transparent;
            this.lblResults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblResults.Font = new System.Drawing.Font("Ink Free", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblResults.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblResults.Location = new System.Drawing.Point(-3, 0);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(517, 46);
            this.lblResults.TabIndex = 1;
            this.lblResults.Text = "Most Profitable Configuration";
            // 
            // Ascending
            // 
            this.Ascending.AutoSize = true;
            this.Ascending.BackColor = System.Drawing.Color.Transparent;
            this.Ascending.Checked = true;
            this.Ascending.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Ascending.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.Ascending.Location = new System.Drawing.Point(1467, 215);
            this.Ascending.Name = "Ascending";
            this.Ascending.Size = new System.Drawing.Size(137, 34);
            this.Ascending.TabIndex = 4;
            this.Ascending.TabStop = true;
            this.Ascending.Text = "Ascending";
            this.Ascending.UseVisualStyleBackColor = false;
            // 
            // Descending
            // 
            this.Descending.AutoSize = true;
            this.Descending.BackColor = System.Drawing.Color.Transparent;
            this.Descending.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Descending.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.Descending.Location = new System.Drawing.Point(1659, 215);
            this.Descending.Name = "Descending";
            this.Descending.Size = new System.Drawing.Size(150, 34);
            this.Descending.TabIndex = 10;
            this.Descending.Text = "Descending";
            this.Descending.UseVisualStyleBackColor = false;
            // 
            // Exit
            // 
            this.Exit.BackColor = System.Drawing.Color.DarkRed;
            this.Exit.FlatAppearance.BorderColor = System.Drawing.Color.DarkRed;
            this.Exit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkRed;
            this.Exit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.Exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Exit.Font = new System.Drawing.Font("Ink Free", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Exit.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.Exit.Location = new System.Drawing.Point(1800, 0);
            this.Exit.Margin = new System.Windows.Forms.Padding(0);
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(75, 39);
            this.Exit.TabIndex = 11;
            this.Exit.Text = "Exit";
            this.Exit.UseVisualStyleBackColor = false;
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // Budget
            // 
            this.Budget.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Budget.Location = new System.Drawing.Point(969, 212);
            this.Budget.MaxLength = 250;
            this.Budget.Name = "Budget";
            this.Budget.PlaceholderText = "0";
            this.Budget.Size = new System.Drawing.Size(108, 37);
            this.Budget.TabIndex = 8;
            // 
            // lblBudget
            // 
            this.lblBudget.AutoSize = true;
            this.lblBudget.BackColor = System.Drawing.Color.Transparent;
            this.lblBudget.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblBudget.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblBudget.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblBudget.Location = new System.Drawing.Point(841, 217);
            this.lblBudget.Name = "lblBudget";
            this.lblBudget.Size = new System.Drawing.Size(109, 30);
            this.lblBudget.TabIndex = 12;
            this.lblBudget.Text = "Budget: ";
            this.lblBudget.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // MaxMyROI
            // 
            this.MaxMyROI.BackColor = System.Drawing.Color.RoyalBlue;
            this.MaxMyROI.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MaxMyROI.Font = new System.Drawing.Font("Ink Free", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.MaxMyROI.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.MaxMyROI.Location = new System.Drawing.Point(1106, 212);
            this.MaxMyROI.Name = "MaxMyROI";
            this.MaxMyROI.Size = new System.Drawing.Size(142, 37);
            this.MaxMyROI.TabIndex = 13;
            this.MaxMyROI.Text = "Max My ROI";
            this.MaxMyROI.UseVisualStyleBackColor = false;
            // 
            // lblBudgetMoneySign
            // 
            this.lblBudgetMoneySign.AutoSize = true;
            this.lblBudgetMoneySign.BackColor = System.Drawing.Color.Transparent;
            this.lblBudgetMoneySign.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblBudgetMoneySign.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblBudgetMoneySign.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblBudgetMoneySign.Location = new System.Drawing.Point(947, 214);
            this.lblBudgetMoneySign.Name = "lblBudgetMoneySign";
            this.lblBudgetMoneySign.Size = new System.Drawing.Size(28, 32);
            this.lblBudgetMoneySign.TabIndex = 14;
            this.lblBudgetMoneySign.Text = "$";
            // 
            // EditGpuPanel
            // 
            this.EditGpuPanel.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.EditGpuPanel.Controls.Add(this.CancelGpuHashrates);
            this.EditGpuPanel.Controls.Add(this.SaveGpuHashrates);
            this.EditGpuPanel.Controls.Add(this.Hashrates);
            this.EditGpuPanel.Controls.Add(this.lblHashrates);
            this.EditGpuPanel.Controls.Add(this.PricePaid);
            this.EditGpuPanel.Controls.Add(this.EbayPrice);
            this.EditGpuPanel.Controls.Add(this.MSRP);
            this.EditGpuPanel.Controls.Add(this.lblPricePaid);
            this.EditGpuPanel.Controls.Add(this.lblEbayPrice);
            this.EditGpuPanel.Controls.Add(this.lblMSRP);
            this.EditGpuPanel.Controls.Add(this.lblDateReleased);
            this.EditGpuPanel.Controls.Add(this.DateReleased);
            this.EditGpuPanel.Controls.Add(this.AmdOrNvidia);
            this.EditGpuPanel.Controls.Add(this.EditGpuList);
            this.EditGpuPanel.Controls.Add(this.GpuListName);
            this.EditGpuPanel.Location = new System.Drawing.Point(19, 941);
            this.EditGpuPanel.Name = "EditGpuPanel";
            this.EditGpuPanel.Size = new System.Drawing.Size(827, 502);
            this.EditGpuPanel.TabIndex = 15;
            this.EditGpuPanel.Visible = false;
            // 
            // CancelGpuHashrates
            // 
            this.CancelGpuHashrates.BackColor = System.Drawing.Color.RoyalBlue;
            this.CancelGpuHashrates.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelGpuHashrates.Font = new System.Drawing.Font("Ink Free", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.CancelGpuHashrates.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.CancelGpuHashrates.Location = new System.Drawing.Point(729, 466);
            this.CancelGpuHashrates.Name = "CancelGpuHashrates";
            this.CancelGpuHashrates.Size = new System.Drawing.Size(83, 30);
            this.CancelGpuHashrates.TabIndex = 16;
            this.CancelGpuHashrates.Text = "Cancel";
            this.CancelGpuHashrates.UseVisualStyleBackColor = false;
            this.CancelGpuHashrates.Click += new System.EventHandler(this.CancelGpuHashrates_Click);
            // 
            // SaveGpuHashrates
            // 
            this.SaveGpuHashrates.BackColor = System.Drawing.Color.RoyalBlue;
            this.SaveGpuHashrates.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveGpuHashrates.Font = new System.Drawing.Font("Ink Free", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SaveGpuHashrates.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.SaveGpuHashrates.Location = new System.Drawing.Point(607, 466);
            this.SaveGpuHashrates.Name = "SaveGpuHashrates";
            this.SaveGpuHashrates.Size = new System.Drawing.Size(83, 30);
            this.SaveGpuHashrates.TabIndex = 15;
            this.SaveGpuHashrates.Text = "Save";
            this.SaveGpuHashrates.UseVisualStyleBackColor = false;
            this.SaveGpuHashrates.Click += new System.EventHandler(this.SaveGpuHashrates_Click);
            // 
            // Hashrates
            // 
            this.Hashrates.FullRowSelect = true;
            this.Hashrates.HideSelection = false;
            this.Hashrates.Location = new System.Drawing.Point(336, 212);
            this.Hashrates.MultiSelect = false;
            this.Hashrates.Name = "Hashrates";
            this.Hashrates.Size = new System.Drawing.Size(476, 236);
            this.Hashrates.SmallImageList = this.coinImageList;
            this.Hashrates.TabIndex = 14;
            this.Hashrates.UseCompatibleStateImageBehavior = false;
            this.Hashrates.View = System.Windows.Forms.View.Details;
            // 
            // lblHashrates
            // 
            this.lblHashrates.AutoSize = true;
            this.lblHashrates.BackColor = System.Drawing.Color.Transparent;
            this.lblHashrates.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblHashrates.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblHashrates.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblHashrates.Location = new System.Drawing.Point(336, 179);
            this.lblHashrates.Name = "lblHashrates";
            this.lblHashrates.Size = new System.Drawing.Size(132, 30);
            this.lblHashrates.TabIndex = 13;
            this.lblHashrates.Text = "Hashrates";
            // 
            // PricePaid
            // 
            this.PricePaid.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.PricePaid.Location = new System.Drawing.Point(680, 106);
            this.PricePaid.MaxLength = 9;
            this.PricePaid.Name = "PricePaid";
            this.PricePaid.PlaceholderText = "900.00";
            this.PricePaid.Size = new System.Drawing.Size(136, 37);
            this.PricePaid.TabIndex = 12;
            this.PricePaid.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // EbayPrice
            // 
            this.EbayPrice.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.EbayPrice.Location = new System.Drawing.Point(510, 106);
            this.EbayPrice.MaxLength = 9;
            this.EbayPrice.Name = "EbayPrice";
            this.EbayPrice.PlaceholderText = "950.00";
            this.EbayPrice.Size = new System.Drawing.Size(136, 37);
            this.EbayPrice.TabIndex = 11;
            this.EbayPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // MSRP
            // 
            this.MSRP.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.MSRP.Location = new System.Drawing.Point(336, 106);
            this.MSRP.MaxLength = 9;
            this.MSRP.Name = "MSRP";
            this.MSRP.PlaceholderText = "750.00";
            this.MSRP.Size = new System.Drawing.Size(136, 37);
            this.MSRP.TabIndex = 10;
            this.MSRP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblPricePaid
            // 
            this.lblPricePaid.AutoSize = true;
            this.lblPricePaid.BackColor = System.Drawing.Color.Transparent;
            this.lblPricePaid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblPricePaid.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblPricePaid.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblPricePaid.Location = new System.Drawing.Point(682, 73);
            this.lblPricePaid.Name = "lblPricePaid";
            this.lblPricePaid.Size = new System.Drawing.Size(120, 30);
            this.lblPricePaid.TabIndex = 9;
            this.lblPricePaid.Text = "Price Paid";
            // 
            // lblEbayPrice
            // 
            this.lblEbayPrice.AutoSize = true;
            this.lblEbayPrice.BackColor = System.Drawing.Color.Transparent;
            this.lblEbayPrice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblEbayPrice.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblEbayPrice.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblEbayPrice.Location = new System.Drawing.Point(512, 73);
            this.lblEbayPrice.Name = "lblEbayPrice";
            this.lblEbayPrice.Size = new System.Drawing.Size(127, 30);
            this.lblEbayPrice.TabIndex = 8;
            this.lblEbayPrice.Text = "Ebay Price";
            // 
            // lblMSRP
            // 
            this.lblMSRP.AutoSize = true;
            this.lblMSRP.BackColor = System.Drawing.Color.Transparent;
            this.lblMSRP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblMSRP.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblMSRP.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblMSRP.Location = new System.Drawing.Point(363, 73);
            this.lblMSRP.Name = "lblMSRP";
            this.lblMSRP.Size = new System.Drawing.Size(77, 30);
            this.lblMSRP.TabIndex = 7;
            this.lblMSRP.Text = "MSRP";
            // 
            // lblDateReleased
            // 
            this.lblDateReleased.AutoSize = true;
            this.lblDateReleased.BackColor = System.Drawing.Color.Transparent;
            this.lblDateReleased.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblDateReleased.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDateReleased.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblDateReleased.Location = new System.Drawing.Point(578, 3);
            this.lblDateReleased.Name = "lblDateReleased";
            this.lblDateReleased.Size = new System.Drawing.Size(161, 30);
            this.lblDateReleased.TabIndex = 6;
            this.lblDateReleased.Text = "Release Date";
            // 
            // DateReleased
            // 
            this.DateReleased.AutoSize = true;
            this.DateReleased.BackColor = System.Drawing.Color.Transparent;
            this.DateReleased.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DateReleased.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.DateReleased.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.DateReleased.Location = new System.Drawing.Point(578, 33);
            this.DateReleased.Name = "DateReleased";
            this.DateReleased.Size = new System.Drawing.Size(127, 30);
            this.DateReleased.TabIndex = 5;
            this.DateReleased.Text = "July 2021";
            // 
            // AmdOrNvidia
            // 
            this.AmdOrNvidia.AutoSize = true;
            this.AmdOrNvidia.BackColor = System.Drawing.Color.Transparent;
            this.AmdOrNvidia.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AmdOrNvidia.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.AmdOrNvidia.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.AmdOrNvidia.Location = new System.Drawing.Point(336, 15);
            this.AmdOrNvidia.Name = "AmdOrNvidia";
            this.AmdOrNvidia.Size = new System.Drawing.Size(140, 30);
            this.AmdOrNvidia.TabIndex = 4;
            this.AmdOrNvidia.Text = "Amd/Nvidia";
            // 
            // EditGpuList
            // 
            this.EditGpuList.FullRowSelect = true;
            this.EditGpuList.HideSelection = false;
            this.EditGpuList.Location = new System.Drawing.Point(6, 50);
            this.EditGpuList.MultiSelect = false;
            this.EditGpuList.Name = "EditGpuList";
            this.EditGpuList.Size = new System.Drawing.Size(320, 446);
            this.EditGpuList.TabIndex = 3;
            this.EditGpuList.UseCompatibleStateImageBehavior = false;
            this.EditGpuList.View = System.Windows.Forms.View.Details;
            this.EditGpuList.SelectedIndexChanged += new System.EventHandler(this.EditGpuList_SelectedIndexChanged);
            // 
            // GpuListName
            // 
            this.GpuListName.AutoSize = true;
            this.GpuListName.BackColor = System.Drawing.Color.Transparent;
            this.GpuListName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GpuListName.Font = new System.Drawing.Font("Ink Free", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.GpuListName.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.GpuListName.Location = new System.Drawing.Point(1, 1);
            this.GpuListName.Name = "GpuListName";
            this.GpuListName.Size = new System.Drawing.Size(273, 46);
            this.GpuListName.TabIndex = 1;
            this.GpuListName.Text = "Gpu List Name";
            // 
            // EditCoinPanel
            // 
            this.EditCoinPanel.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.EditCoinPanel.Controls.Add(this.lblAllCoinsList);
            this.EditCoinPanel.Controls.Add(this.DeleteCoin);
            this.EditCoinPanel.Controls.Add(this.AddCoin);
            this.EditCoinPanel.Controls.Add(this.ListOfAllCoins);
            this.EditCoinPanel.Controls.Add(this.CancelEditCoinList);
            this.EditCoinPanel.Controls.Add(this.SaveCoinList);
            this.EditCoinPanel.Controls.Add(this.EditCoinList);
            this.EditCoinPanel.Controls.Add(this.CoinListName);
            this.EditCoinPanel.Location = new System.Drawing.Point(904, 942);
            this.EditCoinPanel.Name = "EditCoinPanel";
            this.EditCoinPanel.Size = new System.Drawing.Size(957, 501);
            this.EditCoinPanel.TabIndex = 16;
            this.EditCoinPanel.Visible = false;
            // 
            // lblAllCoinsList
            // 
            this.lblAllCoinsList.AutoSize = true;
            this.lblAllCoinsList.BackColor = System.Drawing.Color.Transparent;
            this.lblAllCoinsList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblAllCoinsList.Font = new System.Drawing.Font("Ink Free", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblAllCoinsList.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblAllCoinsList.Location = new System.Drawing.Point(584, 12);
            this.lblAllCoinsList.Name = "lblAllCoinsList";
            this.lblAllCoinsList.Size = new System.Drawing.Size(213, 30);
            this.lblAllCoinsList.TabIndex = 21;
            this.lblAllCoinsList.Text = "All Coins Available";
            // 
            // DeleteCoin
            // 
            this.DeleteCoin.BackColor = System.Drawing.Color.RoyalBlue;
            this.DeleteCoin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DeleteCoin.Font = new System.Drawing.Font("Ink Free", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.DeleteCoin.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.DeleteCoin.Location = new System.Drawing.Point(332, 211);
            this.DeleteCoin.Name = "DeleteCoin";
            this.DeleteCoin.Size = new System.Drawing.Size(80, 30);
            this.DeleteCoin.TabIndex = 20;
            this.DeleteCoin.Text = "Delete";
            this.DeleteCoin.UseVisualStyleBackColor = false;
            this.DeleteCoin.Click += new System.EventHandler(this.DeleteCoin_Click);
            // 
            // AddCoin
            // 
            this.AddCoin.BackColor = System.Drawing.Color.RoyalBlue;
            this.AddCoin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddCoin.Font = new System.Drawing.Font("Ink Free", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.AddCoin.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.AddCoin.Location = new System.Drawing.Point(332, 132);
            this.AddCoin.Name = "AddCoin";
            this.AddCoin.Size = new System.Drawing.Size(80, 30);
            this.AddCoin.TabIndex = 19;
            this.AddCoin.Text = "Add";
            this.AddCoin.UseVisualStyleBackColor = false;
            this.AddCoin.Click += new System.EventHandler(this.AddCoin_Click);
            // 
            // ListOfAllCoins
            // 
            this.ListOfAllCoins.FullRowSelect = true;
            this.ListOfAllCoins.HideSelection = false;
            this.ListOfAllCoins.Location = new System.Drawing.Point(418, 49);
            this.ListOfAllCoins.MultiSelect = false;
            this.ListOfAllCoins.Name = "ListOfAllCoins";
            this.ListOfAllCoins.Size = new System.Drawing.Size(526, 410);
            this.ListOfAllCoins.SmallImageList = this.coinImageList;
            this.ListOfAllCoins.TabIndex = 18;
            this.ListOfAllCoins.UseCompatibleStateImageBehavior = false;
            this.ListOfAllCoins.View = System.Windows.Forms.View.Details;
            // 
            // CancelEditCoinList
            // 
            this.CancelEditCoinList.BackColor = System.Drawing.Color.RoyalBlue;
            this.CancelEditCoinList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelEditCoinList.Font = new System.Drawing.Font("Ink Free", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.CancelEditCoinList.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.CancelEditCoinList.Location = new System.Drawing.Point(744, 465);
            this.CancelEditCoinList.Name = "CancelEditCoinList";
            this.CancelEditCoinList.Size = new System.Drawing.Size(83, 30);
            this.CancelEditCoinList.TabIndex = 17;
            this.CancelEditCoinList.Text = "Cancel";
            this.CancelEditCoinList.UseVisualStyleBackColor = false;
            this.CancelEditCoinList.Click += new System.EventHandler(this.CancelEditCoinList_Click);
            // 
            // SaveCoinList
            // 
            this.SaveCoinList.BackColor = System.Drawing.Color.RoyalBlue;
            this.SaveCoinList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveCoinList.Font = new System.Drawing.Font("Ink Free", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SaveCoinList.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.SaveCoinList.Location = new System.Drawing.Point(632, 465);
            this.SaveCoinList.Name = "SaveCoinList";
            this.SaveCoinList.Size = new System.Drawing.Size(83, 30);
            this.SaveCoinList.TabIndex = 16;
            this.SaveCoinList.Text = "Save";
            this.SaveCoinList.UseVisualStyleBackColor = false;
            this.SaveCoinList.Click += new System.EventHandler(this.SaveCoinList_Click);
            // 
            // EditCoinList
            // 
            this.EditCoinList.FullRowSelect = true;
            this.EditCoinList.HideSelection = false;
            this.EditCoinList.Location = new System.Drawing.Point(6, 49);
            this.EditCoinList.MultiSelect = false;
            this.EditCoinList.Name = "EditCoinList";
            this.EditCoinList.Size = new System.Drawing.Size(320, 446);
            this.EditCoinList.SmallImageList = this.coinImageList;
            this.EditCoinList.TabIndex = 4;
            this.EditCoinList.UseCompatibleStateImageBehavior = false;
            this.EditCoinList.View = System.Windows.Forms.View.Details;
            // 
            // CoinListName
            // 
            this.CoinListName.AutoSize = true;
            this.CoinListName.BackColor = System.Drawing.Color.Transparent;
            this.CoinListName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CoinListName.Font = new System.Drawing.Font("Ink Free", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.CoinListName.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.CoinListName.Location = new System.Drawing.Point(2, 2);
            this.CoinListName.Name = "CoinListName";
            this.CoinListName.Size = new System.Drawing.Size(271, 46);
            this.CoinListName.TabIndex = 2;
            this.CoinListName.Text = "Coin List Name";
            // 
            // MaxGpuRoi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackgroundImage = global::Max_Gpu_Roi.Properties.Resources.space_background;
            this.ClientSize = new System.Drawing.Size(1873, 1460);
            this.Controls.Add(this.EditCoinPanel);
            this.Controls.Add(this.EditGpuPanel);
            this.Controls.Add(this.MaxMyROI);
            this.Controls.Add(this.Budget);
            this.Controls.Add(this.Exit);
            this.Controls.Add(this.Descending);
            this.Controls.Add(this.Ascending);
            this.Controls.Add(this.pnlResultsList);
            this.Controls.Add(this.pnlFilters);
            this.Controls.Add(this.pnlCosts);
            this.Controls.Add(this.pnlCoinLists);
            this.Controls.Add(this.pnlGpuLists);
            this.Controls.Add(this.lblBudget);
            this.Controls.Add(this.lblBudgetMoneySign);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MaxGpuRoi";
            this.Text = "Max-Gpu-Roi";
            this.Load += new System.EventHandler(this.MaxGpuRoi_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MaxGpuRoi_MouseDown);
            this.pnlGpuLists.ResumeLayout(false);
            this.pnlGpuLists.PerformLayout();
            this.pnlCoinLists.ResumeLayout(false);
            this.pnlCoinLists.PerformLayout();
            this.pnlCosts.ResumeLayout(false);
            this.pnlCosts.PerformLayout();
            this.pnlFilters.ResumeLayout(false);
            this.pnlFilters.PerformLayout();
            this.pnlResultsList.ResumeLayout(false);
            this.pnlResultsList.PerformLayout();
            this.EditGpuPanel.ResumeLayout(false);
            this.EditGpuPanel.PerformLayout();
            this.EditCoinPanel.ResumeLayout(false);
            this.EditCoinPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblGpuLists;
        private System.Windows.Forms.Label lblCoinLists;
        private System.Windows.Forms.Label lblCosts;
        private System.Windows.Forms.Label lblFilters;
        private System.Windows.Forms.Panel pnlGpuLists;
        private System.Windows.Forms.Panel pnlCoinLists;
        private System.Windows.Forms.Panel pnlCosts;
        private System.Windows.Forms.Panel pnlFilters;
        private System.Windows.Forms.Panel pnlResultsList;
        private System.Windows.Forms.Label lblResults;
        private System.Windows.Forms.ListView GpuLists;
        private System.Windows.Forms.Button ExportGpuLists;
        private System.Windows.Forms.Button ImportGpuLists;
        private System.Windows.Forms.Button EditGpuLists;
        private System.Windows.Forms.ListView CoinLists;
        private System.Windows.Forms.Button ExportCoinLists;
        private System.Windows.Forms.Button ImportCoinLists;
        private System.Windows.Forms.Button EditCoinLists;
        private System.Windows.Forms.Label lblElectricityRate;
        private System.Windows.Forms.Label lblPoolMinerFee;
        private System.Windows.Forms.Label lblPoolMinerFeePercentSign;
        private System.Windows.Forms.TextBox PoolMinerFee;
        private System.Windows.Forms.TextBox ElectricityRate;
        private System.Windows.Forms.RadioButton Ascending;
        private System.Windows.Forms.RadioButton Descending;
        private System.Windows.Forms.CheckBox ShowNvidia;
        private System.Windows.Forms.CheckBox ShowAmd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox SortBy;
        private System.Windows.Forms.Button Exit;
        private System.Windows.Forms.ListView ResultsList;
        private System.Windows.Forms.ImageList coinImageList;
        private System.Windows.Forms.TextBox Budget;
        private System.Windows.Forms.Label lblBudget;
        private System.Windows.Forms.Button MaxMyROI;
        private System.Windows.Forms.Label lblBudgetMoneySign;
        private System.Windows.Forms.Panel EditGpuPanel;
        private System.Windows.Forms.Label GpuListName;
        private System.Windows.Forms.ListView EditGpuList;
        private System.Windows.Forms.TextBox PricePaid;
        private System.Windows.Forms.TextBox EbayPrice;
        private System.Windows.Forms.TextBox MSRP;
        private System.Windows.Forms.Label lblPricePaid;
        private System.Windows.Forms.Label lblEbayPrice;
        private System.Windows.Forms.Label lblMSRP;
        private System.Windows.Forms.Label lblDateReleased;
        private System.Windows.Forms.Label DateReleased;
        private System.Windows.Forms.Label AmdOrNvidia;
        private System.Windows.Forms.ListView Hashrates;
        private System.Windows.Forms.Label lblHashrates;
        private System.Windows.Forms.Button CancelGpuHashrates;
        private System.Windows.Forms.Button SaveGpuHashrates;
        private System.Windows.Forms.Panel EditCoinPanel;
        private System.Windows.Forms.ListView EditCoinList;
        private System.Windows.Forms.Label CoinListName;
        private System.Windows.Forms.Button DeleteCoin;
        private System.Windows.Forms.Button AddCoin;
        private System.Windows.Forms.ListView ListOfAllCoins;
        private System.Windows.Forms.Button CancelEditCoinList;
        private System.Windows.Forms.Button SaveCoinList;
        private System.Windows.Forms.Label lblAllCoinsList;
    }
}
