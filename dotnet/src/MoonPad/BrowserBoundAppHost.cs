using System;
using CefSharp;
using Newtonsoft.Json;

namespace MoonPad
{
    internal class BrowserBoundAppHost : IDisposable
    {
        private readonly FormWindow formWindow;

        private IJavascriptCallback callback;

        public BrowserBoundAppHost(FormWindow formWindow)
        {
            this.formWindow = formWindow;
            formWindow.LuaRepl.LuaReplPrint += LuaRepl_OnLuaReplPrint;
        }

        public void Dispose()
        {
            formWindow.LuaRepl.LuaReplPrint -= LuaRepl_OnLuaReplPrint;
        }

        private class JsonPrintMsg
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string Output { get; set; }
        }

        private void LuaRepl_OnLuaReplPrint(string s)
        {
            callback.ExecuteAsync(JsonConvert.SerializeObject(new JsonPrintMsg {Output = s}));
        }

        #region Browser script interface

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

        #endregion
    }
}
