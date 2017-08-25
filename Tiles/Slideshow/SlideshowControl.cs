using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using pWonders.App.DockLive.TileInterface;

namespace pWonders.App.DockLive.Tiles.Slideshow
{
	[System.ComponentModel.DesignerCategory("")]
	class SlideshowControl : TileChildControl
	{
		public SlideshowControl(ITile tile) : base(tile)
		{
			// Required for proper color on blur background.
			// Also, use FillRectangle OnPaintBackground, don't use BackColor.
			this.DoubleBuffered = true;

			m_TimerSlide = new Timer();
			m_TimerSlide.Interval = 100;
			m_TimerSlide.Tick += TimerSlide_Tick;
			m_Set = new LinkedList<string>();
			m_DisplaySet = new LinkedList<DisplayData>();
			this.StayForSecond = 5;
			this.ImageFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
		}

		public void PauseSlideShow()
		{
			// If it's running.
			if (m_LastQueueOn != default(DateTime))
			{
				m_LastStayFor = DateTime.Now - m_LastQueueOn;
			}
			m_TimerSlide.Stop();
		}

		public void ResumeSlideShow()
		{
			if (m_Set.Count > 0)
			{
				// m_LastQueueOn is set when init slideshow, so check m_LastStayFor.
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
					init_Slideshow();
				}
			}
			get { return m_ImageFolder; }
		}

		public int StayForSecond { set; get; }

		struct DisplayData
		{
			public DisplayData(Image image, Size size)
			{
				Image = image;
				Size = size;
			}
			public Image Image;
			public Size Size;
		}

		Color m_BackColor;
		Timer m_TimerSlide;
		string m_ImageFolder;
		LinkedList<string> m_Set;
		LinkedList<DisplayData> m_DisplaySet;
		DateTime m_LastQueueOn;
		TimeSpan m_LastStayFor;
		int m_DisplayOffsetX;

		protected override void OnThemeChanged(EventArgs e)
		{
			base.OnThemeChanged(e);
			switch (base.Theme)
			{
			case AppTheme.System:
				m_BackColor = Color.FromArgb(0x7f, Color.White);
				break;
			case AppTheme.Dark:
			case AppTheme.Light:
				break;
			}
			this.Invalidate();
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
			Graphics g = e.Graphics;
			Rectangle r = this.ClientRectangle;
			r.X += m_DisplayOffsetX;
			foreach (DisplayData d in m_DisplaySet)
			{
				r.Size = d.Size;
				g.DrawImage(d.Image, r);
				r.X += r.Width;
			}
		}

		private void TimerSlide_Tick(object sender, EventArgs e)
		{
			if ((DateTime.Now - m_LastQueueOn).TotalSeconds > this.StayForSecond)
			{
				// Have to reconstruct the display set because the number of images that fit may change.
				if (m_Set.Count > 0)
				{
					LinkedListNode<string> first = m_Set.First;
					m_Set.RemoveFirst();
					m_Set.AddLast(first);
					queue_images();
				}
				if (m_DisplaySet.Count > 0)
				{
					this.Invalidate();
				}
			}
		}

		void get_pictures(DirectoryInfo folder)
		{
			foreach (var f in folder.EnumerateFileSystemInfos())
			{
				DirectoryInfo di = f as DirectoryInfo;
				if (di != null)
				{
					get_pictures(di);
				}
				else
				{
					string name = f.Name.ToLowerInvariant();
					if (name.EndsWith(".jpg") || name.EndsWith(".png"))
					{
						m_Set.AddLast(f.FullName);
					}
				}
			}
		}

		void init_Slideshow()
		{
			m_Set.Clear();
			try
			{
				get_pictures(new DirectoryInfo(m_ImageFolder));
			}
			catch { }
			m_LastStayFor = default(TimeSpan);
			if (m_Set.Count > 0)
			{
				queue_images();
				this.Invalidate();
				m_TimerSlide.Start();
			}
		}

		Size fit(Size srcSize)
		{
			Size dstSize = this.ClientSize;
			dstSize.Width = dstSize.Height * srcSize.Width / srcSize.Height;
			return dstSize;
		}

		void queue_images()
		{
			m_DisplaySet.Clear();
			LinkedListNode<string> node = m_Set.First;
			int total_width = 0;
			while (node != null && total_width < this.ClientSize.Width)
			{
				Image image = new Bitmap(node.Value);
				Size size = fit(image.Size);
				m_DisplaySet.AddLast(new DisplayData(image, size));
				total_width += size.Width;
				node = node.Next;
			}
			m_LastQueueOn = DateTime.Now;
		}
	}
}
