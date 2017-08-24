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
			this.lblPick = new System.Windows.Forms.Label();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.btnPick = new System.Windows.Forms.Button();
			this.pnlPick = new System.Windows.Forms.Panel();
			this.lblStayOn = new System.Windows.Forms.Label();
			this.lblSpacer = new System.Windows.Forms.Label();
			this.pnlPick.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblPick
			// 
			this.lblPick.AutoSize = true;
			this.lblPick.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblPick.Location = new System.Drawing.Point(0, 0);
			this.lblPick.Name = "lblPick";
			this.lblPick.Size = new System.Drawing.Size(184, 27);
			this.lblPick.TabIndex = 0;
			this.lblPick.Text = "Pick a picture folder:";
			this.lblPick.UseCompatibleTextRendering = true;
			// 
			// openFileDialog
			// 
			this.openFileDialog.DereferenceLinks = false;
			this.openFileDialog.RestoreDirectory = true;
			this.openFileDialog.SupportMultiDottedExtensions = true;
			// 
			// btnPick
			// 
			this.btnPick.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnPick.Image = global::pWonders.App.DockLive.Tiles.Slideshow.Properties.Resources.OpenFolder_32x;
			this.btnPick.Location = new System.Drawing.Point(209, 3);
			this.btnPick.Name = "btnPick";
			this.btnPick.Size = new System.Drawing.Size(40, 40);
			this.btnPick.TabIndex = 2;
			this.btnPick.UseVisualStyleBackColor = true;
			this.btnPick.Click += new System.EventHandler(this.btnPick_Click);
			// 
			// pnlPick
			// 
			this.pnlPick.AutoSize = true;
			this.pnlPick.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.pnlPick.Controls.Add(this.btnPick);
			this.pnlPick.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlPick.Location = new System.Drawing.Point(0, 27);
			this.pnlPick.Name = "pnlPick";
			this.pnlPick.Size = new System.Drawing.Size(252, 46);
			this.pnlPick.TabIndex = 4;
			this.pnlPick.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlPick_Paint);
			// 
			// lblStayOn
			// 
			this.lblStayOn.AutoSize = true;
			this.lblStayOn.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblStayOn.Location = new System.Drawing.Point(0, 89);
			this.lblStayOn.Name = "lblStayOn";
			this.lblStayOn.Size = new System.Drawing.Size(192, 27);
			this.lblStayOn.TabIndex = 5;
			this.lblStayOn.Text = "Change picture every";
			this.lblStayOn.UseCompatibleTextRendering = true;
			this.lblStayOn.Click += new System.EventHandler(this.lblStayOn_Click);
			// 
			// lblSpacer
			// 
			this.lblSpacer.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblSpacer.Location = new System.Drawing.Point(0, 73);
			this.lblSpacer.Name = "lblSpacer";
			this.lblSpacer.Size = new System.Drawing.Size(252, 16);
			this.lblSpacer.TabIndex = 6;
			// 
			// SettingsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(168F, 168F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.lblStayOn);
			this.Controls.Add(this.lblSpacer);
			this.Controls.Add(this.pnlPick);
			this.Controls.Add(this.lblPick);
			this.Name = "SettingsControl";
			this.Size = new System.Drawing.Size(252, 137);
			this.pnlPick.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblPick;
		private System.Windows.Forms.Button btnPick;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Panel pnlPick;
		private System.Windows.Forms.Label lblStayOn;
		private System.Windows.Forms.Label lblSpacer;
	}
}
