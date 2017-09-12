namespace pWonders.App.DockLive.Tiles.Slideshow
{
	partial class SettingsControl
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
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.lblStayOn = new pWonders.App.DockLive.TileInterface.OptionLabel();
			this.lblFitMode = new pWonders.App.DockLive.TileInterface.OptionLabel();
			this.filePicker = new pWonders.App.DockLive.TileInterface.FilePicker();
			this.SuspendLayout();
			// 
			// openFileDialog
			// 
			this.openFileDialog.DereferenceLinks = false;
			this.openFileDialog.RestoreDirectory = true;
			this.openFileDialog.SupportMultiDottedExtensions = true;
			// 
			// lblStayOn
			// 
			this.lblStayOn.AutoSize = true;
			this.lblStayOn.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblStayOn.Location = new System.Drawing.Point(0, 46);
			this.lblStayOn.Name = "lblStayOn";
			this.lblStayOn.Padding = new System.Windows.Forms.Padding(2);
			this.lblStayOn.Size = new System.Drawing.Size(76, 31);
			this.lblStayOn.TabIndex = 5;
			this.lblStayOn.Text = "StayOn";
			this.lblStayOn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblStayOn.UseCompatibleTextRendering = true;
			this.lblStayOn.Click += new System.EventHandler(this.lblStayOn_Click);
			this.lblStayOn.DoubleClick += new System.EventHandler(this.lblStayOn_Click);
			// 
			// lblFitMode
			// 
			this.lblFitMode.AutoSize = true;
			this.lblFitMode.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblFitMode.Location = new System.Drawing.Point(0, 77);
			this.lblFitMode.Name = "lblFitMode";
			this.lblFitMode.Padding = new System.Windows.Forms.Padding(2);
			this.lblFitMode.Size = new System.Drawing.Size(82, 31);
			this.lblFitMode.TabIndex = 6;
			this.lblFitMode.Text = "FitMode";
			this.lblFitMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblFitMode.UseCompatibleTextRendering = true;
			this.lblFitMode.Click += new System.EventHandler(this.lblFitMode_Click);
			this.lblFitMode.DoubleClick += new System.EventHandler(this.lblFitMode_Click);
			// 
			// filePicker
			// 
			this.filePicker.Dock = System.Windows.Forms.DockStyle.Top;
			this.filePicker.Location = new System.Drawing.Point(0, 0);
			this.filePicker.Name = "filePicker";
			this.filePicker.Size = new System.Drawing.Size(252, 46);
			this.filePicker.TabIndex = 7;
			this.filePicker.FilePicked += new System.EventHandler(this.filePicker_FilePicked);
			// 
			// SettingsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(168F, 168F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.lblFitMode);
			this.Controls.Add(this.lblStayOn);
			this.Controls.Add(this.filePicker);
			this.Name = "SettingsControl";
			this.Size = new System.Drawing.Size(252, 137);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private pWonders.App.DockLive.TileInterface.OptionLabel lblStayOn;
		private pWonders.App.DockLive.TileInterface.OptionLabel lblFitMode;
		private TileInterface.FilePicker filePicker;
	}
}
