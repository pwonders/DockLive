using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace pWonders.App.DockLive
{
	class Animator
	{
		public event EventHandler ShowBegan = delegate { };
		public event EventHandler ShowEnded = delegate { };
		public event EventHandler HideBegan = delegate { };
		public event EventHandler HideEnded = delegate { };

		public Animator(DockForm form)
		{
			m_Form = form;
			m_TimerMouseActivate = new Timer();
			m_TimerMouseActivate.Tick += TimerMouseActivate_Tick;
			///m_TimerMouseActivate.Enabled = true;
			m_TimerAnimate = new Timer();
			m_TimerAnimate.Interval = 10;
			m_TimerAnimate.Tick += TimerAnimate_Tick;
		}

		public bool Showing
		{
			get { return m_AutoShowing; }
		}

		public bool Hiding
		{
			get { return m_AutoHiding; }
		}

		public bool Animating
		{
			get { return m_AutoShowing || m_AutoHiding; }
		}

		public void BeginAutoShow()
		{
			// TODO: check cpu load.
			if (m_AutoHiding)
			{
				OnHideEnded(EventArgs.Empty);
			}
			if (m_AutoShowing == false)
			{
				m_AutoShowing = true;
				ShowBegan(this, EventArgs.Empty);
				///m_TimerMouseActivate.Stop();
				m_WidthShown = Desktop.Screen.WorkingArea.Right - m_Form.Left;
				m_TimerAnimate.Start();
			}
		}

		public void BeginAutoHide()
		{
			if (m_AutoShowing)
			{
				OnShowEnded(EventArgs.Empty);
			}
			if (m_AutoHiding == false)
			{
				m_AutoHiding = true;
				HideBegan(this, EventArgs.Empty);
				///m_TimerMouseActivate.Start();
				m_WidthShown = Desktop.Screen.WorkingArea.Right - m_Form.Left;
				m_TimerAnimate.Start();
			}
		}

		const int EDGE_TOLERANCE = 4;
		const int MIN_SPEED = 1;

		DockForm m_Form;
		Timer m_TimerMouseActivate, m_TimerAnimate;
		bool m_AutoShowTriggered, m_AutoHideTriggered;
		DateTime m_AutoShowTriggeredTime, m_AutoHideTriggeredTime;
		bool m_AutoShowing, m_AutoHiding;
		int m_WidthShown;

		protected void OnShowEnded(EventArgs e)
		{
			m_TimerAnimate.Stop();
			m_AutoShowing = false;
			m_AutoShowTriggered = false;
			ShowEnded(this, e);
		}

		protected void OnHideEnded(EventArgs e)
		{
			m_TimerAnimate.Stop();
			m_AutoHiding = false;
			m_AutoHideTriggered = false;
			HideEnded(this, e);
		}

		private void TimerMouseActivate_Tick(object sender, EventArgs e)
		{
			/* FIXME later: the secure desktop credential prompt triggers auto show from left edge.
				* See also:
				* http://stackoverflow.com/questions/18555998/is-there-any-notification-message-for-switchdesktop-windows-api
				* http://msdn.microsoft.com/en-us/library/windows/desktop/dd373640%28v=vs.85%29.aspx
				* http://msdn.microsoft.com/en-us/library/windows/desktop/ms684309%28v=vs.85%29.aspx
			*/
			if (Animating == false && Desktop.Mouse.Dragging == false)
			{
				// If not already shown, see if user wants to show it.
				if (m_Form.Visible == false)
				{
					check_show_trigger();
				}
				// If already shown or showing, see if user wants to hide it.
				else
				{
					check_hide_trigger();
				}
			}
		}

		private void TimerAnimate_Tick(object sender, EventArgs e)
		{
			m_TimerAnimate.Enabled = false;

			Rectangle rect = Desktop.Screen.WorkingArea;
			int speed;
			if (m_AutoShowing)
			{
				speed = (m_Form.FullWidth - m_WidthShown) / 5;
				if (speed < MIN_SPEED) speed = MIN_SPEED;
				if (speed > m_Form.FullWidth - m_WidthShown) speed = m_Form.FullWidth - m_WidthShown;
				m_WidthShown += speed;
				m_Form.SetDesktopBounds(rect.Right - m_WidthShown, 0, m_Form.FullWidth, rect.Height);
				if (m_Form.Left + m_Form.FullWidth <= rect.Right)
				{
					OnShowEnded(EventArgs.Empty);
					return;
				}
			}
			else if (m_AutoHiding)
			{
				speed = m_WidthShown / 5;
				if (speed < MIN_SPEED) speed = MIN_SPEED;
				if (speed > m_WidthShown) speed = m_WidthShown;
				m_WidthShown -= speed;
				m_Form.SetDesktopBounds(rect.Right - m_WidthShown, 0, m_Form.FullWidth, rect.Height);
				if (m_Form.Left >= rect.Right)
				{
					OnHideEnded(EventArgs.Empty);
					return;
				}
			}
			m_TimerAnimate.Enabled = true;
		}

		void check_show_trigger()
		{
			if (Desktop.Mouse.NearRightEdge(EDGE_TOLERANCE))
			{
				if (m_AutoShowTriggered == false)
				{
					m_AutoShowTriggered = true;
					m_AutoShowTriggeredTime = DateTime.Now;
				}
				else
				{
					check_show_intent();
				}
			}
			else
			{
				m_AutoShowTriggered = false;
			}
		}

		void check_show_intent()
		{
			TimeSpan time_from_trigger = DateTime.Now - m_AutoShowTriggeredTime;
			if (SystemInformation.DoubleClickTime < time_from_trigger.TotalMilliseconds)
			{
				BeginAutoShow();
			}
		}

		void check_hide_trigger()
		{
			Rectangle rect_within = m_Form.Bounds;
			rect_within.Inflate(EDGE_TOLERANCE, EDGE_TOLERANCE);
			if (rect_within.Contains(Cursor.Position) == false)
			{
				if (m_AutoHideTriggered == false)
				{
					m_AutoHideTriggered = true;
					m_AutoHideTriggeredTime = DateTime.Now;
				}
				else
				{
					check_hide_intent();
				}
			}
		}

		void check_hide_intent()
		{
			TimeSpan time_triggered = DateTime.Now - m_AutoHideTriggeredTime;
			if (true || SystemInformation.DoubleClickTime < time_triggered.TotalMilliseconds)
			{
				BeginAutoHide();
			}
		}
	}
}
