using System;

namespace MoonPad
{
    internal static class ErrorHandler
    {
        private static DateTime lastError = new DateTime(1970, 1, 1);
        private static bool showingError;

        public static void HandleException(Exception ex)
        {
            if (showingError || (DateTime.Now - lastError).TotalSeconds < 2)
            {
                return;
            }

            showingError = true;
            CommonDialogs.ShowError("Exception", "Exception (see log for details)");
            showingError = false;

            lastError = DateTime.Now;
        }
    }
}
