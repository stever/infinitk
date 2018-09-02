using System.Drawing;
using System.Windows.Forms;

namespace MoonPad.DockingWindows
{
    public partial class LuaReplWindow : UserControl
    {
        private const int FontLineHeight = 16;

        public LuaReplWindow()
        {
            InitializeComponent();
        }

        private void LuaReplWindow_Load(object sender, System.EventArgs e)
        {
            Font = new Font(ResourceFontLibrary.GetPragmataPro(), FontLineHeight, FontStyle.Regular, GraphicsUnit.Pixel);
        }
    }
}
