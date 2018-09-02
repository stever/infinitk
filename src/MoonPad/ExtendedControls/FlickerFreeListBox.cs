using System.Drawing;
using System.Windows.Forms;

namespace MoonPad.ExtendedControls
{
    internal sealed class FlickerFreeListBox : ListBox
    {
        public FlickerFreeListBox()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);

            DrawMode = DrawMode.OwnerDrawFixed;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var region = new Region(e.ClipRectangle);
            e.Graphics.FillRegion(new SolidBrush(BackColor), region);
            if (Items.Count > 0)
            {
                for (var i = 0; i < Items.Count; ++i)
                {
                    var rect = GetItemRectangle(i);
                    if (!e.ClipRectangle.IntersectsWith(rect)) continue;

                    if (SelectionMode == SelectionMode.One && SelectedIndex == i
                        || SelectionMode == SelectionMode.MultiSimple && SelectedIndices.Contains(i)
                        || SelectionMode == SelectionMode.MultiExtended && SelectedIndices.Contains(i))
                    {
                        OnDrawItem(new DrawItemEventArgs(e.Graphics, Font, rect, i,
                            DrawItemState.Selected, ForeColor, BackColor));
                    }
                    else
                    {
                        OnDrawItem(new DrawItemEventArgs(e.Graphics, Font, rect, i,
                            DrawItemState.Default, ForeColor, BackColor));
                    }

                    region.Complement(rect);
                }
            }

            base.OnPaint(e);
        }
    }
}
