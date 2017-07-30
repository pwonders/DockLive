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
			m_TimerMouseActivate.Enabled = true;
			m_TimerMouseActivate.Tick += TimerMouseActivate_Tick;
			m_TimerAnimate = new Timer();
			m_TimerAnimate.Interval = 1;
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
			begin_autoshow();
		}

		public void BeginAutoHide()
		{
			begin_autohide();
		}

		const int EDGE_TOLERANCE = 4;

		DockForm m_Form;
		Timer m_TimerMouseActivate, m_TimerAnimate;
		bool m_AutoShowTriggered, m_AutoHideTriggered;
		DateTime m_AutoShowTriggeredTime, m_AutoHideTriggeredTime;
		bool m_AutoShowing, m_AutoHiding;
		DateTime m_LastAnimateTime;
		int m_AnimateStep;

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
			// TODO: relate this magic number 8 with system performance.
			if ((DateTime.Now - m_LastAnimateTime).TotalMilliseconds > 8)
			{
				m_LastAnimateTime = DateTime.Now;
				Rectangle rect = Desktop.Screen.WorkingArea;
				int speed;
				if (m_AutoShowing)
				{
					speed = (m_Form.FullWidth - m_Form.Width) / 4;
					if (speed < 1)
					{
						speed = 1;
					}
					m_AnimateStep += speed;
					m_Form.SetBounds(rect.Right - m_AnimateStep, rect.Top, m_AnimateStep, rect.Height);
					if (m_AnimateStep >= m_Form.FullWidth)
					{
						OnShowEnded(EventArgs.Empty);
					}
				}
				else if (m_AutoHiding)
				{
					speed = m_Form.Width / 4;
					if (speed < 1)
					{
						speed = 1;
					}
					m_AnimateStep -= speed;
					m_Form.SetBounds(rect.Right - m_AnimateStep, rect.Top, m_AnimateStep, rect.Height);
					if (m_AnimateStep <= 0)
					{
						m_Form.Width = 0;
						m_Form.Hide();
						OnHideEnded(EventArgs.Empty);
					}
				}
			}
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
				m_LastAnimateTime = DateTime.Now;
				begin_autoshow();
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
				m_LastAnimateTime = DateTime.Now;
				begin_autohide();
			}
		}

		void begin_autoshow()
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
				m_TimerMouseActivate.Stop();
				m_AnimateStep = m_Form.Width;
				m_Form.Visible = true;
				m_TimerAnimate.Start();
			}
		}

		void begin_autohide()
		{
			if (m_AutoShowing)
			{
				OnShowEnded(EventArgs.Empty);
			}
			if (m_AutoHiding == false)
			{
				m_AutoHiding = true;
				HideBegan(this, EventArgs.Empty);
				m_TimerMouseActivate.Start();
				m_AnimateStep = m_Form.Width;
				m_TimerAnimate.Start();
			}
		}
	}
}
