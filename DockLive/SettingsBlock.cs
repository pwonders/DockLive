using System;
using System.Drawing;
using System.Windows.Forms;
using pWonders.App.DockLive.TileInterface;

namespace pWonders.App.DockLive
{
	public partial class SettingsBlock : UserControl
	{
		public SettingsBlock(ITile tile)
		{
			InitializeComponent();

			this.Tile = tile;
			if (tile.SettingsControl != null)
			{
				this.pnlMain.Controls.Add(tile.SettingsControl);
				tile.SettingsControl.Dock = DockStyle.Fill;
			}

			this.lblTileName.Text = tile.Name;
			this.Size = tile.Control.Size;
			this.Dock = DockStyle.Fill;
			this.Margin = tile.Control.Margin;
			this.DoubleBuffered = true;

			m_BackColor = this.BackColor;
		}

		public ITile Tile { set; get; }

		public override Color BackColor
		{
			set { m_BackColor = value; }
			get { return m_BackColor; }
		}

		public event EventHandler GoBack = delegate { };
		public event EventHandler FillDn = delegate { };
		public event EventHandler FillUp = delegate { };
		public event EventHandler Remove = delegate { };

		Color m_BackColor;
		bool m_FillUp;

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			lblTileName.Font = new Font(this.Font, FontStyle.Bold);
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			base.OnForeColorChanged(e);
			this.btnBack.ForeColor = this.ForeColor;
			this.lblTileName.ForeColor = this.ForeColor;
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);
			using (Brush br = new SolidBrush(m_BackColor))
			{
				e.Graphics.FillRectangle(br, e.ClipRectangle);
			}
		}

		private void btnBack_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SetHighQualityMode();

			Control btn = sender as Control;
			RectangleF rect_draw = btn.ClientRectangle;
			rect_draw.Inflate(rect_draw.Width / -4, rect_draw.Height / -4);
			using (Pen pen = new Pen(btn.ForeColor))
			{
				PointF pt_top = new PointF(rect_draw.Left + rect_draw.Width / 2, rect_draw.Top);
				PointF pt_bot = new PointF(pt_top.X, rect_draw.Bottom + 1);
				PointF pt_left = new PointF(rect_draw.Left, rect_draw.Top + rect_draw.Height / 2);
				PointF pt_right = new PointF(rect_draw.Right, pt_left.Y);
				PointF[] points_line = new PointF[] { pt_right, pt_left };
				PointF[] points_arrow = new PointF[] { pt_top, pt_left, pt_bot };
				g.DrawLines(pen, points_line);
				g.DrawLines(pen, points_arrow);
			}
		}

		private void btnBack_Click(object sender, EventArgs e)
		{
			GoBack(this, EventArgs.Empty);
		}

		private void btnFillDn_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SetHighQualityMode();

			Control btn = sender as Control;
			RectangleF rect_draw = btn.ClientRectangle;
			rect_draw.Inflate(rect_draw.Width / -4, rect_draw.Height / -4);
			using (Pen pen = new Pen(btn.ForeColor))
			{
				PointF pt_tl = new PointF(rect_draw.Left, rect_draw.Top);
				PointF pt_br = new PointF(rect_draw.Right, rect_draw.Bottom);
				PointF pt_a0, pt_a1, pt_a2;
				if (m_FillUp)
				{
					pt_a0 = new PointF(rect_draw.Left, rect_draw.Bottom - rect_draw.Height / 4);
					pt_a1 = pt_tl;
					pt_a2 = new PointF(rect_draw.Right - rect_draw.Width / 4, rect_draw.Top);
				}
				else
				{
					pt_a0 = new PointF(rect_draw.Right, rect_draw.Top + rect_draw.Height / 4);
					pt_a1 = pt_br;
					pt_a2 = new PointF(rect_draw.Left + rect_draw.Width / 4, rect_draw.Bottom);
				}
				PointF[] points_arrow = new PointF[] { pt_a0, pt_a1, pt_a2 };
				PointF[] points_bslash = new PointF[] { pt_tl, pt_br };
				g.DrawLines(pen, points_arrow);
				g.DrawLines(pen, points_bslash);
			}
		}

		private void btnFillDn_Click(object sender, EventArgs e)
		{
			if (m_FillUp)
			{
				FillUp(this, EventArgs.Empty);
			}
			else
			{
				FillDn(this, EventArgs.Empty);
			}
			m_FillUp = !m_FillUp;
		}

		private void btnRemove_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			g.SetHighQualityMode();

			Control btn = sender as Control;
			RectangleF rect_draw = btn.ClientRectangle;
			rect_draw.Inflate(rect_draw.Width / -4, rect_draw.Height / -4);
			using (Pen pen = new Pen(btn.ForeColor))
			{
				PointF pt_tl = new PointF(rect_draw.Left, rect_draw.Top);
				PointF pt_tr = new PointF(rect_draw.Right, rect_draw.Top);
				PointF pt_bl = new PointF(rect_draw.Left, rect_draw.Bottom);
				PointF pt_br = new PointF(rect_draw.Right, rect_draw.Bottom);
				PointF[] points_slash = new PointF[] { pt_bl, pt_tr };
				PointF[] points_bslash = new PointF[] { pt_tl, pt_br };
				g.DrawLines(pen, points_slash);
				g.DrawLines(pen, points_bslash);
			}
		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			Remove(this, EventArgs.Empty);
		}
	}
}
