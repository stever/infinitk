using System;
using System.Reflection;
using log4net;
using OpenTK;
using OpenTK.Input;

namespace InfiniTK
{
    public class InfiniGameWindow : GameWindow
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly GameEngine gameEngine = new GameEngine();

        public InfiniGameWindow() : base(640, 480)
        {
			Log.Debug("Creating new InfiniGameWindow");
			
            VSync = VSyncMode.On;

            Keyboard.KeyRepeat = true;
            Keyboard.KeyDown += KeyDownHandler;
            Keyboard.KeyUp += KeyUpHandler;

            Mouse.ButtonDown += MouseButtonDownHandler;
            Mouse.ButtonUp += MouseButtonUpHandler;

            gameEngine.LimitFrameRate = false;
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
    }
}
