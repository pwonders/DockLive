namespace pWonders.App.DockLive.Tiles.Calendar
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
			this.lblShowAlt = new pWonders.App.DockLive.TileInterface.OptionLabel();
			this.SuspendLayout();
			// 
			// lblShowAlt
			// 
			this.lblShowAlt.AutoSize = true;
			this.lblShowAlt.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblShowAlt.Location = new System.Drawing.Point(0, 0);
			this.lblShowAlt.Name = "lblShowAlt";
			this.lblShowAlt.Padding = new System.Windows.Forms.Padding(2);
			this.lblShowAlt.Size = new System.Drawing.Size(222, 31);
			this.lblShowAlt.TabIndex = 5;
			this.lblShowAlt.Text = "Show alternate calendar";
			this.lblShowAlt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblShowAlt.UseCompatibleTextRendering = true;
			this.lblShowAlt.Click += new System.EventHandler(this.lblShowAlt_Click);
			this.lblShowAlt.DoubleClick += new System.EventHandler(this.lblShowAlt_Click);
			// 
			// SettingsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(168F, 168F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.lblShowAlt);
			this.Name = "SettingsControl";
			this.Size = new System.Drawing.Size(252, 137);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private pWonders.App.DockLive.TileInterface.OptionLabel lblShowAlt;
	}
}
