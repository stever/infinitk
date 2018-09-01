using System;
using System.Reflection;
using log4net;

namespace InfiniTK
{
    internal static class Program
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [STAThread]
        private static void Main()
        {
            Log.Info("Started");

            try
            {
                var win = new Window(640, 480) {Title = "InfiniTK"};
                win.Run(60.0);
            }
            catch (Exception ex)
            {
                Log.Error("EXCEPTION", ex);
            }

            Log.Info("Finished");
        }
    }
}
