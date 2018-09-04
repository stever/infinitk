using System;
using System.Drawing;
using System.Windows.Forms;

namespace MoonPad
{
    internal sealed class SideMenuListBox : ListBox
    {
        public SideMenuListBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            Dock = DockStyle.Fill;
            BackColor = Color.FromKnownColor(KnownColor.Control);
            BorderStyle = BorderStyle.None;
            Font = new Font("Microsoft Sans Serif", 14, FontStyle.Regular, GraphicsUnit.Pixel);
            ItemHeight = 26;
            IntegralHeight = false;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // Prevent error from Visual Designer.
            if (Items.Count <= 0) return;

            //e.DrawBackground();
            // if selected, mark the background differently
            e.Graphics.FillRectangle(
                (e.State & DrawItemState.Selected) == DrawItemState.Selected
                    ? new SolidBrush(Color.FromKnownColor(KnownColor.Highlight))
                    : new SolidBrush(Color.FromKnownColor(KnownColor.Control)), e.Bounds);

            // Draw item separator.
            e.Graphics.DrawLine(
                new Pen(Color.FromKnownColor(KnownColor.White)),
                e.Bounds.X, e.Bounds.Y + e.Bounds.Height - 1,
                e.Bounds.X + e.Bounds.Width, e.Bounds.Y + e.Bounds.Height - 1);

            if (e.State == DrawItemState.Focus)
            {
                e.DrawFocusRectangle();
            }

            const int marginTop = 4;
            const int marginBottom = 0;
            const int marginLeft = 4;
            const int marginRight = 0;

            var bounds = new Rectangle(
                e.Bounds.X + marginLeft,
                e.Bounds.Y + marginTop,
                e.Bounds.Width - marginRight,
                e.Bounds.Height - marginBottom);

            if (e.Index < 0 || e.Index >= Items.Count) return;
            var text = Items[e.Index]?.ToString() ?? "(null)";
            using (var brush = new SolidBrush(e.ForeColor))
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                e.Graphics.DrawString(text, e.Font, brush, bounds, new StringFormat(StringFormatFlags.NoWrap));
            }
        }

        private int GetListItemIndex(string findName)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var itemName = Items[i] as string;
                if (itemName == findName) return i;
            }

            throw new Exception("Item not found");
        }

        public void SelectName(string name)
        {
            SelectedIndex = GetListItemIndex(name);
        }
    }
}
