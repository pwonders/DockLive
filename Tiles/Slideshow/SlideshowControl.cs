using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using pWonders.App.DockLive.TileInterface;

namespace pWonders.App.DockLive.Tiles.Slideshow
{
	public enum FitMode { Fit, Fill };

	[System.ComponentModel.DesignerCategory("")]
	class SlideshowControl : TileChildControl
	{
		public SlideshowControl(ITile tile) : base(tile)
		{
			// Required for proper color on blur background.
			// Also, use FillRectangle OnPaintBackground, don't use BackColor.
			this.DoubleBuffered = true;

			m_TimerSlide = new Timer { Interval = 10 };
			m_TimerSlide.Tick += TimerSlide_Tick;
			m_Set = new LinkedList<SlideData>();
			this.ImageFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			this.FitMode = FitMode.Fill;
			this.StayForSecond = 2;
			PrepareSlideShow();
		}

		public void PrepareSlideShow()
		{
			bool has_changed = false;
			if (m_Enumerated == false)
			{
				try_enumerate_images();
				reset_slideshow();
				has_changed = true;
			}
			if (m_Cached == false)
			{
				cache_slides();
				has_changed = true;
			}
			if (has_changed && m_Set.Count > 0)
			{
				this.Invalidate();
				if (m_FirstShow || m_Set.Count > 1)
				{
					m_TimerSlide.Start();
				}
			}
		}

		public void PauseSlideShow()
		{
			// If it's running.
			if (m_LastQueueOn != default(DateTime))
			{
				// Save displayed time.
				m_LastStayFor = DateTime.Now - m_LastQueueOn;
			}
			m_TimerSlide.Stop();
		}

		public void ResumeSlideShow()
		{
			if (m_Set.Count > 0)
			{
				if (m_LastStayFor != default(TimeSpan))
				{
					m_LastQueueOn = DateTime.Now - m_LastStayFor;
				}
				m_TimerSlide.Start();
			}
		}

		public string ImageFolder
		{
			set
			{
				if (m_ImageFolder != value)
				{
					m_ImageFolder = value;
					m_Set.Clear();
					m_Enumerated = false;
					m_Cached = false;
				}
			}
			get { return m_ImageFolder; }
		}

		public int StayForSecond { set; get; }

		public FitMode FitMode
		{
			set
			{
				if (m_FitMode != value)
				{
					m_FitMode = value;
					clear_cache();
				}
			}
			get { return m_FitMode; }
		}

		class SlideData
		{
			public SlideData(string path)
			{
				this.Path = path;
			}
			public string Path;
			public Bitmap Bitmap;
			public bool IsBitmapScaled;
			public Rectangle DisplayBounds;
		}

		Color m_BackColor;
		Timer m_TimerSlide;
		string m_ImageFolder;
		FitMode m_FitMode;
		LinkedList<SlideData> m_Set;
		bool m_Enumerated;
		bool m_Cached;
		bool m_FirstShow;
		DateTime m_LastQueueOn;
		TimeSpan m_LastStayFor;
		float m_BlendFactor;

		protected override void OnThemeChanged(EventArgs e)
		{
			base.OnThemeChanged(e);
			switch (base.Theme)
			{
			case AppTheme.System:
				m_BackColor = Color.FromArgb(0xfe, Color.Black);
				break;
			case AppTheme.Dark:
			case AppTheme.Light:
				break;
			}
			this.Invalidate();
		}

		protected override void OnClientSizeChanged(EventArgs e)
		{
			base.OnClientSizeChanged(e);
			clear_cache();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);
			using (Brush br = new SolidBrush(m_BackColor))
			{
				e.Graphics.FillRectangle(br, e.ClipRectangle);
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (m_Set.Count > 0 && m_Cached)
			{
				if (m_BlendFactor == 0f)
				{
					e.Graphics.DrawImage(m_Set.First.Value.Bitmap, m_Set.First.Value.DisplayBounds);
				}
				else
				{
					ColorMatrix cm = new ColorMatrix();
					ImageAttributes ia = new ImageAttributes();
					SlideData d0 = m_Set.First.Value;
					if (m_FirstShow)
					{
						cm.Matrix33 = m_BlendFactor;
						ia.SetColorMatrix(cm);
						e.Graphics.DrawImage(d0.Bitmap, d0.DisplayBounds, 0, 0, d0.Bitmap.Width, d0.Bitmap.Height, GraphicsUnit.Pixel, ia);
					}
					else if (m_Set.Count > 1)
					{
						SlideData d1 = m_Set.First.Next.Value;
						if (d1.DisplayBounds.Contains(d0.DisplayBounds))
						{
							e.Graphics.DrawImage(d0.Bitmap, d0.DisplayBounds);
						}
						else
						{
							cm.Matrix33 = 1f - m_BlendFactor;
							ia.SetColorMatrix(cm);
							e.Graphics.DrawImage(d0.Bitmap, d0.DisplayBounds, 0, 0, d0.Bitmap.Width, d0.Bitmap.Height, GraphicsUnit.Pixel, ia);
						}
						cm.Matrix33 = m_BlendFactor;
						ia.SetColorMatrix(cm);
						e.Graphics.DrawImage(d1.Bitmap, d1.DisplayBounds, 0, 0, d1.Bitmap.Width, d1.Bitmap.Height, GraphicsUnit.Pixel, ia);
					}
				}
			}
		}

