using System.Windows.Forms;
using pWonders.App.DockLive.TileInterface;

namespace pWonders.App.DockLive.Tiles.Calendar
{
	public class DockLiveTile : ITile
	{
		public DockLiveTile()
		{
			m_Control = new CalendarControl();
		}

		public string Name
		{
			get { return "Calendar"; }
		}

		public string Version
		{
			get { return Application.ProductVersion; }
		}

		public string Developer
		{
			get { return Application.CompanyName; }
		}

		public Control Control
		{
			get { return m_Control; }
		}

		public void OnAttachTile(ITileHost host)
		{
			m_Host = host;
			m_Control.Theme = m_Host.Theme;
			m_Control.SetBounds(0, 0, host.FullBounds.Width, host.FullBounds.Width, BoundsSpecified.Size);
		}

		public void OnDetachTile()
		{
		}

		public void OnThemeChanged(AppTheme theme)
		{
			m_Control.Theme = theme;
		}

		CalendarControl m_Control;
		ITileHost m_Host;
	}
}
