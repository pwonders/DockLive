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
			m_Control.Size = new System.Drawing.Size(m_Host.FullBounds.Width, 2000);
		}

		public void OnDetachTile()
		{
		}

		public void AppThemeChanged()
		{
		}

		CalendarControl m_Control;
		ITileHost m_Host;
	}
}
