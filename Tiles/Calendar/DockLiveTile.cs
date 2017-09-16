using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using pWonders.App.DockLive.TileInterface;

namespace pWonders.App.DockLive.Tiles.Calendar
{
	public class DockLiveTile : ITile
	{
		public DockLiveTile()
		{
			m_Control = new CalendarControl(this);
			m_SettingsControl = new SettingsControl(this);
			m_UniqueName = SingleInstance.GetUniqueName(Assembly.GetExecutingAssembly());
		}

		public void OnAttachTile(ITileHost host)
		{
			OnThemeChanged(host.Theme);
			m_Control.Size = new Size(host.FullBounds.Width - m_Control.Margin.Horizontal, host.FullBounds.Width - m_Control.Margin.Vertical);
		}

		public void OnDetachTile()
		{
		}

		public void OnThemeChanged(AppTheme theme)
		{
			m_Control.Theme = theme;
		}

		public void OnSettingsOpened()
		{
			m_SettingsControl.ShowAltCalendar = m_Control.ShowAltCalendar;
		}

		public void OnSettingsClosed()
		{
			m_Control.ShowAltCalendar = m_SettingsControl.ShowAltCalendar;
		}

		public string Name
		{
			get { return "Calendar"; }
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
		
		CalendarControl m_Control;
		SettingsControl m_SettingsControl;
		string m_UniqueName;
	}
}
