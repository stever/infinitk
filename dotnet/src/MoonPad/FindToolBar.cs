using System;
using System.Windows.Forms;

namespace MoonPad
{
    public partial class FindToolBar : UserControl
    {
        public delegate void ToggleFindToolStripEventHandler();
        public event ToggleFindToolStripEventHandler ToggleFindToolStrip;

        public delegate void FindEventHandler(bool forward);
        public event FindEventHandler Find;

        public ToolStripTextBox FindTextBox { get; private set; }

        public FindToolBar()
        {
            InitializeComponent();
        }

        private void findCloseButton_Click(object sender, EventArgs e)
        {
            ToggleFindToolStrip?.Invoke();
        }

        private void findTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Find?.Invoke(true);
                return;
            }

            if (e.GetModifier().Control && e.GetKeyCode() == KeyCodes.F)
            {
                ToggleFindToolStrip?.Invoke();
            }
        }

        private void findNextButton_Click(object sender, EventArgs e)
        {
            Find?.Invoke(true);
        }

        private void findPreviousButton_Click(object sender, EventArgs e)
        {
            Find?.Invoke(false);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData != Keys.Escape) return false;
            ToggleFindToolStrip?.Invoke();
            return true;
        }
    }
}
