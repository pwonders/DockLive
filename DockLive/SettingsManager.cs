using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Microsoft.Win32;

namespace pWonders.App.DockLive
{
	static class SettingsManager
	{
		public static void Load(string tileName, IDictionary<string, string> settings)
		{
			string path = get_settings_filename();
			foreach (var key in get_settings(tileName, null, path).Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries))
			{
				settings.Add(key, get_settings(tileName, key, path));
			}
		}

		public static void Save(string tileName, IDictionary<string, string> settings)
		{
			string path = get_settings_filename();
			foreach (var p in settings)
			{
				API.WritePrivateProfileString(tileName, p.Key, p.Value, path);
			}
		}

		const string FILENAME = "tiles.ini";

		static string get_settings_filename()
		{
			return get_settings_directory() + Path.DirectorySeparatorChar + FILENAME;
		}

		static string get_settings_directory()
		{
			return Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath);
		}

		static string get_settings(string tileName, string keyName, string fileName)
		{
			char[] ret = new char[256];
			while (true)
			{
				int nsize = API.GetPrivateProfileString(tileName, keyName, null, ret, ret.Length, fileName);
				if (nsize == 0)
				{
					return string.Empty;
				}
				if ((keyName != null && nsize != ret.Length - 1) || (keyName == null && nsize != ret.Length - 2))
				{
					return new string(ret, 0, nsize);
				}
				ret = new char[ret.Length * 2];
			}
		}
	}
}
