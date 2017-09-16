using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using pWonders.App.DockLive.TileInterface;

namespace pWonders.App.DockLive.Tiles.Slideshow
{
	public class DockLiveTile : ITile
	{
		public DockLiveTile()
		{
			m_Control = new SlideshowControl(this);
			m_SettingsControl = new SettingsControl(this);
			m_UniqueName = SingleInstance.GetUniqueName(Assembly.GetExecutingAssembly());
		}

		public void OnAttachTile(ITileHost host)
		{
			this.Host = host;
			OnThemeChanged(host.Theme);
			m_Control.Size = new Size(host.FullBounds.Width - m_Control.Margin.Horizontal, host.FullBounds.Width / 2 - m_Control.Margin.Vertical);
		}

		public void OnDetachTile()
		{
		}

		public void OnThemeChanged(AppTheme theme)
		{
			m_Control.Theme = theme;
			m_SettingsControl.Theme = theme;
		}

		public void OnSettingsOpened()
		{
			m_Control.PauseSlideShow();
			m_SettingsControl.ImageFolder = m_Control.ImagePath;
			m_SettingsControl.StayForSecond = m_Control.StayForSecond;
			m_SettingsControl.FitMode = m_Control.FitMode;
		}

		public void OnSettingsClosed()
		{
			m_Control.ImagePath = m_SettingsControl.ImageFolder;
			m_Control.StayForSecond = m_SettingsControl.StayForSecond;
			m_Control.FitMode = m_SettingsControl.FitMode;
			m_Control.ResumeSlideShow();
		}

		public ITileHost Host { set; get; }

		public string Name
		{
			get { return "Slideshow"; }
		}

		public string UniqueName
		{
			get { return m_UniqueName; }
		}

		public string Version
		{
			get { return Application.ProductVersion; }
		}

		public string Developer
		{
			get { return Application.CompanyName; }
		}

		public TileChildControl Control
		{
			get { return m_Control; }
		}

		public TileChildControl SettingsControl
		{
			get { return m_SettingsControl; }
		}
		
		SlideshowControl m_Control;
		SettingsControl m_SettingsControl;
		string m_UniqueName;
	}
}
