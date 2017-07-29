using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace pWonders.App.DockLive
{
	class Desktop
	{
		public static void SetCurrentScreen()
		{
			s_Screen = Screen.FromPoint(Cursor.Position);
		}

		public static Screen Screen
		{
			get { return s_Screen; }
		}

		public class Taskbar
		{
			public static bool TopOrBottom()
			{
				return s_Screen.Bounds.Bottom != s_Screen.WorkingArea.Bottom || s_Screen.Bounds.Top != s_Screen.WorkingArea.Top;
			}

			public static bool LeftOrRight()
			{
				return s_Screen.Bounds.Left != s_Screen.WorkingArea.Left || s_Screen.Bounds.Right != s_Screen.WorkingArea.Right;
			}
		}

		public static class Mouse
		{
			public static bool Dragging
			{
				get { return Control.MouseButtons == MouseButtons.None; }
			}

			public static bool NearLeftEdge(int tolerance)
			{
				// Since this is horizontal check,
				// allow some tolerance horizontally for a fuzzy detection,
				// but remove tolerance vertically for an exact match.
				return s_Screen.WorkingArea.Left - tolerance < Cursor.Position.X && Cursor.Position.X < s_Screen.WorkingArea.Left + tolerance
					&& s_Screen.WorkingArea.Top + tolerance < Cursor.Position.Y && Cursor.Position.Y < s_Screen.WorkingArea.Bottom - tolerance;
			}

			public static bool NearRightEdge(int tolerance)
			{
				// Since this is horizontal check,
				// allow some tolerance horizontally for a fuzzy detection,
				// but remove tolerance vertically for an exact match.
				return s_Screen.WorkingArea.Right - tolerance < Cursor.Position.X && Cursor.Position.X < s_Screen.WorkingArea.Right + tolerance
					&& s_Screen.WorkingArea.Top + tolerance < Cursor.Position.Y && Cursor.Position.Y < s_Screen.WorkingArea.Bottom - tolerance;
			}
		}

		public static class ActionCenter
		{
			public static int Width
			{
				get
				{
					// FIXME: handle localization.
					IntPtr h = g.FindWindow(CLASS, TITLE_EN);
					g.RECT r;
					if (g.GetWindowRect(h, out r))
					{
						return r.Right - r.Left;
					}
					return 0;
				}
			}
			const string CLASS = "Windows.UI.Core.CoreWindow";
			const string TITLE_EN = "Action center";
		}

		static Screen s_Screen;
	}
}
