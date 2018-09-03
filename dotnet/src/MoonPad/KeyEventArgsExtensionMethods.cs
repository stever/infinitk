using System.Windows.Forms;

namespace MoonPad
{
    internal static class KeyEventArgsExtensionMethods
    {
        public static KeyPressModifier GetModifier(this KeyEventArgs e)
        {
            var modifier = new KeyPressModifier();
            if (e.Control) modifier.Control = true;
            if (e.Alt) modifier.Alt = true;
            if (e.Shift) modifier.Shift = true;
            return modifier;
        }

        public static KeyCodes GetKeyCode(this KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A: return KeyCodes.A;
                case Keys.B: return KeyCodes.B;
                case Keys.C: return KeyCodes.C;
                case Keys.D: return KeyCodes.D;
                case Keys.E: return KeyCodes.E;
                case Keys.F: return KeyCodes.F;
                case Keys.G: return KeyCodes.G;
                case Keys.H: return KeyCodes.H;
                case Keys.I: return KeyCodes.I;
                case Keys.J: return KeyCodes.J;
                case Keys.K: return KeyCodes.K;
                case Keys.L: return KeyCodes.L;
                case Keys.M: return KeyCodes.M;
                case Keys.N: return KeyCodes.N;
                case Keys.O: return KeyCodes.O;
                case Keys.P: return KeyCodes.P;
                case Keys.Q: return KeyCodes.Q;
                case Keys.R: return KeyCodes.R;
                case Keys.S: return KeyCodes.S;
                case Keys.T: return KeyCodes.T;
                case Keys.U: return KeyCodes.U;
                case Keys.V: return KeyCodes.V;
                case Keys.W: return KeyCodes.W;
                case Keys.X: return KeyCodes.X;
                case Keys.Y: return KeyCodes.Y;
                case Keys.Z: return KeyCodes.Z;
                case Keys.F1: return KeyCodes.F1;
                case Keys.F2: return KeyCodes.F2;
                case Keys.F3: return KeyCodes.F3;
                case Keys.F4: return KeyCodes.F4;
                case Keys.F5: return KeyCodes.F5;
                case Keys.F6: return KeyCodes.F6;
                case Keys.F7: return KeyCodes.F7;
                case Keys.F8: return KeyCodes.F8;
                case Keys.F9: return KeyCodes.F9;
                case Keys.F10: return KeyCodes.F10;
                case Keys.F11: return KeyCodes.F11;
                case Keys.F12: return KeyCodes.F12;
                default: return KeyCodes.Unsupported;
            }
        }
    }
}
