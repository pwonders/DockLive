using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using pWonders;
using pWonders.App.DockLive.TileInterface;

namespace pWonders.App.DockLive.Tiles.Slideshow
{
	public class DockLiveTile : ITile
	{
		public DockLiveTile()
		{
			m_Control = new SlideshowControl(this);
			m_SettingsControl = new SettingsControl(this);
		}

		public void OnAttachTile(ITileHost host)
		{
			this.Host = host;
			OnThemeChanged(host.Theme);
			m_Control.Size = new Size(host.FullBounds.Width - m_Control.Margin.Horizontal, host.FullBounds.Width / 3 - m_Control.Margin.Vertical);
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
			m_SettingsControl.ImageFolder = m_Control.ImageFolder;
			m_SettingsControl.StayForSecond = m_Control.StayForSecond;
		}

		public void OnSettingsClosed()
		{
			m_Control.ImageFolder = m_SettingsControl.ImageFolder;
			m_Control.StayForSecond = m_SettingsControl.StayForSecond;
			m_Control.ResumeSlideShow();
		}

		public ITileHost Host { set; get; }

		public string Name
		{
			get { return "Slideshow"; }
		}

		public string UniqueName
		{
			get { return SingleInstance.GetUniqueName(); }
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
	}
}
