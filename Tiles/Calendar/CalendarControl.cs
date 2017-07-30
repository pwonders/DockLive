using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Win32;
using pWonders.App.DockLive.TileInterface;

namespace pWonders.App.DockLive.Tiles.Calendar
{
	[System.ComponentModel.DesignerCategory("")]
	partial class CalendarControl : UserControl
	{
		public enum CalendarView { Month, Day, Year };

		static CalendarControl()
		{
			s_GregorianCalendar = new GregorianCalendar(GregorianCalendarTypes.Localized);
		}

		public static int GetWeekOfYear(DateTime time)
		{
			return s_GregorianCalendar.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
		}

		public static GregorianCalendar GregorianCalendar
		{
			get { return s_GregorianCalendar; }
		}

		public CalendarControl()
		{
			m_Renderer = new CalendarRenderer(this);
			m_NumWeekShown = DEFAULT_NUMWEEKSHOWN;
			this.DoubleBuffered = true;
			this.Theme = AppTheme.System;
			m_View = CalendarView.Month;

			btnNextMonth = new_Label();
			btnNextMonth.MouseUp += btnNextMonth_MouseUp;
			btnNextMonth.Paint += btnNextMonth_Paint;

			btnPrevMonth = new_Label();
			btnPrevMonth.MouseUp += btnPrevMonth_MouseUp;
			btnPrevMonth.Paint += btnPrevMonth_Paint;

			btnToday = new_Label();
			btnToday.MouseUp += btnToday_MouseUp;
			btnToday.Paint += btnToday_Paint;

			this.Controls.AddRange(new Control[] { btnToday, btnPrevMonth, btnNextMonth });

			get_metrics();
			set_navi_bounds();
			SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;

			m_BackBuffer = new Bitmap(this.Width, this.Height);
			m_BackGraphics = Graphics.FromImage(m_BackBuffer);
			SetCurrentMonth(DateTime.Now);
		}

		void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
		{
			if (e.Category == UserPreferenceCategory.Window)
			{
				get_metrics();
				set_navi_bounds();
				request_redraw();
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				m_BackGraphics.Dispose();
				m_BackGraphics = null;
				m_BackBuffer.Dispose();
				m_BackBuffer = null;
			}
		}

		public void BeginUpdate()
		{
			++m_BeginUpdate;
		}

		public void EndUpdate()
		{
			--m_BeginUpdate;
		}

		public DateTime GetFirstLastDayOfWeek()
		{
			return m_FirstLastDayOfWeek;
		}

		public void SetCurrentDateTime(DateTime time)
		{
			m_FirstLastDayOfWeek = time.AddDays(7 - (int) (time.DayOfWeek + 1));
			request_redraw();
		}

		public void SetCurrentMonth(DateTime time)
		{
			DateTime dt_first_of_month = time.AddDays(-time.Day + 1);
			SetCurrentDateTime(dt_first_of_month);
		}

		[Browsable(false)]
		public Rectangle CalendarContentRectangle
		{
			get
			{
				Rectangle rect = this.ClientRectangle;
				return rect;
			}
		}

		[Browsable(false)]
		public Rectangle CalendarTopRectangle
		{
			get
			{
				Rectangle rect = this.CalendarContentRectangle;
				rect.Width = btnToday.Left;
				rect.Height = btnToday.Height;
				return rect;
			}
		}

