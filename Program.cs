using System;
using System.Reflection;
using log4net;

namespace InfiniTK
{
    static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log.Info("Started");
            try
            {
                InfiniGameWindow win = new InfiniGameWindow();
                win.Title = "InfiniTK";
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
