using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using pWonders.App.DockLive.TileInterface;

namespace pWonders.App.DockLive.Tiles.Slideshow
{
	public enum FitMode { Fit, Fill };

	[System.ComponentModel.DesignerCategory("")]
	class SlideshowControl : TileChildControl
	{
		public static string DefaultImagePath
		{
			get { return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); }
		}

		public static FitMode DefaultFitMode
		{
			get { return FitMode.Fill; }
		}

		public static int DefaultStayForSecond
		{
			get { return 2; }
		}

		public SlideshowControl(ITile tile) : base(tile)
		{
			// Required for proper color on blur background.
			// Also, use FillRectangle OnPaintBackground, don't use BackColor.
			this.DoubleBuffered = true;

			m_TimerSlide = new Timer { Interval = 10 };
			m_TimerSlide.Tick += TimerSlide_Tick;
			m_Provider = new LocalImageProvider();
			m_Set = new LinkedList<SlideData>();
			this.ImagePath = SlideshowControl.DefaultImagePath;
			this.FitMode = SlideshowControl.DefaultFitMode;
			this.StayForSecond = SlideshowControl.DefaultStayForSecond;

			// The timer always runs to track changes.
			m_TimerSlide.Start();
		}

		public void PauseSlideShow()
		{
			// If it's running.
			if (m_LastShownOn != default(DateTime))
			{
				// Save displayed time.
				m_LastStayFor = DateTime.Now - m_LastShownOn;
			}
			m_TimerSlide.Stop();
		}

		public void ResumeSlideShow()
		{
			if (m_LastStayFor != default(TimeSpan))
			{
				m_LastShownOn = DateTime.Now - m_LastStayFor;
			}
			// Signal m_Resumed so OnPaint won't reset m_LastShownOn.
			m_Resumed = true;
			m_TimerSlide.Start();
		}

		public string ImagePath
		{
			set
			{
				m_Provider.Path = value;
				if (m_Provider.Retrieved == false)
				{
					m_Changed = true;
					m_FirstShow = true;
				}
			}
			get { return m_Provider.Path; }
		}

		public FitMode FitMode
		{
			set
			{
				if (m_FitMode != value)
				{
					m_FitMode = value;
					m_Changed = true;
					ImageCacher.Clear(m_Set);
				}
			}
			get { return m_FitMode; }
		}

		public int StayForSecond { set; get; }

		const int MS_TRANSITION = 1000;
		Color m_BackColor;
		Timer m_TimerSlide;
		ImageProvider m_Provider;
		FitMode m_FitMode;
		LinkedList<SlideData> m_Set;
		bool m_FirstShow;
		bool m_Changed;
		bool m_Resumed;
		DateTime m_LastShownOn;
		TimeSpan m_LastStayFor;
		float m_BlendFactor;

		protected override void OnThemeChanged(EventArgs e)
		{
			base.OnThemeChanged(e);
			switch (base.Theme)
			{
			case AppTheme.System:
				m_BackColor = Color.FromArgb(0xfe, Color.Black);
				this.ForeColor = UIColor.Background;
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
			ImageCacher.Clear(m_Set);
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
			if (ImageCacher.IsCached(m_Set, 2))
			{
				SlideData d0 = m_Set.First.Value;
				// If not transitioning, just draw one image.
				if (m_BlendFactor == 0f)
				{
					e.Graphics.DrawImage(d0.Bitmap, d0.DisplayBounds);
					if (m_Resumed)
					{
						m_Resumed = false;
					}
					else
					{
						m_LastShownOn = DateTime.Now;
					}
				}
				else
				{
					// If showing for the first time, blend in instead of out.
					if (m_FirstShow)
					{
						draw_slide_with_matrix(e.Graphics, d0, m_BlendFactor);
					}
					else if (m_Set.Count > 1)
					{
						SlideData d1 = m_Set.First.Next.Value;
						draw_slide_with_matrix(e.Graphics, d0, 1f - m_BlendFactor);
						draw_slide_with_matrix(e.Graphics, d1, m_BlendFactor);
					}
				}
				return;
			}
			else
			{
				string msg = (m_Provider.Retrieved && m_Set.Count == 0) ? "No image found." : "Loading images...";
				using (Brush br = new SolidBrush(this.ForeColor))
				{
					StringFormat sf = new StringFormat();
					sf.Alignment = StringAlignment.Center;
					sf.LineAlignment = StringAlignment.Center;
					e.Graphics.DrawString(msg, this.Font, br, this.ClientRectangle, sf);
				}
			}
		}

		private async void TimerSlide_Tick(object sender, EventArgs e)
		{
			m_TimerSlide.Stop();

			await run_slideShow_async();

			int ms_past_due = (int) (DateTime.Now - m_LastShownOn).TotalMilliseconds - this.StayForSecond * 1000;
			if (ms_past_due > 0)
			{
				m_BlendFactor = (float) ms_past_due / MS_TRANSITION;
				if (m_BlendFactor >= 1f)
				{
					m_BlendFactor = 0f;
					if (m_FirstShow)
					{
						m_FirstShow = false;
					}
					else if (m_Set.Count > 1)
					{
						var first = m_Set.First;
						m_Set.RemoveFirst();
						m_Set.AddLast(first);
						await ImageCacher.CacheAsync(m_Set, 2, m_FitMode, this.ClientRectangle);
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

			m_TimerSlide.Start();
		}

		static void draw_slide_with_matrix(Graphics g, SlideData d, float blend)
		{
			ColorMatrix cm = new ColorMatrix();
			cm.Matrix33 = blend;
			ImageAttributes ia = new ImageAttributes();
			ia.SetColorMatrix(cm);
			g.DrawImage(d.Bitmap, d.DisplayBounds, 0, 0, d.Bitmap.Width, d.Bitmap.Height, GraphicsUnit.Pixel, ia);
		}

		async Task run_slideShow_async()
		{
			if (m_Changed)
			{
				// Do this first to clear last image asap while waiting below.
				this.Invalidate();
			}
			if (m_Provider.Retrieved == false)
			{
				m_Set = await m_Provider.RetrieveAsync();
			}
			if (ImageCacher.IsCached(m_Set, 2) == false)
			{
				await ImageCacher.CacheAsync(m_Set, 2, m_FitMode, this.ClientRectangle);
			}
			if (m_Changed)
			{
				m_Changed = false;
				if (m_FirstShow)
				{
					reset_slideshow();
				}
			}
		}

		void reset_slideshow()
		{
			m_LastStayFor = default(TimeSpan);
			m_LastShownOn = DateTime.Now.AddSeconds(-this.StayForSecond);
			m_BlendFactor = 0.001f;
		}
	}
}
