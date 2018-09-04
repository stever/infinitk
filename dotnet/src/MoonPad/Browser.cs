using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using MoonPad.Persistence;
using log4net;

namespace MoonPad
{
    internal partial class Browser : UserControl, IContextMenuHandler, ILifeSpanHandler, IKeyboardHandler, IJsDialogHandler
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string AppDataFolderCache = "Cache";
        private const string AppDataFolderUserData = "User Data";

        #region Events

        public delegate void BrowserDocumentKeyPressEventHandler(KeyPressModifier modifier, KeyCodes keyCode);
        public event BrowserDocumentKeyPressEventHandler BrowserDocumentKeyPress;

        public delegate void FrameLoadedEventHandler();
        public event FrameLoadedEventHandler FrameLoaded;

        #endregion

        // ReSharper disable once MemberCanBePrivate.Global
        public string StartPage { get; }

        private readonly FormWindow formWindow;
        private readonly Invoker invoker;
        private readonly IClosable closable;

        private Panel ContentPanel => (Panel) Controls.Find("contentPanel", false)[0];

        private ChromiumWebBrowser chromium;
        private FindToolBar findToolStrip;
        private BrowserBoundAppHost appHost;

        protected Browser(FormWindow formWindow, string startPage, IClosable closable = null)
        {
            this.formWindow = formWindow;
            StartPage = startPage;
            this.closable = closable;

            CefInit();
            InitializeComponent();

            invoker = new Invoker(this);
            Name = "CefSharpBrowser";

            Disposed += (sender, e) =>
            {
                appHost.Dispose();
            };
        }

        private void CefSharpBrowser_Load(object sender, EventArgs e)
        {
            chromium = new ChromiumWebBrowser($"http://cefsharp/{StartPage}")
            {
                Dock = DockStyle.Fill,
                MenuHandler = this,
                KeyboardHandler = this,
                LifeSpanHandler = this,
                //JsDialogHandler = this, // TODO: Replace the default JS handler with one that looks better.
            };

            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            chromium.RegisterAsyncJsObject("AppHost", appHost = new BrowserBoundAppHost(formWindow));
            chromium.FrameLoadEnd += (o, args) => FrameLoaded?.Invoke();
            chromium.ConsoleMessage += (o, args) => Log.DebugFormat("{0}:{1} {2}", GetSource(args), args.Line, args.Message);

            ContentPanel.Controls.Add(chromium);

            findToolStrip = new FindToolBar();
            findToolStrip.ToggleFindToolStrip += ToggleFindToolStrip;
            findToolStrip.Find += Find;
            findToolStrip.Dock = DockStyle.Bottom;
            Controls.Add(findToolStrip);

            ToggleFindToolStrip();
        }

        private static string GetSource(ConsoleMessageEventArgs args)
        {
            return args.Source.Substring(args.Source.LastIndexOf("/", StringComparison.Ordinal) + 1);
        }

