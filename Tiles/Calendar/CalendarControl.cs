using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Win32;
using pWonders.App.DockLive.TileInterface;

namespace pWonders.App.DockLive.Tiles.Calendar
{
	partial class CalendarControl : TileChildControl
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

		public static bool DefaultShowAltCalendar
		{
			get { return true; }
		}

		public CalendarControl(ITile tile) : base(tile)
		{
			m_Renderer = new CalendarRenderer(this);
			m_NumWeekShown = DEFAULT_NUMWEEKSHOWN;

			this.DoubleBuffered = true;
			m_NumWeekShown = 5;
			m_ShowAltCalendar = CalendarControl.DefaultShowAltCalendar;
			m_View = CalendarView.Month;
			init_Colors();

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

		public DateTime GetFirstFirstDayOfWeek(DateTime month)
		{
			DateTime dt = new DateTime(month.Year, month.Month, 1);
			if (dt.DayOfWeek != DayOfWeek.Sunday)
			{
				dt = dt.AddDays(7 - (int) dt.DayOfWeek);
			}
			return dt;
		}

		public DateTime GetLastLastDayOfWeek(DateTime month)
		{
			DateTime dt = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
			if (dt.DayOfWeek != DayOfWeek.Saturday)
			{
				dt = dt.AddDays(-(int) dt.DayOfWeek - 1);
			}
			return dt;
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

		public bool IsDateTimeVisible(DateTime time)
		{
			DateTime dt = m_FirstLastDayOfWeek.AddDays(-6);
			dt = new DateTime(dt.Year, dt.Month, dt.Day);
			if (time < dt)
			{
				return false;
			}
			dt = m_FirstLastDayOfWeek.AddDays((m_NumWeekShown - 1) * 7);
			dt = new DateTime(dt.Year, dt.Month, dt.Day);
			if (dt < time)
			{
				return false;
			}
			return true;
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

		public bool ShowAltCalendar
		{
			set
			{
				if (m_ShowAltCalendar != value)
				{
					m_ShowAltCalendar = value;
					this.Invalidate();
				}
			}
			get { return m_ShowAltCalendar; }
		}

		const int DEFAULT_NUMWEEKSHOWN = 18;
		static GregorianCalendar s_GregorianCalendar;

		int m_TopbarSize;
		int m_ButtonSize;
		Label btnToday, btnNextMonth, btnPrevMonth;
		int m_BeginUpdate;
		DateTime m_FirstLastDayOfWeek;
		CalendarRenderer m_Renderer;
		int m_NumWeekShown;
		CalendarView m_View;
		bool m_ShowAltCalendar;

		protected override void OnThemeChanged(EventArgs e)
		{
			base.OnThemeChanged(e);
			switch (base.Theme)
			{
			case AppTheme.System:
				int a_header = 0xff, a_fg = 0xff, a_today = 0xff, a_month = 0xbf;
				this.BackColor = Color.Transparent;
				SetColor(AppThemeColor.NavForeColor, Color.FromArgb(a_header, UIColor.Background));
				SetColor(AppThemeColor.NavHiliBackColor, Color.FromArgb(a_header, UIColor.Accent));
				SetColor(AppThemeColor.NavHiliBackColor2, Color.FromArgb(a_header, UIColor.AccentDark2));
				SetColor(AppThemeColor.YearForeColor, Color.FromArgb(a_header, UIColor.Background));
				SetColor(AppThemeColor.MonthForeColor, Color.FromArgb(a_header, UIColor.Background));
				SetColor(AppThemeColor.WeekdayForeColor, Color.FromArgb(a_fg, UIColor.AccentLight3));
				SetColor(AppThemeColor.SunForeColor, Color.FromArgb(a_fg, UIColor.AccentLight1));
				SetColor(AppThemeColor.SatForeColor, Color.FromArgb(a_fg, UIColor.AccentLight2));
				SetColor(AppThemeColor.WeekNumForeColor, Color.FromArgb(a_fg, UIColor.AccentDark3));
				SetColor(AppThemeColor.DayForeColor, Color.FromArgb(a_fg, UIColor.Background));
				SetColor(AppThemeColor.DayForeColor2, Color.FromArgb(a_fg, UIColor.Foreground));
				SetColor(AppThemeColor.DayAltForeColor, Color.FromArgb(a_fg, UIColor.AccentDark1));
				SetColor(AppThemeColor.DayAltForeColor2, Color.FromArgb(a_fg, UIColor.AccentDark3));
				SetColor(AppThemeColor.TodayBackColor, Color.FromArgb(a_today, UIColor.Accent));
				SetColor(AppThemeColor.TodayForeColor, Color.FromArgb(a_today, UIColor.Background));
				SetColor(AppThemeColor.BigMonthBackColor, Color.FromArgb(a_month, UIColor.AccentDark3));
				SetColor(AppThemeColor.BigMonthForeColor, Color.FromArgb(a_fg, UIColor.AccentDark3));
				SetColor(AppThemeColor.BigMonthForeColor2, Color.FromArgb(a_fg, UIColor.AccentDark2));
				break;
			case AppTheme.Dark:
				break;
			case AppTheme.Light:
				break;
			}
			request_redraw();
		}

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

		void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
		{
			if (e.Category == UserPreferenceCategory.Window)
			{
				get_metrics();
				set_navi_bounds();
				request_redraw();
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
							SetCurrentDateTime(m_FirstLastDayOfWeek.AddDays(7));
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
							SetCurrentDateTime(m_FirstLastDayOfWeek.AddDays(-7));
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
			lbl.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			lbl.MouseEnter += btn_MouseEnter;
			lbl.MouseLeave += btn_MouseLeave;
			lbl.MouseDown += btn_MouseDown;
			return lbl;
		}
	}
}
