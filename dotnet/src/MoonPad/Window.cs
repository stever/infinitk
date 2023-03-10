using System;
using System.Reflection;
using MoonPad.GameEngine;
using log4net;
using OpenTK;
using OpenTK.Input;

namespace MoonPad
{
    internal class Window : GameWindow
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Game game = new Game();

        public Window(int width, int height) : base(width, height)
        {
            Log.Debug("Creating new Window");

            VSync = VSyncMode.On;

            // Keyboard
            KeyDown += KeyDownHandler;
            KeyUp += KeyUpHandler;

            // Mouse
            MouseDown += MouseButtonDownHandler;
            MouseUp += MouseButtonUpHandler;

            // World
            game.SetupGame();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            game.SetupGL();
            game.SetupViewport(Width, Height);
            game.Load();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            game.SetupViewport(Width, Height);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            game.Paint();
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            game.Update();
        }

        #region Mouse

        private void MouseButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
                game.EnableMouseControl();
        }

        private void MouseButtonUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
                game.DisableMouseControl();
        }

        #endregion

        #region Keyboard

        private void KeyDownHandler(object sender, KeyboardKeyEventArgs e)
        {
            game.KeyDown(e.Key);

            switch (e.Key)
            {
                case Key.Escape: Exit(); break;

                case Key.Number0: game.Reset(); break;

                case Key.Number1: WindowState = WindowState.Normal; break;
                case Key.Number2: WindowState = WindowState.Maximized; break;
                case Key.Number3: WindowState = WindowState.Fullscreen; break;
                case Key.Number4: WindowState = WindowState.Minimized; break;

                case Key.Number5: WindowBorder = WindowBorder.Resizable; break;
                case Key.Number6: WindowBorder = WindowBorder.Fixed; break;
                case Key.Number7: WindowBorder = WindowBorder.Hidden; break;
            }
        }

        private void KeyUpHandler(object sender, KeyboardKeyEventArgs e)
        {
            game.KeyUp(e.Key);
        }

        #endregion
    }
}
