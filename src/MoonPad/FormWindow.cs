using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using MoonPad.DockingWindows;
using MoonPad.Documents;
using MoonPad.Persistence;
using MoonPad.SandDockRendering;
using TD.SandDock;

namespace MoonPad
{
    internal partial class FormWindow : Form
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public delegate bool ValidatePromptInput(string input);

        #region Events

        public delegate void FileLoadedEventHandler();
        public delegate void FileClosedEventHandler();

        public event FileLoadedEventHandler FileLoaded;
        public event FileClosedEventHandler FileClosed;

        #endregion

        private const string DatabaseFileFilter = "MoonPad File (*.mpdb)|*.mpdb";
        private const string DefaultStatus = "Ready";
        private static readonly Color TabColour = Color.FromArgb(0, 122, 204);

        private readonly string openFileOnLoad;
        private readonly WindowsFormGeometryPersistence geometry;
        private readonly LogWindow logWindow;

        private bool exitInProgress;
        private bool unsavedChanges;
        private string initialWindowTitle;
        private TabbedDocument contextClickTab;

        public Invoker Invoker { get; }
        public LuaRepl LuaRepl { get; }
        public Database Database { get; private set; }

        public FormWindow(string openFileOnLoad = null)
        {
            InitializeComponent();

            this.openFileOnLoad = openFileOnLoad;

            geometry = new WindowsFormGeometryPersistence("FormWindow");
            geometry.Restore(this);

            CommonDialogs.Owner = this;
            CommonDialogs.Invoker = Invoker = new Invoker(this);
            LuaRepl = new LuaRepl(this);

            sandDockManager1.Renderer = new CombinedDockRenderer(TabColour);

            // Restore SandDock layout.
            if (!string.IsNullOrEmpty(Settings.Default.SandDockLayout))
            {
                sandDockManager1.SetLayout(Settings.Default.SandDockLayout);
            }

            logDockWindow.Controls.Add(Border.AddBorder(logWindow = new LogWindow()));
            luaScriptsWindow.Controls.Add(Border.AddBorder(new LuaScriptsList(this)));
            luaReplWindow.Controls.Add(Border.AddBorder(new LuaReplWindow(this)));
        }

        private void FormWindow_Load(object sender, EventArgs e)
        {
            initialWindowTitle = Text;
            if (openFileOnLoad != null)
            {
                // TODO: Open and load from file.
                Invoker.DelayedTryCatchInvoke(() => OpenGlTab(), 0);
            }
        }

        private bool ExitApplication()
        {
            if (!ConfirmCloseUnsaved()) return false;

            exitInProgress = true;
            SetStatus("Closing...");
            geometry.Persist(this);

            Settings.Default.SandDockLayout = sandDockManager1.GetLayout();
            Settings.Default.Save();

            Close();
            return true;
        }

