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

		public void OnSettingsLoaded(IDictionary<string, string> settings)
		{
			m_Control.ImagePath = settings.Get(OPT_IMAGEPATH, SlideshowControl.DefaultImagePath);
			FitMode fit_mode;
			if (Enum.TryParse(settings.Get(OPT_FITMODE, string.Empty), out fit_mode))
			{
				m_Control.FitMode = fit_mode;
			}
			else
			{
				m_Control.FitMode = SlideshowControl.DefaultFitMode;
			}
			m_Control.StayForSecond = settings.Get(OPT_STAYFORSECOND, SlideshowControl.DefaultStayForSecond);
		}

		public void OnSettingsWanted(IDictionary<string, string> settings)
		{
			settings.Set(OPT_IMAGEPATH, m_SettingsControl.ImageFolder);
			settings.Set(OPT_FITMODE, m_SettingsControl.FitMode.ToString());
			settings.Set(OPT_STAYFORSECOND, m_SettingsControl.StayForSecond.ToString());
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

		const string OPT_IMAGEPATH = "ImagePath";
		const string OPT_FITMODE = "FitMode";
		const string OPT_STAYFORSECOND = "StayForSecond";
		SlideshowControl m_Control;
		SettingsControl m_SettingsControl;
		string m_UniqueName;
	}
}
