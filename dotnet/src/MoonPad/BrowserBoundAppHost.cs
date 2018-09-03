namespace MoonPad
{
    internal class BrowserBoundAppHost
    {
        private readonly FormWindow formWindow;

        public BrowserBoundAppHost(FormWindow formWindow)
        {
            this.formWindow = formWindow;
        }

        // ReSharper disable once UnusedMember.Global
        public void Click()
        {
            // The purpose of this method is to remove the quirk where clicking
            // on content does not close open menus, as usual.
            formWindow.CloseMenus();
        }
    }
}
