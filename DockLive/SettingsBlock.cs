using System;
using System.Drawing;
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

		Color m_BackColor;

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
			g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			Control btn = sender as Control;
			RectangleF rect_arrow = btn.ClientRectangle;
			rect_arrow.Inflate(rect_arrow.Width / -4, rect_arrow.Height / -4);
			using (Pen pen = new Pen(btn.ForeColor))
			{
				PointF pt_top = new PointF(rect_arrow.Left + rect_arrow.Width / 2, rect_arrow.Top);
				PointF pt_bot = new PointF(pt_top.X, rect_arrow.Bottom + 1);
				PointF pt_left = new PointF(rect_arrow.Left, rect_arrow.Top + rect_arrow.Height / 2);
				PointF pt_right = new PointF(rect_arrow.Right, pt_left.Y);
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
	}
}
