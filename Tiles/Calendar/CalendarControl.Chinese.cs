using System;
using System.Globalization;

namespace pWonders.App.DockLive.Tiles.Calendar
{
	partial class CalendarControl
	{
		public class Chinese
		{
			static Chinese()
			{
				s_ChineseLunisolarCalendar = new ChineseLunisolarCalendar();
				s_CultureInfo = new CultureInfo("zh");
			}

			public static ChineseLunisolarCalendar ChineseLunisolarCalendar
			{
				get { return s_ChineseLunisolarCalendar; }
			}

			public static CultureInfo CultureInfo
			{
				get { return s_CultureInfo; }
			}

			public static string GetDayNames(int day)
			{
				return s_DayNames[day - 1];
			}

			public static string GetMonthNames(int month)
			{
				return s_CultureInfo.DateTimeFormat.MonthNames[month - 1];
			}

			public static int GetNormalizedMonth(DateTime time)
			{
				int year = s_ChineseLunisolarCalendar.GetYear(time);
				int month = s_ChineseLunisolarCalendar.GetMonth(time);	// This may include leap month.
				for (int m = 1; m <= month; ++m)
				{
					if (s_ChineseLunisolarCalendar.IsLeapMonth(year, m))
					{
						return month - 1;
					}
				}
				return month;
			}

			public static string GetSolarTermNames(int term)
			{
				return s_SolarTermNames[term - 1];
			}

			public static DateTime ToChineseDateTime(DateTime time)
			{
				int y = s_ChineseLunisolarCalendar.GetYear(time);
				int m = s_ChineseLunisolarCalendar.GetMonth(time);
				int d = s_ChineseLunisolarCalendar.GetDayOfMonth(time);
				return new DateTime(y, m, d);
			}

			static ChineseLunisolarCalendar s_ChineseLunisolarCalendar;
			static CultureInfo s_CultureInfo;

			static string[] s_DayNames =
			{
				"初一", "初二", "初三", "初四", "初五", "初六", "初七", "初八", "初九", "初十", 
				"十一", "十二", "十三", "十四", "十五", "十六", "十七", "十八", "十九", "二十", 
				"廿一", "廿二", "廿三", "廿四", "廿五", "廿六", "廿七", "廿八", "廿九", "三十", 
				"卅一"
			};
			static string[] s_SolarTermNames =
			{
				"立春", "雨水", "驚蟄", "春分", "清明", "穀雨", 
				"立夏", "小滿", "芒種", "夏至", "小暑", "大暑", 
				"立秋", "處暑", "白露", "秋分", "寒露", "霜降", 
				"立冬", "小雪", "大雪", "冬至", "小寒", "大寒"
			};
		}
	}
}
