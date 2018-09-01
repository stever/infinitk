using System.Reflection;
using MoonPad.Engine;
using MoonPad.GameEngine.Actions;
using log4net;
using OpenTK.Input;

namespace MoonPad.GameEngine
{
    public class GameControls : SceneViewerControls
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Jump.JumpState JumpState { get; set; }

        private bool jumpKeyDown;

        public new void ResetKeyStates()
        {
            JumpState = Jump.JumpState.NotJumping;
            jumpKeyDown = false;
        }

        public new void KeyDown(Key key)
        {
            Log.DebugFormat("KeyDown: {0}", key);
            ToggleKey(key, true);
        }

        public new void KeyUp(Key key)
        {
            Log.DebugFormat("KeyUp: {0}", key);
            ToggleKey(key, false);
        }

        protected new void ToggleKey(Key key, bool keyDown)
        {
            base.ToggleKey(key, keyDown);

            switch (key)
            {
                case Key.Space:
                    if (!keyDown) jumpKeyDown = false;
                    else if (!jumpKeyDown)
                    {
                        jumpKeyDown = true;
                        if (JumpState == Jump.JumpState.NotJumping)
                            JumpState = Jump.JumpState.InitiateJump;
                    }
                    break;
            }
        }
    }
}
