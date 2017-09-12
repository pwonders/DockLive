using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace pWonders.App.DockLive.Tiles.Slideshow
{
	public abstract class ImageProvider
	{
		public abstract Task<LinkedList<SlideData>> RetrieveAsync();
		public virtual string Path
		{
			set
			{
				if (m_Path != value)
				{
					m_Path = value;
					this.Retrieved = false;
				}
			}
			get { return m_Path; }
		}
		public bool Retrieved { protected set; get; }
		string m_Path;
	}

	public class LocalImageProvider : ImageProvider
	{
		public LocalImageProvider()
		{
			m_Set = new LinkedList<SlideData>();
			this.Filter = ".jpg|.png";
		}

		public async override Task<LinkedList<SlideData>> RetrieveAsync()
		{
			m_Set.Clear();
			DirectoryInfo di = null;
			try
			{
				di = new DirectoryInfo(this.Path);
			}
			catch { }
			if (di != null)
			{
				await Task.Run(() => enumerate_images(di));
			}
			this.Retrieved = true;
			return m_Set;
		}

		public string Filter { set; get; }

		public long FilterSizeInBytes { set; get; }

		LinkedList<SlideData> m_Set;

		// TODO: see if Parallel class can speed up file enumeration:
		// https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-iterate-file-directories-with-the-parallel-class
		void enumerate_images(DirectoryInfo folder)
		{
			IEnumerable<FileSystemInfo> list = new List<FileSystemInfo>(0);
			try
			{
				list = folder.EnumerateFileSystemInfos();
			}
			catch
			{
				return;
			}
			foreach (var f in list)
			{
				DirectoryInfo di = f as DirectoryInfo;
				if (di != null)
				{
					enumerate_images(di);
				}
				else
				{
					bool match = false;
					string name = f.Name.ToLowerInvariant();
					string[] exts = this.Filter.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string ext in exts)
					{
						if (name.EndsWith(ext))
						{
							match = true;
							break;
						}
					}
					match = match && (f as FileInfo).Length > this.FilterSizeInBytes;
					if (match)
					{

						try
						{
							m_Set.AddLast(new SlideData(f.FullName));
						}
						catch { }
					}
				}
			}
		}
	}
}
