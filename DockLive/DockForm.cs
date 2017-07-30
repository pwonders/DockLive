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
			// Save the screen; the main form will be on this screen for the lifetime.
			Desktop.SetCurrentScreen();
			int cx = Desktop.ActionCenter.Width;
			m_FullWidth = cx > 0 ? cx : Desktop.Screen.WorkingArea.Width / 5;

			InitializeComponent();
			
			this.Font = SystemFonts.StatusFont;
			this.Opacity = 0.9961;
			this.Theme = AppTheme.System;
			this.Width = m_FullWidth;

			this.CreateControl();

			SystemEvents.UserPreferenceChanging += SystemEvents_UserPreferenceChanging;
			SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;

			pnlScroller.Width = m_FullWidth + SystemInformation.VerticalScrollBarWidth;

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
						if (UIColor.IsValid)
						{
							this.BackColor = UIColor.Accent;
							this.ForeColor = UIColor.Foreground;
							this.BlurColor = UIColor.FromName("ImmersiveSystemAccentDark2");
						}
						else
						{
							this.BackColor = SystemColors.Window;
							this.ForeColor = SystemColors.WindowText;
						}
						break;
					}
					//
				}
			}
			get { return m_Theme; }
		}

		int m_FullWidth;
		AppTheme m_Theme;
		Animator m_Animator;
		TileLoader m_Loader;
		bool m_FirstShow;

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if (m_FirstShow == false)
			{
				this.Visible = false;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Rectangle rect = Desktop.Screen.WorkingArea;
			this.SetDesktopBounds(rect.Right, rect.Top, 0, rect.Height);

			this.SuspendLayout();
			tbl.SuspendLayout();
			tbl.Dock = DockStyle.Fill;
			foreach (ITile tile in m_Loader.Load())
			{
				tile.Control.Margin = Padding.Empty;
				tbl.Controls.Add(tile.Control);
				tile.OnAttachTile(this);
			}
			foreach (RowStyle rs in tbl.RowStyles)
			{
				rs.SizeType = SizeType.Percent;
				rs.Height = 1.0f / tbl.RowCount;
			}
			tbl.ResumeLayout();
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
			Form f = new Form();
			Label l;
			l = new Label();
			l.Dock = DockStyle.Top;
			l.Text = "ImmersiveSystemAccentDark2";
			l.BackColor = UIColor.FromName(l.Text);
			f.Controls.Add(l);
			l = new Label();
			l.Dock = DockStyle.Top;
			l.BackColor = UIColor.Accent;
			l.Text = "Accent";
			f.Controls.Add(l);
			l = new Label();
			l.Dock = DockStyle.Top;
			l.BackColor = UIColor.AccentDark1;
			l.Text = "AccentDark1";
			f.Controls.Add(l);
			l = new Label();
			l.Dock = DockStyle.Top;
			l.BackColor = UIColor.AccentDark2;
			l.Text = "AccentDark2";
			f.Controls.Add(l);
			l = new Label();
			l.Dock = DockStyle.Top;
			l.BackColor = UIColor.AccentDark3;
			l.Text = "AccentDark3";
			f.Controls.Add(l);
			l = new Label();
			l.Dock = DockStyle.Top;
			l.BackColor = UIColor.AccentLight1;
			l.Text = "AccentLight1";
			f.Controls.Add(l);
			l = new Label();
			l.Dock = DockStyle.Top;
			l.BackColor = UIColor.AccentLight2;
			l.Text = "AccentLight2";
			f.Controls.Add(l);
			l = new Label();
			l.Dock = DockStyle.Top;
			l.BackColor = UIColor.AccentLight3;
			l.Text = "AccentLight3";
			f.Controls.Add(l);
			l = new Label();
			l.Dock = DockStyle.Top;
			l.BackColor = UIColor.Background;
			l.Text = "Background";
			f.Controls.Add(l);
			l = new Label();
			l.Dock = DockStyle.Top;
			l.BackColor = UIColor.Complement;
			l.Text = "Complement";
			f.Controls.Add(l);
			l = new Label();
			l.Dock = DockStyle.Top;
			l.BackColor = UIColor.Foreground;
			l.Text = "Foreground";
			f.Controls.Add(l);
			f.AutoSize = true;
			f.Show();
		}

		private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (this.Visible)
				{
					m_Animator.BeginAutoHide();
				}
				else
				{
					m_FirstShow = true;
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
			notifyIcon.Icon = global::pWonders.App.DockLive.Properties.Resources.trayicon_hidden;
		}

		private void Animator_ShowEnded(object sender, EventArgs e)
		{
			notifyIcon.Icon = global::pWonders.App.DockLive.Properties.Resources.trayicon_shown;
		}
	}
}
