namespace SEServerExtender
{
	using System.ComponentModel;
	using System.Windows.Forms;
	using SEModAPI.Support;

	sealed partial class SEServerExtender
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.TAB_MainTabs = new System.Windows.Forms.TabControl();
            this.TAB_Control_Page = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.LBL_Control_AutosaveInterval = new System.Windows.Forms.Label();
            this.CMB_Control_AutosaveInterval = new System.Windows.Forms.ComboBox();
            this.LBL_Control_Instance = new System.Windows.Forms.Label();
            this.CMB_Control_CommonInstanceList = new System.Windows.Forms.ComboBox();
            this.CHK_Control_CommonDataPath = new System.Windows.Forms.CheckBox();
            this.CHK_Control_Debugging = new System.Windows.Forms.CheckBox();
            this.BTN_ServerControl_Start = new System.Windows.Forms.Button();
            this.BTN_ServerControl_Stop = new System.Windows.Forms.Button();
            this.GRP_Control_Server = new System.Windows.Forms.GroupBox();
            this.BTN_Control_Server_Reset = new System.Windows.Forms.Button();
            this.BTN_Control_Server_Save = new System.Windows.Forms.Button();
            this.PG_Control_Server_Properties = new System.Windows.Forms.PropertyGrid();
            this.TAB_Entities_Page = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.CB_Entity_Sort = new System.Windows.Forms.ComboBox();
            this.TRV_Entities = new System.Windows.Forms.TreeView();
            this.btnRepairEntity = new System.Windows.Forms.Button();
            this.BTN_Entities_Export = new System.Windows.Forms.Button();
            this.BTN_Entities_New = new System.Windows.Forms.Button();
            this.BTN_Entities_Delete = new System.Windows.Forms.Button();
            this.PG_Entities_Details = new System.Windows.Forms.PropertyGrid();
            this.TAB_Chat_Page = new System.Windows.Forms.TabPage();
            this.splitContainer6 = new System.Windows.Forms.SplitContainer();
            this.splitContainer8 = new System.Windows.Forms.SplitContainer();
            this.RTB_Chat_Messages = new System.Windows.Forms.RichTextBox();
            this.splitContainer13 = new System.Windows.Forms.SplitContainer();
            this.LST_Chat_ConnectedPlayers = new System.Windows.Forms.ListBox();
            this.CMS_Chat = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TSM_Kick = new System.Windows.Forms.ToolStripMenuItem();
            this.TSM_Ban = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BTN_Chat_BanSelected = new System.Windows.Forms.Button();
            this.BTN_Chat_KickSelected = new System.Windows.Forms.Button();
            this.splitContainer7 = new System.Windows.Forms.SplitContainer();
            this.TXT_Chat_Message = new System.Windows.Forms.TextBox();
            this.BTN_Chat_Send = new System.Windows.Forms.Button();
            this.TAB_Factions_Page = new System.Windows.Forms.TabPage();
            this.splitContainer9 = new System.Windows.Forms.SplitContainer();
            this.splitContainer10 = new System.Windows.Forms.SplitContainer();
            this.TRV_Factions = new System.Windows.Forms.TreeView();
            this.BTN_Factions_Demote = new System.Windows.Forms.Button();
            this.BTN_Factions_Promote = new System.Windows.Forms.Button();
            this.BTN_Factions_Delete = new System.Windows.Forms.Button();
            this.PG_Factions = new System.Windows.Forms.PropertyGrid();
            this.TAB_Plugins_Page = new System.Windows.Forms.TabPage();
            this.SC_Plugins = new System.Windows.Forms.SplitContainer();
            this.splitContainer12 = new System.Windows.Forms.SplitContainer();
            this.LST_Plugins = new System.Windows.Forms.ListBox();
            this.BTN_Plugins_Enable = new System.Windows.Forms.Button();
            this.BTN_Plugins_Reload = new System.Windows.Forms.Button();
            this.PG_Plugins = new System.Windows.Forms.PropertyGrid();
            this.TAB_Statistics = new System.Windows.Forms.TabPage();
            this.TB_Statistics = new System.Windows.Forms.TextBox();
            this.TAB_Profiler = new System.Windows.Forms.TabPage();
            this.CHK_ProfileGrids = new System.Windows.Forms.CheckBox();
            this.CHK_PauseProfiler = new System.Windows.Forms.CheckBox();
            this.CHK_ProfileBlocks = new System.Windows.Forms.CheckBox();
            this.TB_Profiler = new System.Windows.Forms.TextBox();
            this.SS_Bottom = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.TAB_MainTabs.SuspendLayout();
            this.TAB_Control_Page.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.GRP_Control_Server.SuspendLayout();
            this.TAB_Entities_Page.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            this.TAB_Chat_Page.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).BeginInit();
            this.splitContainer6.Panel1.SuspendLayout();
            this.splitContainer6.Panel2.SuspendLayout();
            this.splitContainer6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer8)).BeginInit();
            this.splitContainer8.Panel1.SuspendLayout();
            this.splitContainer8.Panel2.SuspendLayout();
            this.splitContainer8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer13)).BeginInit();
            this.splitContainer13.Panel1.SuspendLayout();
            this.splitContainer13.Panel2.SuspendLayout();
            this.splitContainer13.SuspendLayout();
            this.CMS_Chat.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).BeginInit();
            this.splitContainer7.Panel1.SuspendLayout();
            this.splitContainer7.Panel2.SuspendLayout();
            this.splitContainer7.SuspendLayout();
            this.TAB_Factions_Page.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer9)).BeginInit();
            this.splitContainer9.Panel1.SuspendLayout();
            this.splitContainer9.Panel2.SuspendLayout();
            this.splitContainer9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer10)).BeginInit();
            this.splitContainer10.Panel1.SuspendLayout();
            this.splitContainer10.Panel2.SuspendLayout();
            this.splitContainer10.SuspendLayout();
            this.TAB_Plugins_Page.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SC_Plugins)).BeginInit();
            this.SC_Plugins.Panel1.SuspendLayout();
            this.SC_Plugins.Panel2.SuspendLayout();
            this.SC_Plugins.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer12)).BeginInit();
            this.splitContainer12.Panel1.SuspendLayout();
            this.splitContainer12.Panel2.SuspendLayout();
            this.splitContainer12.SuspendLayout();
            this.TAB_Statistics.SuspendLayout();
            this.TAB_Profiler.SuspendLayout();
            this.SS_Bottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.TAB_MainTabs);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.SS_Bottom);
            this.splitContainer1.Size = new System.Drawing.Size(951, 598);
            this.splitContainer1.SplitterDistance = 569;
            this.splitContainer1.TabIndex = 0;
            // 
            // TAB_MainTabs
            // 
            this.TAB_MainTabs.Controls.Add(this.TAB_Control_Page);
            this.TAB_MainTabs.Controls.Add(this.TAB_Entities_Page);
            this.TAB_MainTabs.Controls.Add(this.TAB_Chat_Page);
            this.TAB_MainTabs.Controls.Add(this.TAB_Factions_Page);
            this.TAB_MainTabs.Controls.Add(this.TAB_Plugins_Page);
            this.TAB_MainTabs.Controls.Add(this.TAB_Statistics);
            this.TAB_MainTabs.Controls.Add(this.TAB_Profiler);
            this.TAB_MainTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TAB_MainTabs.Location = new System.Drawing.Point(0, 0);
            this.TAB_MainTabs.Multiline = true;
            this.TAB_MainTabs.Name = "TAB_MainTabs";
            this.TAB_MainTabs.SelectedIndex = 0;
            this.TAB_MainTabs.Size = new System.Drawing.Size(951, 569);
            this.TAB_MainTabs.TabIndex = 0;
            this.TAB_MainTabs.SelectedIndexChanged += new System.EventHandler(this.TAB_MainTabs_SelectedIndexChanged);
            // 
            // TAB_Control_Page
            // 
            this.TAB_Control_Page.Controls.Add(this.splitContainer3);
            this.TAB_Control_Page.Location = new System.Drawing.Point(4, 22);
            this.TAB_Control_Page.Name = "TAB_Control_Page";
            this.TAB_Control_Page.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_Control_Page.Size = new System.Drawing.Size(943, 543);
            this.TAB_Control_Page.TabIndex = 0;
            this.TAB_Control_Page.Text = "Control";
            this.TAB_Control_Page.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.LBL_Control_AutosaveInterval);
            this.splitContainer3.Panel1.Controls.Add(this.CMB_Control_AutosaveInterval);
            this.splitContainer3.Panel1.Controls.Add(this.LBL_Control_Instance);
            this.splitContainer3.Panel1.Controls.Add(this.CMB_Control_CommonInstanceList);
            this.splitContainer3.Panel1.Controls.Add(this.CHK_Control_CommonDataPath);
            this.splitContainer3.Panel1.Controls.Add(this.CHK_Control_Debugging);
            this.splitContainer3.Panel1.Controls.Add(this.BTN_ServerControl_Start);
            this.splitContainer3.Panel1.Controls.Add(this.BTN_ServerControl_Stop);
            this.splitContainer3.Panel1MinSize = 165;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.GRP_Control_Server);
            this.splitContainer3.Size = new System.Drawing.Size(937, 537);
            this.splitContainer3.SplitterDistance = 165;
            this.splitContainer3.TabIndex = 4;
            // 
            // LBL_Control_AutosaveInterval
            // 
            this.LBL_Control_AutosaveInterval.AutoSize = true;
            this.LBL_Control_AutosaveInterval.Location = new System.Drawing.Point(5, 156);
            this.LBL_Control_AutosaveInterval.Name = "LBL_Control_AutosaveInterval";
            this.LBL_Control_AutosaveInterval.Size = new System.Drawing.Size(92, 13);
            this.LBL_Control_AutosaveInterval.TabIndex = 8;
            this.LBL_Control_AutosaveInterval.Text = "Auto-save interval";
            // 
            // CMB_Control_AutosaveInterval
            // 
            this.CMB_Control_AutosaveInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_Control_AutosaveInterval.FormattingEnabled = true;
            this.CMB_Control_AutosaveInterval.Location = new System.Drawing.Point(5, 172);
            this.CMB_Control_AutosaveInterval.Name = "CMB_Control_AutosaveInterval";
            this.CMB_Control_AutosaveInterval.Size = new System.Drawing.Size(150, 21);
            this.CMB_Control_AutosaveInterval.TabIndex = 7;
            this.CMB_Control_AutosaveInterval.SelectedIndexChanged += new System.EventHandler(this.CMB_Control_AutosaveInterval_SelectedIndexChanged);
            // 
            // LBL_Control_Instance
            // 
            this.LBL_Control_Instance.AutoSize = true;
            this.LBL_Control_Instance.Location = new System.Drawing.Point(5, 116);
            this.LBL_Control_Instance.Name = "LBL_Control_Instance";
            this.LBL_Control_Instance.Size = new System.Drawing.Size(115, 13);
            this.LBL_Control_Instance.TabIndex = 6;
            this.LBL_Control_Instance.Text = "Common data instance";
            // 
            // CMB_Control_CommonInstanceList
            // 
            this.CMB_Control_CommonInstanceList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_Control_CommonInstanceList.Enabled = false;
            this.CMB_Control_CommonInstanceList.FormattingEnabled = true;
            this.CMB_Control_CommonInstanceList.Location = new System.Drawing.Point(5, 132);
            this.CMB_Control_CommonInstanceList.Name = "CMB_Control_CommonInstanceList";
            this.CMB_Control_CommonInstanceList.Size = new System.Drawing.Size(150, 21);
            this.CMB_Control_CommonInstanceList.TabIndex = 5;
            this.CMB_Control_CommonInstanceList.SelectedIndexChanged += new System.EventHandler(this.CMB_Control_CommonInstanceList_SelectedIndexChanged);
            // 
            // CHK_Control_CommonDataPath
            // 
            this.CHK_Control_CommonDataPath.AutoSize = true;
            this.CHK_Control_CommonDataPath.Enabled = false;
            this.CHK_Control_CommonDataPath.Location = new System.Drawing.Point(5, 96);
            this.CHK_Control_CommonDataPath.Name = "CHK_Control_CommonDataPath";
            this.CHK_Control_CommonDataPath.Size = new System.Drawing.Size(153, 17);
            this.CHK_Control_CommonDataPath.TabIndex = 4;
            this.CHK_Control_CommonDataPath.Text = "Use common program data";
            this.CHK_Control_CommonDataPath.UseVisualStyleBackColor = true;
            this.CHK_Control_CommonDataPath.CheckedChanged += new System.EventHandler(this.CHK_Control_CommonDataPath_CheckedChanged);
            // 
            // CHK_Control_Debugging
            // 
            this.CHK_Control_Debugging.AutoSize = true;
            this.CHK_Control_Debugging.Location = new System.Drawing.Point(5, 64);
            this.CHK_Control_Debugging.Name = "CHK_Control_Debugging";
            this.CHK_Control_Debugging.Size = new System.Drawing.Size(87, 17);
            this.CHK_Control_Debugging.TabIndex = 0;
            this.CHK_Control_Debugging.Text = "Debug mode";
            this.CHK_Control_Debugging.UseVisualStyleBackColor = true;
            this.CHK_Control_Debugging.CheckedChanged += new System.EventHandler(this.CHK_Control_Debugging_CheckedChanged);
            // 
            // BTN_ServerControl_Start
            // 
            this.BTN_ServerControl_Start.Location = new System.Drawing.Point(5, 6);
            this.BTN_ServerControl_Start.Name = "BTN_ServerControl_Start";
            this.BTN_ServerControl_Start.Size = new System.Drawing.Size(87, 23);
            this.BTN_ServerControl_Start.TabIndex = 0;
            this.BTN_ServerControl_Start.Text = "Start Server";
            this.BTN_ServerControl_Start.UseVisualStyleBackColor = true;
            this.BTN_ServerControl_Start.Click += new System.EventHandler(this.BTN_ServerControl_Start_Click);
            // 
            // BTN_ServerControl_Stop
            // 
            this.BTN_ServerControl_Stop.Location = new System.Drawing.Point(5, 35);
            this.BTN_ServerControl_Stop.Name = "BTN_ServerControl_Stop";
            this.BTN_ServerControl_Stop.Size = new System.Drawing.Size(87, 23);
            this.BTN_ServerControl_Stop.TabIndex = 1;
            this.BTN_ServerControl_Stop.Text = "Stop Server";
            this.BTN_ServerControl_Stop.UseVisualStyleBackColor = true;
            this.BTN_ServerControl_Stop.Click += new System.EventHandler(this.BTN_ServerControl_Stop_Click);
            // 
            // GRP_Control_Server
            // 
            this.GRP_Control_Server.Controls.Add(this.BTN_Control_Server_Reset);
            this.GRP_Control_Server.Controls.Add(this.BTN_Control_Server_Save);
            this.GRP_Control_Server.Controls.Add(this.PG_Control_Server_Properties);
            this.GRP_Control_Server.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GRP_Control_Server.Location = new System.Drawing.Point(0, 0);
            this.GRP_Control_Server.Name = "GRP_Control_Server";
            this.GRP_Control_Server.Size = new System.Drawing.Size(768, 537);
            this.GRP_Control_Server.TabIndex = 2;
            this.GRP_Control_Server.TabStop = false;
            this.GRP_Control_Server.Text = "Server properties";
            // 
            // BTN_Control_Server_Reset
            // 
            this.BTN_Control_Server_Reset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BTN_Control_Server_Reset.Location = new System.Drawing.Point(573, 511);
            this.BTN_Control_Server_Reset.Name = "BTN_Control_Server_Reset";
            this.BTN_Control_Server_Reset.Size = new System.Drawing.Size(75, 23);
            this.BTN_Control_Server_Reset.TabIndex = 3;
            this.BTN_Control_Server_Reset.Text = "Reset";
            this.BTN_Control_Server_Reset.UseVisualStyleBackColor = true;
            this.BTN_Control_Server_Reset.Click += new System.EventHandler(this.BTN_Control_Server_Reset_Click);
            // 
            // BTN_Control_Server_Save
            // 
            this.BTN_Control_Server_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BTN_Control_Server_Save.Location = new System.Drawing.Point(654, 511);
            this.BTN_Control_Server_Save.Name = "BTN_Control_Server_Save";
            this.BTN_Control_Server_Save.Size = new System.Drawing.Size(108, 23);
            this.BTN_Control_Server_Save.TabIndex = 2;
            this.BTN_Control_Server_Save.Text = "Save Properties";
            this.BTN_Control_Server_Save.UseVisualStyleBackColor = true;
            this.BTN_Control_Server_Save.Click += new System.EventHandler(this.BTN_Control_Server_Save_Click);
            // 
            // PG_Control_Server_Properties
            // 
            this.PG_Control_Server_Properties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PG_Control_Server_Properties.Location = new System.Drawing.Point(6, 19);
            this.PG_Control_Server_Properties.Name = "PG_Control_Server_Properties";
            this.PG_Control_Server_Properties.Size = new System.Drawing.Size(756, 486);
            this.PG_Control_Server_Properties.TabIndex = 1;
            this.PG_Control_Server_Properties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PG_Control_Server_Properties_PropertyValueChanged);
            // 
            // TAB_Entities_Page
            // 
            this.TAB_Entities_Page.Controls.Add(this.splitContainer2);
            this.TAB_Entities_Page.Location = new System.Drawing.Point(4, 22);
            this.TAB_Entities_Page.Name = "TAB_Entities_Page";
            this.TAB_Entities_Page.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_Entities_Page.Size = new System.Drawing.Size(943, 543);
            this.TAB_Entities_Page.TabIndex = 1;
            this.TAB_Entities_Page.Text = "Entities";
            this.TAB_Entities_Page.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer5);
            this.splitContainer2.Panel1MinSize = 300;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.PG_Entities_Details);
            this.splitContainer2.Size = new System.Drawing.Size(937, 537);
            this.splitContainer2.SplitterDistance = 300;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer5
            // 
            this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer5.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer5.Location = new System.Drawing.Point(0, 0);
            this.splitContainer5.Name = "splitContainer5";
            this.splitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.CB_Entity_Sort);
            this.splitContainer5.Panel1.Controls.Add(this.TRV_Entities);
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.btnRepairEntity);
            this.splitContainer5.Panel2.Controls.Add(this.BTN_Entities_Export);
            this.splitContainer5.Panel2.Controls.Add(this.BTN_Entities_New);
            this.splitContainer5.Panel2.Controls.Add(this.BTN_Entities_Delete);
            this.splitContainer5.Size = new System.Drawing.Size(300, 537);
            this.splitContainer5.SplitterDistance = 502;
            this.splitContainer5.TabIndex = 0;
            // 
            // CB_Entity_Sort
            // 
            this.CB_Entity_Sort.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.CB_Entity_Sort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Entity_Sort.FormattingEnabled = true;
            this.CB_Entity_Sort.Items.AddRange(new object[] {
            "Display Name",
            "Owner Name",
            "Block Count",
            "Distance From Center",
            "Mass",
            "EntityId"});
            this.CB_Entity_Sort.Location = new System.Drawing.Point(0, 481);
            this.CB_Entity_Sort.Name = "CB_Entity_Sort";
            this.CB_Entity_Sort.Size = new System.Drawing.Size(300, 21);
            this.CB_Entity_Sort.TabIndex = 1;
            this.CB_Entity_Sort.SelectionChangeCommitted += new System.EventHandler(this.CB_Entity_Sort_SelectionChangeCommitted);
            // 
            // TRV_Entities
            // 
            this.TRV_Entities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TRV_Entities.Location = new System.Drawing.Point(0, 0);
            this.TRV_Entities.Name = "TRV_Entities";
            this.TRV_Entities.Size = new System.Drawing.Size(300, 502);
            this.TRV_Entities.TabIndex = 0;
            this.TRV_Entities.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TRV_Entities_AfterSelect);
            this.TRV_Entities.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TRV_Entities_NodeRefresh);
            // 
            // btnRepairEntity
            // 
            this.btnRepairEntity.Enabled = false;
            this.btnRepairEntity.Location = new System.Drawing.Point(133, 4);
            this.btnRepairEntity.Name = "btnRepairEntity";
            this.btnRepairEntity.Size = new System.Drawing.Size(53, 23);
            this.btnRepairEntity.TabIndex = 3;
            this.btnRepairEntity.Text = "Repair";
            this.btnRepairEntity.UseVisualStyleBackColor = true;
            this.btnRepairEntity.Click += new System.EventHandler(this.btnRepairEntity_Click);
            // 
            // BTN_Entities_Export
            // 
            this.BTN_Entities_Export.Enabled = false;
            this.BTN_Entities_Export.Location = new System.Drawing.Point(4, 4);
            this.BTN_Entities_Export.Name = "BTN_Entities_Export";
            this.BTN_Entities_Export.Size = new System.Drawing.Size(53, 23);
            this.BTN_Entities_Export.TabIndex = 2;
            this.BTN_Entities_Export.Text = "Export";
            this.BTN_Entities_Export.UseVisualStyleBackColor = true;
            this.BTN_Entities_Export.Click += new System.EventHandler(this.BTN_Entities_Export_Click);
            // 
            // BTN_Entities_New
            // 
            this.BTN_Entities_New.Enabled = false;
            this.BTN_Entities_New.Location = new System.Drawing.Point(188, 4);
            this.BTN_Entities_New.Name = "BTN_Entities_New";
            this.BTN_Entities_New.Size = new System.Drawing.Size(53, 23);
            this.BTN_Entities_New.TabIndex = 1;
            this.BTN_Entities_New.Text = "New";
            this.BTN_Entities_New.UseVisualStyleBackColor = true;
            this.BTN_Entities_New.Click += new System.EventHandler(this.BTN_Entities_New_Click);
            // 
            // BTN_Entities_Delete
            // 
            this.BTN_Entities_Delete.Enabled = false;
            this.BTN_Entities_Delete.Location = new System.Drawing.Point(243, 4);
            this.BTN_Entities_Delete.Name = "BTN_Entities_Delete";
            this.BTN_Entities_Delete.Size = new System.Drawing.Size(53, 23);
            this.BTN_Entities_Delete.TabIndex = 0;
            this.BTN_Entities_Delete.Text = "Delete";
            this.BTN_Entities_Delete.UseVisualStyleBackColor = true;
            this.BTN_Entities_Delete.Click += new System.EventHandler(this.BTN_Entities_Delete_Click);
            // 
            // PG_Entities_Details
            // 
            this.PG_Entities_Details.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PG_Entities_Details.Location = new System.Drawing.Point(0, 0);
            this.PG_Entities_Details.Name = "PG_Entities_Details";
            this.PG_Entities_Details.Size = new System.Drawing.Size(633, 537);
            this.PG_Entities_Details.TabIndex = 0;
            this.PG_Entities_Details.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PG_Entities_Details_Click);
            this.PG_Entities_Details.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.PG_Entities_Details_Click);
            this.PG_Entities_Details.Validated += new System.EventHandler(this.PG_Entities_Details_Click);
            // 
            // TAB_Chat_Page
            // 
            this.TAB_Chat_Page.Controls.Add(this.splitContainer6);
            this.TAB_Chat_Page.Location = new System.Drawing.Point(4, 22);
            this.TAB_Chat_Page.Name = "TAB_Chat_Page";
            this.TAB_Chat_Page.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_Chat_Page.Size = new System.Drawing.Size(943, 543);
            this.TAB_Chat_Page.TabIndex = 2;
            this.TAB_Chat_Page.Text = "Chat";
            this.TAB_Chat_Page.UseVisualStyleBackColor = true;
            // 
            // splitContainer6
            // 
            this.splitContainer6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer6.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer6.Location = new System.Drawing.Point(3, 3);
            this.splitContainer6.Name = "splitContainer6";
            this.splitContainer6.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer6.Panel1
            // 
            this.splitContainer6.Panel1.Controls.Add(this.splitContainer8);
            // 
            // splitContainer6.Panel2
            // 
            this.splitContainer6.Panel2.Controls.Add(this.splitContainer7);
            this.splitContainer6.Size = new System.Drawing.Size(937, 537);
            this.splitContainer6.SplitterDistance = 504;
            this.splitContainer6.TabIndex = 4;
            // 
            // splitContainer8
            // 
            this.splitContainer8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer8.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer8.Location = new System.Drawing.Point(0, 0);
            this.splitContainer8.Name = "splitContainer8";
            // 
            // splitContainer8.Panel1
            // 
            this.splitContainer8.Panel1.Controls.Add(this.RTB_Chat_Messages);
            // 
            // splitContainer8.Panel2
            // 
            this.splitContainer8.Panel2.Controls.Add(this.splitContainer13);
            this.splitContainer8.Size = new System.Drawing.Size(937, 504);
            this.splitContainer8.SplitterDistance = 750;
            this.splitContainer8.TabIndex = 4;
            // 
            // RTB_Chat_Messages
            // 
            this.RTB_Chat_Messages.BackColor = System.Drawing.SystemColors.Window;
            this.RTB_Chat_Messages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RTB_Chat_Messages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RTB_Chat_Messages.HideSelection = false;
            this.RTB_Chat_Messages.Location = new System.Drawing.Point(0, 0);
            this.RTB_Chat_Messages.Name = "RTB_Chat_Messages";
            this.RTB_Chat_Messages.ReadOnly = true;
            this.RTB_Chat_Messages.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.RTB_Chat_Messages.Size = new System.Drawing.Size(750, 504);
            this.RTB_Chat_Messages.TabIndex = 0;
            this.RTB_Chat_Messages.Text = "";
            // 
            // splitContainer13
            // 
            this.splitContainer13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer13.Location = new System.Drawing.Point(0, 0);
            this.splitContainer13.Name = "splitContainer13";
            this.splitContainer13.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer13.Panel1
            // 
            this.splitContainer13.Panel1.Controls.Add(this.LST_Chat_ConnectedPlayers);
            // 
            // splitContainer13.Panel2
            // 
            this.splitContainer13.Panel2.Controls.Add(this.panel1);
            this.splitContainer13.Size = new System.Drawing.Size(183, 504);
            this.splitContainer13.SplitterDistance = 443;
            this.splitContainer13.TabIndex = 0;
            // 
            // LST_Chat_ConnectedPlayers
            // 
            this.LST_Chat_ConnectedPlayers.ContextMenuStrip = this.CMS_Chat;
            this.LST_Chat_ConnectedPlayers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LST_Chat_ConnectedPlayers.FormattingEnabled = true;
            this.LST_Chat_ConnectedPlayers.Location = new System.Drawing.Point(0, 0);
            this.LST_Chat_ConnectedPlayers.Name = "LST_Chat_ConnectedPlayers";
            this.LST_Chat_ConnectedPlayers.Size = new System.Drawing.Size(183, 443);
            this.LST_Chat_ConnectedPlayers.TabIndex = 1;
            // 
            // CMS_Chat
            // 
            this.CMS_Chat.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSM_Kick,
            this.TSM_Ban});
            this.CMS_Chat.Name = "ChatMenuStrip";
            this.CMS_Chat.Size = new System.Drawing.Size(97, 48);
            // 
            // TSM_Kick
            // 
            this.TSM_Kick.Name = "TSM_Kick";
            this.TSM_Kick.Size = new System.Drawing.Size(96, 22);
            this.TSM_Kick.Text = "Kick";
            this.TSM_Kick.Click += new System.EventHandler(this.TSM_Kick_Click);
            // 
            // TSM_Ban
            // 
            this.TSM_Ban.Name = "TSM_Ban";
            this.TSM_Ban.Size = new System.Drawing.Size(96, 22);
            this.TSM_Ban.Text = "Ban";
            this.TSM_Ban.Click += new System.EventHandler(this.TSM_Ban_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BTN_Chat_BanSelected);
            this.panel1.Controls.Add(this.BTN_Chat_KickSelected);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(183, 57);
            this.panel1.TabIndex = 0;
            // 
            // BTN_Chat_BanSelected
            // 
            this.BTN_Chat_BanSelected.Location = new System.Drawing.Point(3, 31);
            this.BTN_Chat_BanSelected.Name = "BTN_Chat_BanSelected";
            this.BTN_Chat_BanSelected.Size = new System.Drawing.Size(175, 23);
            this.BTN_Chat_BanSelected.TabIndex = 1;
            this.BTN_Chat_BanSelected.Text = "Ban Selected";
            this.BTN_Chat_BanSelected.UseVisualStyleBackColor = true;
            this.BTN_Chat_BanSelected.Click += new System.EventHandler(this.BTN_Chat_BanSelected_Click);
            // 
            // BTN_Chat_KickSelected
            // 
            this.BTN_Chat_KickSelected.Location = new System.Drawing.Point(3, 2);
            this.BTN_Chat_KickSelected.Name = "BTN_Chat_KickSelected";
            this.BTN_Chat_KickSelected.Size = new System.Drawing.Size(177, 23);
            this.BTN_Chat_KickSelected.TabIndex = 0;
            this.BTN_Chat_KickSelected.Text = "Kick Selected";
            this.BTN_Chat_KickSelected.UseVisualStyleBackColor = true;
            this.BTN_Chat_KickSelected.Click += new System.EventHandler(this.BTN_Chat_KickSelected_Click);
            // 
            // splitContainer7
            // 
            this.splitContainer7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer7.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer7.Location = new System.Drawing.Point(0, 0);
            this.splitContainer7.Name = "splitContainer7";
            // 
            // splitContainer7.Panel1
            // 
            this.splitContainer7.Panel1.Controls.Add(this.TXT_Chat_Message);
            // 
            // splitContainer7.Panel2
            // 
            this.splitContainer7.Panel2.Controls.Add(this.BTN_Chat_Send);
            this.splitContainer7.Size = new System.Drawing.Size(937, 29);
            this.splitContainer7.SplitterDistance = 800;
            this.splitContainer7.TabIndex = 2;
            // 
            // TXT_Chat_Message
            // 
            this.TXT_Chat_Message.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TXT_Chat_Message.Location = new System.Drawing.Point(0, 0);
            this.TXT_Chat_Message.Name = "TXT_Chat_Message";
            this.TXT_Chat_Message.Size = new System.Drawing.Size(800, 20);
            this.TXT_Chat_Message.TabIndex = 0;
            this.TXT_Chat_Message.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TXT_Chat_Message_KeyDown);
            // 
            // BTN_Chat_Send
            // 
            this.BTN_Chat_Send.Location = new System.Drawing.Point(53, 3);
            this.BTN_Chat_Send.Name = "BTN_Chat_Send";
            this.BTN_Chat_Send.Size = new System.Drawing.Size(75, 23);
            this.BTN_Chat_Send.TabIndex = 1;
            this.BTN_Chat_Send.Text = "Send";
            this.BTN_Chat_Send.UseVisualStyleBackColor = true;
            this.BTN_Chat_Send.Click += new System.EventHandler(this.BTN_Chat_Send_Click);
            // 
            // TAB_Factions_Page
            // 
            this.TAB_Factions_Page.Controls.Add(this.splitContainer9);
            this.TAB_Factions_Page.Location = new System.Drawing.Point(4, 22);
            this.TAB_Factions_Page.Name = "TAB_Factions_Page";
            this.TAB_Factions_Page.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_Factions_Page.Size = new System.Drawing.Size(943, 543);
            this.TAB_Factions_Page.TabIndex = 3;
            this.TAB_Factions_Page.Text = "Factions";
            this.TAB_Factions_Page.UseVisualStyleBackColor = true;
            // 
            // splitContainer9
            // 
            this.splitContainer9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer9.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer9.Location = new System.Drawing.Point(3, 3);
            this.splitContainer9.Name = "splitContainer9";
            // 
            // splitContainer9.Panel1
            // 
            this.splitContainer9.Panel1.Controls.Add(this.splitContainer10);
            this.splitContainer9.Panel1MinSize = 300;
            // 
            // splitContainer9.Panel2
            // 
            this.splitContainer9.Panel2.Controls.Add(this.PG_Factions);
            this.splitContainer9.Size = new System.Drawing.Size(937, 537);
            this.splitContainer9.SplitterDistance = 300;
            this.splitContainer9.TabIndex = 0;
            // 
            // splitContainer10
            // 
            this.splitContainer10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer10.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer10.Location = new System.Drawing.Point(0, 0);
            this.splitContainer10.Name = "splitContainer10";
            this.splitContainer10.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer10.Panel1
            // 
            this.splitContainer10.Panel1.Controls.Add(this.TRV_Factions);
            // 
            // splitContainer10.Panel2
            // 
            this.splitContainer10.Panel2.Controls.Add(this.BTN_Factions_Demote);
            this.splitContainer10.Panel2.Controls.Add(this.BTN_Factions_Promote);
            this.splitContainer10.Panel2.Controls.Add(this.BTN_Factions_Delete);
            this.splitContainer10.Size = new System.Drawing.Size(300, 537);
            this.splitContainer10.SplitterDistance = 502;
            this.splitContainer10.TabIndex = 0;
            // 
            // TRV_Factions
            // 
            this.TRV_Factions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TRV_Factions.Location = new System.Drawing.Point(0, 0);
            this.TRV_Factions.Name = "TRV_Factions";
            this.TRV_Factions.Size = new System.Drawing.Size(300, 502);
            this.TRV_Factions.TabIndex = 0;
            this.TRV_Factions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TRV_Factions_AfterSelect);
            // 
            // BTN_Factions_Demote
            // 
            this.BTN_Factions_Demote.Enabled = false;
            this.BTN_Factions_Demote.Location = new System.Drawing.Point(5, 4);
            this.BTN_Factions_Demote.Name = "BTN_Factions_Demote";
            this.BTN_Factions_Demote.Size = new System.Drawing.Size(75, 23);
            this.BTN_Factions_Demote.TabIndex = 3;
            this.BTN_Factions_Demote.Text = "Demote";
            this.BTN_Factions_Demote.UseVisualStyleBackColor = true;
            this.BTN_Factions_Demote.Click += new System.EventHandler(this.BTN_Factions_Demote_Click);
            // 
            // BTN_Factions_Promote
            // 
            this.BTN_Factions_Promote.Enabled = false;
            this.BTN_Factions_Promote.Location = new System.Drawing.Point(86, 4);
            this.BTN_Factions_Promote.Name = "BTN_Factions_Promote";
            this.BTN_Factions_Promote.Size = new System.Drawing.Size(75, 23);
            this.BTN_Factions_Promote.TabIndex = 2;
            this.BTN_Factions_Promote.Text = "Promote";
            this.BTN_Factions_Promote.UseVisualStyleBackColor = true;
            this.BTN_Factions_Promote.Click += new System.EventHandler(this.BTN_Factions_Promote_Click);
            // 
            // BTN_Factions_Delete
            // 
            this.BTN_Factions_Delete.Enabled = false;
            this.BTN_Factions_Delete.Location = new System.Drawing.Point(222, 4);
            this.BTN_Factions_Delete.Name = "BTN_Factions_Delete";
            this.BTN_Factions_Delete.Size = new System.Drawing.Size(75, 23);
            this.BTN_Factions_Delete.TabIndex = 1;
            this.BTN_Factions_Delete.Text = "Delete";
            this.BTN_Factions_Delete.UseVisualStyleBackColor = true;
            this.BTN_Factions_Delete.Click += new System.EventHandler(this.BTN_Factions_Delete_Click);
            // 
            // PG_Factions
            // 
            this.PG_Factions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PG_Factions.Location = new System.Drawing.Point(0, 0);
            this.PG_Factions.Name = "PG_Factions";
            this.PG_Factions.Size = new System.Drawing.Size(633, 537);
            this.PG_Factions.TabIndex = 0;
            // 
            // TAB_Plugins_Page
            // 
            this.TAB_Plugins_Page.Controls.Add(this.SC_Plugins);
            this.TAB_Plugins_Page.Location = new System.Drawing.Point(4, 22);
            this.TAB_Plugins_Page.Name = "TAB_Plugins_Page";
            this.TAB_Plugins_Page.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_Plugins_Page.Size = new System.Drawing.Size(943, 543);
            this.TAB_Plugins_Page.TabIndex = 4;
            this.TAB_Plugins_Page.Text = "Plugins";
            this.TAB_Plugins_Page.UseVisualStyleBackColor = true;
            // 
            // SC_Plugins
            // 
            this.SC_Plugins.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SC_Plugins.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.SC_Plugins.Location = new System.Drawing.Point(3, 3);
            this.SC_Plugins.Name = "SC_Plugins";
            // 
            // SC_Plugins.Panel1
            // 
            this.SC_Plugins.Panel1.Controls.Add(this.splitContainer12);
            this.SC_Plugins.Panel1MinSize = 300;
            // 
            // SC_Plugins.Panel2
            // 
            this.SC_Plugins.Panel2.Controls.Add(this.PG_Plugins);
            this.SC_Plugins.Size = new System.Drawing.Size(937, 537);
            this.SC_Plugins.SplitterDistance = 300;
            this.SC_Plugins.TabIndex = 0;
            // 
            // splitContainer12
            // 
            this.splitContainer12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer12.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer12.Location = new System.Drawing.Point(0, 0);
            this.splitContainer12.Name = "splitContainer12";
            this.splitContainer12.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer12.Panel1
            // 
            this.splitContainer12.Panel1.Controls.Add(this.LST_Plugins);
            // 
            // splitContainer12.Panel2
            // 
            this.splitContainer12.Panel2.Controls.Add(this.BTN_Plugins_Enable);
            this.splitContainer12.Panel2.Controls.Add(this.BTN_Plugins_Reload);
            this.splitContainer12.Size = new System.Drawing.Size(300, 537);
            this.splitContainer12.SplitterDistance = 502;
            this.splitContainer12.TabIndex = 0;
            // 
            // LST_Plugins
            // 
            this.LST_Plugins.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LST_Plugins.FormattingEnabled = true;
            this.LST_Plugins.Location = new System.Drawing.Point(0, 0);
            this.LST_Plugins.Name = "LST_Plugins";
            this.LST_Plugins.Size = new System.Drawing.Size(300, 502);
            this.LST_Plugins.TabIndex = 0;
            this.LST_Plugins.SelectedIndexChanged += new System.EventHandler(this.LST_Plugins_SelectedIndexChanged);
            // 
            // BTN_Plugins_Enable
            // 
            this.BTN_Plugins_Enable.Location = new System.Drawing.Point(5, 5);
            this.BTN_Plugins_Enable.Name = "BTN_Plugins_Enable";
            this.BTN_Plugins_Enable.Size = new System.Drawing.Size(75, 23);
            this.BTN_Plugins_Enable.TabIndex = 1;
            this.BTN_Plugins_Enable.Text = "Disable";
            this.BTN_Plugins_Enable.UseVisualStyleBackColor = true;
            this.BTN_Plugins_Enable.Click += new System.EventHandler(this.BTN_Plugins_Enable_Click);
            // 
            // BTN_Plugins_Reload
            // 
            this.BTN_Plugins_Reload.Location = new System.Drawing.Point(222, 3);
            this.BTN_Plugins_Reload.Name = "BTN_Plugins_Reload";
            this.BTN_Plugins_Reload.Size = new System.Drawing.Size(75, 23);
            this.BTN_Plugins_Reload.TabIndex = 0;
            this.BTN_Plugins_Reload.Text = "Reload";
            this.BTN_Plugins_Reload.UseVisualStyleBackColor = true;
            this.BTN_Plugins_Reload.Click += new System.EventHandler(this.BTN_Plugins_Reload_Click);
            // 
            // PG_Plugins
            // 
            this.PG_Plugins.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PG_Plugins.Location = new System.Drawing.Point(0, 0);
            this.PG_Plugins.Name = "PG_Plugins";
            this.PG_Plugins.Size = new System.Drawing.Size(633, 537);
            this.PG_Plugins.TabIndex = 0;
            // 
            // TAB_Statistics
            // 
            this.TAB_Statistics.Controls.Add(this.TB_Statistics);
            this.TAB_Statistics.Location = new System.Drawing.Point(4, 22);
            this.TAB_Statistics.Name = "TAB_Statistics";
            this.TAB_Statistics.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_Statistics.Size = new System.Drawing.Size(943, 543);
            this.TAB_Statistics.TabIndex = 6;
            this.TAB_Statistics.Text = "Statistics";
            this.TAB_Statistics.UseVisualStyleBackColor = true;
            // 
            // TB_Statistics
            // 
            this.TB_Statistics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TB_Statistics.Location = new System.Drawing.Point(3, 3);
            this.TB_Statistics.Multiline = true;
            this.TB_Statistics.Name = "TB_Statistics";
            this.TB_Statistics.Size = new System.Drawing.Size(937, 537);
            this.TB_Statistics.TabIndex = 0;
            // 
            // TAB_Profiler
            // 
            this.TAB_Profiler.Controls.Add(this.CHK_ProfileGrids);
            this.TAB_Profiler.Controls.Add(this.CHK_PauseProfiler);
            this.TAB_Profiler.Controls.Add(this.CHK_ProfileBlocks);
            this.TAB_Profiler.Controls.Add(this.TB_Profiler);
            this.TAB_Profiler.Location = new System.Drawing.Point(4, 22);
            this.TAB_Profiler.Name = "TAB_Profiler";
            this.TAB_Profiler.Padding = new System.Windows.Forms.Padding(3);
            this.TAB_Profiler.Size = new System.Drawing.Size(943, 543);
            this.TAB_Profiler.TabIndex = 7;
            this.TAB_Profiler.Text = "Profiler";
            this.TAB_Profiler.UseVisualStyleBackColor = true;
            // 
            // CHK_ProfileGrids
            // 
            this.CHK_ProfileGrids.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CHK_ProfileGrids.AutoSize = true;
            this.CHK_ProfileGrids.Location = new System.Drawing.Point(104, 518);
            this.CHK_ProfileGrids.Name = "CHK_ProfileGrids";
            this.CHK_ProfileGrids.Size = new System.Drawing.Size(82, 17);
            this.CHK_ProfileGrids.TabIndex = 5;
            this.CHK_ProfileGrids.Text = "Profile Grids";
            this.CHK_ProfileGrids.UseVisualStyleBackColor = true;
            this.CHK_ProfileGrids.CheckedChanged += new System.EventHandler(this.CHK_ProfileGrids_CheckedChanged);
            // 
            // CHK_PauseProfiler
            // 
            this.CHK_PauseProfiler.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CHK_PauseProfiler.AutoSize = true;
            this.CHK_PauseProfiler.Location = new System.Drawing.Point(192, 518);
            this.CHK_PauseProfiler.Name = "CHK_PauseProfiler";
            this.CHK_PauseProfiler.Size = new System.Drawing.Size(56, 17);
            this.CHK_PauseProfiler.TabIndex = 4;
            this.CHK_PauseProfiler.Text = "Pause";
            this.CHK_PauseProfiler.UseVisualStyleBackColor = true;
            this.CHK_PauseProfiler.CheckedChanged += new System.EventHandler(this.CHK_PauseProfiler_CheckedChanged);
            // 
            // CHK_ProfileBlocks
            // 
            this.CHK_ProfileBlocks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CHK_ProfileBlocks.AutoSize = true;
            this.CHK_ProfileBlocks.Location = new System.Drawing.Point(8, 518);
            this.CHK_ProfileBlocks.Name = "CHK_ProfileBlocks";
            this.CHK_ProfileBlocks.Size = new System.Drawing.Size(90, 17);
            this.CHK_ProfileBlocks.TabIndex = 2;
            this.CHK_ProfileBlocks.Text = "Profile Blocks";
            this.CHK_ProfileBlocks.UseVisualStyleBackColor = true;
            this.CHK_ProfileBlocks.CheckedChanged += new System.EventHandler(this.CHK_ProfileBlocks_CheckedChanged);
            // 
            // TB_Profiler
            // 
            this.TB_Profiler.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Profiler.Location = new System.Drawing.Point(3, 3);
            this.TB_Profiler.Multiline = true;
            this.TB_Profiler.Name = "TB_Profiler";
            this.TB_Profiler.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TB_Profiler.Size = new System.Drawing.Size(937, 509);
            this.TB_Profiler.TabIndex = 1;
            // 
            // SS_Bottom
            // 
            this.SS_Bottom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.SS_Bottom.Location = new System.Drawing.Point(0, 3);
            this.SS_Bottom.Name = "SS_Bottom";
            this.SS_Bottom.Size = new System.Drawing.Size(951, 22);
            this.SS_Bottom.TabIndex = 0;
            this.SS_Bottom.Text = "None";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(124, 17);
            this.toolStripStatusLabel1.Text = "Updates Per Second: 0";
            // 
            // SEServerExtender
            // 
            this.AcceptButton = this.BTN_Chat_Send;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(951, 598);
            this.Controls.Add(this.splitContainer1);
            this.Name = "SEServerExtender";
            this.Text = "SEServerExtender";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.TAB_MainTabs.ResumeLayout(false);
            this.TAB_Control_Page.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.GRP_Control_Server.ResumeLayout(false);
            this.TAB_Entities_Page.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
            this.splitContainer5.ResumeLayout(false);
            this.TAB_Chat_Page.ResumeLayout(false);
            this.splitContainer6.Panel1.ResumeLayout(false);
            this.splitContainer6.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).EndInit();
            this.splitContainer6.ResumeLayout(false);
            this.splitContainer8.Panel1.ResumeLayout(false);
            this.splitContainer8.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer8)).EndInit();
            this.splitContainer8.ResumeLayout(false);
            this.splitContainer13.Panel1.ResumeLayout(false);
            this.splitContainer13.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer13)).EndInit();
            this.splitContainer13.ResumeLayout(false);
            this.CMS_Chat.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.splitContainer7.Panel1.ResumeLayout(false);
            this.splitContainer7.Panel1.PerformLayout();
            this.splitContainer7.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).EndInit();
            this.splitContainer7.ResumeLayout(false);
            this.TAB_Factions_Page.ResumeLayout(false);
            this.splitContainer9.Panel1.ResumeLayout(false);
            this.splitContainer9.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer9)).EndInit();
            this.splitContainer9.ResumeLayout(false);
            this.splitContainer10.Panel1.ResumeLayout(false);
            this.splitContainer10.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer10)).EndInit();
            this.splitContainer10.ResumeLayout(false);
            this.TAB_Plugins_Page.ResumeLayout(false);
            this.SC_Plugins.Panel1.ResumeLayout(false);
            this.SC_Plugins.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SC_Plugins)).EndInit();
            this.SC_Plugins.ResumeLayout(false);
            this.splitContainer12.Panel1.ResumeLayout(false);
            this.splitContainer12.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer12)).EndInit();
            this.splitContainer12.ResumeLayout(false);
            this.TAB_Statistics.ResumeLayout(false);
            this.TAB_Statistics.PerformLayout();
            this.TAB_Profiler.ResumeLayout(false);
            this.TAB_Profiler.PerformLayout();
            this.SS_Bottom.ResumeLayout(false);
            this.SS_Bottom.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private SplitContainer splitContainer1;
		private ContextMenuStrip CMS_Chat;
		private ToolStripMenuItem TSM_Kick;
		private ToolStripMenuItem TSM_Ban;
		private StatusStrip SS_Bottom;
		private ToolStripStatusLabel toolStripStatusLabel1;
		private TabControl TAB_MainTabs;
		private TabPage TAB_Control_Page;
		private SplitContainer splitContainer3;
		private Label LBL_Control_AutosaveInterval;
		private ComboBox CMB_Control_AutosaveInterval;
		private Label LBL_Control_Instance;
		internal ComboBox CMB_Control_CommonInstanceList;
		private CheckBox CHK_Control_CommonDataPath;
		private CheckBox CHK_Control_Debugging;
		private Button BTN_ServerControl_Start;
		private Button BTN_ServerControl_Stop;
		private GroupBox GRP_Control_Server;
		private Button BTN_Control_Server_Reset;
		private Button BTN_Control_Server_Save;
		private PropertyGrid PG_Control_Server_Properties;
		private TabPage TAB_Entities_Page;
		private SplitContainer splitContainer2;
		private SplitContainer splitContainer5;
		private ComboBox CB_Entity_Sort;
		private TreeView TRV_Entities;
		private Button btnRepairEntity;
		private Button BTN_Entities_Export;
		private Button BTN_Entities_New;
		private Button BTN_Entities_Delete;
		private PropertyGrid PG_Entities_Details;
		private TabPage TAB_Chat_Page;
		private SplitContainer splitContainer6;
		private SplitContainer splitContainer8;
		private RichTextBox RTB_Chat_Messages;
		private SplitContainer splitContainer13;
		private ListBox LST_Chat_ConnectedPlayers;
		private Panel panel1;
		private Button BTN_Chat_BanSelected;
		private Button BTN_Chat_KickSelected;
		private SplitContainer splitContainer7;
		private TextBox TXT_Chat_Message;
		private Button BTN_Chat_Send;
		private TabPage TAB_Factions_Page;
		private SplitContainer splitContainer9;
		private SplitContainer splitContainer10;
		private TreeView TRV_Factions;
		private Button BTN_Factions_Delete;
		private PropertyGrid PG_Factions;
		private TabPage TAB_Plugins_Page;
		private SplitContainer SC_Plugins;
		private SplitContainer splitContainer12;
		private ListBox LST_Plugins;
		private Button BTN_Plugins_Enable;
		private Button BTN_Plugins_Reload;
		private PropertyGrid PG_Plugins;
		private TabPage TAB_Statistics;
		private TextBox TB_Statistics;
        private TabPage TAB_Profiler;
        private TextBox TB_Profiler;
        private Button BTN_Factions_Demote;
        private Button BTN_Factions_Promote;
        private CheckBox CHK_ProfileBlocks;
        private CheckBox CHK_PauseProfiler;
        private CheckBox CHK_ProfileGrids;
    }
}