		[DefaultValue(AppTheme.System)]
		public AppTheme Theme
		{
			set
			{
				if (m_Theme != value)
				{
					m_Theme = value;
					switch (m_Theme)
					{
					case AppTheme.Dark:
						this.BackColor = DARK_BACKCOLOR;
						m_NavigationColor = DARK_NAVIGATION_COLOR;
						m_YearColor = DARK_YEAR_COLOR;
						m_MonthColor = DARK_MONTH_COLOR;
						m_DayColor = DARK_DAY_COLOR;
						m_DayAltColor = DARK_DAYALT_COLOR;
						m_SundayColor = DARK_SUNDAY_COLOR;
						m_SaturdayColor = DARK_SATURDAY_COLOR;
						m_WeekdayColor = DARK_WEEKDAY_COLOR;
						m_WeekNumberColor = DARK_WEEKNUMBER_COLOR;
						m_TodayBackColor = DARK_TODAY_BACKCOLOR;
						m_TodayForeColor = DARK_TODAY_FORECOLOR;
						m_MonthBackColor = DARK_MONTH_BACKCOLOR;
						m_MonthForeColor = DARK_MONTH_FORECOLOR;
						break;
					case AppTheme.Light:
						break;
					case AppTheme.System:
						if (UIColor.IsValid)
						{
							this.BackColor = UIColor.AccentDark2;
							m_NavigationColor = UIColor.Background;
							m_HiliColor = UIColor.Accent;
							m_HiliAltColor = UIColor.AccentDark3;
							m_YearColor = UIColor.Background;
							m_MonthColor = UIColor.Background;
							m_DayColor = UIColor.Background;
							m_DayAltColor = UIColor.AccentDark1;
							m_SundayColor = UIColor.AccentLight1;
							m_SaturdayColor = UIColor.AccentLight2;
							m_WeekdayColor = UIColor.AccentLight3;
							m_WeekNumberColor = UIColor.AccentDark3;
							m_TodayBackColor = UIColor.Accent;
							m_TodayForeColor = UIColor.Background;
							m_MonthBackColor = UIColor.AccentDark3;
							m_MonthForeColor = UIColor.Foreground;
						}
						else
						{
						}
						break;
					}
					request_redraw();
				}
			}
			get { return m_Theme; }
		}

		[Browsable(false)]
		public CalendarView View
		{
			get { return m_View; }
		}

		[DefaultValue(DEFAULT_NUMWEEKSHOWN)]
		public int NumWeekShown
		{
			set
			{
				if (0 < value && value <= 52)
				{
					m_NumWeekShown = value;
					request_redraw();
				}
			}
			get { return m_NumWeekShown; }
		}

		public Color NavigationColor
		{
			set
			{
				if (m_NavigationColor != value)
				{
					m_NavigationColor = value;
					btnToday.ForeColor = m_NavigationColor;
					btnPrevMonth.ForeColor = m_NavigationColor;
					btnNextMonth.ForeColor = m_NavigationColor;
					request_redraw();
				}
			}
			get { return m_NavigationColor; }
		}

		public Color HiliColor
		{
			set { if (m_HiliColor != value) { m_HiliColor = value; request_redraw(); } }
			get { return m_HiliColor; }
		}

		public Color HiliAltColor
		{
			set { if (m_HiliAltColor != value) { m_HiliAltColor = value; request_redraw(); } }
			get { return m_HiliAltColor; }
		}

		public Color YearColor
		{
			set { if (m_YearColor != value) { m_YearColor = value; request_redraw(); } }
			get { return m_YearColor; }
		}

		public Color MonthColor
		{
			set { if (m_MonthColor != value) { m_MonthColor = value; request_redraw(); } }
			get { return m_MonthColor; }
		}

		public Color SundayColor
		{
			set { if (m_SundayColor != value) { m_SundayColor = value; request_redraw(); } }
			get { return m_SundayColor; }
		}

		public Color SaturdayColor
		{
			set { if (m_SaturdayColor != value) { m_SaturdayColor = value; request_redraw(); } }
			get { return m_SaturdayColor; }
		}

		public Color WeekdayColor
		{
			set { if (m_WeekdayColor != value) { m_WeekdayColor = value; request_redraw(); } }
			get { return m_WeekdayColor; }
		}

		public Color WeekNumberColor
		{
			set { if (m_WeekNumberColor != value) { m_WeekNumberColor = value; request_redraw(); } }
			get { return m_WeekNumberColor; }
		}

		public Color DayColor
		{
			set { if (m_DayColor != value) { m_DayColor = value; request_redraw(); } }
			get { return m_DayColor; }
		}

		public Color DayAltColor
		{
			set { if (m_DayAltColor != value) { m_DayAltColor = value; request_redraw(); } }
			get { return m_DayAltColor; }
		}

