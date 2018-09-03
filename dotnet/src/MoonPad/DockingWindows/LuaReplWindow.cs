namespace MoonPad.DockingWindows
{
    internal partial class LuaReplWindow : Browser
    {
        public LuaReplWindow(FormWindow formWindow)
            : base(formWindow, "term/index.html")
        {
            InitializeComponent();
        }
    }
}
