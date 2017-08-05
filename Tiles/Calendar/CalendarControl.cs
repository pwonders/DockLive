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
			m_NumWeekShown = 5;
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

		public void BeginUpdate()
		{
			++m_BeginUpdate;
		}

		public void EndUpdate(bool invalidate)
		{
			--m_BeginUpdate;
			if (invalidate)
			{
				request_redraw();
			}
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
						break;
					case AppTheme.Light:
						break;
					case AppTheme.System:
						int a_header = 0xff, a_fg = 0xff, a_today = 0xff, a_month = 0xbf;
						this.BackColor = Color.FromArgb(0, UIColor.AccentDark2);
						this.NavForeColor = Color.FromArgb(a_header, UIColor.Background);
						m_NavHiliBackColor = Color.FromArgb(a_header, UIColor.Accent);
						m_NavHiliBackColor2 = Color.FromArgb(a_header, UIColor.AccentDark2);
						m_YearForeColor = Color.FromArgb(a_header, UIColor.Background);
						m_MonthForeColor = Color.FromArgb(a_header, UIColor.Background);
						m_SunForeColor = Color.FromArgb(a_fg, UIColor.AccentLight1);
						m_SatForeColor = Color.FromArgb(a_fg, UIColor.AccentLight2);
						m_WeekdayForeColor = Color.FromArgb(a_fg, UIColor.AccentLight3);
						m_WeekNumForeColor = Color.FromArgb(a_fg, UIColor.AccentDark3);
						m_DayForeColor = Color.FromArgb(a_fg, UIColor.Background);
						m_DayForeColor2 = Color.FromArgb(a_fg, UIColor.Foreground);
						m_DayAltForeColor = Color.FromArgb(a_fg, UIColor.AccentDark1);
						m_DayAltForeColor2 = Color.FromArgb(a_fg, UIColor.AccentDark3);
						m_TodayBackColor = Color.FromArgb(a_today, UIColor.Accent);
						m_TodayForeColor = Color.FromArgb(a_today, UIColor.Background);
						m_BigMonthForeColor = Color.FromArgb(a_fg, UIColor.AccentDark3);
						m_BigMonthForeColor2 = Color.FromArgb(a_fg, UIColor.AccentDark2);
						m_BigMonthBackColor = Color.FromArgb(a_month, UIColor.AccentDark3);
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
				if (value < 0) value = 0;
				if (value > 52) value = 52;
				if (m_NumWeekShown != value)
				{
					m_NumWeekShown = value;
					request_redraw();
				}
			}
			get { return m_NumWeekShown; }
		}

		public Color NavForeColor
		{
			set
			{
				if (m_NavForeColor != value)
				{
					m_NavForeColor = value;
					btnToday.ForeColor = m_NavForeColor;
					btnPrevMonth.ForeColor = m_NavForeColor;
					btnNextMonth.ForeColor = m_NavForeColor;
					request_redraw();
				}
			}
			get { return m_NavForeColor; }
		}

		public Color NavHiliBackColor
		{
			set { if (m_NavHiliBackColor != value) { m_NavHiliBackColor = value; request_redraw(); } }
			get { return m_NavHiliBackColor; }
		}

		public Color NavHiliBackColor2
		{
			set { if (m_NavHiliBackColor2 != value) { m_NavHiliBackColor2 = value; request_redraw(); } }
			get { return m_NavHiliBackColor2; }
		}

		public Color YearForeColor
		{
			set { if (m_YearForeColor != value) { m_YearForeColor = value; request_redraw(); } }
			get { return m_YearForeColor; }
		}

		public Color MonthForeColor
		{
			set { if (m_MonthForeColor != value) { m_MonthForeColor = value; request_redraw(); } }
			get { return m_MonthForeColor; }
		}

		public Color SunForeColor
		{
			set { if (m_SunForeColor != value) { m_SunForeColor = value; request_redraw(); } }
			get { return m_SunForeColor; }
		}

		public Color SatForeColor
		{
			set { if (m_SatForeColor != value) { m_SatForeColor = value; request_redraw(); } }
			get { return m_SatForeColor; }
		}

		public Color WeekdayForeColor
		{
			set { if (m_WeekdayForeColor != value) { m_WeekdayForeColor = value; request_redraw(); } }
			get { return m_WeekdayForeColor; }
		}

		public Color WeekNumForeColor
		{
			set { if (m_WeekNumForeColor != value) { m_WeekNumForeColor = value; request_redraw(); } }
			get { return m_WeekNumForeColor; }
		}

		public Color DayForeColor
		{
			set { if (m_DayForeColor != value) { m_DayForeColor = value; request_redraw(); } }
			get { return m_DayForeColor; }
		}

		public Color DayForeColor2
		{
			set { if (m_DayForeColor2 != value) { m_DayForeColor2 = value; request_redraw(); } }
			get { return m_DayForeColor2; }
		}

		public Color DayAltForeColor
		{
			set { if (m_DayAltForeColor != value) { m_DayAltForeColor = value; request_redraw(); } }
			get { return m_DayAltForeColor; }
		}

		public Color DayAltForeColor2
		{
			set { if (m_DayAltForeColor2 != value) { m_DayAltForeColor2 = value; request_redraw(); } }
			get { return m_DayAltForeColor2; }
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

		public Color BigMonthForeColor
		{
			set { if (m_BigMonthForeColor != value) { m_BigMonthForeColor = value; request_redraw(); } }
			get { return m_BigMonthForeColor; }
		}

		public Color BigMonthForeColor2
		{
			set { if (m_BigMonthForeColor2 != value) { m_BigMonthForeColor2 = value; request_redraw(); } }
			get { return m_BigMonthForeColor2; }
		}

		public Color BigMonthBackColor
		{
			set { if (m_BigMonthBackColor != value) { m_BigMonthBackColor = value; request_redraw(); } }
			get { return m_BigMonthBackColor; }
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

		int m_TopbarSize;
		int m_ButtonSize;
		Label btnToday, btnNextMonth, btnPrevMonth;
		int m_BeginUpdate;
		DateTime m_FirstLastDayOfWeek;
		CalendarRenderer m_Renderer;
		int m_NumWeekShown;
		AppTheme m_Theme;
		CalendarView m_View;

		Color m_NavForeColor, m_NavHiliBackColor, m_NavHiliBackColor2;
		Color m_YearForeColor, m_MonthForeColor;
		Color m_SunForeColor, m_SatForeColor, m_WeekdayForeColor, m_WeekNumForeColor;
		Color m_DayForeColor, m_DayForeColor2, m_DayAltForeColor, m_DayAltForeColor2;
		Color m_TodayBackColor, m_TodayForeColor;
		Color m_BigMonthForeColor, m_BigMonthForeColor2, m_BigMonthBackColor;

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if (m_View == CalendarView.Month)
			{
				if (Control.ModifierKeys.HasFlag(Keys.Control) == false)
				{
					this.SetCurrentDateTime(m_FirstLastDayOfWeek.AddDays(-Math.Sign(e.Delta) * 7));
				}
				else
				{
					this.NumWeekShown += -Math.Sign(e.Delta);
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
				//m_View = CalendarView.Day;
				System.Diagnostics.Debug.WriteLine(dt);
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (m_BeginUpdate == 0)
			{
				if (e.ClipRectangle.Width == this.ClientRectangle.Width)
				{
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
						if (Control.ModifierKeys.HasFlag(Keys.Shift) == false)
						{
							SetCurrentMonth(m_FirstLastDayOfWeek.AddMonths(1));
						}
						else
						{
							SetCurrentMonth(m_FirstLastDayOfWeek.AddDays(7));
						}
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
						if (Control.ModifierKeys.HasFlag(Keys.Shift) == false)
						{
							SetCurrentMonth(m_FirstLastDayOfWeek.AddMonths(-1));
						}
						else
						{
							SetCurrentMonth(m_FirstLastDayOfWeek.AddDays(-7));
						}
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
				this.Invalidate();
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
			lbl.ForeColor = m_NavForeColor;
			lbl.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lbl.MouseEnter += btn_MouseEnter;
			lbl.MouseLeave += btn_MouseLeave;
			lbl.MouseDown += btn_MouseDown;
			return lbl;
		}
	}
}
