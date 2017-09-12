using System;
using System.Drawing;
using System.Windows.Forms;

namespace pWonders.App.DockLive.TileInterface
{
	[System.ComponentModel.DesignerCategory("")]
	public class OptionLabel : Label
	{
		public OptionLabel() { }

		protected override void OnParentFontChanged(EventArgs e)
		{
			base.OnParentFontChanged(e);
			if (this.Parent != null && this.Parent.Font != null)
			{
				this.Font = new Font(this.Parent.Font, FontStyle.Underline);
			}
		}
	}
}
