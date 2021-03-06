﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Microsoft.Win32;
using pWonders.App.DockLive.TileInterface;

namespace pWonders.App.DockLive
{
	/* TODO:
	 * https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/double-buffered-graphics
	 */
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
			this.pnlScroller.Width = m_FullWidth;
			//this.DoubleBuffered = true;
			this.Font = SystemFonts.StatusFont;
			this.Opacity = 254 / 255.0;
			this.Theme = Properties.Settings.Default.Theme;
			mnuTheme.MenuItems.Cast<MenuItem>().Where(m => m.Text == this.Theme.ToString()).First().Checked = true;
			m_Shown = true;

			m_Animator = new Animator(this);
			m_Animator.ShowEnded += Animator_ShowEnded;
			m_Animator.HideEnded += Animator_HideEnded;

			m_Loader = new TileLoader();
			m_AllSettings = new Dictionary<string, Dictionary<string, string>>();
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
		Dictionary<string, Dictionary<string, string>> m_AllSettings;

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
			int num = 0;
			foreach (ITile tile in m_Loader.Load())
			{
				num++;
				tile.Control.Dock = DockStyle.Fill;
				if (num == 1)
				{
					tile.Control.Margin = new Padding(0, 0, 0, 0);
				}
				else
				{
					tile.Control.Margin = new Padding(0, 2, 0, 0);
				}
				var set = new Dictionary<string, string>();
				SettingsManager.Load(tile.UniqueName, set);
				tile.OnSettingsLoaded(set);
				m_AllSettings.Add(tile.UniqueName, set);
				if (tblTiles.RowStyles.Count <= tblTiles.Controls.Count)
				{
					tblTiles.RowStyles.Add(new RowStyle(SizeType.AutoSize));
				}
				tblTiles.Controls.Add(tile.Control);
				tile.OnAttachTile(this);
				tile.Control.MouseUp += Tile_Control_MouseUp;
			}
			// Add a dummy one to fill up the rest of space.
			tblTiles.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
			tblTiles.Controls.Add(new Label());
			tblTiles.ResumeLayout();
			this.ResumeLayout();
		}

		protected override void OnDeactivate(EventArgs e)
		{
			base.OnDeactivate(e);
			if (this.Tag == null)
			{
				m_Animator.BeginAutoHide();
			}
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
				if (m_Shown)
				{
					int cx = Desktop.ActionCenter.Bounds.Width;
					m_FullWidth = cx > 0 ? cx : Desktop.Screen.WorkingArea.Width / 5;
					this.SetDesktopBounds(rect.Right - m_FullWidth, 0, m_FullWidth, rect.Height);
					m_FullWidth = this.Width;
				}
				else
				{
					this.SetDesktopBounds(this.Left, 0, this.Width, rect.Height);
				}
			}
			// Is this scroll bar width?
			if (e.Category == UserPreferenceCategory.Window || e.Category == UserPreferenceCategory.VisualStyle)
			{
				pnlScroller.Width = m_FullWidth + SystemInformation.VerticalScrollBarWidth;
			}
			// TODO: WM_DWMCOLORIZATIONCOLORCHANGED
		}

		private void Tile_Control_MouseUp(object sender, MouseEventArgs e)
		{
			TileChildControl ctrl = sender as TileChildControl;
			if (e.Button == MouseButtons.Right && ctrl.ClientRectangle.Contains(e.Location))
			{
				// If not already in settings mode.
				if (ctrl.Parent == tblTiles)
				{
					ctrl.Tile.OnSettingsOpened();
					tblTiles.SuspendLayout();
					TableLayoutPanelCellPosition pos = tblTiles.GetPositionFromControl(ctrl);
					SettingsBlock block = new SettingsBlock(ctrl.Tile);
					switch (m_Theme)
					{
					case AppTheme.System:
						block.BackColor = XColor.Rotate(Color.FromArgb(0x7f, UIColor.Accent), -45);
						block.ForeColor = UIColor.Background;
						break;
					case AppTheme.Dark:
					case AppTheme.Light:
						break;
					}
					block.GoBack += SettingsBlock_GoBack;
					block.FillUp += SettingsBlock_FillUp;
					block.FillDn += SettingsBlock_FillDn;
					block.Remove += SettingsBlock_Remove;
					tblTiles.Controls.Remove(ctrl);
					tblTiles.Controls.Add(block, pos.Column, pos.Row);
					tblTiles.ResumeLayout();
				}
			}
		}

		private void SettingsBlock_GoBack(object sender, EventArgs e)
		{
			SettingsBlock block = sender as SettingsBlock;
			tblTiles.SuspendLayout();
			TableLayoutPanelCellPosition pos = tblTiles.GetPositionFromControl(block);
			tblTiles.Controls.Remove(block);
			tblTiles.Controls.Add(block.Tile.Control, pos.Column, pos.Row);
			tblTiles.ResumeLayout();
			block.Tile.OnSettingsClosed();
			block.Tile.OnSettingsWanted(m_AllSettings[block.Tile.UniqueName]);
			SettingsManager.Save(block.Tile.UniqueName, m_AllSettings[block.Tile.UniqueName]);
		}

		private void SettingsBlock_FillUp(object sender, EventArgs e)
		{
			SettingsBlock block = sender as SettingsBlock;
			tblTiles.SuspendLayout();
			TableLayoutPanelCellPosition pos = tblTiles.GetPositionFromControl(block);
			tblTiles.RowStyles[tblTiles.RowStyles.Count - 1].Height = 100f;
			tblTiles.RowStyles[pos.Row].SizeType = SizeType.AutoSize;
			block.Tile.Control.Size = block.Tile.DefaultSize;
			block.Tile.SettingsControl.Size = block.Tile.DefaultSize;
			tblTiles.ResumeLayout();
		}

		private void SettingsBlock_FillDn(object sender, EventArgs e)
		{
			SettingsBlock block = sender as SettingsBlock;
			tblTiles.SuspendLayout();
			TableLayoutPanelCellPosition pos = tblTiles.GetPositionFromControl(block);
			tblTiles.RowStyles[pos.Row].SizeType = SizeType.Percent;
			tblTiles.RowStyles[pos.Row].Height = 100f;
			tblTiles.RowStyles[tblTiles.RowStyles.Count - 1].Height = 0;
			tblTiles.ResumeLayout();
		}

		private void SettingsBlock_Remove(object sender, EventArgs e)
		{
			SettingsBlock block = sender as SettingsBlock;
			tblTiles.SuspendLayout();
			TableLayoutPanelCellPosition pos = tblTiles.GetPositionFromControl(block);
			block.Tile.OnDetachTile();
			tblTiles.Controls.Remove(block);
			tblTiles.RowStyles.RemoveAt(pos.Row);
			tblTiles.ResumeLayout();
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

		private void mnuThemeName_Click(object sender, EventArgs e)
		{
			foreach (MenuItem mnu in mnuTheme.MenuItems)
			{
				if (mnu == sender)
				{
					mnu.Checked = true;
					this.Theme = (AppTheme) Enum.Parse(typeof(AppTheme), mnu.Text);
				}
				else
				{
					mnu.Checked = false;
				}
			}
		}
	}
}