        private void CefInit()
        {
            if (Cef.IsInitialized)
            {
                return;
            }

            var appDataPath = Settings.AppDataPath;

            var settings = new CefSettings();

            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            settings.LogFile = Path.Combine(appDataPath, "CEF.log");
            settings.CachePath = Path.Combine(appDataPath, AppDataFolderCache);
            settings.UserDataPath = Path.Combine(appDataPath, AppDataFolderUserData);

#if DEBUG
            settings.RemoteDebuggingPort = 8088;
#endif

            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "http",
                SchemeHandlerFactory = new BrowserSchemeHandlerFactory(formWindow,
                    // ReSharper disable ArgumentsStyleStringLiteral
                    schemeName: "http", // No schemeName checking if null
                    hostName: "cefsharp", // No hostName checking if null
                    defaultPage: "index.html") // Default to index.html
                    // ReSharper restore ArgumentsStyleStringLiteral
            });

            Cef.Initialize(settings);
        }

        public void ShowDevTools()
        {
            chromium.ShowDevTools();
        }

        protected void ExecuteJavaScriptAsync(string javascript)
        {
            if (chromium == null || !chromium.IsBrowserInitialized || !chromium.CanExecuteJavascriptInMainFrame) return;
            chromium.ExecuteScriptAsync(javascript);
        }

        protected void Reload()
        {
            var ready = chromium != null && chromium.IsBrowserInitialized;
            var address = chromium != null ? chromium.Address : "about:blank";
            Log.DebugFormat("Reload ({0}, \"{1}\")", ready, address);
            if (!ready) return;
            chromium.Reload(false);
        }

        /* NOTE: This method may not have worked.
        public void Navigate(string url)
        {
            if (chromium == null || !chromium.IsBrowserInitialized) return;
            chromium.Load(url);
        }
        */

        public void RecieveKey(int windowsKeyCode)
        {
            chromium.GetBrowser().GetHost().SendKeyEvent(new KeyEvent
            {
                WindowsKeyCode = windowsKeyCode,
                FocusOnEditableField = true,
                IsSystemKey = false,
                Type = KeyEventType.Char
            });
        }

        private void ToggleFindToolStrip()
        {
            var panel = ContentPanel;

            if (findToolStrip.Visible)
            {
                if (chromium.IsBrowserInitialized)
                {
                    chromium.StopFinding(true);
                }

                panel.Dock = DockStyle.Fill;

                // TODO: There's a bell when executing the next line, but not when findTextBox is manually de-focused.
                findToolStrip.Visible = false;
            }
            else
            {
                findToolStrip.Visible = true;

                var size = new Size(panel.Size.Width, panel.Size.Height);
                panel.Dock = DockStyle.None;
                panel.Size = size;
                panel.Anchor = AnchorStyles.Top &
                               AnchorStyles.Left &
                               AnchorStyles.Bottom &
                               AnchorStyles.Right;

                findToolStrip.FindTextBox.Focus();
            }
        }

        private void Find(bool next)
        {
            if (!string.IsNullOrEmpty(findToolStrip.FindTextBox.Text))
            {
                chromium.Find(0, findToolStrip.FindTextBox.Text, next, false, false);
            }
        }

        #region IContextMenuHandler

        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            model.Clear();
        }

        bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {

            return false;
        }

        void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {

        }

        bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }

        #endregion

        #region ILifeSpanHandler

        bool ILifeSpanHandler.OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName,
            WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo,
            IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = null;

            // Check the URI scheme to ensure that we're not allowing execution of arbitrary programs.
            var uri = new Uri(targetUrl);
            if (uri.Scheme != "http" && uri.Scheme != "https")
            {
                throw new Exception($"Invalid URI scheme: {targetUrl}");
            }

            Process.Start(uri.AbsoluteUri);

            return true; // Return true to cancel the popup creation.
        }

        void ILifeSpanHandler.OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {

        }

        bool ILifeSpanHandler.DoClose(IWebBrowser browserControl, IBrowser browser)
        {
            // NOTE: This seems to be called when the devtools window is closed.
            // Not currently sure how to ignore the devtools window close here.
            if (closable == null)
            {
                return false;
            }

            invoker.TryCatchInvoke(() => closable.Close());

            return true;
        }

        void ILifeSpanHandler.OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
        {

        }

        #endregion

        #region IKeyboardHandler

        bool IKeyboardHandler.OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode,
            CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            return false;
        }

        bool IKeyboardHandler.OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode,
            CefEventFlags modifiers, bool isSystemKey)
        {
            // Only listen to the raw key-down events.
            if (type != KeyType.RawKeyDown) return false;

            // Only A-Z and F1-F12 keys are currently interesting.
            var isAtoZ = windowsKeyCode >= 65 && windowsKeyCode <= 90;
            var isFunctionKey = windowsKeyCode >= 112 && windowsKeyCode <= 123; // F1-F12
            if (!isAtoZ && !isFunctionKey) return false;

            // Only A-Z with modifier is currently interesting.
            if (modifiers == CefEventFlags.None && isAtoZ) return false;

            invoker.TryCatchInvoke(() =>
            {
                Log.DebugFormat("OnKeyEvent {0}:{1}", modifiers, windowsKeyCode);

                var modifier = new KeyPressModifier();
                if ((modifiers & CefEventFlags.ControlDown) != 0) modifier.Control = true;
                if ((modifiers & CefEventFlags.AltDown) != 0) modifier.Alt = true;
                if ((modifiers & CefEventFlags.ShiftDown) != 0) modifier.Shift = true;

                var keyCode = (KeyCodes) windowsKeyCode;

                BrowserDocumentKeyPress?.Invoke(modifier, keyCode);

                if (modifier.Control && keyCode == KeyCodes.F)
                {
                    ToggleFindToolStrip();
                }

                if (chromium.IsBrowserInitialized && keyCode == KeyCodes.F5)
                {
                    chromium.Reload();
                }

#if DEBUG
                if (chromium.IsBrowserInitialized && keyCode == KeyCodes.F12)
                {
                    chromium.ShowDevTools();
                }
#endif
            });

            return false; // Always returns false.
        }

        #endregion

        #region IJsDialogHandler

        bool IJsDialogHandler.OnJSDialog(IWebBrowser browserControl, IBrowser browser, string originUrl, CefJsDialogType dialogType,
            string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage)
        {
            Log.Debug("OnJSDialog");
            return true;
        }

        bool IJsDialogHandler.OnJSBeforeUnload(IWebBrowser browserControl, IBrowser browser, string message, bool isReload,
            IJsDialogCallback callback)
        {
            Log.Debug("OnJSBeforeUnload");
            return true;
        }

        void IJsDialogHandler.OnResetDialogState(IWebBrowser browserControl, IBrowser browser)
        {
            Log.Debug("OnResetDialogState");
        }

        void IJsDialogHandler.OnDialogClosed(IWebBrowser browserControl, IBrowser browser)
        {
            Log.Debug("OnDialogClosed");
        }

        #endregion
    }
}
