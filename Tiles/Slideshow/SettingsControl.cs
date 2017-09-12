using System;
using System.Drawing;
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
				filePicker.File = value;
			}
			get { return m_ImageFolder; }
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

		readonly int[] STAYFOROPTIONS = { 2, 5, 10, 20, 40, 60 * 2 };
		string m_ImageFolder;
		int m_StayForSecond;
		FitMode m_FitMode;

		protected override void OnThemeChanged(EventArgs e)
		{
			base.OnThemeChanged(e);
			switch (base.Theme)
			{
			case AppTheme.System:
				filePicker.BackColor = Color.FromArgb(0xfe, 0xbf, 0xbf, 0xbf);
				filePicker.ForeColor = Color.FromArgb(0x3f, 0x3f, 0x3f);
				break;
			case AppTheme.Dark:
			case AppTheme.Light:
				break;
			}
			this.Invalidate();
		}

		private void lblStayOn_Click(object sender, EventArgs e)
		{
			int i = 0;
			for (; i < STAYFOROPTIONS.Length; i++)
			{
				if (this.StayForSecond == STAYFOROPTIONS[i])
				{
					this.StayForSecond = STAYFOROPTIONS[(i + 1) % STAYFOROPTIONS.Length];
					break;
				}
			}
			if (i == STAYFOROPTIONS.Length)
			{
				this.StayForSecond = STAYFOROPTIONS[0];
			}
		}

		private void lblFitMode_Click(object sender, EventArgs e)
		{
			this.FitMode = (FitMode) (((int) (this.FitMode + 1)) % Enum.GetValues(typeof(FitMode)).Length);
		}

		private void filePicker_FilePicked(object sender, EventArgs e)
		{
			m_ImageFolder = filePicker.File;
		}
	}
}