        /// <summary>
        /// Check & confirm if there are unsaved changes.
        /// </summary>
        /// <returns>True to continue closing, otherwise cancel close.</returns>
        private bool ConfirmCloseUnsaved()
        {
            if (!unsavedChanges) return true;

            using (new DialogCenteringService(this))
            {
                var confirm = MessageBox.Show("Do you want to save your new file?",
                    "Unsaved file", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                switch (confirm)
                {
                    case DialogResult.Cancel:
                        return false;

                    case DialogResult.Yes:
                        return SaveFileAs();
                }
            }

            return true;
        }

        /// <summary>
        /// Sets status bar message, with optional time delay.
        /// </summary>
        /// <param name="msg">Status message</param>
        /// <param name="delay">Delay in milliseconds</param>
        private void SetStatus(string msg = null, int delay = 0)
        {
            if (msg == null) msg = DefaultStatus;
            void Action() => toolStripStatusLabel1.Text = msg;
            if (delay > 0) Invoker.DelayedTryCatchInvoke(Action, delay);
            else Invoker.TryCatchInvoke(Action);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ExitApplication();
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        private void FormWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (!exitInProgress && !ExitApplication())
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        private TabbedDocument OpenTab(Control control, string name, bool closable = true, bool border = true)
        {
            var controlPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromKnownColor(KnownColor.Control)
            };

            var tabTopped = (Control) new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = TabColour,
                Padding = new Padding(0, 2, 0, 0)
            };

            control.Dock = DockStyle.Fill;
            controlPanel.Controls.Add(control);
            tabTopped.Controls.Add(controlPanel);

            if (border)
            {
                tabTopped = Border.AddBorder(tabTopped, 0, 1, 1, 1);
            }

            var tab = new TabbedDocument(sandDockManager1, tabTopped, name)
            {
                BackColor = TabColour,
                ForeColor = Color.White,
                DockingRules = {AllowFloat = true},
                AllowClose = closable,
                PersistState = true,
            };

            tab.Open();
            return tab;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private TabbedDocument OpenGlTab()
        {
            // Check that the tab is not already open.
            foreach (var tab in GetOpenTabs())
            {
                var control = GetFirstNonPanelChild(tab);
                if (!(control is OpenGlDocument)) continue;
                tab.Activate();
                return tab;
            }

            return OpenTab(new OpenGlDocument(), "OpenGL");
        }

        public TabbedDocument OpenLuaScriptTab(string name)
        {
            // Check if the tab is already open. If so, then activate it.
            foreach (var tab in GetOpenTabs())
            {
                var tabControl = GetFirstNonPanelChild(tab);
                if (!(tabControl is LuaEditor control)) continue;
                if (control.ScriptName != name) continue;
                tab.Activate();
                return tab;
            }

            // Otherwise open new tab document.
            return OpenTab(new LuaEditor(this, name), name);
        }

        private IEnumerable<TabbedDocument> GetOpenTabs()
        {
            var result = new HashSet<TabbedDocument>();
            var controls = sandDockManager1.GetDockControls(DockSituation.Document);
            foreach (var control in controls)
            {
                if (control is TabbedDocument tab)
                {
                    result.Add(tab);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the first child control that is not a Panel, or null.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private static Control GetFirstNonPanelChild(Control control)
        {
            if (control == null || control.Controls.Count == 0) return null;

            var child = control.Controls[0];
            while (child is Panel)
            {
                if (child.Controls.Count == 0) return null;
                child = child.Controls[0];
            }

            return child;
        }

        private void openGLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenGlTab();
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                logDockWindow.Open();
                logDockWindow.Focus();
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        private void luaScriptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                luaScriptsWindow.Open();
                luaScriptsWindow.Focus();
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        private void luaReplToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                luaReplWindow.Open();
                luaReplWindow.Focus();
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: Create new temp file database and require save-as on close.

                closeToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
                openToolStripMenuItem.Enabled = false;
                newToolStripMenuItem.Enabled = false;

                unsavedChanges = true;
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = DatabaseFileFilter,
                    Title = "Open File",
                    RestoreDirectory = true,
                };

                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                logWindow.Clear();
                UpdateWindowTitle(dialog.FileName);
                Database = new Database(dialog.FileName);

                closeToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
                openToolStripMenuItem.Enabled = false;
                newToolStripMenuItem.Enabled = false;

                unsavedChanges = false;
                FileLoaded?.Invoke();
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                logWindow.Clear();

                // TODO: Store layout in database.

                Database.Dispose();
                Database = null;

                UpdateWindowTitle();

                closeToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;
                openToolStripMenuItem.Enabled = true;
                newToolStripMenuItem.Enabled = true;

                unsavedChanges = false;

                FileClosed?.Invoke();
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        private bool SaveFileAs()
        {
            var dialog = new SaveFileDialog
            {
                Filter = DatabaseFileFilter,
                Title = "Save File"
            };

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            SaveFileAs(dialog.FileName);
            return true;
        }

        private void SaveFileAs(string filename)
        {
            SetStatus("Saving file...");
            CommonDialogs.ShowActionModal("Saving", "Saving file...", () =>
            {
                if (/*filename != currentFilename && */File.Exists(filename))
                {
                    // Allow overwrite of file currently used. No need to delete it in that case.
                    File.Delete(filename);
                }

                // TODO: Store layout in database.
                // TODO: Copy database to new filename.

                Invoker.InvokeAndWaitFor(() =>
                {
                    UpdateWindowTitle(filename);
                    ShowTemporaryStatus("File saved");
                });

                unsavedChanges = false;
            });
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileAs();
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        private void UpdateWindowTitle(string filename = null)
        {
            Invoker.InvokeAndWaitFor(() =>
            {
                if (filename == null)
                {
                    Text = initialWindowTitle;
                    return;
                }

                var name = Path.GetFileNameWithoutExtension(filename);
                Debug.Assert(initialWindowTitle != null);
                Text = !string.IsNullOrEmpty(name)
                    ? $"{name} - {initialWindowTitle}"
                    : initialWindowTitle;
            });
        }

        private void ShowTemporaryStatus(string msg, int delay = 0)
        {
            void Action()
            {
                toolStripStatusLabel1.Text = msg;
                SetStatus(null, delay: 5000);
            }

            if (delay > 0) Invoker.DelayedTryCatchInvoke(Action, delay);
            else Invoker.TryCatchInvoke(Action);
        }

        private void sandDockManager1_ShowControlContextMenu(object sender, ShowControlContextMenuEventArgs e)
        {
            contextClickTab = e.DockControl as TabbedDocument;
            if (contextClickTab == null) return;
            tabContextMenu.Show(this, PointToClient(MousePosition));
        }

        private void closeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            contextClickTab.Close();
        }

        private void closeAllButThisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var docs = sandDockManager1.GetDockControls(DockSituation.Document).ToList();
            foreach (var doc in docs)
            {
                if (doc != contextClickTab)
                {
                    doc.Close();
                }
            }
        }

        private void closeAllDocumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var docs = sandDockManager1.GetDockControls(DockSituation.Document).ToList();
            foreach (var doc in docs)
            {
                doc.Close();
            }
        }

        private void developerToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var browser = FindBrowserInActiveControl(ActiveControl);
                if (browser == null) return;
                browser.ShowDevTools();
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        private static Browser FindBrowserInActiveControl(Control control)
        {
            while (control is ContainerControl container)
            {
                control = container.ActiveControl;
                if (control is Browser browser)
                    return browser;
            }

            return null;
        }
    }
}
