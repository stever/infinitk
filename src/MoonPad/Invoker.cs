using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using log4net;

namespace MoonPad
{
    /// <summary>
    /// The Invoker class is used to provide common BeginInvoke functionality
    /// across forms and controls. Use the Invoker.Invoke method on all
    /// ViewState event handlers.
    /// </summary>
    internal class Invoker
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Control control;

        public Invoker(Control control)
        {
            this.control = control;
        }

        public void TryCatchInvoke(Action action)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke((Action) (() => TryCatch(action)));
            }
            else
            {
                TryCatch(action);
            }
        }

        public void DelayedTryCatchInvoke(Action action, int delay)
        {
            System.Threading.Timer timer = null;

            void Callback(object _)
            {
                TryCatchInvoke(action);
                // ReSharper disable once PossibleNullReferenceException
                // ReSharper disable once AccessToModifiedClosure
                timer.Dispose();
            }

            timer = new System.Threading.Timer(Callback, null, delay, Timeout.Infinite);
        }

        public void InvokeAndWaitFor(Action action)
        {
            if (control.InvokeRequired)
            {
                var inQ = new BlockingCollection<object>();
                control.BeginInvoke((Action) (() =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception e)
                    {
                        inQ.Add(e);
                        return;
                    }

                    inQ.Add(null);
                }));

                var output = inQ.Take();
                if (output is Exception ex) throw ex;
            }
            else
            {
                action();
            }
        }

        private static void TryCatch(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
                ErrorHandler.HandleException(ex);
            }
        }
    }
}
