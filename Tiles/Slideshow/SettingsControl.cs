using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using pWonders.App.DockLive.TileInterface;

namespace pWonders.App.DockLive.Tiles.Slideshow
{
	public partial class SettingsControl : TileChildControl
	{
		public SettingsControl(ITile tile) : base(tile)
		{
			this.DoubleBuffered = true;
			InitializeComponent();
		}

		public string ImageFolder
		{
			set
			{
				if (m_ImageFolder != value)
				{
					m_ImageFolder = value;
					this.ImageFolderDisplayName = get_display_name(m_ImageFolder);
				}
			}
			get { return m_ImageFolder; }
		}

		public string ImageFolderDisplayName
		{
			set
			{
				if (m_ImageFolderDisplayName != value)
				{
					m_ImageFolderDisplayName = value;
					pnlPick.Invalidate();
				}
			}
			get { return m_ImageFolderDisplayName; }
		}

		public int StayForSecond
		{
			set
			{
				if (m_StayForSecond != value)
				{
					m_StayForSecond = value;
					string val = m_StayForSecond < 60 ? m_StayForSecond + " seconds" : (m_StayForSecond / 60) + " minutes";
					lblStayOn.Text = "Change picture every " + val;
				}
			}
			get { return m_StayForSecond; }
		}

		public FitMode FitMode
		{
			set
			{
				if (m_FitMode != value)
				{
					m_FitMode = value;
					lblFitMode.Text = m_FitMode + " picture";
				}
			}
			get { return m_FitMode; }
		}

		readonly int[] m_StayForOptions = { 2, 5, 10, 20, 40, 60 * 2 };
		string m_ImageFolder, m_ImageFolderDisplayName;
		int m_StayForSecond;
		FitMode m_FitMode;
		Color m_TextBackColor, m_TextForeColor;

		protected override void OnThemeChanged(EventArgs e)
		{
			base.OnThemeChanged(e);
			switch (base.Theme)
			{
			case AppTheme.System:
				m_TextBackColor = Color.FromArgb(0xbf, 0xbf, 0xbf);
				m_TextForeColor = Color.FromArgb(0x3f, 0x3f, 0x3f);
				break;
			case AppTheme.Dark:
			case AppTheme.Light:
				break;
			}
			this.Invalidate();
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			Font font = new Font(this.Font, FontStyle.Underline);
			if (lblStayOn != null)
			{
				lblStayOn.Font = font;
			}
			if (lblFitMode != null)
			{
				lblFitMode.Font = font;
			}
		}

		private void btnPick_Click(object sender, EventArgs e)
		{
			this.ParentForm.Tag = openFileDialog;
			if (openFileDialog.ShowDialog(this.ParentForm) == DialogResult.OK)
			{
				this.ImageFolder = Path.GetDirectoryName(openFileDialog.FileName);
			}
			this.ParentForm.Tag = null;
		}

		private void lblStayOn_Click(object sender, EventArgs e)
		{
			int i = 0;
			for (; i < m_StayForOptions.Length; i++)
			{
				if (this.StayForSecond == m_StayForOptions[i])
				{
					this.StayForSecond = m_StayForOptions[(i + 1) % m_StayForOptions.Length];
					break;
				}
			}
			if (i == m_StayForOptions.Length)
			{
				this.StayForSecond = m_StayForOptions[0];
			}
		}

		private void lblFitMode_Click(object sender, EventArgs e)
		{
			this.FitMode = (FitMode) (((int) (this.FitMode + 1)) % Enum.GetValues(typeof(FitMode)).Length);
		}

		private void pnlPick_Paint(object sender, PaintEventArgs e)
		{
			Control ctrl = sender as Control;
			using (Brush br_bg = new SolidBrush(Color.FromArgb(0xfe, m_TextBackColor)))
			{
				e.Graphics.FillRectangle(br_bg, e.ClipRectangle);
				TextFormatFlags format = TextFormatFlags.PathEllipsis | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.TextBoxControl;
				TextRenderer.DrawText(e.Graphics, m_ImageFolderDisplayName, this.Font, ctrl.ClientRectangle, m_TextForeColor, format);
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
