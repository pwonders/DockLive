using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using pWonders.App.DockLive.TileInterface;

namespace pWonders.App.DockLive
{
	class TileLoader
	{
		public TileLoader()
		{
		}

		public List<ITile> Load()
		{
			List<ITile> tiles = new List<ITile>();
			DirectoryInfo di;
			try
			{
				di = new DirectoryInfo(get_tiles_directory());
				foreach (FileInfo fi in di.EnumerateFiles(EXT_TILES, SearchOption.TopDirectoryOnly))
				{
					Assembly asm = Assembly.LoadFrom(fi.FullName);
					foreach (Module mod in asm.GetModules(false))
					{
						foreach (Type type in mod.FindTypes(Module.FilterTypeName, CLASS_TILES))
						{
							if (type != null)
							{
								ITile t = Activator.CreateInstance(type) as ITile;
								if (t != null)
								{
									tiles.Add(t);
								}
							}
						}
					}
				}
			}
			catch { }
			return tiles;
		}

		const string DIR_TILES = "Tiles";
		const string EXT_TILES = "*.dll";
		const string CLASS_TILES = "DockLiveTile";

		string get_tiles_directory()
		{
			return Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + DIR_TILES;
		}
	}
}
