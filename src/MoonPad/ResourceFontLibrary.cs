using System.Drawing;

namespace MoonPad
{
    internal static class ResourceFontLibrary
    {
        private static ResourceFont proFontWindows;
        private static ResourceFont pragmataPro;

        public static FontFamily GetProFontWindows()
        {
            if (proFontWindows == null)
            {
                proFontWindows = new ResourceFont(Properties.Resources.ProFontWindows);
            }

            return proFontWindows.GetFontFamily();
        }

        public static FontFamily GetPragmataPro()
        {
            if (pragmataPro == null)
            {
                pragmataPro = new ResourceFont(Properties.Resources.PragmataPro_Mono_R_liga_0826);
            }

            return pragmataPro.GetFontFamily();
        }
    }
}
