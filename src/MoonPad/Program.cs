using System;
using System.Reflection;
using System.Windows.Forms;
using log4net;

namespace MoonPad
{
    internal static class Program
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [STAThread]
        private static void Main(string[] args)
        {
            Log.Info("Started");

            try
            {
                /*
                var win = new Window(640, 480) {Title = "MoonPad"};
                win.Run(60.0);
                */

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.Run(args.Length > 0 ? new FormWindow(args[0]) : new FormWindow());
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
            }

            Log.Info("Finished");
        }
    }
}
