namespace MoonPad.DockingWindows
{
    internal partial class LuaReplWindow : Browser
    {
        public LuaReplWindow(FormWindow formWindow)
            : base(formWindow, "about:blank")
        {
            InitializeComponent();
        }
    }
}
