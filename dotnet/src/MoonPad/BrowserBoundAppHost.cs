using CefSharp;
using Newtonsoft.Json;

namespace MoonPad
{
    internal class BrowserBoundAppHost
    {
        private readonly FormWindow formWindow;

        // This property may be accessed by FormWindow, not the browser script.
        private IJavascriptCallback callback;

        public BrowserBoundAppHost(FormWindow formWindow)
        {
            this.formWindow = formWindow;
        }

        private class JsonPrintMsg
        {
            public string Output { get; set; }
        }

        internal void SendPrintOutput(string s)
        {
            callback.ExecuteAsync(JsonConvert.SerializeObject(new JsonPrintMsg {Output = s}));
        }

        // This method is called by the browser script.
        // ReSharper disable once UnusedMember.Global
        public void Click()
        {
            // The purpose of this method is to remove the quirk where clicking
            // on content does not close open menus, as usual.
            formWindow.CloseMenus();
        }

        // This method is called by the browser script.
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once ParameterHidesMember
        public void RegisterCallback(IJavascriptCallback callback)
        {
            // NOTE: If the page is refreshed, this is re-assigned.
            this.callback = callback;
        }
    }
}