		private void TimerSlide_Tick(object sender, EventArgs e)
		{
			int ms_past_due = (int) (DateTime.Now - m_LastQueueOn).TotalMilliseconds - this.StayForSecond * 1000;
			if (ms_past_due > 0)
			{
				const int MS_TRANSITION = 1000;
				m_BlendFactor = (float) ms_past_due / MS_TRANSITION;
				if (m_BlendFactor >= 1f)
				{
					m_BlendFactor = 0f;
					m_LastQueueOn = DateTime.Now;
					if (m_FirstShow)
					{
						m_FirstShow = false;
					}
					else if (m_Set.Count > 1)
					{
						var first = m_Set.First;
						m_Set.RemoveFirst();
						m_Set.AddLast(first);
						cache_slides();
					}
					else
					{
						m_TimerSlide.Stop();
					}
				}
				if (m_Set.Count > 1 || m_FirstShow)
				{
					this.Invalidate();
				}
			}
		}

		void try_enumerate_images()
		{
			try
			{
				enumerate_images(new DirectoryInfo(m_ImageFolder));
			}
			catch { }
			m_Enumerated = true;
		}

		void enumerate_images(DirectoryInfo folder)
		{
			foreach (var f in folder.EnumerateFileSystemInfos())
			{
				DirectoryInfo di = f as DirectoryInfo;
				if (di != null)
				{
					enumerate_images(di);
				}
				else
				{
					string name = f.Name.ToLowerInvariant();
					if (name.EndsWith(".jpg") || name.EndsWith(".png"))
					{
						m_Set.AddLast(new SlideData(f.FullName));
					}
				}
			}
		}

		void reset_slideshow()
		{
			m_LastStayFor = default(TimeSpan);
			m_LastQueueOn = DateTime.Now.AddSeconds(-this.StayForSecond);
			m_BlendFactor = 0.001f;
			m_FirstShow = true;
		}

		void clear_cache()
		{
			foreach (var d in m_Set)
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
			m_Cached = false;
		}

		void cache_slides()
		{
			if (m_Set.Count > 0)
			{
				cache_slide_at(0);
				if (m_Set.Count > 1)
				{
					cache_slide_at(1);
				}
			}
			m_Cached = true;
		}

		void cache_slide_at(int index)
		{
			SlideData d = try_load_slide(index);
			if (d != null)
			{
				Rectangle draw_bounds = Rectangle.Empty;
				if (d.DisplayBounds.IsEmpty)
				{
					switch (this.FitMode)
					{
					case FitMode.Fit:
						d.DisplayBounds = fit_in_box(this.ClientRectangle, d.Bitmap.Size);
						draw_bounds = new Rectangle(Point.Empty, d.DisplayBounds.Size);
						break;
					case FitMode.Fill:
					default:
						d.DisplayBounds = this.ClientRectangle;
						draw_bounds = fit_out_box(this.ClientRectangle, d.Bitmap.Size);
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

		SlideData try_load_slide(int index)
		{
			var node = m_Set.First;
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
					node.Value.Bitmap = new Bitmap(node.Value.Path);
					break;
				}
				catch
				{
					LinkedListNode<SlideData> invalid = node;
					node = node.Next;
					m_Set.Remove(invalid);
				}
			}
			return node?.Value;
		}

		Rectangle fit_in_box(Rectangle box, Size srcSize)
		{
			return fit_box(box, srcSize, true);
		}

		Rectangle fit_out_box(Rectangle box, Size srcSize)
		{
			return fit_box(box, srcSize, false);
		}

		Rectangle fit_box(Rectangle box, Size srcSize, bool inside)
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
