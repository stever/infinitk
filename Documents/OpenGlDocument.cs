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

        private Game game;
        private bool loaded;

        public OpenGlDocument()
        {
            InitializeComponent();
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
            CommonDialogs.ShowActionModal("OpenGL", "Creating game world...", () =>
            {
                game = new Game();
                game.SetupGame();
            });

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

        private void glControl1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                game.EnableMouseControl();
        }

        private void glControl1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
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

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
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

        private void glControl1_KeyUp(object sender, KeyEventArgs e)
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
