using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace pWonders.App.DockLive.Tiles.Slideshow
{
	static class ImageCacher
	{
		public static bool IsCached(LinkedList<SlideData> set, int numSlides)
		{
			bool cached = false;
			var node = set.First;
			while (node != null && numSlides-- > 0)
			{
				cached = cached || (node.Value.Bitmap != null && node.Value.IsBitmapScaled);
				node = node.Next;
			}
			return cached;
		}

		public static void Clear(LinkedList<SlideData> set)
		{
			foreach (var d in set)
			{
				if (d.Bitmap != null)
				{
					d.Bitmap.Dispose();
					d.Bitmap = null;
					d.IsBitmapScaled = false;
					d.DisplayBounds = Rectangle.Empty;
				}
				else
				{
					break;
				}
			}
		}

		public static async Task CacheAsync(LinkedList<SlideData> set, int numSlides, FitMode fit, Rectangle bounds)
		{
			for (int i = 0; i < numSlides && i < set.Count; i++)
			{
				await Task.Run(() => cache_slide_at(set, i, fit, bounds));
			}
		}

		static void cache_slide_at(LinkedList<SlideData> set, int index, FitMode fit, Rectangle bounds)
		{
			SlideData d = try_load_slide(set, index);
			if (d != null)
			{
				Rectangle draw_bounds = Rectangle.Empty;
				if (d.DisplayBounds.IsEmpty)
				{
					switch (fit)
					{
					case FitMode.Fit:
						d.DisplayBounds = fit_in_box(bounds, d.Bitmap.Size);
						draw_bounds = new Rectangle(Point.Empty, d.DisplayBounds.Size);
						break;
					case FitMode.Fill:
					default:
						d.DisplayBounds = bounds;
						draw_bounds = fit_out_box(bounds, d.Bitmap.Size);
						break;
					}
				}
				if (d.IsBitmapScaled == false)
				{
					Bitmap scaled = new Bitmap(d.DisplayBounds.Width, d.DisplayBounds.Height);
					using (Graphics g = Graphics.FromImage(scaled))
					{
						g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
						g.DrawImage(d.Bitmap, draw_bounds);
					}
					d.Bitmap.Dispose();
					d.Bitmap = scaled;
					d.IsBitmapScaled = true;
				}
			}
		}

		static SlideData try_load_slide(LinkedList<SlideData> set, int index)
		{
			var node = set.First;
			for (int i = 0; i < index && node != null; i++)
			{
				node = node.Next;
			}
			while (node != null)
			{
				if (node.Value.Bitmap != null)
				{
					break;
				}
				try
				{
					node.Value.Bitmap = new Bitmap(node.Value.LocalPath);
					break;
				}
				catch
				{
					LinkedListNode<SlideData> invalid = node;
					node = node.Next;
					set.Remove(invalid);
				}
			}
			return node?.Value;
		}

		static Rectangle fit_in_box(Rectangle box, Size srcSize)
		{
			return fit_box(box, srcSize, true);
		}

		static Rectangle fit_out_box(Rectangle box, Size srcSize)
		{
			return fit_box(box, srcSize, false);
		}

		static Rectangle fit_box(Rectangle box, Size srcSize, bool inside)
		{
			Rectangle dst = Rectangle.Empty;
			float src_ar = (float) srcSize.Width / srcSize.Height;
			float box_ar = (float) box.Width / box.Height;
			if (inside ? src_ar > box_ar : src_ar < box_ar)
			{
				dst.Width = box.Width;
				dst.Height = (int) (dst.Width / src_ar);
				dst.X = 0;
				dst.Y = (box.Height - dst.Height) / 2;
			}
			else
			{
				dst.Height = box.Height;
				dst.Width = (int) (dst.Height * src_ar);
				dst.Y = 0;
				dst.X = (box.Width - dst.Width) / 2;
			}
			return dst;
		}
	}
}
