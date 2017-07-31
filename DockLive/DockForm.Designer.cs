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
			this.tbl = new System.Windows.Forms.TableLayoutPanel();
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
			this.notifyIcon.Icon = global::pWonders.App.DockLive.Properties.Resources.icon_hide_16;
			this.notifyIcon.Visible = true;
			this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
			this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
			// 
			// pnlScroller
			// 
			this.pnlScroller.AutoScroll = true;
			this.pnlScroller.Controls.Add(this.tbl);
			this.pnlScroller.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlScroller.Location = new System.Drawing.Point(0, 0);
			this.pnlScroller.Name = "pnlScroller";
			this.pnlScroller.Size = new System.Drawing.Size(180, 800);
			this.pnlScroller.TabIndex = 1;
			// 
			// tbl
			// 
			this.tbl.AutoSize = true;
			this.tbl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tbl.ColumnCount = 1;
			this.tbl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tbl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tbl.Location = new System.Drawing.Point(0, 0);
			this.tbl.Name = "tbl";
			this.tbl.RowCount = 1;
			this.tbl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tbl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tbl.Size = new System.Drawing.Size(0, 0);
			this.tbl.TabIndex = 1;
			// 
			// DockForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(168F, 168F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(400, 800);
			this.Controls.Add(this.pnlScroller);
			this.DoubleBuffered = true;
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
		private System.Windows.Forms.TableLayoutPanel tbl;
	}
}

