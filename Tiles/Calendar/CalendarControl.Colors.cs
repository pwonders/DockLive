using System;
using System.Collections.Generic;
using System.Drawing;

namespace pWonders.App.DockLive.Tiles.Calendar
{
	public enum AppThemeColor
	{
		NavForeColor,
		NavHiliBackColor,
		NavHiliBackColor2,
		YearForeColor,
		MonthForeColor,
		WeekdayForeColor,
		SunForeColor,
		SatForeColor,
		WeekNumForeColor,
		DayForeColor,
		DayForeColor2,
		DayAltForeColor,
		DayAltForeColor2,
		TodayBackColor,
		TodayForeColor,
		BigMonthBackColor,
		BigMonthForeColor,
		BigMonthForeColor2,
	}

	partial class CalendarControl
	{
		public void SetColor(AppThemeColor index, Color value)
		{
			if (m_ThemeColors[index] != value)
			{
				m_ThemeColors[index] = value;
				if (m_ThemeBrushes[index] != null)
				{
					m_ThemeBrushes[index].Dispose();
				}
				m_ThemeBrushes[index] = new SolidBrush(value);
				request_redraw();
				switch (index)
				{
				case AppThemeColor.NavForeColor:
					btnToday.ForeColor = btnPrevMonth.ForeColor = btnNextMonth.ForeColor = value;
					break;
				}
			}
		}

		public Color GetColor(AppThemeColor index)
		{
			return m_ThemeColors[index];
		}

		public Brush GetBrush(AppThemeColor index)
		{
			return m_ThemeBrushes[index];
		}

		Dictionary<AppThemeColor, Color> m_ThemeColors;
		Dictionary<AppThemeColor, Brush> m_ThemeBrushes;

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

		void init_Colors()
		{
			m_ThemeColors = new Dictionary<AppThemeColor, Color>();
			m_ThemeColors.Add(AppThemeColor.NavForeColor, Color.Empty);
			m_ThemeColors.Add(AppThemeColor.NavHiliBackColor, Color.Empty);
			m_ThemeColors.Add(AppThemeColor.NavHiliBackColor2, Color.Empty);
			m_ThemeColors.Add(AppThemeColor.YearForeColor, Color.Empty);
			m_ThemeColors.Add(AppThemeColor.MonthForeColor, Color.Empty);
			m_ThemeColors.Add(AppThemeColor.WeekdayForeColor, Color.Empty);
			m_ThemeColors.Add(AppThemeColor.SunForeColor, Color.Empty);
			m_ThemeColors.Add(AppThemeColor.SatForeColor, Color.Empty);
			m_ThemeColors.Add(AppThemeColor.WeekNumForeColor, Color.Empty);
			m_ThemeColors.Add(AppThemeColor.DayForeColor, Color.Empty);
			m_ThemeColors.Add(AppThemeColor.DayForeColor2, Color.Empty);
			m_ThemeColors.Add(AppThemeColor.DayAltForeColor, Color.Empty);
			m_ThemeColors.Add(AppThemeColor.DayAltForeColor2, Color.Empty);
			m_ThemeColors.Add(AppThemeColor.TodayBackColor, Color.Empty);
			m_ThemeColors.Add(AppThemeColor.TodayForeColor, Color.Empty);
			m_ThemeColors.Add(AppThemeColor.BigMonthBackColor, Color.Empty);
			m_ThemeColors.Add(AppThemeColor.BigMonthForeColor, Color.Empty);
			m_ThemeColors.Add(AppThemeColor.BigMonthForeColor2, Color.Empty);
			m_ThemeBrushes = new Dictionary<AppThemeColor, Brush>();
			m_ThemeBrushes.Add(AppThemeColor.NavForeColor, null);
			m_ThemeBrushes.Add(AppThemeColor.NavHiliBackColor, null);
			m_ThemeBrushes.Add(AppThemeColor.NavHiliBackColor2, null);
			m_ThemeBrushes.Add(AppThemeColor.YearForeColor, null);
			m_ThemeBrushes.Add(AppThemeColor.MonthForeColor, null);
			m_ThemeBrushes.Add(AppThemeColor.WeekdayForeColor, null);
			m_ThemeBrushes.Add(AppThemeColor.SunForeColor, null);
			m_ThemeBrushes.Add(AppThemeColor.SatForeColor, null);
			m_ThemeBrushes.Add(AppThemeColor.WeekNumForeColor, null);
			m_ThemeBrushes.Add(AppThemeColor.DayForeColor, null);
			m_ThemeBrushes.Add(AppThemeColor.DayForeColor2, null);
			m_ThemeBrushes.Add(AppThemeColor.DayAltForeColor, null);
			m_ThemeBrushes.Add(AppThemeColor.DayAltForeColor2, null);
			m_ThemeBrushes.Add(AppThemeColor.TodayBackColor, null);
			m_ThemeBrushes.Add(AppThemeColor.TodayForeColor, null);
			m_ThemeBrushes.Add(AppThemeColor.BigMonthBackColor, null);
			m_ThemeBrushes.Add(AppThemeColor.BigMonthForeColor, null);
			m_ThemeBrushes.Add(AppThemeColor.BigMonthForeColor2, null);
		}
	}
}
