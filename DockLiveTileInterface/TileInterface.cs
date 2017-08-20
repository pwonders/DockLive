using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace pWonders.App.DockLive.TileInterface
{
	public enum AppTheme { System = 1, Dark, Light };

	public interface ITileHost
	{
		AppTheme Theme { get; }
		Rectangle FullBounds { get; }
	}

	public interface ITile
	{
		void OnAttachTile(ITileHost host);
		void OnDetachTile();
		void OnThemeChanged(AppTheme theme);
		void OnSettingsOpened();
		void OnSettingsClosed();
		string Name { get; }
		string UniqueName { get; }
		string Version { get; }
		string Developer { get; }
		TileChildControl Control { get; }
		TileChildControl SettingsControl { get; }
	}

	// This class isn't abstract because the VS designer can't open inherited TileChildControl that way.
	public class TileChildControl : UserControl
	{
		// This parameterless constructor is just to make VS designer happy.
		public TileChildControl()
		{
		}

		public TileChildControl(ITile tile)
		{
			this.Tile = tile;
		}

		[Browsable(false)]
		public ITile Tile { set; get; }

		[DefaultValue(AppTheme.System)]
		public AppTheme Theme
		{
			set
			{
				if (m_Theme != value)
				{
					m_Theme = value;
					OnThemeChanged(EventArgs.Empty);
				}
			}
			get { return m_Theme; }
		}

		AppTheme m_Theme;

		protected virtual void OnThemeChanged(EventArgs e)
		{
		}
	}
}
