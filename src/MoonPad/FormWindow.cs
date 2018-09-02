using System;
using System.Collections.Generic;
using System.Drawing;
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

        private const string DefaultStatus = "Ready";
        private static readonly Color TabColour = Color.FromArgb(0, 122, 204);

        private readonly WindowsFormGeometryPersistence geometry;
        private readonly Invoker invoker;
        private readonly LogWindow logWindow;

        private bool exitInProgress;

        public FormWindow(string openFileOnLoad = null)
        {
            InitializeComponent();

            geometry = new WindowsFormGeometryPersistence("FormWindow");
            geometry.Restore(this);

            CommonDialogs.Owner = this;
            CommonDialogs.Invoker = invoker = new Invoker(this);

            sandDockManager1.Renderer = new CombinedDockRenderer(TabColour);

            // Restore SandDock layout.
            if (!string.IsNullOrEmpty(Settings.Default.SandDockLayout))
            {
                sandDockManager1.SetLayout(Settings.Default.SandDockLayout);
            }

            logDockWindow.Controls.Add(Border.AddBorder(logWindow = new LogWindow()));
            luaScriptsWindow.Controls.Add(Border.AddBorder(new LuaScriptsList()));
        }

        private void FormWindow_Load(object sender, EventArgs e)
        {
            invoker.DelayedTryCatchInvoke(() => OpenGlTab(), 0);
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
            /*
            if (WorkbookContext == null || !WorkbookContext.UnsavedChanges) return true;

            using (new DialogCenteringService(this))
            {
                var confirm = MessageBox.Show("Do you want to save your changes?",
                    "File content modified", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                switch (confirm)
                {
                    case DialogResult.Cancel:
                        return false;

                    case DialogResult.Yes:
                        if (WorkbookContext.Filename != null)
                        {
                            SaveFile();
                        }
                        else
                        {
                            return SaveFileAs();
                        }
                        break;
                }
            }
            */

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
            if (delay > 0) invoker.DelayedTryCatchInvoke(Action, delay);
            else invoker.TryCatchInvoke(Action);
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

            return OpenTab(new OpenGlDocument(), "OpenGL", border: false);
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
    }
}
