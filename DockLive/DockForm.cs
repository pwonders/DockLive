using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using pWonders.App.DockLive.TileInterface;

namespace pWonders.App.DockLive
{
	[System.ComponentModel.DesignerCategory("Form")]
	public partial class DockForm : VForm, ITileHost
	{
		public DockForm()
		{
			SystemEvents.UserPreferenceChanging += SystemEvents_UserPreferenceChanging;
			SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;

			// Save the screen; the main form will be on this screen for the lifetime.
			Desktop.SetCurrentScreen();
			int cx = Desktop.ActionCenter.Bounds.Width;
			m_FullWidth = cx > 0 ? cx : Desktop.Screen.WorkingArea.Width / 5;

			InitializeComponent();

			this.notifyIcon.Icon = global::pWonders.App.DockLive.Properties.Resources.icon_show_16;
			this.pnlScroller.Width = m_FullWidth + SystemInformation.VerticalScrollBarWidth;
			//this.DoubleBuffered = true;
			this.Font = SystemFonts.StatusFont;
			this.Opacity = 254 / 255.0;
			this.Theme = AppTheme.System;
			m_Shown = true;

			m_Animator = new Animator(this);
			m_Animator.ShowEnded += Animator_ShowEnded;
			m_Animator.HideEnded += Animator_HideEnded;

			m_Loader = new TileLoader();
		}

		[Browsable(false)]
		public Rectangle FullBounds
		{
			get
			{
				Rectangle bounds = Desktop.Screen.WorkingArea;
				bounds.Width = m_FullWidth;
				bounds.X = bounds.Right - bounds.Width;
				return bounds;
			}
		}

		[Browsable(false)]
		public int FullWidth
		{
			get { return m_FullWidth; }
		}

		// BUG: this property is invisible even when Browsable is true.
		[Browsable(true)]
		[DefaultValue(AppTheme.System)]
		public AppTheme Theme
		{
			set
			{
				if (m_Theme != value)
				{
					m_Theme = value;
					switch (m_Theme)
					{
					case AppTheme.Dark:
						break;
					case AppTheme.Light:
						break;
					case AppTheme.System:
						this.BlurBorder = AccentBorder.Left;
						this.BlurColor = UIColor.ShellWithTransparency;
						this.ForeColor = UIColor.Foreground;
						this.BlurWin10 = true;
						break;
					}
				}
			}
			get { return m_Theme; }
		}

		bool m_Shown;
		int m_FullWidth;
		AppTheme m_Theme;
		Animator m_Animator;
		TileLoader m_Loader;

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= API.WS_EX_TOOLWINDOW;
				return cp;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Rectangle rect = Desktop.Screen.WorkingArea;
			this.SetDesktopBounds(rect.Right - m_FullWidth, 0, m_FullWidth, rect.Height);

			this.SuspendLayout();
			tblTiles.SuspendLayout();
			tblTiles.Dock = DockStyle.Fill;
			foreach (ITile tile in m_Loader.Load())
			{
				tile.Control.Margin = Padding.Empty;
				tblTiles.Controls.Add(tile.Control);
				tile.OnAttachTile(this);
			}
			/*
			foreach (RowStyle rs in tblTiles.RowStyles)
			{
				rs.SizeType = SizeType.Percent;
				rs.Height = 1.0f / tblTiles.RowCount;
			}
			*/
			tblTiles.ResumeLayout();
			this.ResumeLayout();
		}

		protected override void OnDeactivate(EventArgs e)
		{
			base.OnDeactivate(e);
			m_Animator.BeginAutoHide();
		}

		protected override void OnClosed(EventArgs e)
		{
			Properties.Settings.Default.Save();
			base.OnClosed(e);
		}

		void SystemEvents_UserPreferenceChanging(object sender, UserPreferenceChangingEventArgs e)
		{
			Debug.WriteLine("UserPreferenceChanging: " + e.Category, this.Name);
		}

		void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
		{
			// Maybe dpi or taskbar position has changed.
			if (e.Category == UserPreferenceCategory.Desktop)
			{
				Rectangle rect = Desktop.Screen.WorkingArea;
				if (this.Visible)
				{
					int cx = Desktop.ActionCenter.Bounds.Width;
					m_FullWidth = cx > 0 ? cx : Desktop.Screen.WorkingArea.Width / 5;
					this.SetDesktopBounds(rect.Right - m_FullWidth, rect.Top, m_FullWidth, rect.Height);
					m_FullWidth = this.Width;
				}
				else
				{
					this.SetDesktopBounds(rect.Right - this.Width, rect.Top, this.Width, rect.Height);
				}
			}
			// Is this scroll bar width?
			if (e.Category == UserPreferenceCategory.Window || e.Category == UserPreferenceCategory.VisualStyle)
			{
				pnlScroller.Width = m_FullWidth + SystemInformation.VerticalScrollBarWidth;
			}
			// TODO: WM_DWMCOLORIZATIONCOLORCHANGED
		}

		private void mnuExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void mnuSettings_Click(object sender, EventArgs e)
		{
		}

		private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (m_Shown)
				{
					m_Animator.BeginAutoHide();
				}
				else
				{
					m_Animator.BeginAutoShow();
				}
			}
		}

		private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			notifyIcon_MouseClick(sender, e);
		}

		private void contextMenu_Popup(object sender, EventArgs e)
		{
		}

		private void Animator_HideEnded(object sender, EventArgs e)
		{
			notifyIcon.Icon = global::pWonders.App.DockLive.Properties.Resources.icon_hide_16;
			m_Shown = false;
		}

		private void Animator_ShowEnded(object sender, EventArgs e)
		{
			notifyIcon.Icon = global::pWonders.App.DockLive.Properties.Resources.icon_show_16;
			m_Shown = true;
			// https://blogs.msdn.microsoft.com/oldnewthing/20080801-00/?p=21393/
			this.Activate();
		}
	}
}
