using System.Drawing;

namespace pWonders.App.DockLive.Tiles.Slideshow
{
	public class SlideData
	{
		public SlideData(string localPath)
		{
			this.LocalPath = localPath;
		}
		public string LocalPath;
		public Bitmap Bitmap;
		public bool IsBitmapScaled;
		public Rectangle DisplayBounds;
	}
}
