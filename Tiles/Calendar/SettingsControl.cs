using System;
using System.Drawing;
using pWonders.App.DockLive.TileInterface;

namespace pWonders.App.DockLive.Tiles.Calendar
{
	public partial class SettingsControl : TileChildControl
	{
		public SettingsControl(ITile tile) : base(tile)
		{
			this.DoubleBuffered = true;
			InitializeComponent();
		}

		public bool ShowAltCalendar
		{
			set
			{
				m_ShowAltCalendar = value;
				if (m_ShowAltCalendar)
				{
					lblShowAlt.Text = "Show alternate calendar";
				}
				else
				{
					lblShowAlt.Text = "Do not show alternate calendar";
				}
			}
			get { return m_ShowAltCalendar; }
		}

		bool m_ShowAltCalendar;

		private void lblShowAlt_Click(object sender, EventArgs e)
		{
			this.ShowAltCalendar = !ShowAltCalendar;
		}
	}
}