		public Color TodayBackColor
		{
			set { if (m_TodayBackColor != value) { m_TodayBackColor = value; request_redraw(); } }
			get { return m_TodayBackColor; }
		}

		public Color TodayForeColor
		{
			set { if (m_TodayForeColor != value) { m_TodayForeColor = value; request_redraw(); } }
			get { return m_TodayForeColor; }
		}

		public Color MonthBackColor
		{
			set { if (m_MonthBackColor != value) { m_MonthBackColor = value; request_redraw(); } }
			get { return m_MonthBackColor; }
		}

		public Color MonthForeColor
		{
			set { if (m_MonthForeColor != value) { m_MonthForeColor = value; request_redraw(); } }
			get { return m_MonthForeColor; }
		}

		const int DEFAULT_NUMWEEKSHOWN = 18;
		static readonly Color DARK_BACKCOLOR = Color.FromArgb(0x30, 0x30, 0x30);
		static readonly Color DARK_NAVIGATION_COLOR = Color.FromArgb(0x80, 0x80, 0x80);
		static readonly Color DARK_YEAR_COLOR = Color.FromArgb(0xff, 0xff, 0xff);
		static readonly Color DARK_MONTH_COLOR = Color.FromArgb(0xff, 0xff, 0xff);
		static readonly Color DARK_DAY_COLOR = Color.FromArgb(0xff, 0xff, 0xff);
		static readonly Color DARK_DAYALT_COLOR = Color.FromArgb(0x80, 0x80, 0x80);
		static readonly Color DARK_SUNDAY_COLOR = Color.Red;
		static readonly Color DARK_SATURDAY_COLOR = Color.Orange;
		static readonly Color DARK_WEEKDAY_COLOR = Color.FromArgb(0xff, 0xff, 0xff);
		static readonly Color DARK_WEEKNUMBER_COLOR = Color.FromArgb(0x80, 0x80, 0x80);
		static readonly Color DARK_TODAY_BACKCOLOR = Color.FromArgb(0x80, 0x80, 0x80);
		static readonly Color DARK_TODAY_FORECOLOR = Color.FromArgb(0x80, 0x80, 0x80);
		static readonly Color DARK_MONTH_BACKCOLOR = Color.FromArgb(0x10, 0x10, 0x10);
		static readonly Color DARK_MONTH_FORECOLOR = Color.FromArgb(0x30, 0x30, 0x30);
		static GregorianCalendar s_GregorianCalendar;

		Image m_BackBuffer;
		Graphics m_BackGraphics;
		bool m_BackDirty;
		int m_TopbarSize;
		int m_ButtonSize;
		Label btnToday, btnNextMonth, btnPrevMonth;
		int m_BeginUpdate;
		DateTime m_FirstLastDayOfWeek;
		CalendarRenderer m_Renderer;
		int m_NumWeekShown;
		AppTheme m_Theme;
		CalendarView m_View;

