using System.Windows.Forms;

namespace MoonPad.Persistence
{
    internal class WindowsFormGeometry
    {
        public int? Top { get; set; }
        public int? Left { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public FormWindowState State { get; set; } = FormWindowState.Normal;
    }
}
