using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using log4net;

namespace MoonPad.DockingWindows
{
    internal partial class LuaScriptsList : AbstractListControl, AbstractListControl.IListControl
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Database Database => formWindow?.Database;

        protected LuaScriptsList()
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

            formWindow.FileLoaded += FormWindow_FileLoaded;
            formWindow.FileClosed += FormWindow_FileClosed;
        }

        private void LuaScriptsList_Load(object sender, EventArgs e)
        {
            try
            {
                if (Database == null) return;
                LoadList();
                contextMenuStrip1.Enabled = true;
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
            var names = Database.GetLuaScriptNames();
            names.Sort();
            foreach (var name in names)
            {
                SideMenuListBox.Items.Add(name);
            }
        }

        private bool IsNameAvailable(string name)
        {
            var names = new HashSet<string>(Database.GetLuaScriptNames());
            return !names.Contains(name);
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
            formWindow.OpenLuaScriptTab((string) selectedItem);
        }

        #endregion

        #region Event handlers

        private void FormWindow_FileLoaded()
        {
            Invoker.TryCatchInvoke(() =>
            {
                LoadList();
                contextMenuStrip1.Enabled = true;
            });
        }

        private void FormWindow_FileClosed()
        {
            Invoker.TryCatchInvoke(() =>
            {
                SideMenuListBox.Items.Clear();
                contextMenuStrip1.Enabled = false;
            });
        }

        #endregion

        #region Context menu

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DeselectItem();

                var name = CommonDialogs.Prompt(
                    "Provide a unique name for the Lua Script:",
                    "Add Lua Script",
                    validate: IsNameAvailable);

                if (string.IsNullOrEmpty(name)) return;

                Database.AddLuaScript(name);
                LoadList();
                SideMenuListBox.SelectName(name);
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (new DialogCenteringService(formWindow))
            {
                try
                {
                    var confirm = MessageBox.Show(
                        "This will permanently delete this Lua script. Note that this operation cannot be undone!",
                        "Delete confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

                    if (confirm == DialogResult.OK)
                    {
                        var name = (string) SideMenuListBox.SelectedItem;
                        Database.DeleteLuaScript(name);
                        LoadList();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("EXCEPTION", ex);
                    ErrorHandler.HandleException(ex);
                }
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var oldName = (string) SideMenuListBox.SelectedItem;
                var newName = CommonDialogs.Prompt("Provide a new name for the Lua Script:", "Rename Lua Script", oldName, IsNameAvailable);
                if (string.IsNullOrEmpty(newName)) return;

                Database.RenameLuaScript(oldName, newName);
                LoadList();
                SideMenuListBox.SelectName(newName);
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
