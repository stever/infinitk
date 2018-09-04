using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using log4net;

namespace MoonPad
{
    /// <remarks>
    /// This would be an abstract class but the Windows Forms designer doesn't
    /// like it. Instead, constructor is protected.
    /// </remarks>>
    internal partial class AbstractListControl : UserControl
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private object previouslySelectedItem;

        protected readonly FormWindow formWindow;
        protected readonly Invoker Invoker;
        protected readonly SideMenuListBox SideMenuListBox;

        protected IListControl Handler;

        protected AbstractListControl()
        {
            InitializeComponent();
        }

        protected AbstractListControl(FormWindow formWindow)
        {
            InitializeComponent();

            this.formWindow = formWindow;
            Invoker = new Invoker(this);

            // Side-menu list box.
            SideMenuListBox = new SideMenuListBox {Dock = DockStyle.Fill};
            SideMenuListBox.MouseDown += sideMenuListBox_MouseDown;
            SideMenuListBox.MouseClick += sideMenuListBox_MouseClick;
            SideMenuListBox.MouseDoubleClick += sideMenuListBox_MouseDoubleClick;
            Controls.Add(SideMenuListBox);
        }

        private void AbstractListControl_Leave(object sender, EventArgs e)
        {
            if (SideMenuListBox == null) return;
            SideMenuListBox.SelectedIndex = -1;
        }

        protected void DeselectItem()
        {
            SideMenuListBox.SelectedIndex = -1;
            previouslySelectedItem = null;
        }

        #region sideMenuListBox event handlers.

        private void sideMenuListBox_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                var index = SideMenuListBox.IndexFromPoint(new Point(e.X, e.Y));
                if (index < 0)
                {
                    SideMenuListBox.SelectedIndex = -1;
                    Handler.NonItemMouseDown();
                }
                else
                {
                    SideMenuListBox.SelectedIndex = index;
                    Handler.ItemMouseDown();
                }
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        private void sideMenuListBox_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                var selectedItem = SideMenuListBox.SelectedItem;
                if (previouslySelectedItem != null && previouslySelectedItem == selectedItem)
                {
                    DeselectItem();
                    return;
                }

                previouslySelectedItem = selectedItem;
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        private void sideMenuListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                var selectedItem = SideMenuListBox.SelectedItem;
                if (selectedItem == null) return;
                Handler.OpenItem(selectedItem);
                DeselectItem();
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }

        #endregion

        internal interface IListControl
        {
            void ItemMouseDown();
            void NonItemMouseDown();
            void OpenItem(object selectedItem);
        }
    }
}
