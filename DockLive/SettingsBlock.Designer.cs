namespace pWonders.App.DockLive
{
	partial class SettingsBlock
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblSettings = new System.Windows.Forms.Label();
			this.btnBack = new System.Windows.Forms.Label();
			this.pnlMain = new System.Windows.Forms.Panel();
			this.lblTileName = new System.Windows.Forms.Label();
			this.tblHeader = new System.Windows.Forms.VTableLayoutPanel();
			this.tblHeader.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblSettings
			// 
			this.lblSettings.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblSettings.Image = global::pWonders.App.DockLive.Properties.Resources.VSO_Settings_whiteNoHalo_16x;
			this.lblSettings.Location = new System.Drawing.Point(525, 0);
			this.lblSettings.Name = "lblSettings";
			this.lblSettings.Size = new System.Drawing.Size(32, 32);
			this.lblSettings.TabIndex = 1;
			// 
			// btnBack
			// 
			this.btnBack.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnBack.Location = new System.Drawing.Point(3, 0);
			this.btnBack.Name = "btnBack";
			this.btnBack.Size = new System.Drawing.Size(32, 32);
			this.btnBack.TabIndex = 0;
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			this.btnBack.Paint += new System.Windows.Forms.PaintEventHandler(this.btnBack_Paint);
			this.btnBack.DoubleClick += new System.EventHandler(this.btnBack_Click);
			// 
			// pnlMain
			// 
			this.pnlMain.BackColor = System.Drawing.Color.Transparent;
			this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMain.Location = new System.Drawing.Point(0, 32);
			this.pnlMain.Name = "pnlMain";
			this.pnlMain.Padding = new System.Windows.Forms.Padding(8);
			this.pnlMain.Size = new System.Drawing.Size(560, 292);
			this.pnlMain.TabIndex = 0;
			// 
			// lblTileName
			// 
			this.lblTileName.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblTileName.AutoSize = true;
			this.lblTileName.Location = new System.Drawing.Point(41, 2);
			this.lblTileName.Name = "lblTileName";
			this.lblTileName.Size = new System.Drawing.Size(0, 28);
			this.lblTileName.TabIndex = 2;
			this.lblTileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblTileName.UseCompatibleTextRendering = true;
			// 
			// tblHeader
			// 
			this.tblHeader.AutoSize = true;
			this.tblHeader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tblHeader.BackColor = System.Drawing.Color.Transparent;
			this.tblHeader.ColumnCount = 3;
			this.tblHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tblHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tblHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tblHeader.Controls.Add(this.lblSettings, 2, 0);
			this.tblHeader.Controls.Add(this.lblTileName, 1, 0);
			this.tblHeader.Controls.Add(this.btnBack, 0, 0);
			this.tblHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.tblHeader.FixTransparency = true;
			this.tblHeader.Location = new System.Drawing.Point(0, 0);
			this.tblHeader.Name = "tblHeader";
			this.tblHeader.RowCount = 1;
			this.tblHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tblHeader.Size = new System.Drawing.Size(560, 32);
			this.tblHeader.TabIndex = 1;
			// 
			// SettingsBlock
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(168F, 168F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.pnlMain);
			this.Controls.Add(this.tblHeader);
			this.Name = "SettingsBlock";
			this.Size = new System.Drawing.Size(560, 324);
			this.tblHeader.ResumeLayout(false);
			this.tblHeader.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label lblSettings;
		private System.Windows.Forms.Label btnBack;
		private System.Windows.Forms.Panel pnlMain;
		private System.Windows.Forms.Label lblTileName;
		private System.Windows.Forms.VTableLayoutPanel tblHeader;
	}
}
