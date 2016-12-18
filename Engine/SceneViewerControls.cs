using System.Reflection;
using InfiniTK.GameEngine.Actions;
using log4net;
using OpenTK.Input;

namespace InfiniTK.Engine
{
    public class SceneViewerControls
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // TODO: Move the JumpState to the Player class.
        public Jump.JumpState JumpState { get; set; }

        public bool MouseControlEnabled { get; set; }

        #region Movement state properties

        public bool IsMovingForward
        {
            get
            {
                if (moveForwardKeyDown) 
                    return !moveBackwardKeyDown || moveForwardOverBackward;
                return false;
            }
        }

        public bool IsMovingBackward
        {
            get
            {
                if (moveBackwardKeyDown)
                    return !moveForwardKeyDown || !moveForwardOverBackward;
                return false;
            }
        }

        public bool IsMovingLeft
        {
            get
            {
                if (moveLeftKeyDown)
                    return !moveRightKeyDown || moveLeftOverRight;
                return false;
            }
        }

        public bool IsMovingRight
        {
            get
            {
                if (moveRightKeyDown)
                    return !moveLeftKeyDown || !moveLeftOverRight;
                return false;
            }
        }

        public bool IsMovingUp
        {
            get
            {
                if (moveUpKeyDown)
                    return !moveDownKeyDown || moveUpOverDown;
                return false;
            }
        }

        public bool IsMovingDown
        {
            get
            {
                if (moveDownKeyDown)
                    return !moveUpKeyDown || !moveUpOverDown;
                return false;
            }
        }

        public bool IsTurningLeft
        {
            get
            {
                if (turnLeftKeyDown)
                    return !turnRightKeyDown || turnLeftOverRight;
                return false;
            }
        }

        public bool IsTurningRight
        {
            get
            {
                if (turnRightKeyDown)
                    return !turnLeftKeyDown || !turnLeftOverRight;
                return false;
            }
        }

        public bool IsTurningUp
        {
            get
            {
                if (turnUpKeyDown)
                    return !turnDownKeyDown || turnUpOverDown;
                return false;
            }
        }

        public bool IsTurningDown
        {
            get
            {
                if (turnDownKeyDown)
                    return !turnUpKeyDown || !turnUpOverDown;
                return false;
            }
        }

        #endregion

        #region Keyboard state variables

        private bool jumpKeyDown;

        // Forward and backward movement.
        private bool moveForwardKeyDown;
        private bool moveBackwardKeyDown;
        private bool moveForwardOverBackward = true;

        // Left and right movement.
        private bool moveLeftKeyDown;
        private bool moveRightKeyDown;
        private bool moveLeftOverRight = true;

        // Up and down movement.
        private bool moveUpKeyDown;
        private bool moveDownKeyDown;
        private bool moveUpOverDown = true;

        // Left and right spinning.
        private bool turnLeftKeyDown;
        private bool turnRightKeyDown;
        private bool turnLeftOverRight = true;

        // Up and down tilting.
        private bool turnUpKeyDown;
        private bool turnDownKeyDown;
        private bool turnUpOverDown = true;

        #endregion

        /// <summary>
        /// Do a complete reset of the controls, except mouse controls.
        /// </summary>
        public void Reset()
        {
            ResetKeyStates();
            JumpState = Jump.JumpState.NotJumping;
        }

        #region Keyboard handling methods

        /// <summary>
        /// Resets the variables used for key states.
        /// </summary>
        public void ResetKeyStates()
        {
            jumpKeyDown = false;
            moveForwardKeyDown = false;
            moveBackwardKeyDown = false;
            moveLeftKeyDown = false;
            moveRightKeyDown = false;
            moveUpKeyDown = false;
            moveDownKeyDown = false;
            turnLeftKeyDown = false;
            turnRightKeyDown = false;
            turnUpKeyDown = false;
            turnDownKeyDown = false;
        }

        public void KeyDown(Key key)
        {
            Log.DebugFormat("KeyDown: {0}", key);
            ToggleKey(key, true);
        }

        public void KeyUp(Key key)
        {
            Log.DebugFormat("KeyUp: {0}", key);
            ToggleKey(key, false);
        }

        private void ToggleKey(Key key, bool keyDown)
        {
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

                case Key.Plus:
                    moveUpKeyDown = keyDown;
                    if (keyDown) moveUpOverDown = true;
                    break;

                case Key.Minus:
                    moveDownKeyDown = keyDown;
                    if (keyDown) moveUpOverDown = false;
                    break;

                case Key.W:
                    moveForwardKeyDown = keyDown;
                    if (keyDown) moveForwardOverBackward = true;
                    break;

                case Key.S:
                    moveBackwardKeyDown = keyDown;
                    if (keyDown) moveForwardOverBackward = false;
                    break;
                case Key.A:
                    moveLeftKeyDown = keyDown;
                    if (keyDown) moveLeftOverRight = true;
                    break;

                case Key.D:
                    moveRightKeyDown = keyDown;
                    if (keyDown) moveLeftOverRight = false;
                    break;
            }
        }

        #endregion
    }
}
