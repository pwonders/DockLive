using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace pWonders.App.DockLive.Tiles.Calendar
{
	class CalendarRenderer
	{
		public CalendarRenderer(CalendarControl hostControl)
		{
			m_HostControl = hostControl ?? throw new ArgumentNullException();

			m_TodayDrawFmt = new StringFormat(StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap);
			m_TodayDrawFmt.Alignment = StringAlignment.Center;
			m_TodayDrawFmt.LineAlignment = StringAlignment.Center;
			m_YearDrawFmt = new StringFormat(StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap);
			m_YearDrawFmt.Alignment = StringAlignment.Near;
			m_YearDrawFmt.LineAlignment = StringAlignment.Center;
			m_MonthDrawFmt = new StringFormat(StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap);
			m_MonthDrawFmt.Alignment = StringAlignment.Near;
			m_MonthDrawFmt.LineAlignment = StringAlignment.Center;
			m_DayOfWeekDrawFmt = new StringFormat(StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap);
			m_DayOfWeekDrawFmt.Alignment = StringAlignment.Near;
			m_DayOfWeekDrawFmt.LineAlignment = StringAlignment.Far;
			m_WeekNumDrawFmt = new StringFormat(StringFormatFlags.DirectionVertical | StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap);
			m_WeekNumDrawFmt.Alignment = StringAlignment.Center;
			m_WeekNumDrawFmt.LineAlignment = StringAlignment.Center;
			m_WeekNumDrawFmt.Trimming = StringTrimming.None;
			m_DayDrawFmt = new StringFormat(StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap);
			m_DayDrawFmt.Alignment = StringAlignment.Center;
			m_DayDrawFmt.LineAlignment = StringAlignment.Near;
			m_DayDrawFmt.Trimming = StringTrimming.None;
			m_DayAltDrawFmt = new StringFormat(StringFormatFlags.FitBlackBox);
			m_DayAltDrawFmt.Alignment = StringAlignment.Center;
			m_DayAltDrawFmt.LineAlignment = StringAlignment.Near;
			m_DayAltDrawFmt.Trimming = StringTrimming.Character;
			m_DayAltTestFmt = m_DayAltDrawFmt.Clone() as StringFormat;
			m_DayAltTestFmt.FormatFlags |= StringFormatFlags.NoClip;
			m_BigMonthDrawFmt = new StringFormat(StringFormatFlags.FitBlackBox | StringFormatFlags.NoWrap);
			m_BigMonthDrawFmt.Alignment = StringAlignment.Center;
			m_BigMonthDrawFmt.LineAlignment = StringAlignment.Center;
			m_BigMonthDrawFmt.Trimming = StringTrimming.None;
		}

		public virtual bool HitTestDate(Point pt, out DateTime time)
		{
			using (Graphics g = m_HostControl.CreateGraphics())
			{
				int x_day = m_HostControl.CalendarContentRectangle.Left + get_weeknum_width(g);
				int y_day = m_HostControl.CalendarContentRectangle.Top + m_HostControl.CalendarTopRectangle.Height + (int) get_daysofweek_height(g);
				if (pt.X >= x_day && pt.Y >= y_day)
				{
					pt.Offset(-x_day, -y_day);
					DateTime dt = m_HostControl.GetFirstLastDayOfWeek();
					float cx_day = get_day_width(g);
					float cy_day = get_week_height(g);
					int day_offset = (int) (pt.X / cx_day);
					int week_offset = (int) (pt.Y / cy_day);
					time = dt.AddDays(day_offset - 6).AddDays(7 * week_offset);
					return true;
				}
				else
				{
					time = DateTime.MinValue;
					return false;
				}
			}
		}

		public void Draw(Graphics g)
		{
			setup_Graphics(g);
			draw_calendar(g, m_HostControl.GetFirstLastDayOfWeek());
		}

		public virtual void DrawYearMonth(Graphics g, DateTime dateTime_last_dayofweek)
		{
			int x = m_HostControl.CalendarTopRectangle.Left + SystemInformation.HorizontalResizeBorderThickness;
			int y = m_HostControl.CalendarTopRectangle.Top;
			int cx = m_HostControl.CalendarTopRectangle.Width - SystemInformation.HorizontalResizeBorderThickness * 2;
			int cy = m_HostControl.CalendarTopRectangle.Height;
			Rectangle rect = new Rectangle(x, y, cx, cy);

			using (Brush br_year = new SolidBrush(m_HostControl.YearForeColor))
			using (Brush br_month = new SolidBrush(m_HostControl.MonthForeColor))
			using (Font font = new Font(m_HostControl.Font.FontFamily, get_point_from_pixel(cy, g) * 5 / 6, FontStyle.Regular))
			{
				g.DrawString(dateTime_last_dayofweek.Year.ToString(), font, br_year, rect, m_YearDrawFmt);
				int cx_year = (int) g.MeasureString(dateTime_last_dayofweek.Year.ToString(), font, cx, m_YearDrawFmt).Width;
				rect.X += cx_year;
				rect.Width -= cx_year;
				g.DrawString(dateTime_last_dayofweek.ToString("MMMM"), font, br_month, rect, m_MonthDrawFmt);
			}
		}

		public virtual void DrawDaysOfWeek(Graphics g)
		{
			int x_dayofweek = m_HostControl.CalendarContentRectangle.Left + get_weeknum_width(g);
			int y_dayofweek = m_HostControl.CalendarTopRectangle.Bottom;
			float cx_dayofweek = get_day_width(g);
			int cy_dayofweek = (int) get_daysofweek_height(g);
			RectangleF rect_dayofweek = new RectangleF(x_dayofweek, y_dayofweek, cx_dayofweek, cy_dayofweek);

			using (Brush br_sun = new SolidBrush(m_HostControl.SunForeColor))
			using (Brush br_sat = new SolidBrush(m_HostControl.SatForeColor))
			using (Brush br_day = new SolidBrush(m_HostControl.WeekdayForeColor))
			using (Brush br_todaybg = new SolidBrush(m_HostControl.TodayBackColor))
			{
				string[] day_names = DateTimeFormatInfo.InvariantInfo.AbbreviatedDayNames;
				Brush[] br_dayofweek = new Brush[7] { br_sun, br_day, br_day, br_day, br_day, br_day, br_sat };
				for (int d = 0; d < 7; ++d, rect_dayofweek.X += cx_dayofweek)
				{
					if (d == (int) m_DateTimeNow.DayOfWeek)
					{
						g.FillRectangle(br_todaybg, rect_dayofweek);
					}
					g.DrawString(day_names[d], m_HostControl.Font, br_dayofweek[d], rect_dayofweek, m_DayOfWeekDrawFmt);
				}
			}
		}

		public virtual void DrawWeekNumbers(Graphics g, DateTime dateTime_last_dayofweek)
		{
			int x_weeknum = m_HostControl.CalendarContentRectangle.Left;
			int y_weeknum = m_HostControl.CalendarTopRectangle.Bottom + (int) get_daysofweek_height(g);
			int cx_weeknum = get_weeknum_width(g);
			float cy_weeknum = get_week_height(g);
			int cy_weeknum_content = get_weeknum_width(g);
			RectangleF rect_weeknum = new RectangleF(x_weeknum, y_weeknum, cx_weeknum, cy_weeknum);
			RectangleF rect_weeknum_content = new RectangleF(x_weeknum, y_weeknum, cx_weeknum, cy_weeknum_content);
			int weeknum_this = CalendarControl.GetWeekOfYear(m_DateTimeNow);

			using (Brush brush = new SolidBrush(m_HostControl.WeekNumForeColor))
			using (Brush br_todaybg = new SolidBrush(m_HostControl.TodayBackColor))
			using (Brush br_todayfg = new SolidBrush(m_HostControl.TodayForeColor))
			{
				Brush br;
				for (int w = 0; w < m_HostControl.NumWeekShown; ++w, rect_weeknum.Y = rect_weeknum_content.Y += cy_weeknum, dateTime_last_dayofweek = dateTime_last_dayofweek.AddDays(7))
				{
					int weeknum = CalendarControl.GetWeekOfYear(dateTime_last_dayofweek);
					if (weeknum != weeknum_this || dateTime_last_dayofweek.Year != m_DateTimeNow.Year)
					{
						br = brush;
					}
					else
					{
						br = br_todayfg;
						g.FillRectangle(br_todaybg, rect_weeknum);
					}
					g.DrawString(weeknum.ToString(), m_HostControl.Font, br, rect_weeknum_content, m_WeekNumDrawFmt);
				}
			}
		}

		public virtual void DrawDates(Graphics g, DateTime dateTime_last_dayofweek)
		{
			int x_day = m_HostControl.CalendarContentRectangle.Left + get_weeknum_width(g);
			int y_day = m_HostControl.CalendarTopRectangle.Bottom + (int) get_daysofweek_height(g);
			float cx_day = get_day_width(g);
			float cy_day = get_week_height(g);
			float cx_day_content = get_day_content_width(g);
			float cy_day_content = get_day_content_height(g);
			RectangleF rect_day_box = new RectangleF(x_day, y_day, cx_day, cy_day);

			// Clip partial big month number away.
			g.SetClip(new Rectangle(x_day, y_day, m_HostControl.CalendarContentRectangle.Width - x_day, m_HostControl.CalendarContentRectangle.Height - y_day));

			DateTime dt = dateTime_last_dayofweek.AddDays(-6);

			// Draw day background, and background month.
			// Background month occupies the center box from first sun to last sat.
			// There're at least 3 full weeks for any month.
			using (Brush br_bigmonfg = new SolidBrush(m_HostControl.BigMonthForeColor))
			using (Brush br_bigmonfg2 = new SolidBrush(m_HostControl.BigMonthForeColor2))
			using (Brush br_bigmonbg = new SolidBrush(m_HostControl.BigMonthBackColor))
			using (Font font = get_font_for_size(new SizeF(cx_day * 7, cy_day * 3), m_HostControl.Font, FontStyle.Bold, g, "12", m_BigMonthDrawFmt))
			{
				RectangleF rect_area = RectangleF.Empty, rect_area2 = RectangleF.Empty;
				bool rect_area_ended = false, rect_area_ended2 = false;
				DateTime dt_area = DateTime.MinValue, dt_area2 = DateTime.MinValue;
				for (int w = 0; w < m_HostControl.NumWeekShown; ++w, rect_day_box.Y += cy_day, rect_day_box.X = x_day)
				{
					RectangleF rect_week = RectangleF.Empty, rect_week2 = RectangleF.Empty;
					rect_day_box.Height = (float) (Math.Round(rect_day_box.Y + cy_day) - Math.Round(rect_day_box.Y));
					for (int d = 0; d < 7; ++d, dt = dt.AddDays(1), rect_day_box.X += cx_day)
					{
						if (is_DateTime_OddMonth(dt) == false)
						{
							rect_week = rect_week.IsEmpty ? rect_day_box : RectangleF.Union(rect_week, rect_day_box);
							dt_area = new DateTime(dt.Ticks);
						}
						else
						{
							rect_week2 = rect_week2.IsEmpty ? rect_day_box : RectangleF.Union(rect_week2, rect_day_box);
							dt_area2 = new DateTime(dt.Ticks);
						}
					}
					// One week has passed...
					if (rect_week.IsEmpty == false)
					{
						// Is there no month transition?
						if ((int) Math.Round(rect_week.Left) == x_day && (int) Math.Round(rect_week.Right) == m_HostControl.CalendarContentRectangle.Right)
						{
							// There isn't, so add the rect to be filled at once.
							rect_area = rect_area.IsEmpty ? rect_week : RectangleF.Union(rect_area, rect_week);
							rect_area_ended = false;
						}
						else
						{
							// There is, so fill partial week.
							// Does it follow a larger rect?
							if (rect_week.Left == x_day)
							{
								rect_area_ended = true;
							}
							g.FillRectangle(br_bigmonbg, Rectangle.Round(rect_week));
						}
					}
					else
					{
						rect_area_ended = true;
					}
					if ((w == m_HostControl.NumWeekShown - 1 || rect_area_ended) && rect_area.IsEmpty == false)
					{
						g.FillRectangle(br_bigmonbg, Rectangle.Round(rect_area));
						DateTime dt_topleft = m_HostControl.GetFirstFirstDayOfWeek(dt_area);
						DateTime dt_bottomright = m_HostControl.GetLastLastDayOfWeek(dt_area);
						if (m_HostControl.IsDateTimeVisible(dt_topleft) == false)
						{
							float bottom = rect_area.Bottom;
							rect_area.Height = cy_day * get_num_of_full_weeks(dt_topleft, dt_bottomright);
							rect_area.Y = bottom - rect_area.Height;
						}
						else if (m_HostControl.IsDateTimeVisible(dt_bottomright) == false)
						{
							rect_area.Height = cy_day * get_num_of_full_weeks(dt_topleft, dt_bottomright);
						}
						string month_text = dt_area.Month > 1 ? dt_area.Month.ToString() : dt_area.Year.ToString();
						g.DrawString(month_text, font, br_bigmonfg, rect_area, m_BigMonthDrawFmt);
						rect_area = RectangleF.Empty;
					}
					if (rect_week2.IsEmpty == false)
					{
						if ((int) Math.Round(rect_week2.Left) == x_day && (int) Math.Round(rect_week2.Right) == m_HostControl.CalendarContentRectangle.Right)
						{
							rect_area2 = rect_area2.IsEmpty ? rect_week2 : RectangleF.Union(rect_area2, rect_week2);
							rect_area_ended2 = false;
						}
						else
						{
							if (rect_week2.Left == x_day)
							{
								rect_area_ended2 = true;
							}
						}
					}
					else
					{
						rect_area_ended2 = true;
					}
					if ((w == m_HostControl.NumWeekShown - 1 || rect_area_ended2) && rect_area2.IsEmpty == false)
					{
						DateTime dt_topleft = m_HostControl.GetFirstFirstDayOfWeek(dt_area2);
						DateTime dt_bottomright = m_HostControl.GetLastLastDayOfWeek(dt_area2);
						if (m_HostControl.IsDateTimeVisible(dt_topleft) == false)
						{
							float bottom = rect_area2.Bottom;
							rect_area2.Height = cy_day * get_num_of_full_weeks(dt_topleft, dt_bottomright);
							rect_area2.Y = bottom - rect_area2.Height;
						}
						else if (m_HostControl.IsDateTimeVisible(dt_bottomright) == false)
						{
							rect_area2.Height = cy_day * get_num_of_full_weeks(dt_topleft, dt_bottomright);
						}
						string month_text = dt_area2.Month > 1 ? dt_area2.Month.ToString() : dt_area2.Year.ToString();
						g.DrawString(month_text, font, br_bigmonfg2, rect_area2, m_BigMonthDrawFmt);
						rect_area2 = RectangleF.Empty;
					}
				}
			}

			dt = dateTime_last_dayofweek.AddDays(-6);
			rect_day_box.Y = y_day;

			using (Brush br_day = new SolidBrush(m_HostControl.DayForeColor))
			using (Brush br_day2 = new SolidBrush(m_HostControl.DayForeColor2))
			using (Brush br_dayalt = new SolidBrush(m_HostControl.DayAltForeColor))
			using (Brush br_dayalt2 = new SolidBrush(m_HostControl.DayAltForeColor2))
			using (Brush br_todaybg = new SolidBrush(m_HostControl.TodayBackColor))
			using (Brush br_todayfg = new SolidBrush(m_HostControl.TodayForeColor))
			{
				for (int w = 0; w < m_HostControl.NumWeekShown; ++w, rect_day_box.Y += cy_day, rect_day_box.X = x_day)
				{
					for (int d = 0; d < 7; ++d, rect_day_box.X += cx_day, dt = dt.AddDays(1))
					{
						Brush brush_fg;
						if (is_DateTime_Today(dt))
						{
							brush_fg = br_todayfg;
							g.FillRectangle(br_todaybg, rect_day_box);
						}
						else
						{
							brush_fg = is_DateTime_ThisMonth(dt) ? br_day : br_day2;
						}
						RectangleF rect_day_content = rect_day_box;
						rect_day_content.Width = cx_day_content;
						rect_day_content.Height = cy_day_content;
						g.DrawString(dt.Day.ToString(), m_HostControl.Font, brush_fg, rect_day_content, m_DayDrawFmt);

						RectangleF rect_dayalt_content = rect_day_content;
						rect_dayalt_content.Y += cy_day_content;
						rect_dayalt_content.Height = rect_day_box.Bottom - rect_dayalt_content.Y;
						brush_fg = (((dt.Month - m_DateTimeNow.Month) % 2) == 0) ? br_dayalt : br_dayalt2;
						DrawAltDates(g, dt, brush_fg, rect_dayalt_content, rect_day_box);
						DrawMoments(g, dt);
					}
				}
			}
		}

		public virtual void DrawAltDates(Graphics g, DateTime dateTime, Brush brush, RectangleF rect, RectangleF bounds)
		{
			if (true)   // Depends on CalendarControl show properties.
			{
				if (rect.Height > 0)
				{
					string alt_day_string;
					int alt_day = CalendarControl.Chinese.ChineseLunisolarCalendar.GetDayOfMonth(dateTime);
					if (alt_day != 1)
					{
						alt_day_string = CalendarControl.Chinese.GetDayNames(alt_day);
					}
					else
					{
						int alt_month = CalendarControl.Chinese.GetNormalizedMonth(dateTime);
						alt_day_string = CalendarControl.Chinese.GetMonthNames(alt_month);
					}
					//if (g.MeasureString(alt_day_string.Substring(0, 2), m_HostControl.Font, rect.Size, m_AltDayTestFmt).Height <= bounds.Bottom - rect.Top)
					{
						g.DrawString(alt_day_string, m_HostControl.Font, brush, rect, m_DayAltDrawFmt);
					}
				}
			}
		}

		public virtual void DrawMoments(Graphics g, DateTime dateTime)
		{

		}

		public virtual void PaintButtonToday(Control sender, Graphics g)
		{
			paint_button_nonclient
			(
				sender as Control,
				g,
				new Action<Rectangle, Pen>
				(
					delegate (Rectangle rect_border, Pen pen_client)
					{
						string day = m_DateTimeNow.Day.ToString();
						float size = get_point_from_pixel(rect_border.Height, g);
						if (day.Length > 1)
						{
							size *= 4 / 6.0f;
						}
						else
						{
							size *= 5 / 6.0f;
						}
						using (Brush brush = new SolidBrush(pen_client.Color))
						using (Font font = new Font(sender.Font.FontFamily, size, FontStyle.Regular))
						{
							g.DrawString(day, font, brush, rect_border, m_TodayDrawFmt);
						}
					}
				)
			);
		}

		public virtual void PaintButtonNextMonth(Control sender, Graphics g)
		{
			paint_button_nonclient
			(
				sender as Control,
				g,
				new Action<Rectangle, Pen>
				(
					delegate (Rectangle rect_border, Pen pen_client)
					{
						RectangleF rect_arrow = rect_border;
						float arrow_border_margin = rect_border.Height / 4F;
						rect_arrow.Inflate(-arrow_border_margin, -arrow_border_margin);
						PointF pt_top = new PointF(rect_arrow.Left + rect_arrow.Width / 2, rect_arrow.Top - 1);
						PointF pt_bot = new PointF(pt_top.X, rect_arrow.Bottom);
						PointF pt_left = new PointF(rect_arrow.Left, rect_arrow.Top + rect_arrow.Height / 2);
						PointF pt_right = new PointF(rect_arrow.Right, pt_left.Y);
						PointF[] points_left = new PointF[] { pt_top, pt_bot, pt_left };
						PointF[] points_right = new PointF[] { pt_top, pt_bot, pt_right };
						g.DrawCurve(pen_client, points_left);
						g.DrawCurve(pen_client, points_right);
					}
				)
			);
		}

		public virtual void PaintButtonPrevMonth(Control sender, Graphics g)
		{
			paint_button_nonclient
			(
				sender as Control,
				g,
				new Action<Rectangle, Pen>
				(
					delegate (Rectangle rect_border, Pen pen_client)
					{
						RectangleF rect_arrow = rect_border;
						float arrow_border_margin = rect_border.Height / 4F;
						rect_arrow.Inflate(-arrow_border_margin, -arrow_border_margin);
						PointF pt_top = new PointF(rect_arrow.Left + rect_arrow.Width / 2, rect_arrow.Top);
						PointF pt_bot = new PointF(pt_top.X, rect_arrow.Bottom + 1);
						PointF pt_left = new PointF(rect_arrow.Left, rect_arrow.Top + rect_arrow.Height / 2);
						PointF pt_right = new PointF(rect_arrow.Right, pt_left.Y);
						PointF[] points_left = new PointF[] { pt_bot, pt_top, pt_left };
						PointF[] points_right = new PointF[] { pt_bot, pt_top, pt_right };
						g.DrawCurve(pen_client, points_left);
						g.DrawCurve(pen_client, points_right);
					}
				)
			);
		}

		const int POINTS_PER_INCH = 72;
		CalendarControl m_HostControl;
		DateTime m_DateTimeNow;
		StringFormat m_TodayDrawFmt;
		StringFormat m_YearDrawFmt, m_MonthDrawFmt, m_DayOfWeekDrawFmt, m_WeekNumDrawFmt;
		StringFormat m_DayDrawFmt, m_DayAltDrawFmt, m_DayAltTestFmt, m_BigMonthDrawFmt;

		protected virtual float get_daysofweek_height(Graphics g)
		{
			return get_day_content_height(g);
		}

		protected virtual int get_weeknum_width(Graphics g)
		{
			return (int) get_day_content_height(g);
		}

		protected virtual float get_week_height(Graphics g)
		{
			return (m_HostControl.CalendarContentRectangle.Height - m_HostControl.CalendarTopRectangle.Height - get_daysofweek_height(g)) / (float) m_HostControl.NumWeekShown;
		}

		protected virtual float get_day_width(Graphics g)
		{
			return (m_HostControl.CalendarContentRectangle.Width - get_weeknum_width(g)) / 7.0F;
		}

		protected virtual float get_day_content_width(Graphics g)
		{
			return get_day_content_height(g);   // assume square.
		}

		protected virtual float get_day_content_height(Graphics g)
		{
			return g.MeasureString("0123456789", m_HostControl.Font).Height;
		}

		protected float get_pixel_from_point(float size_in_point, Graphics g)
		{
			return size_in_point * g.DpiX / POINTS_PER_INCH;
		}

		protected float get_point_from_pixel(int size_in_pixel, Graphics g)
		{
			return size_in_pixel * POINTS_PER_INCH / g.DpiY;
		}

		protected Font get_font_for_size(SizeF size, Font baseFont, FontStyle fontStyle, Graphics g, string text, StringFormat format)
		{
			float em_size_A = 1, em_size_B = size.Height;
			while (em_size_B - em_size_A > 1)
			{
				float em_size = (em_size_A + em_size_B) / 2;
				using (Font font = new Font(baseFont.FontFamily, em_size, fontStyle))
				{
					SizeF calculated_size = g.MeasureString(text, font, size, format);
					if (calculated_size.Width < size.Width && calculated_size.Height < size.Height)
					{
						em_size_A = em_size;
					}
					else
					{
						em_size_B = em_size;
					}
				}
			}
			return new Font(baseFont.FontFamily, em_size_A, fontStyle);
		}

		/*
		int get_week_of_year(DateTime dateTime_last_dayofweek)
		{
			DateTime dt_first_this_year = new DateTime(dateTime_last_dayofweek.Year, 1, 1);
			int days_in_first_week_this_year = 7 - (int) dt_first_this_year.DayOfWeek;
			DateTime dt_first_next_year = dt_first_this_year.AddYears(1);
			int days_in_first_week_next_year = 7 - (int) dt_first_next_year.DayOfWeek;

			int days_in_first_week = (dateTime_last_dayofweek.Year == dt_first_this_year.Year) ? days_in_first_week_this_year : days_in_first_week_next_year;
			int weeknum = (dateTime_last_dayofweek.DayOfYear - 1 - days_in_first_week + 7) / 7 + 1;

			return weeknum;
		}
		*/

		void setup_Graphics(Graphics g)
		{
			g.CompositingQuality = CompositingQuality.HighQuality;
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.PixelOffsetMode = PixelOffsetMode.HighQuality;
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
		}

		void draw_calendar(Graphics g, DateTime dateTime_last_dayofweek)
		{
			m_DateTimeNow = DateTime.Now;
			this.DrawYearMonth(g, dateTime_last_dayofweek);
			this.DrawDaysOfWeek(g);
			this.DrawWeekNumbers(g, dateTime_last_dayofweek);
			this.DrawDates(g, dateTime_last_dayofweek);
		}

		bool is_DateTime_Today(DateTime time)
		{
			return time.Year == m_DateTimeNow.Year && time.Month == m_DateTimeNow.Month && time.Day == m_DateTimeNow.Day;
		}

		bool is_DateTime_ThisMonth(DateTime time)
		{
			return time.Year == m_DateTimeNow.Year && time.Month == m_DateTimeNow.Month;
		}

		bool is_DateTime_OddMonth(DateTime time)
		{
			return ((time.Month - m_DateTimeNow.Month) % 2) != 0;
		}

		void paint_button_nonclient(Control btn, Graphics g, Action<Rectangle, Pen> paint_client)
		{
			setup_Graphics(g);
			Rectangle rect_border = btn.ClientRectangle;
			rect_border.Inflate(-SystemInformation.Border3DSize.Width, -SystemInformation.Border3DSize.Width);
			float border_pen_width = rect_border.Height / 20F;
			float client_pen_width = rect_border.Height / 20F;
			--rect_border.Width;
			--rect_border.Height;

			using (Pen pen_border = new Pen(btn.ForeColor, border_pen_width))
			{
				if (btn.ClientRectangle.Contains(btn.PointToClient(Control.MousePosition)))
				{
					Color brush_color;
					if (Control.MouseButtons == MouseButtons.None)
					{
						brush_color = m_HostControl.NavHiliBackColor;
					}
					else
					{
						brush_color = m_HostControl.NavHiliBackColor2;
					}
					using (Brush brush = new SolidBrush(brush_color))
					{
						g.FillEllipse(brush, rect_border);
					}
				}
				g.DrawEllipse(pen_border, rect_border);
				using (Pen pen_client = new Pen(btn.ForeColor, client_pen_width))
				{
					paint_client(rect_border, pen_client);
				}
			}
		}

		int get_num_of_full_weeks(DateTime dt_topleft, DateTime dt_bottomright)
		{
			return (int) (dt_bottomright.AddDays(1) - dt_topleft).TotalDays / 7;
		}
	}
}
