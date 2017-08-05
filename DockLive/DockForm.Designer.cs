namespace pWonders.App.DockLive
{
	partial class DockForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.mnuSettings = new System.Windows.Forms.MenuItem();
			this.mnuSep = new System.Windows.Forms.MenuItem();
			this.mnuExit = new System.Windows.Forms.MenuItem();
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.pnlScroller = new System.Windows.Forms.Panel();
			this.tblTiles = new System.Windows.Forms.TableLayoutPanel();
			this.pnlScroller.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuSettings,
            this.mnuSep,
            this.mnuExit});
			this.contextMenu.Popup += new System.EventHandler(this.contextMenu_Popup);
			// 
			// mnuSettings
			// 
			this.mnuSettings.Index = 0;
			this.mnuSettings.Text = "Settings...";
			this.mnuSettings.Click += new System.EventHandler(this.mnuSettings_Click);
			// 
			// mnuSep
			// 
			this.mnuSep.Index = 1;
			this.mnuSep.Text = "-";
			// 
			// mnuExit
			// 
			this.mnuExit.Index = 2;
			this.mnuExit.Text = "E&xit";
			this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
			// 
			// notifyIcon
			// 
			this.notifyIcon.ContextMenu = this.contextMenu;
			this.notifyIcon.Visible = true;
			this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
			this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
			// 
			// pnlScroller
			// 
			this.pnlScroller.AutoScroll = true;
			this.pnlScroller.BackColor = System.Drawing.Color.Transparent;
			this.pnlScroller.Controls.Add(this.tblTiles);
			this.pnlScroller.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlScroller.Location = new System.Drawing.Point(0, 0);
			this.pnlScroller.Name = "pnlScroller";
			this.pnlScroller.Size = new System.Drawing.Size(180, 800);
			this.pnlScroller.TabIndex = 1;
			this.pnlScroller.ClientSizeChanged += new System.EventHandler(this.pnlScroller_ClientSizeChanged);
			// 
			// tblTiles
			// 
			this.tblTiles.AutoSize = true;
			this.tblTiles.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tblTiles.ColumnCount = 1;
			this.tblTiles.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tblTiles.Dock = System.Windows.Forms.DockStyle.Top;
			this.tblTiles.Location = new System.Drawing.Point(0, 0);
			this.tblTiles.Name = "tblTiles";
			this.tblTiles.RowCount = 1;
			this.tblTiles.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tblTiles.Size = new System.Drawing.Size(180, 0);
			this.tblTiles.TabIndex = 1;
			// 
			// DockForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(168F, 168F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(400, 800);
			this.Controls.Add(this.pnlScroller);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Margin = new System.Windows.Forms.Padding(6);
			this.Name = "DockForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.TopMost = true;
			this.pnlScroller.ResumeLayout(false);
			this.pnlScroller.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.MenuItem mnuExit;
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.MenuItem mnuSettings;
		private System.Windows.Forms.MenuItem mnuSep;
		private System.Windows.Forms.Panel pnlScroller;
		private System.Windows.Forms.TableLayoutPanel tblTiles;
	}
}

