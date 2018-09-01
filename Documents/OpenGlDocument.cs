using System;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using MoonPad.GameEngine;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace MoonPad.Documents
{
    public partial class OpenGlDocument : UserControl
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Game game = new Game();

        private bool loaded;

        public OpenGlDocument()
        {
            // TODO: Try to speed up loading of the tab, perhaps delaying the loading of the GLControl?

            InitializeComponent();

            Log.Debug("Creating new GL document control");

            glControl1.VSync = true;

            // Keyboard
            glControl1.KeyDown += KeyDownHandler;
            glControl1.KeyUp += KeyUpHandler;

            // Mouse
            glControl1.MouseDown += MouseButtonDownHandler;
            glControl1.MouseUp += MouseButtonUpHandler;

            // World
            game.SetupGame();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!loaded) return;
            game.Update();
            game.Paint();
            glControl1.SwapBuffers();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            game.SetupGL();
            game.SetupViewport(Width, Height);
            game.Load();
            loaded = true;
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (!loaded) return;
            game.SetupViewport(Width, Height);
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded) return;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            glControl1.SwapBuffers();
        }

        #region Mouse

        private void MouseButtonDownHandler(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                game.EnableMouseControl();
        }

        private void MouseButtonUpHandler(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                game.DisableMouseControl();
        }

        #endregion

        #region Keyboard

        private static Key? TranslateKey(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.D0:
                    return Key.Number0;
                case Keys.W:
                    return Key.W;
                case Keys.S:
                    return Key.S;
                case Keys.A:
                    return Key.A;
                case Keys.D:
                    return Key.D;
                case Keys.Space:
                    return Key.Space;
                default:
                    return null;
            }
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            var key = TranslateKey(e.KeyCode);
            if (key.HasValue)
            {
                switch (key.Value)
                {
                    case Key.Number0:
                        game.Reset();
                        break;
                    default:
                        game.KeyDown(key.Value);
                        break;
                }
            }
        }

        private void KeyUpHandler(object sender, KeyEventArgs e)
        {
            var key = TranslateKey(e.KeyCode);
            if (key.HasValue)
            {
                game.KeyUp(key.Value);
            }
        }

        #endregion
    }
}
