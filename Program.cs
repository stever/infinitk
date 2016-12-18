using System;
using System.Reflection;
using log4net;
using OpenTK;
using OpenTK.Input;

namespace InfiniTK
{
    internal static class Program
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
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

        private class Window : GameWindow
        {
            private readonly GameEngine.GameEngine gameEngine = new GameEngine.GameEngine();

            public Window(int width, int height) : base(width, height)
            {
                Log.Debug("Creating new Window");

                VSync = VSyncMode.On;

                // Keyboard
                Keyboard.KeyRepeat = true;
                Keyboard.KeyDown += KeyDownHandler;
                Keyboard.KeyUp += KeyUpHandler;

                // Mouse
                Mouse.ButtonDown += MouseButtonDownHandler;
                Mouse.ButtonUp += MouseButtonUpHandler;

                // World
                gameEngine.SetupGame();
            }

            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);
                gameEngine.SetupGL();
                gameEngine.SetupViewport(Width, Height);
                gameEngine.Load();
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                gameEngine.SetupViewport(Width, Height);
            }

            protected override void OnRenderFrame(FrameEventArgs e)
            {
                base.OnRenderFrame(e);
                gameEngine.Paint();
                SwapBuffers();
            }

            protected override void OnUpdateFrame(FrameEventArgs e)
            {
                gameEngine.Update();
            }

            #region Mouse

            private void MouseButtonDownHandler(object sender, MouseButtonEventArgs e)
            {
                if (e.Button == MouseButton.Left)
                    gameEngine.EnableMouseControl();
            }

            private void MouseButtonUpHandler(object sender, MouseButtonEventArgs e)
            {
                if (e.Button == MouseButton.Left)
                    gameEngine.DisableMouseControl();
            }

            #endregion

            #region Keyboard

            private void KeyDownHandler(object sender, KeyboardKeyEventArgs e)
            {
                gameEngine.KeyDown(e.Key);

                switch (e.Key)
                {
                    case Key.Escape: Exit(); break;

                    case Key.Number0: gameEngine.Reset(); break;

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
                gameEngine.KeyUp(e.Key);
            }

            #endregion
        }
    }
}
