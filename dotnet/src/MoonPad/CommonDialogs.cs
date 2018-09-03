using System;
using System.Windows.Forms;

namespace MoonPad
{
    internal static class CommonDialogs
    {
        public delegate bool Validate(string input);

        public static IWin32Window Owner { get; set; }
        public static Invoker Invoker { private get; set; }

        public static void ShowActionModal(string caption, string message, Action action)
        {
            Invoker.InvokeAndWaitFor(() => new ActionModal(caption, message).ShowDialog(action));
        }

        public static void ShowActionModal(string caption, string message, Action<IProgress> action)
        {
            Invoker.InvokeAndWaitFor(() => new ActionModal(caption, message).ShowDialog(action));
        }

        public static void ShowError(string caption, string message)
        {
            Invoker.TryCatchInvoke(() =>
            {
                using (new DialogCenteringService(Owner))
                {
                    MessageBox.Show(message, caption,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        public static void ShowWarning(string caption, string message)
        {
            Invoker.TryCatchInvoke(() =>
            {
                using (new DialogCenteringService(Owner))
                {
                    MessageBox.Show(message, caption,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            });
        }

        public static void ShowMessage(string caption, string message)
        {
            Invoker.TryCatchInvoke(() =>
            {
                using (new DialogCenteringService(Owner))
                {
                    MessageBox.Show(message, caption,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            });
        }

        public static string Prompt(string text, string caption, string prefill = "", Validate validate = null)
        {
            // Based on answer from StackOverflow:
            // https://stackoverflow.com/a/5427121

            var prompt = new Form
            {
                Width = 440,
                Height = 130,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var textLabel = new Label {Left = 10, Top = 10, Width = 400, Text = text};
            var textBox = new TextBox {Left = 12, Top = 30, Width = 400};

            textBox.Validating += (sender, args) =>
            {
                if (validate != null && !validate(textBox.Text))
                {
                    args.Cancel = true;
                }
            };

            var ok = new Button
            {
                Text = "Ok",
                Left = 313,
                Width = 100,
                Top = 55,
                DialogResult = DialogResult.OK
            };

            ok.Click += (sender, args) => prompt.Close();

            var cancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel
            };

            cancel.Click += (sender, args) => prompt.Close();

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(ok);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = ok;
            prompt.CancelButton = cancel;

            textBox.Text = prefill;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : null;
        }
    }
}
