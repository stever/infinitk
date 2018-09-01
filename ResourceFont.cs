using System;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace MoonPad
{
    internal class ResourceFont
    {
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
            IntPtr pdv, [In] ref uint pcFonts);

        private readonly PrivateFontCollection fonts = new PrivateFontCollection();
        private readonly FontFamily fontFamily;

        public ResourceFont(byte[] resourceBytes)
        {
            var len = resourceBytes.Length;
            var fontPtr = Marshal.AllocCoTaskMem(len);
            Marshal.Copy(resourceBytes, 0, fontPtr, len);
            uint dummy = 0;
            fonts.AddMemoryFont(fontPtr, len);
            AddFontMemResourceEx(fontPtr, (uint)len, IntPtr.Zero, ref dummy);
            Marshal.FreeCoTaskMem(fontPtr);
            fontFamily = fonts.Families.Last();
        }

        public FontFamily GetFontFamily()
        {
            return fontFamily;
        }
    }
}
