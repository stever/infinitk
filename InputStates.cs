using System.Reflection;
using log4net;
using OpenTK.Input;

namespace InfiniTK
{
    public class InputStates
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public JumpState JumpState { get; set; }

        #region Mouse handling data

        public bool MouseControlEnabled { get; set; }

        private MouseState previousMouseState;

        #endregion

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
            JumpState = JumpState.NotJumping;
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
                        if (JumpState == JumpState.NotJumping)
                            JumpState = JumpState.InitiateJump;
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
                    /*
                case Key.Up:
                    if (!keyDown) turnUpKeyDown = false;
                    if (e.Control && keyDown)
                    {
                        turnUpKeyDown = true;
                        turnUpOverDown = true;
                    }
                    break;
                case Key.Down:
                    if (!keyDown) turnDownKeyDown = false;
                    if (e.Control && keyDown)
                    {
                        turnDownKeyDown = true;
                        turnUpOverDown = false;
                    }
                    break;
                case Key.Left:
                    if (!keyDown) turnLeftKeyDown = false;
                    if (e.Control && keyDown)
                    {
                        turnLeftKeyDown = true;
                        turnLeftOverRight = true;
                    }
                    break;
                case Key.Right:
                    if (!keyDown) turnRightKeyDown = false;
                    if (e.Control && keyDown)
                    {
                        turnRightKeyDown = true;
                        turnLeftOverRight = false;
                    }
                    break;*/
            }
        }

        #endregion

        #region Mouse handling methods

        /// <summary>
        /// This method saves the current mouse state so that deltas are compared
        /// to this snapshot and not one from earlier.
        /// </summary>
        public void SnapshotCurrentMouseState()
        {
            previousMouseState = Mouse.GetState();
        }

        /// <summary>
        /// This method returns the delta to show how much the mouse has moved
        /// since the last time this method was called.
        /// </summary>
        /// <returns></returns>
        public MouseDelta GetMouseDelta()
        {
            var mouse = Mouse.GetState();

            var deltaX = mouse.X - previousMouseState.X;
            var deltaY = mouse.Y - previousMouseState.Y;
            var deltaZ = mouse.Wheel - previousMouseState.Wheel;

            previousMouseState = mouse;

            return new MouseDelta(deltaX, deltaY, deltaZ);
        }

        #endregion
    }
}
