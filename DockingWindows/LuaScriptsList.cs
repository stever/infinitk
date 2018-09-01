using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;

namespace MoonPad.DockingWindows
{
    internal partial class LuaScriptsList : AbstractListControl, AbstractListControl.IListControl
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public LuaScriptsList()
        {
            InitializeComponent();
        }

        public LuaScriptsList(FormWindow formWindow)
            : base(formWindow)
        {
            InitializeComponent();

            Handler = this;
            contextMenuStrip1.Enabled = false;
            SideMenuListBox.ContextMenuStrip = contextMenuStrip1;

            /*
            // TODO: event handlers.
            formWindow.WorkbookOpened += ViewState_WorkbookOpened;
            ViewState.WorkbookClosed += ViewState_WorkbookClosed;
            ViewState.LuaScriptAdded += ViewState_LuaScriptAdded;
            ViewState.LuaScriptRenamed += ViewState_LuaScriptRenamed;
            ViewState.LuaScriptRemoved += ViewState_LuaScriptRemoved;
            */
        }

        private void LuaScriptsList_Load(object sender, EventArgs e)
        {
            try
            {
                /* TODO
                // If there is an active workbook, load the list box.
                var activeWorkbook = WorkbookContext?.Workbook;
                if (activeWorkbook == null) return;
                LoadList();
                contextMenuStrip1.Enabled = true;
                */
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        private void LoadList()
        {
            SideMenuListBox.Items.Clear();
            /* TODO
            var names = WorkbookContext.GetLuaScripts().Keys.ToList();
            names.Sort();
            foreach (var name in names)
            {
                SideMenuListBox.Items.Add(name);
            }
            */
        }

        private bool IsNameAvailable(string name)
        {
            /* TODO
            var names = new HashSet<string>(WorkbookContext.GetLuaScripts().Keys.ToList());
            return !names.Contains(name);
            */

            return true;
        }

        #region IListControl

        void IListControl.NonItemMouseDown()
        {
            contextMenuStrip1.Items["deleteToolStripMenuItem"].Enabled = false;
            contextMenuStrip1.Items["renameToolStripMenuItem"].Enabled = false;
        }

        void IListControl.ItemMouseDown()
        {
            contextMenuStrip1.Items["deleteToolStripMenuItem"].Enabled = true;
            contextMenuStrip1.Items["renameToolStripMenuItem"].Enabled = true;
        }

        void IListControl.OpenItem(object selectedItem)
        {
            // TODO: ViewState.RaiseLuaScriptOpeningEvent((string) selectedItem);
        }

        #endregion

        /*
        #region ViewState event handlers

        private void ViewState_WorkbookOpened(WorkbookContext workbookContext)
        {
            Invoker.TryCatchInvoke(() =>
            {
                LoadList();
                contextMenuStrip1.Enabled = true;
            });
        }

        private void ViewState_WorkbookClosed()
        {
            Invoker.TryCatchInvoke(() =>
            {
                SideMenuListBox.Items.Clear();
                contextMenuStrip1.Enabled = false;
            });
        }

        private void ViewState_LuaScriptAdded(string name)
        {
            Invoker.TryCatchInvoke(() =>
            {
                LoadList();
                SideMenuListBox.SelectName(name);
            });
        }

        private void ViewState_LuaScriptRemoved(string name)
        {
            Invoker.TryCatchInvoke(LoadList);
        }

        private void ViewState_LuaScriptRenamed(string oldName, string newName)
        {
            Invoker.TryCatchInvoke(() =>
            {
                LoadList();
                SideMenuListBox.SelectName(newName);
            });
        }

        #endregion
        */

        #region Context menu

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DeselectItem();

                var name = CommonDialogs.Prompt("Provide a unique name for the Lua Script:", "Add Lua Script", validate:IsNameAvailable);
                if (string.IsNullOrEmpty(name)) return;

                // TODO: WorkbookContext.AddLuaScript(name);
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var name = (string) SideMenuListBox.SelectedItem;
                // TODO: WorkbookContext.RemoveLuaScript(name);
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var oldName = (string) SideMenuListBox.SelectedItem;
                var newName = CommonDialogs.Prompt("Provide a new name for the Lua Script:", "Rename Lua Script", oldName, IsNameAvailable);
                if (string.IsNullOrEmpty(newName)) return;

                // TODO: WorkbookContext.RenameLuaScript(oldName, newName);
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        #endregion
    }
}
