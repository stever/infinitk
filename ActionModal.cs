using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;

namespace MoonPad
{
    internal partial class ActionModal : Form, IProgress
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string caption;
        private readonly string message;
        private readonly Invoker invoker;

        public ActionModal(string caption, string message)
        {
            this.message = message;
            this.caption = caption;
            invoker = new Invoker(this);
            InitializeComponent();
        }

        int IProgress.Maximum
        {
            set => invoker.InvokeAndWaitFor(() => progressBar.Maximum = value);
        }

        int IProgress.Step
        {
            set => invoker.InvokeAndWaitFor(() => progressBar.Step = value);
        }

        int IProgress.Value
        {
            set => invoker.InvokeAndWaitFor(() => progressBar.Value = value);
        }

        string IProgress.Message
        {
            set => invoker.InvokeAndWaitFor(() => messageLabel.Text = value);
        }

        private void ActionModal_Load(object sender, EventArgs e)
        {
            Text = caption;
            messageLabel.Text = message;
        }

        public new DialogResult ShowDialog()
        {
            throw new InvalidOperationException();
        }

        public DialogResult ShowDialog(Action action)
        {
            Text = caption;
            messageLabel.Text = message;
            progressBar.Style = ProgressBarStyle.Marquee;

            invoker.DelayedTryCatchInvoke(() =>
            {
                Task.Run(() =>
                {
                    try
                    {
                        action();
                    }
                    catch (IOException ex)
                    {
                        if (ex.Message.StartsWith("The process cannot access the file"))
                        {
                            Hide();
                            CommonDialogs.ShowError("File in use", "The file cannot be opened because it is being used by another program.");
                        }
                        else
                        {
                            Log.Error("EXCEPTION", ex);
                            Hide();
                            ErrorHandler.HandleException(ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("EXCEPTION", ex);
                        Hide();
                        ErrorHandler.HandleException(ex);
                    }
                    finally
                    {
                        invoker.InvokeAndWaitFor(Close);
                    }
                });
            }, 500);

            return base.ShowDialog();
        }

        public DialogResult ShowDialog(Action<IProgress> action)
        {
            Text = caption;
            messageLabel.Text = message;
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Maximum = 100;
            progressBar.Step = 1;
            progressBar.Value = 0;

            // NOTE: The only purpose of the following is to delay the action.
            // NOTE: Probably a better way to achieve this.
            System.Threading.Timer timer = null;

            void Callback(object _)
            {
                try
                {
                    action(this);
                }
                catch (IOException ex)
                {
                    if (ex.Message.StartsWith("The process cannot access the file"))
                    {
                        invoker.InvokeAndWaitFor(Hide);
                        CommonDialogs.ShowError("File in use", "The file cannot be opened because it is being used by another program.");
                    }
                    else
                    {
                        Log.Error("EXCEPTION", ex);
                        invoker.InvokeAndWaitFor(Hide);
                        ErrorHandler.HandleException(ex);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("EXCEPTION", ex);
                    invoker.InvokeAndWaitFor(Hide);
                    ErrorHandler.HandleException(ex);
                }
                finally
                {
                    invoker.InvokeAndWaitFor(Close);
                }

                // ReSharper disable once PossibleNullReferenceException
                // ReSharper disable once AccessToModifiedClosure
                timer.Dispose();
            }

            timer = new System.Threading.Timer(Callback, null, 500, Timeout.Infinite);

            return base.ShowDialog();
        }
    }
}
