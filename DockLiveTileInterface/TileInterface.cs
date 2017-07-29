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
		string Name { get; }
		string Version { get; }
		string Developer { get; }
		Control Control { get; }
		void OnAttachTile(ITileHost host);
		void OnDetachTile();
		void AppThemeChanged();
	}
}
