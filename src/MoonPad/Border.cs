using System.Drawing;
using System.Windows.Forms;

namespace MoonPad
{
    internal static class Border
    {
        public static Control AddTopBorder(Control control = null)
        {
            return AddBorder(control, 1, 0, 0, 0);
        }

        public static Control AddBorder(Control control = null)
        {
            return AddBorder(control, 1, 1, 1, 1);
        }

        public static Control AddBorder(Control control, int top, int left, int bottom, int right)
        {
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromKnownColor(KnownColor.Control)
            };
            var borderPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromKnownColor(KnownColor.ControlDark),
                Padding = new Padding(left, top, right, bottom)
            };
            if (control != null)
            {
                control.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(control);
            }
            borderPanel.Controls.Add(contentPanel);
            return borderPanel;
        }
    }
}
