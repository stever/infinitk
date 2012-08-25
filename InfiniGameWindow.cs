using System;
using System.Reflection;
using log4net;
using OpenTK;
using OpenTK.Input;

namespace InfiniTK
{
    public class InfiniGameWindow : GameWindow
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly GameEngine _gameEngine = new GameEngine();

        public InfiniGameWindow() : base(640, 480)
        {
			Log.Debug("Creating new InfiniGameWindow");
			
            VSync = VSyncMode.On;

            Keyboard.KeyRepeat = true;
            Keyboard.KeyDown += KeyDownHandler;
            Keyboard.KeyUp += KeyUpHandler;

            Mouse.ButtonDown += MouseButtonDownHandler;
            Mouse.ButtonUp += MouseButtonUpHandler;

            _gameEngine.LimitFrameRate = false;
            _gameEngine.SetupGame();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _gameEngine.SetupGL();
            _gameEngine.SetupViewport(Width, Height);
            _gameEngine.Load();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _gameEngine.SetupViewport(Width, Height);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            _gameEngine.Paint();
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            _gameEngine.Update();
        }

        private void MouseButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
                _gameEngine.EnableMouseControl();
        }

        private void MouseButtonUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
                _gameEngine.DisableMouseControl();
        }

        private void KeyDownHandler(object sender, KeyboardKeyEventArgs e)
        {
            _gameEngine.KeyDown(e.Key);

            switch (e.Key)
            {
                case Key.Escape: Exit(); break;

                case Key.Number0: _gameEngine.Reset(); break;

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
            _gameEngine.KeyUp(e.Key);
        }
    }
}
