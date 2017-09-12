using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace pWonders.App.DockLive.TileInterface
{
	[System.ComponentModel.DesignerCategory("")]
	[DefaultEvent("FilePicked")]
	public class FilePicker : Panel
	{
		public event EventHandler FilePicked = delegate { };

		public FilePicker()
		{
			openFileDialog = new OpenFileDialog();
			btnPick = new Button();
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
			this.btnPick.Image = global::pWonders.App.DockLive.TileInterface.Properties.Resources.OpenFolder_32x;
			this.btnPick.Location = new System.Drawing.Point(200 - 40 - 3, 3);
			this.btnPick.Name = "btnPick";
			this.btnPick.Size = new System.Drawing.Size(40, 40);
			this.btnPick.TabIndex = 2;
			this.btnPick.UseVisualStyleBackColor = true;
			this.btnPick.Click += new System.EventHandler(this.btnPick_Click);
			// 
			// FilePicker
			// 
			this.ClientSize = new System.Drawing.Size(200, 40 + 3 + 3);
			this.Controls.Add(this.btnPick);

			m_File = string.Empty;
		}

		[Browsable(false)]
		public string File
		{
			set
			{
				if (m_File != value)
				{
					m_File = value;
					m_FileDisplayName = get_display_name(m_File);
					this.Invalidate();
					OnFilePicked(EventArgs.Empty);
				}
			}
			get { return m_File; }
		}

		Button btnPick;
		OpenFileDialog openFileDialog;
		string m_File, m_FileDisplayName;

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			Rectangle rect_bg = this.ClientRectangle, rect_fg = rect_bg;
			rect_fg.Width -= (btnPick.Width + btnPick.Margin.Horizontal);
			using (Bitmap bmp = new Bitmap(rect_bg.Width, rect_bg.Height))
			using (Graphics g = Graphics.FromImage(bmp))
			using (Brush br_bg = new SolidBrush(this.BackColor))
			{
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
				g.FillRectangle(br_bg, e.ClipRectangle);
				TextFormatFlags format = TextFormatFlags.NoPrefix | TextFormatFlags.PathEllipsis | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.TextBoxControl;
				// Need to specify backcolor or the antialiasing will look poor.
				TextRenderer.DrawText(g, m_FileDisplayName, this.Font, rect_fg, this.ForeColor, this.BackColor, format);
				e.Graphics.DrawImage(bmp, Point.Empty);
			}
		}

		protected virtual void OnFilePicked(EventArgs e)
		{
			FilePicked(this, e);
		}

		private void btnPick_Click(object sender, EventArgs e)
		{
			Form form = this.FindForm();
			if (form != null)
			{
				form.Tag = openFileDialog;
				if (openFileDialog.ShowDialog(form) == DialogResult.OK)
				{
					this.File = Path.GetDirectoryName(openFileDialog.FileName);
				}
				form.Tag = null;
			}
		}

		string get_display_name(string path)
		{
			API.SHFILEINFO shfi;
			if (API.SHGetFileInfo(path, API.FILE_ATTRIBUTE_NORMAL, out shfi, Marshal.SizeOf(typeof(API.SHFILEINFO)), API.SHGFI_DISPLAYNAME) != 0)
			{
				if (path.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
				{
					path = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar + shfi.szDisplayName;
				}
				else
				{
					path = shfi.szDisplayName;
				}
			}
			return path;
		}
	}
}