		Color m_NavigationColor, m_HiliColor, m_HiliAltColor;
		Color m_YearColor, m_MonthColor, m_DayColor, m_DayAltColor;
		Color m_SundayColor, m_SaturdayColor, m_WeekdayColor, m_WeekNumberColor;
		Color m_TodayBackColor, m_TodayForeColor, m_MonthBackColor, m_MonthForeColor;

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if (m_View == CalendarView.Month)
			{
				if (Control.ModifierKeys == Keys.Control)
				{
					this.NumWeekShown += -Math.Sign(e.Delta);
				}
				else
				{
					this.SetCurrentDateTime(m_FirstLastDayOfWeek.AddDays(-Math.Sign(e.Delta) * 7 * this.NumWeekShown / DEFAULT_NUMWEEKSHOWN));
				}
			}
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			if (e.Button == MouseButtons.Middle)
			{
				SetCurrentMonth(DateTime.Now);
			}
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);
			DateTime dt;
			if (m_Renderer.HitTestDate(this.PointToClient(Control.MousePosition), out dt))
			{
				m_View = CalendarView.Day;
				System.Diagnostics.Debug.WriteLine(dt);
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			if (this.Width > m_BackBuffer.Width || this.Height > m_BackBuffer.Height)
			{
				BeginUpdate();
				m_BackGraphics.Dispose();
				m_BackBuffer.Dispose();
				m_BackBuffer = new Bitmap(this.Width, this.Height);
				m_BackGraphics = Graphics.FromImage(m_BackBuffer);
				EndUpdate();
				this.Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (m_BeginUpdate == 0)
			{
				if (m_BackBuffer != null)
				{
					//e.Graphics.DrawImage(m_BackBuffer, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
					m_Renderer.Draw(e.Graphics);
				}
			}
		}

		private void btn_MouseEnter(object sender, EventArgs e)
		{
			(sender as Control).Invalidate();
		}

		private void btn_MouseLeave(object sender, EventArgs e)
		{
			(sender as Control).Invalidate();
		}

		private void btn_MouseDown(object sender, MouseEventArgs e)
		{
			(sender as Control).Invalidate();
		}

		private void btn_MouseUp(Control sender, MouseEventArgs e, Action action)
		{
			sender.Invalidate();
			if (e.Button == MouseButtons.Left && sender.ClientRectangle.Contains(e.Location))
			{
				action();
			}
		}

		private void btnToday_MouseUp(object sender, MouseEventArgs e)
		{
			btn_MouseUp
			(
				sender as Control,
				e,
				new Action
				(
					delegate
					{
						SetCurrentMonth(DateTime.Now);
					}
				)
			);
		}

		private void btnToday_Paint(object sender, PaintEventArgs e)
		{
			m_Renderer.PaintButtonToday(sender as Control, e.Graphics);
		}

		private void btnNextMonth_MouseUp(object sender, MouseEventArgs e)
		{
			btn_MouseUp
			(
				sender as Control,
				e,
				new Action
				(
					delegate
					{
						SetCurrentMonth(m_FirstLastDayOfWeek.AddMonths(1));
					}
				)
			);
		}

		private void btnNextMonth_Paint(object sender, PaintEventArgs e)
		{
			m_Renderer.PaintButtonNextMonth(sender as Control, e.Graphics);
		}

		private void btnPrevMonth_MouseUp(object sender, MouseEventArgs e)
		{
			btn_MouseUp
			(
				sender as Control,
				e,
				new Action
				(
					delegate
					{
						SetCurrentMonth(m_FirstLastDayOfWeek.AddMonths(-1));
					}
				)
			);
		}

		private void btnPrevMonth_Paint(object sender, PaintEventArgs e)
		{
			m_Renderer.PaintButtonPrevMonth(sender as Control, e.Graphics);
		}

		void request_redraw()
		{
			if (m_BeginUpdate == 0)
			{
				if (m_BackGraphics != null)
				{
					//m_Renderer.Draw(m_BackGraphics);
					this.Invalidate();
				}
			}
		}

		void get_metrics()
		{
			m_ButtonSize = SystemInformation.IconSize.Height;
			m_TopbarSize = m_ButtonSize;
		}

		void set_navi_bounds()
		{
			btnToday.Size = btnNextMonth.Size = btnPrevMonth.Size = new Size(m_ButtonSize, m_ButtonSize);
			btnNextMonth.Location = new Point(this.ClientRectangle.Right - btnNextMonth.Width, this.ClientRectangle.Top);
			btnPrevMonth.Location = new Point(btnNextMonth.Left - btnPrevMonth.Width, this.ClientRectangle.Top);
			btnToday.Location = new Point(btnPrevMonth.Left - btnToday.Width, this.ClientRectangle.Top);
		}

		Label new_Label()
		{
			Label lbl = new Label();
			lbl.ForeColor = m_NavigationColor;
			lbl.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lbl.MouseEnter += btn_MouseEnter;
			lbl.MouseLeave += btn_MouseLeave;
			lbl.MouseDown += btn_MouseDown;
			return lbl;
		}
	}
}
