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
			public static string Title
			{
				get
				{
					RegistryKey root = Registry.ClassesRoot;
					RegistryKey key = null;
					try
					{
						key = root.OpenSubKey(REGKEY);
						return key.GetValue(null).ToString();
					}
					catch { }
					finally
					{
						if (key != null)
						{
							key.Close();
						}
					}
					return TITLE_EN;
				}
			}
			public static IntPtr Handle
			{
				get { return API.FindWindow(CLASS, Title); }
			}
			public static bool Visible
			{
				get { return API.IsWindowVisible(Handle); }
			}
			public static Rectangle Bounds
			{
				set
				{
					API.SetWindowPos(Handle, IntPtr.Zero, value.Left, value.Top, value.Width, value.Height, API.SWP_NOZORDER);
				}
				get
				{
					API.RECT r;
					if (API.GetWindowRect(Handle, out r))
					{
						return new Rectangle(r.left, r.top, r.right - r.left, r.bottom - r.top);
					}
					return Rectangle.Empty;
				}
			}
			const string REGKEY = "AppXak1hygz1tpjjnxhr1pwtcgnkpr24r5e7";
			const string TITLE_EN = "Action center";
			const string CLASS = "Windows.UI.Core.CoreWindow";
		}

		static Screen s_Screen;
	}
}
